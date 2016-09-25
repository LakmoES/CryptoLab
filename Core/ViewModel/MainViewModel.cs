using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

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
        public ICommand ChooseOriginalFileCommand => _chooseOriginalFileCommand ?? (_chooseOriginalFileCommand = new RelayCommand(null));

        private RelayCommand _chooseEncodedFileCommand;
        public ICommand ChooseEncodedFileCommand => _chooseEncodedFileCommand ?? (_chooseEncodedFileCommand = new RelayCommand(null));

        private RelayCommand _showCersCommand;
        public ICommand ShowCersCommand => _showCersCommand ?? (_showCersCommand = new RelayCommand(RaiseShowCers));
        #endregion

        #region Events
        public event EventHandler<string> OpenOriginalPath;
        private void RaiseOpenOriginalPath(string path)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    OpenOriginalPath?.Invoke(this, path));
        }

        public event EventHandler<string> OpenEncodedPath;
        private void RaiseOpenEncodedPath(string path)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    OpenEncodedPath?.Invoke(this, path));
        }

        public event EventHandler ShowCers;
        private void RaiseShowCers()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    ShowCers?.Invoke(this, EventArgs.Empty));
        }
        #endregion

        #region Private Fields
        private string _title;
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
        #endregion
        
    }
}