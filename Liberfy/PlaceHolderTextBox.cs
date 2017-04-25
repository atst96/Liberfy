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
	class PlaceHolderTextBox : TextBox
	{
		public PlaceHolderTextBox() : base()
		{
		}

		public string PlaceHolder
		{
			get { return (string)GetValue(PlaceHolderProperty); }
			set { SetValue(PlaceHolderProperty, value); }
		}

		public bool IsPlaceHolderShown
		{
			get { return (bool)GetValue(IsPlaceHolderShownProperty); }
			private set { SetValue(IsPlaceHolderShownPropertyKey, value); }
		}

		public Brush PlaceHolderForeground
		{
			get { return (Brush)GetValue(PlaceHolderForegroundProperty); }
			set { SetValue(PlaceHolderForegroundProperty, value); }
		}

		public static readonly DependencyProperty PlaceHolderProperty =
			DependencyProperty.Register("PlaceHolder",
				typeof(string), typeof(PlaceHolderTextBox),
				new FrameworkPropertyMetadata(null));

		public static readonly DependencyProperty PlaceHolderForegroundProperty =
			DependencyProperty.Register("PlaceHolderForeground",
				typeof(Brush), typeof(PlaceHolderTextBox),
				new PropertyMetadata(Brushes.Gray));

		private static readonly DependencyPropertyKey IsPlaceHolderShownPropertyKey
			= DependencyProperty.RegisterReadOnly(
				"IsPlaceHolderShown",
				typeof(bool), typeof(PlaceHolderTextBox),
				new PropertyMetadata(true));

		public static readonly DependencyProperty IsPlaceHolderShownProperty
			= IsPlaceHolderShownPropertyKey.DependencyProperty;

		protected virtual void OnPlaceHolderShownChanged(bool newValue) { }

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			bool newIsPlaceHolderShown = string.IsNullOrEmpty(Text);

			if (!Equals(newIsPlaceHolderShown, IsPlaceHolderShown))
			{
				IsPlaceHolderShown = newIsPlaceHolderShown;
				OnPlaceHolderShownChanged(newIsPlaceHolderShown);
			}

			base.OnTextChanged(e);
		}
	}
}
