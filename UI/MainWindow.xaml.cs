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
            vm.ChooseCertificateForEncrypting += 
                (o, eventArgs) =>
                {
                    ViewShower.ShowCertSelector(new CertificateSelectWindow(), new CertificateSelectViewModel(), (cert) =>
                    {
                        vm.EncryptingCertificate = cert;
                    });
                };
            vm.ChooseCertificateForDecrypting +=
                (o, eventArgs) =>
                {
                    ViewShower.ShowCertSelector(new CertificateSelectWindow(), new CertificateSelectViewModel(), (cert) =>
                    {
                        vm.DecryptingCertificate = cert;
                    });
                };

            vm.OpenSessionFilePath += (o, s) => { (DataContext as MainViewModel).SessionFilePath = OpenFile(s); };
            vm.OpenOriginalPath += (o, s) => { (DataContext as MainViewModel).OriginalPath = OpenFile(s); };
            vm.OpenEncryptedPath += (o, s) => { (DataContext as MainViewModel).EncryptedPath = OpenFile(s); };
        }

        private string OpenFile(string title)
        {
            var ofd = new OpenFileDialog { Title = title };
            if (ofd.ShowDialog(this) == true)
                return ofd.FileName;
            return null;
        }
    }
}
