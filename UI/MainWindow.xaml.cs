using System.Windows;
using Core.ViewModel;
using Microsoft.Win32;

namespace UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += WindowLoaded;
        }

        public void WindowLoaded(object sender, RoutedEventArgs args)
        {
            var vm = new Core.ViewModel.MainViewModel();
            this.DataContext = vm;
            vm.ShowCers +=
                (o, eventArgs) =>
                {
                    ViewShower.Show(new CertificatesWindow {DataContext = new CertificatesViewModel()}, true, (b) => { });
                };

            vm.OpenOriginalPath += OpenOriginalFile;
        }

        private void OpenOriginalFile(object sender, string title)
        {
            var ofd = new OpenFileDialog {Title = title};
            if (ofd.ShowDialog(this) == true)
            {
                var mainViewModel = DataContext as MainViewModel;
                if (mainViewModel != null) mainViewModel.OriginalPath = ofd.FileName;
            }
        }
    }
}
