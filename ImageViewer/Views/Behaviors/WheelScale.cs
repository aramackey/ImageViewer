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
            if (img == null) return;

            if ((bool)e.NewValue)
            {
                img.Loaded += img_Loaded;
            }
            else img_Unloaded(img, new RoutedEventArgs());
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
    }
}
