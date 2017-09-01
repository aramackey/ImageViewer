using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;

namespace ImageViewer.ViewModels
{
    class ImageViewer : BindableBase
    {
        private BitmapSource bitmap;
        public BitmapSource Bitmap
        {
            get { return bitmap; }
            set { this.SetProperty(ref this.bitmap, value); }
        }

        public ImageViewer()
        {

        }
    }
}
