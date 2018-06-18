using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Liberfy
{
	internal static class WpfExtensions
	{
		public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
		{
			var parentVisual = GetParentVisual(obj);

			var parent = VisualTreeHelper.GetParent(GetParentVisual(obj));

			if (parent != null)
			{
				return parent as T ?? FindAncestor<T>(parent);
			}

			return default(T);
		}

		public static T FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
		{
			if (obj != null)
			{
				int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
				for (int i = 0; i < childrenCount; i++)
				{
					var childVisual = VisualTreeHelper.GetChild(obj, i);
					if (childVisual is T castedChild)
					{
						return castedChild;
					}

					var foundChild = FindVisualChild<T>(childVisual);
					if (foundChild != null)
					{
						return foundChild;
					}
				}
			}

			return default(T);
		}

		public static DependencyObject GetParentVisual(this DependencyObject obj)
		{
			if (obj == null) return null;
			if (obj is Visual) return obj;

			var parentVisual = LogicalTreeHelper.GetParent(obj);

			if (parentVisual != null)
			{
				return parentVisual as Visual ?? GetParentVisual(parentVisual);
			}

			return default(DependencyObject);
		}

		public static T FindFromTemplate<T>(this Control element, string name) where T : FrameworkElement
		{
			return element?.Template.FindName(name, element) as T;
		}
	}
}
