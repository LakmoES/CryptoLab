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
            Title = "��������";
            IsEnabled = true;

            DispatcherHelper.Initialize();

            CryptoAlgorithms = new ObservableCollection<ICryptoAlgorithm>
            {
                new DESAlgotithm()
            };
        }

        #region Commands
        private RelayCommand _chooseOriginalFileCommand;
        public ICommand ChooseOriginalFileCommand => _chooseOriginalFileCommand ?? (_chooseOriginalFileCommand = new RelayCommand(() => RaiseOpenOriginalPath("���� ��� ����������")));

        private RelayCommand _chooseEncryptedFileCommand;
        public ICommand ChooseEncryptedFileCommand => _chooseEncryptedFileCommand ?? (_chooseEncryptedFileCommand = new RelayCommand(() => RaiseOpenEncryptedPath("������������� ����")));

        private RelayCommand _chooseSessionFileCommand;
        public ICommand ChooseSessionFileCommand => _chooseSessionFileCommand ?? (_chooseSessionFileCommand = new RelayCommand(() => RaiseOpenSessionFilePath("���� ����������� �����")));

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
        public ICommand EncryptCommand => _encryptCommand ?? (_encryptCommand = new RelayCommand(async () => await Task.Run(() => StartEncrypt())));

        private RelayCommand _decryptCommand;
        public ICommand DecryptCommand => _decryptCommand ?? (_decryptCommand = new RelayCommand(async () => await Task.Run(() => StartDecrypt())));

        private RelayCommand _chooseSessionFileForEncryptingCommand;

        public ICommand ChooseSessionFileForEncryptingCommand
            => _chooseSessionFileForEncryptingCommand ?? (_chooseSessionFileForEncryptingCommand = new RelayCommand(() => RaiseOpenSessionFileForEncryptingPath("���������� ���� ��� ����������")));
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

        public event EventHandler<string> OpenSessionFileForEncryptingPath;
        private void RaiseOpenSessionFileForEncryptingPath(string title)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    OpenSessionFileForEncryptingPath?.Invoke(this, title));
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
        private string _sessionFilePath;
        private string _sessionFileForEncryptingPath;
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
        public string SessionFilePath
        {
            set
            {
                _sessionFilePath = value;
                RaisePropertyChanged(() => SessionFilePath);
            }
            get { return _sessionFilePath; }
        }

        public string SessionFileForEncryptingPath
        {
            set
            {
                _sessionFileForEncryptingPath = value;
                RaisePropertyChanged(() => SessionFileForEncryptingPath);
            }
            get { return _sessionFileForEncryptingPath; }
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

        private string ImportKeyFromFile(X509Certificate2 cert, string path)
        {
            var encryptedKey = File.ReadAllText(path);
            var rsa = new RSAAlgorithm();

            return rsa.Decrypt(cert, encryptedKey);
        }

        private bool StartEncrypt()
        {
            IsEnabled = false;
            List<string> errorList = new List<string>();
            if(string.IsNullOrEmpty(OriginalPath))
                errorList.Add("�� ����� ���� ��� ����������");
            if (PartnerCertificate == null)
                errorList.Add("�� ������ ���������� ��� ����������");
            if (SelectedCryptoAlgorithm == null)
                errorList.Add("�� ������ �������� ����������");
            if (!string.IsNullOrEmpty(SessionFileForEncryptingPath) && OwnCertificate == null)
                errorList.Add("������� ��� ����������");
            if (errorList.Count > 0)
            {
                RaiseShowErrors(errorList.ToArray());
                IsEnabled = true;
                return false;
            }
            Status = "���� ����������...";
            Encrypt(out errorList);
            Status = "��������";
            IsEnabled = true;
            return errorList.Count <= 0;
        }

        private void Encrypt(out List<string> errorList)
        {
            errorList = new List<string>();
            string sessionKey;

            if (string.IsNullOrEmpty(SessionFileForEncryptingPath))
                sessionKey = KeyGenerator.GetRandomKey(1024);
            else
                sessionKey = ImportKeyFromFile(OwnCertificate, SessionFileForEncryptingPath);

            var rsa = new RSAAlgorithm();
            var encryptedSessionKey = rsa.Encrypt(PartnerCertificate, sessionKey);

            
        }
        private bool StartDecrypt()
        {
            IsEnabled = false;
            List<string> errorList = new List<string>();
            if (string.IsNullOrEmpty(EncryptedPath))
                errorList.Add("�� ����� ���� ��� ������������");
            if(string.IsNullOrEmpty(SessionFilePath))
                errorList.Add("���� � ���������� ������ �� ������");
            if(OwnCertificate == null)
                errorList.Add("�� ������ ���������� ��� ������������");
            if (SelectedCryptoAlgorithm == null)
                errorList.Add("�� ������ �������� ����������");
            if (errorList.Count > 0)
            {
                RaiseShowErrors(errorList.ToArray());
                IsEnabled = true;
                return false;
            }
            Status = "���� ������������...";
            Thread.Sleep(1000);
            Status = "��������";
            IsEnabled = true;

            return errorList.Count <= 0;
        }
    }
}