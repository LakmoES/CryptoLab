using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Core.ViewModel
{
    public class CertificatesViewModel : ViewModelBase
    {
        public CertificatesViewModel()
        {
            Title = "Сертификаты";
            LoadCers();
        }

        #region Commands
        private RelayCommand<X509Certificate2> _selectItemRelayCommand;
        /// <summary>
        /// Relay command associated with the selection of an item in the observablecollection
        /// </summary>
        public RelayCommand<X509Certificate2> SelectItemRelayCommand
        {
            get
            {
                if (_selectItemRelayCommand == null)
                {
                    _selectItemRelayCommand = new RelayCommand<X509Certificate2>(async (subject) =>
                    {
                        await SelectItem(subject);
                    });
                }

                return _selectItemRelayCommand;
            }
            set { _selectItemRelayCommand = value; }
        }
        #endregion

        #region Events
        #endregion

        #region Private Fields

        private string _cersAmount;
        #endregion

        #region Public Properties
        public string Title { private set; get; }
        public ObservableCollection<X509Certificate2> CersCollection { private set; get; }

        public string CersAmount
        {
            private set { _cersAmount = value; RaisePropertyChanged(() => CersAmount); }
            get { return _cersAmount; }
        }

        #endregion

        private async Task<int> SelectItem(X509Certificate2 cert)
        {
            var selectedItem = cert;
            Debug.WriteLine(selectedItem != null
                ? $"Selected item issuer:{selectedItem.Issuer}"
                : "null object selected");
            //Do async work

            return await Task.FromResult(1);
        }

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
