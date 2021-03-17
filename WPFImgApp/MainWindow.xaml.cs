using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFImgApp.ViewModel;

namespace WPFImgApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVM vm;
        public MainWindow()
        {
            InitializeComponent();
            //var u = new Uri(new Uri(System.AppDomain.CurrentDomain.BaseDirectory), new Uri(@".\.\1.png"));
            vm = new MainWindowVM();
            DataContext = vm;


            vm.AddImageVM((new ImageVM { Bitmap = new BitmapImage(new Uri(@"C:\Users\gavre\source\repos\WPFImgApp\WPFImgApp\img\1.png")), Name = "адын" }));
            vm.AddImageVM((new ImageVM { Bitmap = new BitmapImage(new Uri(@"C:\Users\gavre\source\repos\WPFImgApp\WPFImgApp\img\2.png")), Name = "два" }));
            vm.AddImageVM((new ImageVM { Bitmap = new BitmapImage(new Uri(@"C:\Users\gavre\source\repos\WPFImgApp\WPFImgApp\img\3.png")), Name = "тхри" }));
            vm.AddImageVM((new ImageVM { Bitmap = new BitmapImage(new Uri(@"C:\Users\gavre\source\repos\WPFImgApp\WPFImgApp\img\4.png")), Name = "чятыри" }));
            
           
            this.AllowDrop = true;
            this.Drop += (s, a) =>
            {
                string[] files = (string[])a.Data.GetData(DataFormats.FileDrop);
                foreach (var f in files)
                {
                    FileInfo fi = new FileInfo(f);
                    vm.AddImageVM((new ImageVM { Bitmap = new BitmapImage(new Uri(f)), Name = fi.Name}));
                }
            };

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task t = new Task( () =>
                {
                   vm.CalculateLayers();
                });
                t.RunSynchronously();
        }
    }
}
