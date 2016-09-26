using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Core.CryptoAlgorithms;

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
            IsEnabled = true;

            DispatcherHelper.Initialize();

            CryptoAlgorithms = new ObservableCollection<ICryptoAlgorithm>
            {
                new RSAAlgorithm()
            };
        }

        #region Commands
        private RelayCommand _chooseOriginalFileCommand;
        public ICommand ChooseOriginalFileCommand => _chooseOriginalFileCommand ?? (_chooseOriginalFileCommand = new RelayCommand(() => RaiseOpenOriginalPath("Файл для шифрования")));

        private RelayCommand _chooseEncryptedFileCommand;
        public ICommand ChooseEncryptedFileCommand => _chooseEncryptedFileCommand ?? (_chooseEncryptedFileCommand = new RelayCommand(() => RaiseOpenEncryptedPath("Зашифрованный файл")));

        private RelayCommand _chooseSessionFileCommand;
        public ICommand ChooseSessionFileCommand => _chooseSessionFileCommand ?? (_chooseSessionFileCommand = new RelayCommand(() => RaiseOpenSessionFilePath("Файл сессионного ключа")));

        private RelayCommand _showCersCommand;
        public ICommand ShowCersCommand => _showCersCommand ?? (_showCersCommand = new RelayCommand(RaiseShowCers));

        private RelayCommand _chooseCertificateForEncryptingCommand;

        public ICommand ChooseCertificateForEncryptingCommand
            =>
                _chooseCertificateForEncryptingCommand ??
                (_chooseCertificateForEncryptingCommand = new RelayCommand(RaiseChooseCertificateForEncrypting));

        private RelayCommand _chooseCertificateForDecryptingCommand;
        public ICommand ChooseCertificateForDecryptingCommand
            =>
                _chooseCertificateForDecryptingCommand ??
                (_chooseCertificateForDecryptingCommand = new RelayCommand(RaiseChooseCertificateForDecrypting));

        private RelayCommand _encryptCommand;
        public ICommand EncryptCommand => _encryptCommand ?? (_encryptCommand = new RelayCommand(async () => await Task.Run(() => Encrypt())));

        private RelayCommand _decryptCommand;
        public ICommand DecryptCommand => _decryptCommand ?? (_decryptCommand = new RelayCommand(async () => await Task.Run(() => Decrypt())));
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
        public event EventHandler ChooseCertificateForDecrypting;
        private void RaiseChooseCertificateForDecrypting()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    ChooseCertificateForDecrypting?.Invoke(this, EventArgs.Empty));
        }
        public event EventHandler<string> OpenSessionFilePath;
        private void RaiseOpenSessionFilePath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    OpenSessionFilePath?.Invoke(this, title));
        }

        public event EventHandler<IEnumerable<string>> ShowErrors;

        private void RaiseShowErrors(params string[] errors)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    ShowErrors?.Invoke(this, errors));
        }
        #endregion

        #region Private Fields
        private bool _isEnabled;
        private string _title;
        private X509Certificate2 _encryptingCertificate;
        private X509Certificate2 _decryptingCertificate;
        private string _originalPath;
        private ICryptoAlgorithm _selectedCryptoAlgorithm;
        private string _sessionFilePath;
        private string _encryptedPath;
        #endregion

        #region Public Properties

        public bool IsEnabled
        {
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => IsEnabled);
            }
            get { return _isEnabled; }
        }
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
        public X509Certificate2 DecryptingCertificate
        {
            set
            {
                _decryptingCertificate = value;
                RaisePropertyChanged(() => DecryptingCertificate);
            }
            get { return _decryptingCertificate; }
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
        public string SessionFilePath
        {
            set
            {
                _sessionFilePath = value;
                RaisePropertyChanged(() => SessionFilePath);
            }
            get { return _sessionFilePath; }
        }
        public string EncryptedPath
        {
            set
            {
                _encryptedPath = value;
                RaisePropertyChanged(() => EncryptedPath);
            }
            get { return _encryptedPath; }
        }

        public ObservableCollection<ICryptoAlgorithm> CryptoAlgorithms { set; get; }

        public ICryptoAlgorithm SelectedCryptoAlgorithm
        {
            set
            {
                _selectedCryptoAlgorithm = value;
                RaisePropertyChanged(() => SelectedCryptoAlgorithm);
            }
            get { return _selectedCryptoAlgorithm; }
        }
        #endregion

        private bool Encrypt()
        {
            IsEnabled = false;
            List<string> errorList = new List<string>();
            if(String.IsNullOrEmpty(OriginalPath))
                errorList.Add("Не задан файл для шифрования");
            if (EncryptingCertificate == null)
                errorList.Add("Не указан сертификат для шифрования");
            if (errorList.Count > 0)
                RaiseShowErrors(errorList.ToArray());
            Thread.Sleep(1000);
            IsEnabled = true;
            return errorList.Count <= 0;
        }
        private bool Decrypt()
        {
            IsEnabled = false;
            List<string> errorList = new List<string>();
            if (String.IsNullOrEmpty(EncryptedPath))
                errorList.Add("Не задан файл для дешифрования");
            if(String.IsNullOrEmpty(SessionFilePath))
                errorList.Add("Файл с сессионным ключем не указан");
            if(DecryptingCertificate == null)
                errorList.Add("Не указан сертификат для дешифрования");
            if (errorList.Count > 0)
                RaiseShowErrors(errorList.ToArray());
            Thread.Sleep(1000);
            IsEnabled = true;

            return errorList.Count <= 0;
        }
    }
}