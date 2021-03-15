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

namespace WPFImgApp.ViewModel
{
    class ImageVM : BaseViewModel
    {
        private string _Name;
        private BitmapImage _bitmap;
        public BitmapImage Bitmap
        {
            set
            {
                _bitmap = value;
                OnPropertyChanged(nameof(Bitmap));
            }
            get => _bitmap;
        }

        public MainWindowVM Parent { set; get; }



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
                    var b = ImageOperations.imageToBytes(Bitmap);
                    b = b.Select(x => (byte) (x * 0.5)).ToArray();
                    Bitmap = ImageOperations.bytesToBitmap(b,Bitmap);
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
