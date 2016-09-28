using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            Title = "Шифратор";
            IsEnabled = true;

            DispatcherHelper.Initialize();

            CryptoAlgorithms = new ObservableCollection<ICryptoAlgorithm>
            {
                new DESAlgorithm()
            };
        }

        #region Commands
        private RelayCommand _chooseOriginalFileCommand;
        public ICommand ChooseOriginalFileCommand => _chooseOriginalFileCommand ?? (_chooseOriginalFileCommand = new RelayCommand(() => RaiseOpenOriginalPath("Файл для шифрования")));

        private RelayCommand _chooseEncryptedFileCommand;
        public ICommand ChooseEncryptedFileCommand => _chooseEncryptedFileCommand ?? (_chooseEncryptedFileCommand = new RelayCommand(() => RaiseOpenEncryptedPath("Зашифрованный файл")));

        private RelayCommand _chooseSessionFileDecryptingCommand;
        public ICommand ChooseSessionFileDecryptingCommand => _chooseSessionFileDecryptingCommand ?? (_chooseSessionFileDecryptingCommand = new RelayCommand(() => RaiseOpenSessionFileDecryptingPath("Сессионный ключ для дешифрования")));

        private RelayCommand _showCersCommand;
        public ICommand ShowCersCommand => _showCersCommand ?? (_showCersCommand = new RelayCommand(RaiseShowCers));

        private RelayCommand _choosePartnerCertificateCommand;
        public ICommand ChoosePartnerCertificateCommand
            =>
                _choosePartnerCertificateCommand ??
                (_choosePartnerCertificateCommand = new RelayCommand(() => RaiseChoosePartnerCertificate("Сертификат партнера")));

        private RelayCommand _chooseOwnCertificateCommand;
        public ICommand ChooseOwnCertificateCommand
            =>
                _chooseOwnCertificateCommand ??
                (_chooseOwnCertificateCommand = new RelayCommand(() => RaiseChooseOwnCertificate("Ваш сертификат")));

        private RelayCommand _encryptCommand;

        public ICommand EncryptCommand
            =>
                _encryptCommand ??
                (_encryptCommand = new RelayCommand(() => PreEncryptingCheck()));

        private RelayCommand _encryptPart2Command;

        public ICommand EncryptPart2Command
            =>
                _encryptPart2Command ??
                (_encryptPart2Command = new RelayCommand(async () => await Task.Run(() => StartEncrypt())));

        private RelayCommand _decryptCommand;
        public ICommand DecryptCommand => _decryptCommand ?? (_decryptCommand = new RelayCommand(() => PreDecryptCheck()));

        private RelayCommand _decryptPart2Command;
        public ICommand DecryptPart2Command => _decryptPart2Command ?? (_decryptPart2Command = new RelayCommand(async () => await Task.Run(() => StartDecrypt())));

        private RelayCommand _chooseSessionFileEncryptingCommand;
        public ICommand ChooseSessionFileEncryptingCommand
            => _chooseSessionFileEncryptingCommand ?? (_chooseSessionFileEncryptingCommand = new RelayCommand(() => RaiseOpenSessionFileEncryptingPath("Сессионный ключ для шифрования")));
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

        public event EventHandler<string> SelectTargetEncryptedPath;
        private void RaiseSelectTargetEncryptedPath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    SelectTargetEncryptedPath?.Invoke(this, title));
        }
        public event EventHandler<string> SelectTargetDecryptedPath;
        private void RaiseSelectTargetDecryptedPath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    SelectTargetDecryptedPath?.Invoke(this, title));
        }
        public event EventHandler<string> SelectTargetSessionPath;
        private void RaiseSelectTargetSessionPath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    SelectTargetSessionPath?.Invoke(this, title));
        }

        public event EventHandler ShowCers;
        private void RaiseShowCers()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    ShowCers?.Invoke(this, EventArgs.Empty));
        }
        public event EventHandler<string> ChoosePartnerCertificate;
        private void RaiseChoosePartnerCertificate(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    ChoosePartnerCertificate?.Invoke(this, title));
        }
        public event EventHandler<string> ChooseOwnCertificate;
        private void RaiseChooseOwnCertificate(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    ChooseOwnCertificate?.Invoke(this, title));
        }
        public event EventHandler<string> OpenSessionFileDecryptingPath;
        private void RaiseOpenSessionFileDecryptingPath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    OpenSessionFileDecryptingPath?.Invoke(this, title));
        }

        public event EventHandler<string> OpenSessionFileEncryptingPath;
        private void RaiseOpenSessionFileEncryptingPath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    OpenSessionFileEncryptingPath?.Invoke(this, title));
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
        private string _status;
        private X509Certificate2 _partnerCertificate;
        private X509Certificate2 _ownCertificate;
        private string _originalPath;
        private ICryptoAlgorithm _selectedCryptoAlgorithm;
        private string _sessionFileDecryptingPath;
        private string _sessionFileEncryptingPath;
        private string _encryptedPath;
        private string _targetEncryptedFilePath;
        private string _targetSessionFilePath;
        private string _targetDecryptedFilePath;
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
        public string Status
        {
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
            get { return _status; }
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
        public X509Certificate2 PartnerCertificate
        {
            set
            {
                _partnerCertificate = value;
                RaisePropertyChanged(() => PartnerCertificate);
            }
            get { return _partnerCertificate; }
        }
        public X509Certificate2 OwnCertificate
        {
            set
            {
                _ownCertificate = value;
                RaisePropertyChanged(() => OwnCertificate);
            }
            get { return _ownCertificate; }
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
        public string SessionFileDecryptingPath
        {
            set
            {
                _sessionFileDecryptingPath = value;
                RaisePropertyChanged(() => SessionFileDecryptingPath);
            }
            get { return _sessionFileDecryptingPath; }
        }

        public string SessionFileEncryptingPath
        {
            set
            {
                _sessionFileEncryptingPath = value;
                RaisePropertyChanged(() => SessionFileEncryptingPath);
            }
            get { return _sessionFileEncryptingPath; }
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

        public string TargetEncryptedFilePath
        {
            set
            {
                _targetEncryptedFilePath = value;
                RaisePropertyChanged(() => TargetEncryptedFilePath);
            }
            get { return _targetEncryptedFilePath; }
        }

        public string TargetSessionFilePath
        {
            set
            {
                _targetSessionFilePath = value;
                RaisePropertyChanged(() => TargetSessionFilePath);
            }
            get { return _targetSessionFilePath; }
        }

        public string TargetDecryptedFilePath
        {
            set
            {
                _targetDecryptedFilePath = value;
                RaisePropertyChanged(() => TargetDecryptedFilePath);
            }
            get { return _targetDecryptedFilePath; }
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

        private bool PreEncryptingCheck()
        {
            IsEnabled = false;
            List<string> errorList = new List<string>();
            if (string.IsNullOrEmpty(OriginalPath))
                errorList.Add("Не задан файл для шифрования");
            if (PartnerCertificate == null)
                errorList.Add("Не указан сертификат для шифрования");
            if (SelectedCryptoAlgorithm == null)
                errorList.Add("Не указан алгоритм шифрования");
            if (!string.IsNullOrEmpty(SessionFileEncryptingPath) && OwnCertificate == null)
                errorList.Add("Укажите Ваш сертификат");
            if (errorList.Count > 0)
            {
                RaiseShowErrors(errorList.ToArray());
                IsEnabled = true;
                return false;
            }

            if (string.IsNullOrEmpty(SessionFileEncryptingPath))
            {
                RaiseSelectTargetSessionPath("Файл с ключом");
                if (string.IsNullOrEmpty(TargetSessionFilePath))
                    errorList.Add("Не указан путь сохранения файла с ключом");
            }
            RaiseSelectTargetEncryptedPath("Зашифрованный файл");
            if (string.IsNullOrEmpty(TargetEncryptedFilePath))
                errorList.Add("Вы не выбрали куда сохранить зашифрованный файл");


            if (errorList.Count > 0)
            {
                RaiseShowErrors(errorList.ToArray());
                IsEnabled = true;
                return false;
            }

            return true;
        }

        private bool StartEncrypt()
        {
            List<string> errorList;
            Status = "Идет шифрование...";
            if (
                !CryptoProcessor.Encrypt(out errorList, SelectedCryptoAlgorithm, TargetEncryptedFilePath, TargetSessionFilePath, OriginalPath,
                    SessionFileEncryptingPath, OwnCertificate, PartnerCertificate))
                RaiseShowErrors(errorList.ToArray());
            Status = "Свободен";
            IsEnabled = true;
            return errorList.Count <= 0;
        }

        private bool PreDecryptCheck()
        {
            IsEnabled = false;
            List<string> errorList = new List<string>();
            if (string.IsNullOrEmpty(EncryptedPath))
                errorList.Add("Не задан файл для дешифрования");
            if (string.IsNullOrEmpty(SessionFileDecryptingPath))
                errorList.Add("Файл с сессионным ключем не указан");
            if (OwnCertificate == null)
                errorList.Add("Не указан сертификат для дешифрования");
            if (SelectedCryptoAlgorithm == null)
                errorList.Add("Не указан алгоритм шифрования");
            if (errorList.Count > 0)
            {
                RaiseShowErrors(errorList.ToArray());
                IsEnabled = true;
                return false;
            }

            RaiseSelectTargetDecryptedPath("Расшифрованный файл");
            if (string.IsNullOrEmpty(TargetDecryptedFilePath))
                errorList.Add("Вы не выбрали место сохрания файла");

            if (errorList.Count > 0)
            {
                RaiseShowErrors(errorList.ToArray());
                IsEnabled = true;
                return false;
            }

            return true;
        }

        private bool StartDecrypt()
        {
            List<string> errorList;
            Status = "Идет дешифрование...";
            Thread.Sleep(1000);
            Status = "Свободен";
            IsEnabled = true;

            if(!CryptoProcessor.Decrypt(out errorList, SelectedCryptoAlgorithm, TargetDecryptedFilePath, EncryptedPath, SessionFileDecryptingPath,
                OwnCertificate))
                RaiseShowErrors(errorList.ToArray());

            return errorList.Count <= 0;
        }
    }
}