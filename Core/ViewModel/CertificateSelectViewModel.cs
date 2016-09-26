using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Core.ViewModel
{
    public class CertificateSelectViewModel : ViewModelBase
    {
        public CertificateSelectViewModel()
        {
            Title = "Выбор сертификата";
            LoadCers();
        }
        #region Private Fields
        private X509Certificate2 _selectedCertificate;
        private string _title;
        private string _cersAmount;
        #endregion

        #region Public Properties

        public X509Certificate2 SelectedCertificate
        {
            set
            {
                _selectedCertificate = value;
                RaisePropertyChanged(() => SelectedCertificate);
            }
            get { return _selectedCertificate; }
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
        public ObservableCollection<X509Certificate2> CersCollection { private set; get; }
        public string CersAmount
        {
            private set { _cersAmount = value; RaisePropertyChanged(() => CersAmount); }
            get { return _cersAmount; }
        }
        #endregion

        private void LoadCers()
        {
            var cers = CertRepository.GetCertificates();
            if (cers.Count <= 0)
            {
                CersCollection = new ObservableCollection<X509Certificate2>();
                CersAmount = "Н/А";
            }
            CersAmount = cers.Count.ToString();
            X509Certificate2[] certArray = new X509Certificate2[cers.Count];
            cers.CopyTo(certArray, 0);

            CersCollection = new ObservableCollection<X509Certificate2>(certArray);
        }
    }
}
