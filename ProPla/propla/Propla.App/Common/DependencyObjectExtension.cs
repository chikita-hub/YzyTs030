using System;
using System.Windows;
using System.Windows.Media;

namespace Propla
{
    /// <summary>
    /// DependencyObject（WPF要素）の子要素を走査する拡張メソッド。
    /// 視覚ツリー（VisualTree）を優先し、取得できない場合は論理ツリー（LogicalTree）をフォローします。
    /// </summary>
    public static class DependencyObjectExtension
    {
        /// <summary>
        /// 再帰的に子要素へ降りながら <paramref name="action"/> を適用します。
        /// </summary>
        /// <param name="obj">起点となる <see cref="DependencyObject"/>。</param>
        /// <param name="action">各ノードで実行する処理。</param>
        private static void Walk(DependencyObject obj, Action<DependencyObject> action)
        {
            if (obj is null) return;

            // 自身に対してまず実行
            action(obj);

            // 1) 視覚ツリーを優先して子要素をたどる
            int visualCount = 0;
            try
            {
                visualCount = VisualTreeHelper.GetChildrenCount(obj);
            }
            catch
            {
                // Visual でない要素などは例外になり得るため握りつぶし（後段の Logical へ）
                visualCount = 0;
            }

            if (visualCount > 0)
            {
                for (int i = 0; i < visualCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child is DependencyObject d)
                    {
                        Walk(d, action);
                    }
                }
                return;
            }

            // 2) 視覚ツリーで子が得られない場合は論理ツリーをたどる
            foreach (var child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child is DependencyObject d)
                {
                    Walk(d, action);
                }
            }
        }

        /// <summary>
        /// <paramref name="obj"/> を起点にツリーを走査し、全ての子（自身を含む）に対して <paramref name="action"/> を実行します。
        /// </summary>
        /// <param name="obj">this で受ける <see cref="DependencyObject"/>。</param>
        /// <param name="action">各ノードで実行する処理。</param>
        public static void WalkInChildren(this DependencyObject obj, Action<DependencyObject> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            Walk(obj, action);
        }
    }
}
