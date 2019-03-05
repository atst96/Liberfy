using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Liberfy
{
    internal static class WpfExtensions
    {
        public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(GetParentVisual(obj));

            return parent == null ? default : parent as T ?? parent.FindAncestor<T>();
        }

        public static T FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(obj);

                for (int i = 0; i < childrenCount; ++i)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T t_Cihld)
                    {
                        return t_Cihld;
                    }

                    var grandchild = child.FindVisualChild<T>();
                    if (grandchild != null)
                    {
                        return grandchild;
                    }
                }
            }

            return default;
        }

        public static DependencyObject GetParentVisual(this DependencyObject obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is Visual)
            {
                return obj;
            }

            var parent = LogicalTreeHelper.GetParent(obj);

            return parent == null ? default : parent as Visual ?? parent.GetParentVisual();
        }

        public static void SaveToStream<T>(this BitmapSource bitmapSource, Stream stream) where T : BitmapEncoder
        {
            var encoder = Activator.CreateInstance<T>();
            var frame = BitmapFrame.Create(bitmapSource);

            encoder.Frames.Add(frame);
            encoder.Save(stream);
            encoder = null;
        }
    }
}
