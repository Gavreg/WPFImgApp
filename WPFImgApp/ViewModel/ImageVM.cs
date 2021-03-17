using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFImgApp.Models;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace WPFImgApp.ViewModel
{
    class ImageVM : BaseViewModel
    {
        private string _Name;
        private BitmapSource _bitmap;
        private double _opacity = 1;
        private PerPixelOperation _selectedOperation = PerPixelOperation.getOperationsList()[0];

        public byte[] Bytes { private set; get; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }



        public PerPixelOperation SelectedOperation
        {
            get => _selectedOperation;
            set
            {
                _selectedOperation = value;
                OnPropertyChanged(nameof(SelectedOperation));
            }
        }

        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }

        public BitmapSource Bitmap
        {
            set
            {

                FormatConvertedBitmap converted = new FormatConvertedBitmap();
                converted.BeginInit();
                converted.Source = value;
                converted.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
                converted.EndInit();

                int stride = (int)converted.PixelWidth * (converted.Format.BitsPerPixel / 8);
                byte[] b = new byte[converted.PixelHeight * stride];
                converted.CopyPixels(b, stride, 0);



                _bitmap = new BitmapImage();
                Bytes = b;
                var src = BitmapSource.Create(converted.PixelWidth, converted.PixelHeight, 96, 96, converted.Format,
                    converted.Palette, b, stride);
                _bitmap = src;

                Width = src.PixelWidth;
                Height = src.PixelHeight;

                OnPropertyChanged(nameof(Bitmap));
            }
            get => _bitmap;
        }


        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private ICommand _doSomething;
       public ICommand DoSomething
        {
            get
            {
                return _doSomething ??= new RelayCommand(p => true, (obj) =>
                {

                });
            }
        }

        ~ImageVM()
        {
            //Bitmap.Dispose();
        }
    }

    internal class Bitmaps
    {
    }
}
