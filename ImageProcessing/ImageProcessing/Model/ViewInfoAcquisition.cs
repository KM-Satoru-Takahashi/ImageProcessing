using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ImageProcessing.Model
{
    /// <summary>
    /// Viewにおける座標等の情報を取得するクラス
    /// </summary>
    internal class ViewInfoAcquisition
    {

        internal Point GetMousePosition()
        {
            return new Point();
        }


        /// <summary>
        /// 渡されたパラメタの親から対象要素を取得する
        /// </summary>
        /// <typeparam name="T">xamlコントロール要素(button, canvas等)</typeparam>
        /// <param name="d">VMが受け取ったパラメタ</param>
        /// <returns>対象要素があれば当該要素、なければnull</returns>
        private T FindVisualParent<T>(DependencyObject d) where T : DependencyObject
        {
            if (d == null) return null;

            try
            {
                // 一つ上の親要素を取得する
                DependencyObject root = VisualTreeHelper.GetParent(d);

                // 目的と一致すれば返す
                if (root != null && root is T)
                {
                    return root as T;
                }
                else
                {
                    // 目的と違う場合は再帰的に処理する
                    T parent = FindVisualParent<T>(root);
                    if (parent != null) return parent;
                }

                return null;
            }
            catch
            {
                if (d is FrameworkElement)
                {
                    FrameworkElement element = (FrameworkElement)d;
                    if (element.Parent is T) return element.Parent as T;
                    return FindVisualParent<T>(element.Parent);
                }
                else if (d is FrameworkContentElement)
                {
                    FrameworkContentElement element = (FrameworkContentElement)d;
                    if (element.Parent is T) return element.Parent as T;
                    return FindVisualParent<T>(element.Parent);
                }
                else
                {
                    return null;
                }
            }

        }
}
