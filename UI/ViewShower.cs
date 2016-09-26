using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Core.ViewModel;
using GalaSoft.MvvmLight;

namespace UI
{
    public class ViewShower
    {
        public static void Show(Window view, ViewModelBase viewModel, bool isModal, Action<bool?> closeAction)
        {
            view.DataContext = viewModel;
            if (isModal)
            {
                view.Closed += (sender, args) => closeAction(view.DialogResult);
                view.ShowDialog();
            }
            else
                view.Show();
        }

        public static void ShowCertSelector(CertificateSelectWindow view, CertificateSelectViewModel viewModel, Action<X509Certificate2> closeAction)
        {
            if (view == null)
                throw new ArgumentNullException("view is null");
            if (viewModel == null)
                throw new ArgumentNullException("viewModel is null");

            view.DataContext = viewModel;
            (view as Window).Closed += (sender, args) => closeAction(viewModel.SelectedCertificate);
            (view as Window).ShowDialog();
        }
    }
}
