using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFImgApp.Models;

namespace WPFImgApp.ViewModel
{
    class MainWindowVM : BaseViewModel
    {
        private BitmapSource _selectedImage;
        public ObservableCollection<ImageVM> Bitmaps { set; get; }
        private ICommand moveUpCommand;
        private ICommand moveDownCommand;
        private ICommand deleteCommand;
        
        private int _blendTime;
        public int BlendTime
        {
            get => _blendTime;
            set
            {
                _blendTime = value;
                OnPropertyChanged(nameof(BlendTime));
            }
        }

        public List<PerPixelOperation> OperationsList => PerPixelOperation.getOperationsList();

        public ICommand MoveUpCommand
        {
            get
            {
                return moveUpCommand ??= new RelayCommand(_canMoveUp, (obj) =>
                {
                    
                    var param = obj as ImageVM;
                    var ii = Bitmaps.IndexOf(param);
                    Bitmaps.Remove(param);
                    Bitmaps.Insert(ii - 1, param);

                });
            }
        }

        public ICommand MoveDownCommand
        {
            get
            {
                return moveDownCommand ??= new RelayCommand(_canMoveDown, (obj) =>
                {
                   
                    var param = obj as ImageVM;
                    var ii = Bitmaps.IndexOf(param);
                    Bitmaps.Remove(param);
                    Bitmaps.Insert(ii + 1, param);

                });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ??= new RelayCommand(t => true, (obj) => Bitmaps.Remove(obj as ImageVM));
            }
        }

        private readonly Predicate<object> _canMoveUp;
        private readonly Predicate<object> _canMoveDown;

        public void AddImageVM(ImageVM vm)
        {
            vm.PropertyChanged += (s,prop_name) =>
            {
                if (prop_name.PropertyName == nameof(ImageVM.Opacity) ||
                    prop_name.PropertyName == nameof(ImageVM.SelectedOperation) )
                {
                    CalculateLayers();
                }
            };
            Bitmaps.Add(vm);
        }

        public BitmapSource SelectedImage
        {
            get => _selectedImage;
            set
            {
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }

        public MainWindowVM()
        {
            Bitmaps = new ObservableCollection<ImageVM>();
            _canMoveDown = (obj) => Bitmaps.LastOrDefault() != (obj as ImageVM);
            _canMoveUp = (obj) => Bitmaps.FirstOrDefault() != (obj as ImageVM);
            Bitmaps.CollectionChanged += (s, a) =>
            {
               // CalculateLayers();
            };
        }

        public void CalculateLayers()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            int max_width = Bitmaps.Max(x => x.Width);
            int max_height = Bitmaps.Max(x => x.Height);
            byte[] bytes = new byte[max_width * max_height * 4];
            Parallel.For(0, max_width * max_height, (i) =>
            {
                int y = i / max_width;
                int x = i - y * max_width;


                int _x = x - Bitmaps[0].OffsetX;
                int _y = y - Bitmaps[0].OffsetY;
                int _i = _y * Bitmaps[0].Width + _x;

                /*if (_x > 0 && _x < Bitmaps[0].Width && _y > 0 && _y < Bitmaps[0].Height)
                {
                    bytes[4 * i + 0] = Bitmaps[0].Bytes[4 * _i + 0];
                    bytes[4 * i + 1] = Bitmaps[0].Bytes[4 * _i + 1];
                    bytes[4 * i + 2] = Bitmaps[0].Bytes[4 * _i + 2];
                    bytes[4 * i + 3] = Bitmaps[0].Bytes[4 * _i + 3];

                }*/
                for (int j = 0; j < Bitmaps.Count; ++j)
                {
                    _x = x - Bitmaps[j].OffsetX;
                    _y = y - Bitmaps[j].OffsetY;
                    _i = _y * Bitmaps[j].Width + _x;

                    if (_x > 0 && _x < Bitmaps[j].Width && _y > 0 && _y < Bitmaps[j].Height)
                        for (int c = 0; c <= 3; c++)
                           bytes[i * 4 + c] = Bitmaps[j].SelectedOperation.ByteOperation(Bitmaps[j].Bytes[_i * 4 + c], bytes[i * 4 + c], Bitmaps[j].Opacity);
                        
                }//j++
            });


            //WriteableBitmap wb = new WriteableBitmap(max_width, max_height, 96, 96, PixelFormats.Bgra32,
            //    null);
            //wb.Lock();
            //Marshal.Copy(bytes, 0, wb.BackBuffer, bytes.Length);
            //wb.AddDirtyRect(new Int32Rect(0,0,max_width,max_height));
            //wb.Unlock();
            //SelectedImage = wb;

            
            var bs = BitmapSource.Create(max_width, max_height, 96, 96, PixelFormats.Bgra32, null, bytes,
                max_width * PixelFormats.Bgra32.BitsPerPixel / 8);
            SelectedImage = bs;
            bs.Freeze();

            stopWatch.Stop();
            BlendTime = (int)stopWatch.ElapsedMilliseconds;
            


        }
    }
}
