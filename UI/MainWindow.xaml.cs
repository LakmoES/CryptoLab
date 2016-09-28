using System;
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
                    ViewShower.Show(new CertificatesWindow(), new CertificatesViewModel(), true, (b) => { });
                };
            vm.ChoosePartnerCertificate +=
                (o, s) =>
                {
                    ViewShower.ShowCertSelector(new CertificateSelectWindow(),
                        new CertificateSelectViewModel {Title = s}, (cert) =>
                        {
                            vm.PartnerCertificate = cert;
                        });
                };
            vm.ChooseOwnCertificate +=
                (o, s) =>
                {
                    ViewShower.ShowCertSelector(new CertificateSelectWindow(),
                        new CertificateSelectViewModel {Title = s}, (cert) =>
                        {
                            vm.OwnCertificate = cert;
                        });
                };

            vm.OpenSessionFileDecryptingPath += (o, s) => { (DataContext as MainViewModel).SessionFileDecryptingPath = ShowOpenFileDialog(s); };
            vm.OpenOriginalPath += (o, s) => { (DataContext as MainViewModel).OriginalPath = ShowOpenFileDialog(s); };
            vm.OpenEncryptedPath += (o, s) => { (DataContext as MainViewModel).EncryptedPath = ShowOpenFileDialog(s); };
            vm.OpenSessionFileEncryptingPath += (o, s) => { (DataContext as MainViewModel).SessionFileEncryptingPath = ShowOpenFileDialog(s); };

            vm.SelectTargetEncryptedPath += (o, s) => 
            {
                MainViewModel _vm = (DataContext as MainViewModel);
                _vm.TargetEncryptedFilePath = ShowSaveFileDialog(s);
                if (!string.IsNullOrEmpty(vm.TargetEncryptedFilePath))
                    _vm.EncryptPart2Command.Execute(null);
            };
            vm.SelectTargetDecryptedPath += (o, s) => { (DataContext as MainViewModel).TargetDecryptedFilePath = ShowSaveFileDialog(s); };
            vm.SelectTargetSessionPath += (o, s) => { (DataContext as MainViewModel).TargetSessionFilePath = ShowSaveFileDialog(s); };

            vm.ShowErrors += (o, enumerable) =>
            {
                MessageBox.Show(string.Join(Environment.NewLine, enumerable), "Произошла ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            };
        }

        private string ShowOpenFileDialog(string title)
        {
            var ofd = new OpenFileDialog { Title = title };
            return ofd.ShowDialog(this) == true ? ofd.FileName : null;
        }

        private string ShowSaveFileDialog(string title)
        {
            var sfd = new SaveFileDialog { Title = title };
            return sfd.ShowDialog(this) == true ? sfd.FileName : null;
        }
    }
}