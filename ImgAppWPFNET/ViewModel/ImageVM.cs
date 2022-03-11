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
using ImgAppWPFNET.Models;

namespace ImgAppWPFNET.ViewModel
{
    class ImageVM : BaseViewModel
    {
        private string _Name;
        private BitmapSource _bitmap;
        private double _opacity = 1;
        private PerPixelOperation _selectedOperation = PerPixelOperation.getOperationsList()[0];
        private int _offsetX;
        private int _offsetY;
        private bool _r = true;
        private bool _g = true;
        private bool _b = true;
        private string _sizeString = "";

        public bool R
        {
            get => _r;
            set
            {
                _r = value;
                OnPropertyChanged(nameof(R));
            }
        }
        public bool G
        {
            get => _g;
            set
            {
                _g = value;
                OnPropertyChanged(nameof(G));
            }
        }
        public bool B
        {
            get => _b;
            set
            {
                _b = value;
                OnPropertyChanged(nameof(B));
            }
        }

        public string SizeString
        {
            get => _sizeString;
            set
            {
                _sizeString = value;
                OnPropertyChanged(nameof(SizeString));
            }
        }

        public byte[] Bytes { private set; get; }

        public int OffsetX
        {
            get => _offsetX;
            set
            {
                _offsetX = value;
                OnPropertyChanged(nameof(OffsetX));
            }
        }

        public int OffsetY
        {
            get => _offsetY;
            set
            {
                _offsetY = value;
                OnPropertyChanged(nameof(OffsetY));
            }
        }
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
                var src = BitmapSource.Create(converted.PixelWidth, converted.PixelHeight, converted.DpiX, converted.DpiY, converted.Format,
                    converted.Palette, b, stride);
                _bitmap = src;

                Width = src.PixelWidth;
                Height = src.PixelHeight;
                SizeString = $"{Width}x{Height} DPI: {Math.Round(converted.DpiX,0)}x{Math.Round(converted.DpiY,0)}";

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
