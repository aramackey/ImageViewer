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
                System.Diagnostics.Debug.WriteLine("----------------->img Loaded;");
                img.Loaded += img_Loaded;
            }
            else img_Unloaded(img, new RoutedEventArgs());

            if ((bool)e.NewValue && scrollViewer != null)
            {
                System.Diagnostics.Debug.WriteLine("----------------->scrollViewer Loaded;");
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
        static int num = 0;
        static void scrollViewer_keepCentre(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            System.Diagnostics.Debug.WriteLine("scrollViewer_keepCentre Method was called. count: {0}", num);
            num++;

            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                double xMousePositionOnScrollViewer = Mouse.GetPosition(scrollViewer).X;
                double yMousePositionOnScrollViewer = Mouse.GetPosition(scrollViewer).Y;
                double offsetX = e.HorizontalOffset + xMousePositionOnScrollViewer;
                double offsetY = e.HorizontalOffset + yMousePositionOnScrollViewer;

                double oldExtentWidth = e.ExtentWidth - e.ExtentWidthChange;
                double oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;

                double relx = offsetX / oldExtentWidth;
                double rely = offsetY / oldExtentHeight;

                offsetX = Math.Max(relx * e.ExtentWidth - xMousePositionOnScrollViewer, 0);
                offsetY = Math.Max(rely * e.ExtentWidth - yMousePositionOnScrollViewer, 0);

                ScrollViewer scrollViewerTemp = sender as ScrollViewer;
                scrollViewerTemp.ScrollToHorizontalOffset(offsetX);
                scrollViewerTemp.ScrollToVerticalOffset(offsetY);
            }
        }
    }
}
