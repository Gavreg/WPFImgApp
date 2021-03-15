using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFImgApp.Models;

namespace WPFImgApp.ViewModel
{
    class MainWindowVM : BaseViewModel
    {
        private ImageVM _selectedImage;
        public ObservableCollection<ImageVM> Bitmaps { set; get; }
        private ICommand moveUpCommand;
        private ICommand moveDownCommand;
        private ICommand deleteCommand;

        public ICommand MoveUpCommand
        {
            get
            {
                return moveUpCommand ??= new RelayCommand(_canMoveUp, (obj) =>
                {
                    var sel = SelectedImage;
                    var param = obj as ImageVM;
                    var ii = Bitmaps.IndexOf(param);
                    Bitmaps.Remove(param);
                    Bitmaps.Insert(ii - 1, param);
                    if (sel == param)
                    {
                        SelectedImage = param;
                    }
                });
            }
        }

        public ICommand MoveDownCommand
        {
            get
            {
                return moveDownCommand ??= new RelayCommand(_canMoveDown, (obj) =>
                {
                    var sel = SelectedImage;
                    var param = obj as ImageVM;
                    var ii = Bitmaps.IndexOf(param);
                    Bitmaps.Remove(param);
                    Bitmaps.Insert(ii + 1, param);
                    if (sel == param)
                    {
                        SelectedImage = param;
                    }
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

        public ImageVM SelectedImage
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
        }
    }
}
