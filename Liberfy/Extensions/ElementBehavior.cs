using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy
{
	static class ElementBehavior
	{
		public static bool GetVisible(DependencyObject obj)
		{
			return (bool)obj.GetValue(VisibleProperty);
		}

		public static void SetVisible(DependencyObject obj, bool value)
		{
			obj.SetValue(VisibleProperty, value);
		}

		public static bool GetInvisible(DependencyObject obj)
		{
			return (bool)obj.GetValue(InvisibleProperty);
		}

		public static void SetInvisible(DependencyObject obj, bool value)
		{
			obj.SetValue(InvisibleProperty, value);
		}

		public static bool GetHidden(DependencyObject obj)
		{
			return (bool)obj.GetValue(HiddenProperty);
		}

		public static void SetHidden(DependencyObject obj, bool value)
		{
			obj.SetValue(HiddenProperty, value);
		}

		public static bool GetShow(DependencyObject obj)
		{
			return (bool)obj.GetValue(ShowProperty);
		}

		public static void SetShow(DependencyObject obj, bool value)
		{
			obj.SetValue(ShowProperty, value);
		}


		public static readonly DependencyProperty VisibleProperty =
			DependencyProperty.RegisterAttached("Visible",
				typeof(bool), typeof(ElementBehavior), 
				new PropertyMetadata(true, VisibleChanged));

		public static readonly DependencyProperty InvisibleProperty =
			DependencyProperty.RegisterAttached("Invisible",
				typeof(bool), typeof(ElementBehavior),
				new PropertyMetadata(false, InvisibleChanged));

		public static readonly DependencyProperty HiddenProperty =
			DependencyProperty.RegisterAttached("Hidden",
				typeof(bool), typeof(ElementBehavior), 
				new PropertyMetadata(false, HiddenChanged));

		public static readonly DependencyProperty ShowProperty =
			DependencyProperty.RegisterAttached("Show", 
				typeof(bool), typeof(ElementBehavior),
				new PropertyMetadata(true, ShowChanged));


		private static void VisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.SetValue(UIElement.VisibilityProperty, (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed);
		}


		private static void InvisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.SetValue(UIElement.VisibilityProperty, (bool)e.NewValue ? Visibility.Collapsed : Visibility.Visible);
		}

		private static void HiddenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.SetValue(UIElement.VisibilityProperty, (bool)e.NewValue ? Visibility.Hidden : Visibility.Visible);
		}

		private static void ShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.SetValue(UIElement.VisibilityProperty, (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden);
		}
	}
}
