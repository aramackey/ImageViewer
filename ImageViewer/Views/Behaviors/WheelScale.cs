using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageViewer.Views.Behaviors
{
    public class WheelScale : DependencyObject
    {
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }
        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(WheelScale), new UIPropertyMetadata(false, IsEnabledChanged));

        static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var img = d as Image;
            var scrollViewer = d as ScrollViewer;

            if ((bool)e.NewValue && img != null)
            {
                img.Loaded += img_Loaded;
            }
            else img_Unloaded(img, new RoutedEventArgs());

            if ((bool)e.NewValue && scrollViewer != null)
            {
                scrollViewer.Loaded += scrollViewer_Loaded;
            }
            else scrollViewer_Unloaded(scrollViewer, new RoutedEventArgs());
        }

        static void img_Loaded(object sender, RoutedEventArgs e)
        {
            var img = sender as Image;
            if (img == null) return;
            img.Unloaded += img_Unloaded;
            img.PreviewMouseWheel += imgScaleChange;
        }

        static void img_Unloaded(object sender, RoutedEventArgs e)
        {
            var img = sender as Image;
            if (img == null) return;
            img.Loaded -= img_Loaded;
            img.Unloaded -= img_Unloaded;
        }

        static void scrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;
            scrollViewer.Unloaded += scrollViewer_Unloaded;
            scrollViewer.ScrollChanged += scrollViewer_keepCentre;
        }

        static void scrollViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;
            scrollViewer.Loaded -= scrollViewer_Loaded;
            scrollViewer.Unloaded -= scrollViewer_Unloaded;
        }

        // Change scale displaied image when mouse wheel has been changed.
        static double total_scale = 1.0;
        static void imgScaleChange(object sender, MouseWheelEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                double scale = (0 < e.Delta) ? 1.1 : (1.0 / 1.1);
                total_scale = total_scale * scale;
                img.LayoutTransform = new ScaleTransform(total_scale, total_scale, e.GetPosition(img).X, e.GetPosition(img).Y);
                e.Handled = true;
            }
        }
        
        // Keep center mouse pointer when scaling has been changed.
        static void scrollViewer_keepCentre(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            // Scroll control when only zoom facter has been changed.
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                double magX = e.ExtentWidth/(e.ExtentWidth - e.ExtentWidthChange);
                double magY = e.ExtentHeight/(e.ExtentHeight - e.ExtentHeightChange) ;

                /*
                System.Diagnostics.Debug.WriteLine("===== DEBUG INFORMATION =====");
                System.Diagnostics.Debug.WriteLine("Magnification: {0}/{1}",mag_X,mag_Y);
                System.Diagnostics.Debug.WriteLine("Mouse.GetPosition: X-{0} / Y-{1}", Mouse.GetPosition(scrollViewer).X, Mouse.GetPosition(scrollViewer).Y);
                System.Diagnostics.Debug.WriteLine("e.ViewportHeight-e.ViewportHeightChange: {0} - {1}", e.ViewportHeight, e.ViewportHeightChange);
                System.Diagnostics.Debug.WriteLine("e.ExtentHeight: {0}\n", e.ExtentHeight);
                */

                double PointX = e.HorizontalOffset - e.HorizontalChange + Mouse.GetPosition(scrollViewer).X;
                double PointY = e.VerticalOffset - e.VerticalChange + Mouse.GetPosition(scrollViewer).Y;

                double newPointX = PointX * magX;
                double newPointY = PointY * magY;

                double newOffsetX = newPointX - Mouse.GetPosition(scrollViewer).X;
                double newOffsetY = newPointY - Mouse.GetPosition(scrollViewer).Y;

                scrollViewer.ScrollToHorizontalOffset(Math.Max(newOffsetX, 0));
                scrollViewer.ScrollToVerticalOffset(Math.Max(newOffsetY,0));
            }
        }
    }
}
