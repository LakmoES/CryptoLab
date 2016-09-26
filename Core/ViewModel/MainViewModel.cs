using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace Core.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Title = "Crypto";

            DispatcherHelper.Initialize();
        }

        #region Commands
        private RelayCommand _chooseOriginalFileCommand;
        public ICommand ChooseOriginalFileCommand => _chooseOriginalFileCommand ?? (_chooseOriginalFileCommand = new RelayCommand(() => RaiseOpenOriginalPath("Файл для шифрования")));

        private RelayCommand _chooseEncryptedFileCommand;
        public ICommand ChooseEncryptedFileCommand => _chooseEncryptedFileCommand ?? (_chooseEncryptedFileCommand = new RelayCommand(() => RaiseOpenEncryptedPath("Зашифрованный файл")));

        private RelayCommand _showCersCommand;
        public ICommand ShowCersCommand => _showCersCommand ?? (_showCersCommand = new RelayCommand(RaiseShowCers));

        private RelayCommand _chooseCertificateForEncryptingCommand;

        public ICommand ChooseCertificateForEncryptingCommand
            =>
                _chooseCertificateForEncryptingCommand ??
                (_chooseCertificateForEncryptingCommand = new RelayCommand(RaiseChooseCertificateForEncrypting));
        #endregion

        #region Events
        public event EventHandler<string> OpenOriginalPath;
        private void RaiseOpenOriginalPath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    OpenOriginalPath?.Invoke(this, title));
        }

        public event EventHandler<string> OpenEncryptedPath;
        private void RaiseOpenEncryptedPath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    OpenEncryptedPath?.Invoke(this, title));
        }

        public event EventHandler ShowCers;
        private void RaiseShowCers()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    ShowCers?.Invoke(this, EventArgs.Empty));
        }
        public event EventHandler ChooseCertificateForEncrypting;
        private void RaiseChooseCertificateForEncrypting()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    ChooseCertificateForEncrypting?.Invoke(this, EventArgs.Empty));
        }
        #endregion

        #region Private Fields
        private string _title;
        private X509Certificate2 _encryptingCertificate;
        private string _originalPath;
        #endregion

        #region Public Properties
        public string Title
        {
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
            get { return _title; }
        }

        public X509Certificate2 EncryptingCertificate
        {
            set
            {
                _encryptingCertificate = value;
                RaisePropertyChanged(() => EncryptingCertificate);
            }
            get { return _encryptingCertificate; }
        }

        public string OriginalPath
        {
            set
            {
                _originalPath = value;
                RaisePropertyChanged(() => OriginalPath);
            }
            get { return _originalPath; }
        }
        #endregion

    }
}