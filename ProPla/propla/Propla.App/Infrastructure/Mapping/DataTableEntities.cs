// =============================================================
// DataTableEntities – 完全リファクタ版（C# 7/8 互換、既存置換用 / Indexer パッチ適用）
//  - FIX: Indexer（既定のインデクサ）プロパティを反射対象から除外し
//         RuntimePropertyInfo.GetValue(...) の TargetParameterCountException を回避
//  - 反射結果の静的キャッシュ、高機能 ConvertTo、大小無視列照合 などは従来通り
// =============================================================

#nullable disable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Propla
{
    /// <summary>
    /// <see cref="DataTable"/> と任意のエンティティ型との相互マッピングを提供します。
    /// 反射結果をキャッシュして高速化し、列名は大文字小文字を無視して照合します。
    /// </summary>
    public static class DataTableEntities
    {
        /// <summary>型ごとのプロパティ/フィールドマップを保持するキャッシュ。</summary>
        private static readonly ConcurrentDictionary<Type, ReflectionMap> _mapCache
            = new ConcurrentDictionary<Type, ReflectionMap>();

        /// <summary>列名 → 値取得/設定先（プロパティ/フィールド）のマップを表します。</summary>
        private sealed class ReflectionMap
        {
            public Dictionary<string, PropertyInfo> PropertiesByName { get; }
            public Dictionary<string, FieldInfo> FieldsByName { get; }

            public ReflectionMap(Type t)
            {
                // public instance のみ対象。
                // FIX: インデクサ（GetIndexParameters().Length > 0）を除外、かつ CanRead/CanWrite の両方を満たすもののみ採用。
                PropertiesByName = t
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                    .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                FieldsByName = t
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .GroupBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// オブジェクトを指定型へ変換します。<br/>
        /// 変換前と変換後の型が同一の場合はそのまま返し、<see cref="DBNull"/> は <c>null</c> に変換します。<br/>
        /// <see cref="Nullable{T}"/>、<see cref="Enum"/>、<see cref="Guid"/>、<see cref="DateTime"/> などもサポートします。
        /// </summary>
        public static object ConvertTo(object sourceObject, Type targetType)
        {
            if (sourceObject == null || sourceObject == DBNull.Value)
            {
                if (!IsNullableType(targetType) && targetType.IsValueType)
                    return Activator.CreateInstance(targetType);
                return null;
            }

            var srcType = sourceObject.GetType();
            if (srcType == targetType || targetType.IsAssignableFrom(srcType))
                return sourceObject;

            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (underlying.IsEnum)
                {
                    if (sourceObject is string s)
                        return Enum.Parse(underlying, s, ignoreCase: true);
                    return Enum.ToObject(underlying, System.Convert.ChangeType(sourceObject, Enum.GetUnderlyingType(underlying), CultureInfo.InvariantCulture));
                }

                if (underlying == typeof(Guid))
                {
                    if (sourceObject is Guid g) return g;
                    return Guid.Parse(sourceObject.ToString());
                }

                if (underlying == typeof(DateTime))
                {
                    if (sourceObject is DateTime dt) return dt;
                    if (sourceObject is string ds)
                    {
                        if (DateTime.TryParse(ds, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
                            return parsed;
                    }
                }

                var convSrc = TypeDescriptor.GetConverter(srcType);
                if (convSrc != null && convSrc.CanConvertTo(underlying))
                    return convSrc.ConvertTo(null, CultureInfo.InvariantCulture, sourceObject, underlying);

                var convDest = TypeDescriptor.GetConverter(underlying);
                if (convDest != null && convDest.CanConvertFrom(srcType))
                    return convDest.ConvertFrom(null, CultureInfo.InvariantCulture, sourceObject);

                return System.Convert.ChangeType(sourceObject, underlying, CultureInfo.InvariantCulture);
            }
            catch
            {
                return sourceObject;
            }
        }

        /// <summary>
        /// <see cref="DataTable"/> をエンティティへ投影し、指定の <see cref="ObservableCollection{T}"/> に格納します（互換API）。
        /// </summary>
        public static void DataTableToEntities<T>(DataTable dataTable, ObservableCollection<T> entities)
            where T : class, new()
        {
            var list = DataTableToList<T>(dataTable);
            foreach (var item in list)
                entities.Add(item);
        }

        // 戻り値を void から T (または T?) に変更
        public static T? DataTableToEntitie<T>(DataTable dataTable, ObservableCollection<T> entities)
            where T : class, new()
        {
            var list = DataTableToList<T>(dataTable);

            foreach (var item in list)
                entities.Add(item);

            // リストの先頭を返す。リストが空なら null を返す。
            return list.FirstOrDefault();
        }

        /// <summary>
        /// <see cref="DataTable"/> をエンティティの <see cref="List{T}"/> に変換して返します（推奨API）。
        /// </summary>
        public static List<T> DataTableToList<T>(DataTable dataTable) where T : class, new()
        {
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));

            var map = _mapCache.GetOrAdd(typeof(T), t => new ReflectionMap(t));

            var columnIndexByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < dataTable.Columns.Count; i++)
                columnIndexByName[dataTable.Columns[i].ColumnName] = i;

            var result = new List<T>(dataTable.Rows.Count);

            foreach (DataRow row in dataTable.Rows)
            {
                var entity = new T();

                foreach (var kv in map.PropertiesByName)
                {
                    var name = kv.Key;
                    var prop = kv.Value;

                    if (!columnIndexByName.TryGetValue(name, out var idx)) continue;
                    var raw = row[idx];
                    var converted = ConvertTo(raw, prop.PropertyType);
                    prop.SetValue(entity, converted, null);
                }

                foreach (var kv in map.FieldsByName)
                {
                    var name = kv.Key;
                    var field = kv.Value;

                    if (!columnIndexByName.TryGetValue(name, out var idx)) continue;
                    var raw = row[idx];
                    var converted = ConvertTo(raw, field.FieldType);
                    field.SetValue(entity, converted);
                }

                result.Add(entity);
            }

            return result;
        }

        /// <summary>
        /// エンティティ集合を指定の <see cref="DataTable"/> に書き戻します（互換API）。列が存在しない場合は自動追加します。
        /// </summary>
        public static void EntitiesToDataTable<T>(IEnumerable<T> entities, DataTable dataTable)
            where T : class, new()
            => FillDataTable(entities, dataTable);

        /// <summary>
        /// エンティティ集合から新しい <see cref="DataTable"/> を作成して返します（推奨API）。
        /// </summary>
        public static DataTable EntitiesToDataTable<T>(IEnumerable<T> entities) where T : class, new()
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            var dt = new DataTable(typeof(T).Name);
            FillDataTable(entities, dt);
            return dt;
        }

        /// <summary>
        /// 既存の <see cref="DataTable"/> にエンティティの内容を書き込みます。列がなければ自動追加します。
        /// </summary>
        private static void FillDataTable<T>(IEnumerable<T> entities, DataTable dataTable) where T : class, new()
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));

            var type = typeof(T);
            var map = _mapCache.GetOrAdd(type, t => new ReflectionMap(t));

            foreach (var entity in entities)
            {
                var row = dataTable.NewRow();

                // プロパティ → 列（※Indexers は ReflectionMap で除外済み）
                foreach (var kv in map.PropertiesByName)
                {
                    var name = kv.Key;
                    var prop = kv.Value;

                    EnsureColumn(dataTable, name, prop.PropertyType);
                    object val = null;
                    try
                    {
                        val = prop.GetValue(entity, null);
                    }
                    catch (TargetParameterCountException)
                    {
                        // 念のための保険：Indexers 等は読み取りスキップ
                        continue;
                    }
                    row[name] = NormalizeForDataTable(val);
                }

                // フィールド → 列
                foreach (var kv in map.FieldsByName)
                {
                    var name = kv.Key;
                    var field = kv.Value;

                    EnsureColumn(dataTable, name, field.FieldType);
                    var val = field.GetValue(entity);
                    row[name] = NormalizeForDataTable(val);
                }

                dataTable.Rows.Add(row);
            }
        }

        /// <summary>DataTable 列の存在を保証し、なければ追加します。</summary>
        private static void EnsureColumn(DataTable table, string name, Type type)
        {
            if (!table.Columns.Contains(name))
            {
                var colType = Nullable.GetUnderlyingType(type) ?? type;
                table.Columns.Add(name, colType);
            }
        }

        /// <summary>DataTable に書き込む値を正規化します（null → DBNull）。</summary>
        private static object NormalizeForDataTable(object value) => value ?? DBNull.Value;

        /// <summary><c>Nullable&lt;T&gt;</c> かどうかを判定します。</summary>
        private static bool IsNullableType(Type t) => Nullable.GetUnderlyingType(t) != null;
    }
}
