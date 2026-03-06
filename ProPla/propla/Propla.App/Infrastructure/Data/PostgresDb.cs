using Npgsql;
using System;
using System.Data;
using System.Windows;

/// <summary>
/// Postgres アクセス
/// </summary>
namespace Propla
{

    public class PostgresDb
    {
        public const string STRING = "System.String";
        public const string INT32 = "System.Int32";
        public const string BOOLEAN = "System.Boolean";
        public const string DATETIME = "System.DateTime";
        public const string DECIMAL = "System.Decimal";
        public const string BYTE = "System.Byte[]";

        private Npgsql.NpgsqlConnection _connection;
        private Npgsql.NpgsqlDataAdapter _adapter;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PostgresDb()
        {
            _connection = null;
            _adapter = null;
        }

        /// <summary>
        /// NpgsqlDbType 取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns>NpgsqlDbType</returns>
        NpgsqlTypes.NpgsqlDbType? GetNpgsqlDbType(string name)
        {
            switch (name)
            {
                case STRING:
                    return NpgsqlTypes.NpgsqlDbType.Varchar;
                case INT32:
                    return NpgsqlTypes.NpgsqlDbType.Integer;
                case BOOLEAN:
                    return NpgsqlTypes.NpgsqlDbType.Boolean;
                case DATETIME:
                    return NpgsqlTypes.NpgsqlDbType.Timestamp;
                case DECIMAL:
                    return NpgsqlTypes.NpgsqlDbType.Numeric;
                case BYTE:
                    return NpgsqlTypes.NpgsqlDbType.Bytea;
                default:
                    MessageBox.Show("パラメータエラー：NpgsqlTypes.NpgsqlDbType");
                    return null;
            }
        }
        /// <summary>
        /// Postgres　Open　⇒  接続
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        public void DbOpen(string connectionString)
        {
            try
            {
                _connection = new Npgsql.NpgsqlConnection(connectionString);
                _connection.Open();
                _adapter = new Npgsql.NpgsqlDataAdapter("", _connection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// SelectCommandへのパラメータ追加
        /// </summary>
        /// <param name="name"></param>f
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        public void DbParAdd(string name, string value, string dbType)
        {
            try
            {
                _adapter.SelectCommand.Parameters.Add(name, (NpgsqlTypes.NpgsqlDbType)GetNpgsqlDbType(dbType));
                _adapter.SelectCommand.Parameters[name].Direction = ParameterDirection.Input;
                switch (dbType)
                {
                    case INT32:
                        _adapter.SelectCommand.Parameters[name].Value = Int32.Parse(value);
                        break;
                    case STRING:
                        _adapter.SelectCommand.Parameters[name].Value = value;
                        break;
                    default:
                        MessageBox.Show("パラメータエラー：NpgsqlTypes.NpgsqlDbType");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// SelectCommandのパラメータクリア
        /// </summary>
        public void DbParClear()
        {
            if (_adapter == null) return;

            _adapter.SelectCommand.Parameters.Clear();
        }
        /// <summary>
        /// Select実行
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="dataTable"></param>
        public void DbSelectDataTable(string columns, string table, string where, DataTable dataTable)
        {
            string SQL = "";
            try
            {
                SQL = "select " + columns + " from " + table + " where " + where;
                _adapter.SelectCommand.CommandText = SQL;
                _adapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                string s = ex.Message + "\r\n" + SQL;
                TxtMsgBox _txt = new(s);
                _txt.ShowDialog();

            }
        }
        /// <summary>
        /// Inset実行
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        public void DbInsert(DataTable dataTable, string columnName, string tableName)
        {
            try
            {
                NpgsqlCommand cmd = new();
                cmd.Connection = _connection;
                string wcol = "";
                string wval = "";

                //'-------------------------------
                //'ループでデータを取得
                //'-------------------------------
                foreach (DataRow row in dataTable.Rows)
                {
                    // -------------------------------
                    //  SELECT したカラムの一覧
                    // -------------------------------
                    cmd.Parameters.Clear();
                    int i = 0;
                    wcol = "";
                    wval = "";
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        i++;

                        //'-------------------------------
                        //'  指定があるカラムのみセット
                        //'-------------------------------
                        string string1 = "," + columnName.ToLower() + ",";
                        string string2 = "," + col.ColumnName.ToLower() + ",";

                        if (string1.IndexOf(string2) + 1 != 0)
                        {

                            wcol += col.ColumnName + ",";
                            wval += ":P" + i.ToString() + ",";

                            string paramName = "P" + i.ToString();
                            string type = col.DataType.ToString();

                            cmd.Parameters.Add(paramName, (NpgsqlTypes.NpgsqlDbType)GetNpgsqlDbType(type));

                            cmd.Parameters[paramName].Value = row[col.ColumnName];
                        }

                    }

                    wcol = wcol.Substring(0, (wcol.Length - 1));
                    wval = wval.Substring(0, (wval.Length - 1));
                    cmd.CommandText = " insert into " + tableName + "(" + wcol + ") values(" + wval + ");";
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Inset実行
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        public void DbUpdate(DataTable dataTable, string columnName, string tableName, string whereColumn)
        {
            NpgsqlCommand cmd = new();
            cmd.Connection = _connection;
            string wcol = "";
            string wval = "";
            string whereSql = "";

            //入力カラムチェック
            try
            {
                string[] _cols = columnName.Split(',');
                foreach (var _col in _cols)
                {
                    Boolean _errFlg = true;
                    foreach (DataColumn _DtCol in dataTable.Columns)
                    {
                        if (_col.ToLower() == _DtCol.ColumnName.ToLower())
                        {
                            _errFlg = false;
                            break;
                        }
                    }
                    if (_errFlg == true)
                    {
                        MessageBox.Show("カラムがありません　columnName" + _col);
                        return;
                    }
                }
                _cols = whereColumn.Split(',');
                foreach (var _col in _cols)
                {
                    Boolean _errFlg = true;
                    foreach (DataColumn _DtCol in dataTable.Columns)
                    {
                        if (_col.ToLower() == _DtCol.ColumnName.ToLower())
                        {
                            _errFlg = false;
                            break;
                        }
                    }
                    if (_errFlg == true)
                    {
                        MessageBox.Show("カラムがありません　whereColumn" + _col);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            try
            {
                //'-------------------------------
                //'ループでデータを取得
                //'-------------------------------
                foreach (DataRow row in dataTable.Rows)
                {
                    // -------------------------------
                    //  SELECT したカラムの一覧
                    // -------------------------------
                    cmd.Parameters.Clear();
                    int i = 0;
                    wcol = "";
                    wval = "";

                    foreach (DataColumn col in dataTable.Columns)
                    {
                        i++;

                        //'-------------------------------
                        //'  指定があるカラムのみセット
                        //'-------------------------------
                        string _TargetCol = "," + columnName.ToLower() + ",";
                        string _Col = "," + col.ColumnName.ToLower() + ",";
                        string _WhereCol = "," + whereColumn.ToLower() + ",";

                        if (_TargetCol.IndexOf(_Col) + 1 != 0)
                        {
                            if (_WhereCol.IndexOf(_Col) + 1 != 0)
                            {
                                if (whereSql != "") { whereSql += " and "; }
                                whereSql += col.ColumnName + "=" + ":P" + i.ToString();
                            }
                            else
                            {
                                wcol += col.ColumnName + ",";
                                wval += ":P" + i.ToString() + ",";
                            }

                            string paramName = "P" + i.ToString();
                            string type = col.DataType.ToString();

                            cmd.Parameters.Add(paramName, (NpgsqlTypes.NpgsqlDbType)GetNpgsqlDbType(type));

                            cmd.Parameters[paramName].Value = row[col.ColumnName];
                        }

                    }

                    wcol = wcol.Substring(0, (wcol.Length - 1));
                    wval = wval.Substring(0, (wval.Length - 1));
                    cmd.CommandText = " update " + tableName + " set (" + wcol + ") = ROW(" + wval + ") where " + whereSql + "; ";
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                string s = ex.Message + "\r\n" + " update " + tableName + "(" + wcol + ") = (" + wval + ");";
                TxtMsgBox _txt = new(s);
                _txt.ShowDialog();
            }
        }


        public void DbSelectInsert(DataTable dataTable, string pFromTableName, string pToTableName, string whereColumn)
        {
            NpgsqlCommand cmd = new();
            cmd.Connection = _connection;
            string whereSql = "";

            //入力カラムチェック
            try
            {
                string[] _cols = whereColumn.Split(',');
                foreach (var _col in _cols)
                {
                    Boolean _errFlg = true;
                    foreach (DataColumn _DtCol in dataTable.Columns)
                    {
                        if (_col.ToLower() == _DtCol.ColumnName.ToLower())
                        {
                            _errFlg = false;
                            break;
                        }
                    }
                    if (_errFlg == true)
                    {
                        MessageBox.Show("カラムがありません　whereColumn" + _col);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                //'-------------------------------
                //'ループでデータを取得
                //'-------------------------------
                foreach (DataRow row in dataTable.Rows)
                {

                    // -------------------------------
                    //  SELECT したカラムの一覧
                    // -------------------------------
                    cmd.Parameters.Clear();
                    int i = 0;

                    foreach (DataColumn col in dataTable.Columns)
                    {
                        i++;

                        //'-------------------------------
                        //'  Whereカラムセット
                        //'-------------------------------
                        string _Col = "," + col.ColumnName.ToLower() + ",";
                        string _WhereCol = "," + whereColumn.ToLower() + ",";

                        if (_WhereCol.IndexOf(_Col) + 1 != 0)
                        {
                            if (whereSql != "") { whereSql += " and "; }
                            whereSql += col.ColumnName + "=" + ":P" + i.ToString();

                            string paramName = "P" + i.ToString();
                            string type = col.DataType.ToString();

                            cmd.Parameters.Add(paramName, (NpgsqlTypes.NpgsqlDbType)GetNpgsqlDbType(type));
                            cmd.Parameters[paramName].Value = row[col.ColumnName];
                        }
                    }

                    cmd.CommandText = " insert into " + pToTableName + " select * from " + pFromTableName + " where " + whereSql + " ;";
                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                string s = ex.Message + "\r\n" + " insert into " + pToTableName + " select * from " + pFromTableName + " where " + whereSql + " ;";
                TxtMsgBox _txt = new(s);
                _txt.ShowDialog();
                //MessageBox.Show(ex.Message + "\r\n" + " update " + tableName + "(" + wcol + ") = (" + wval + ");");
            }
        }


        /// <summary>
        /// DBクローズ
        /// </summary>
        public void DbClose()
        {
            try
            {
                _connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// DataTable の全てのカラムをInsert
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        public void DbInsertAll(DataTable dataTable, string tableName)
        {
            try
            {
                string columnNames = "";

                // '-------------------------------
                // '  SELECT カラムの一覧作成
                // '-------------------------------
                foreach (DataColumn col in dataTable.Columns)
                {
                    columnNames += col.ToString() + ",";
                }
                columnNames = columnNames.Substring(0, columnNames.Length - 1);

                //'-------------------------------
                //' Insert 実行
                //'-------------------------------
                DbInsert(dataTable, columnNames, tableName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        public void DbUpdateAll(DataTable dataTable, string tableName, string whereColumn)
        {
            try
            {
                string columnNames = "";

                // '-------------------------------
                // '  SELECT カラムの一覧作成
                // '-------------------------------
                foreach (DataColumn col in dataTable.Columns)
                {
                    columnNames += col.ToString() + ",";
                }
                columnNames = columnNames.Substring(0, columnNames.Length - 1);

                //'-------------------------------
                //' Update 実行
                //'-------------------------------
                DbUpdate(dataTable, columnNames, tableName, whereColumn);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}




