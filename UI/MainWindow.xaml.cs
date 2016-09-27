﻿using System;
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

            vm.OpenSessionFileDecryptingPath += (o, s) => { (DataContext as MainViewModel).SessionFileDecryptingPath = OpenFile(s); };
            vm.OpenOriginalPath += (o, s) => { (DataContext as MainViewModel).OriginalPath = OpenFile(s); };
            vm.OpenEncryptedPath += (o, s) => { (DataContext as MainViewModel).EncryptedPath = OpenFile(s); };
            vm.OpenSessionFileEncryptingPath += (o, s) => { (DataContext as MainViewModel).SessionFileEncryptingPath = OpenFile(s); };
            vm.ShowErrors += (o, enumerable) =>
            {
                MessageBox.Show(string.Join(Environment.NewLine, enumerable), "Произошла ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            };
        }

        private string OpenFile(string title)
        {
            var ofd = new OpenFileDialog { Title = title };
            return ofd.ShowDialog(this) == true ? ofd.FileName : null;
        }
    }
}