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
using ImgAppWPFNET.Models;
using ImgAppWPFNET.Util;


namespace ImgAppWPFNET.ViewModel
{
    class MainWindowVM : BaseViewModel
    {
        private BitmapSource _selectedImage;
        private ObservableCollection<Task> operations = new ObservableCollection<Task>();
        private BitmapSource _canvas;
        private byte[] canvas_bytes;
        private string _ImageSizeString = "0x0";

        private ICommand moveUpCommand;
        private ICommand moveDownCommand;
        private ICommand deleteCommand;
        private SimpleTasksQueue tasks = new SimpleTasksQueue();


        public ObservableCollection<ImageVM> Bitmaps { set; get; }
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

        public string ImageSizeString
        {
            get => _ImageSizeString;
            set
            {
                _ImageSizeString = value;
                OnPropertyChanged(nameof(ImageSizeString));
            }
        }

        public Visibility isEmpty => (Bitmaps.Count == 0) ? Visibility.Visible : Visibility.Hidden;
           

        public List<PerPixelOperation> OperationsList => PerPixelOperation.getOperationsList();


        public ICommand MoveUpCommand
        {
            get
            {
                return moveUpCommand ??= new RelayCommand(_canMoveUp, (obj) =>
                {
                    var param = obj as ImageVM;
                    var ii = Bitmaps.IndexOf(param);
                    Bitmaps.Move(ii, ii - 1);
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
                    Bitmaps.Move(ii,ii+1);
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
                if (prop_name.PropertyName == nameof(ImageVM.Opacity)
                    || prop_name.PropertyName == nameof(ImageVM.SelectedOperation)
                    || prop_name.PropertyName == nameof(ImageVM.OffsetX) 
                    || prop_name.PropertyName == nameof(ImageVM.OffsetY)
                    || prop_name.PropertyName == nameof(ImageVM.R)
                    || prop_name.PropertyName == nameof(ImageVM.G)
                    || prop_name.PropertyName == nameof(ImageVM.B)   )
                {
                    tasks.AddTask(new Task(CalculateLayers));
                }
            };
            tasks.Wait();
            Bitmaps.Add(vm);
        }

        public BitmapSource SelectedImage
        {
            get => _canvas;
            set
            {
                _canvas = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }

        public MainWindowVM()
        {
            tasks.StartQueue();
            Bitmaps = new ObservableCollection<ImageVM>();
            _canMoveDown = (obj) => Bitmaps.LastOrDefault() != (obj as ImageVM);
            _canMoveUp = (obj) => Bitmaps.FirstOrDefault() != (obj as ImageVM);
            Bitmaps.CollectionChanged += (s, a) =>
            {
                if (Bitmaps.Count != 0)
                {
                    updateCanvas();
                    tasks.AddTask(new Task(CalculateLayers));
                    
                }
                else
                {
                    SelectedImage = null;
                }
                OnPropertyChanged(nameof(isEmpty));

            };
        }

        private void updateCanvas()
        {
            int max_width = Bitmaps.Max(x => x.Width);
            int max_height = Bitmaps.Max(x => x.Height);
            //canvas_bytes = new byte[max_width * max_height * 4];
            _canvas = new WriteableBitmap(max_width, max_height, 96, 96, PixelFormats.Bgra32, null);
        }

        byte[] bytes = Array.Empty<byte>();

        public unsafe void CalculateLayers()
        {
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var props = (from b in Bitmaps
                select new { offX = b.OffsetX, 
                    offY = b.OffsetY, 
                    op = b.Opacity, 
                    w = b.Width, 
                    h = b.Height, 
                    so = b.SelectedOperation, 
                    r = b.R, 
                    g = b.G, 
                    b = b.B
                }).ToArray();

            TimeSpan ts = stopWatch.Elapsed;
            int max_width = Bitmaps.Max(x => x.Width);
            int max_height = Bitmaps.Max(x => x.Height);
            ImageSizeString = $"{max_width} x {max_height}";
            if (bytes.Length != max_width * max_height * 4)
                bytes = new byte[max_width * max_height * 4];
            else
               Array.Clear(bytes,0,bytes.Length);
            

            Parallel.For(0, max_width * max_height,  (i) =>
                {
                    int y = i / max_width;
                    int x = i - y * max_width;

                    int _x = x - props[0].offX;
                    int _y = y - props[0].offY;
                    int _i = _y * props[0].w + _x;

                    for (int j = 0; j < props.Length; ++j)
                    {
                        _x = x - props[j].offX;
                        _y = y - props[j].offY;
                        _i = _y * props[j].w + _x;

                        var channels = new(int offset, bool enabled)[] { 
                                (0, props[j].b), 
                                (1, props[j].g),
                                (2,props[j].r), 
                                /*A:*/(3, true)
                        };

                        if (_x > 0 && _x < props[j].w && _y > 0 && _y < props[j].h)
                        {
                            foreach (var c in channels.Where( x=> x.enabled))
                            {
                                byte tmp = (byte)(props[j].so.ByteOperation(Bitmaps[j].Bytes[_i * 4 + c.offset],
                                                                            bytes[i * 4 + c.offset]));
                                bytes[i * 4 + c.offset] = (byte)((byte)(tmp * props[j].op) +
                                                                 (byte)(bytes[i * 4 + c.offset] * (1 -  props[j].op)));
                            }
                        }
  
                    }//j++
                });

            BitmapSource so = BitmapSource.Create(max_width,max_height,96,96,PixelFormats.Bgra32,null,bytes,max_width*4);
            so.Freeze();
            SelectedImage = so;

            stopWatch.Stop();
            BlendTime = (int)stopWatch.ElapsedMilliseconds;
        }
    }
}
