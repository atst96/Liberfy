using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    static class ElementBehavior
    {
        public static bool GetVisible(DependencyObject obj) => (bool)obj.GetValue(VisibleProperty);

        public static void SetVisible(DependencyObject obj, bool value) => obj.SetValue(VisibleProperty, value);

        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.RegisterAttached("Visible",
                typeof(bool), typeof(ElementBehavior),
                new PropertyMetadata(true,
                    (obj, args) => obj.SetValue(UIElement.VisibilityProperty, (bool)args.NewValue ? Visibility.Visible : Visibility.Collapsed)));


        public static bool GetInvisible(DependencyObject obj) => (bool)obj.GetValue(InvisibleProperty);

        public static void SetInvisible(DependencyObject obj, bool value) => obj.SetValue(InvisibleProperty, value);

        public static readonly DependencyProperty InvisibleProperty =
            DependencyProperty.RegisterAttached("Invisible",
                typeof(bool), typeof(ElementBehavior),
                new PropertyMetadata(false,
                    (obj, args) => obj.SetValue(UIElement.VisibilityProperty, (bool)args.NewValue ? Visibility.Collapsed : Visibility.Visible)));


        public static bool GetHidden(DependencyObject obj) => (bool)obj.GetValue(HiddenProperty);

        public static void SetHidden(DependencyObject obj, bool value) => obj.SetValue(HiddenProperty, value);

        public static readonly DependencyProperty HiddenProperty =
            DependencyProperty.RegisterAttached("Hidden",
                typeof(bool), typeof(ElementBehavior),
                new PropertyMetadata(false,
                    (obj, args) => obj.SetValue(UIElement.VisibilityProperty, (bool)args.NewValue ? Visibility.Hidden : Visibility.Visible)));


        public static bool GetShow(DependencyObject obj) => (bool)obj.GetValue(dp: ShowProperty);

        public static void SetShow(DependencyObject obj, bool value) => obj.SetValue(ShowProperty, value);

        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.RegisterAttached("Show",
                typeof(bool), typeof(ElementBehavior),
                new PropertyMetadata(true,
                    (obj, args) => obj.SetValue(UIElement.VisibilityProperty, (bool)args.NewValue ? Visibility.Visible : Visibility.Hidden)));


        public static object GetCollapseIsNull(DependencyObject obj) => (bool)obj.GetValue(CollapseIsNullProperty);

        public static void SetCollapseIsNull(DependencyObject obj, bool value) => obj.SetValue(CollapseIsNullProperty, value);

        public static readonly DependencyProperty CollapseIsNullProperty =
            DependencyProperty.RegisterAttached("CollapseIsNull",
                typeof(object), typeof(ElementBehavior),
                new PropertyMetadata(null,
                    (obj, args) => obj.SetValue(UIElement.VisibilityProperty, args.NewValue == null ? Visibility.Collapsed : Visibility.Visible)));



        public static string GetWebBrowserVideoPlayerUrl(DependencyObject obj)
        {
            return (string)obj.GetValue(WebBrowserVideoPlayerUrlProperty);
        }

        public static void SetWebBrowserVideoPlayerUrl(DependencyObject obj, string value)
        {
            obj.SetValue(WebBrowserVideoPlayerUrlProperty, value);
        }

        public static readonly DependencyProperty WebBrowserVideoPlayerUrlProperty =
            DependencyProperty.RegisterAttached("WebBrowserVideoPlayerUrl",
                typeof(string), typeof(ElementBehavior),
                new PropertyMetadata(null, OnWebBrowserVideoPlayerUrlChanged));

        private static void OnWebBrowserVideoPlayerUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is WebBrowser webBrowser))
                throw new NotImplementedException();

            if (e.NewValue is string videoUrl)
            {
                var videoPlayerTemplate = Properties.Resources.VideoPlayerTemplate;
                var html = videoPlayerTemplate.Replace("{%video_url%}", WebUtility.HtmlEncode(videoUrl));

                webBrowser.NavigateToString(html);
            }
            else
            {
                webBrowser.Navigate("about:blank");
            }
        }
    }
}
