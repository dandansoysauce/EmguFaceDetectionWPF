using EmguFaceDetectionWPF.Contents;
using EmguFaceDetectionWPF.Helpers;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmguFaceDetectionWPF.Domain
{
    public class MainWindowViewModel : ViewModelBase
    {
        SqliteHelper db;
        private bool isDialogOpen;
        private object mainDialogContent;

        public bool IsDialogOpen
        {
            get { return isDialogOpen; }
            set
            {
                if (isDialogOpen == value) return;
                isDialogOpen = value;
                RaisePropertyChangedEvent();
            }
        }

        public object MainDialogContent
        {
            get { return mainDialogContent; }
            set
            {
                if (mainDialogContent == value) return;
                mainDialogContent = value;
                RaisePropertyChangedEvent();
            }
        }

        public ICommand AddUserDialogCommand => new DelegateCommand
        {
            CanExecuteDelegate = x => true,
            ExecuteDelegate = x =>
            {
                MainDialogContent = new AddUserDialog { DataContext = new AddUserViewModel() };
                IsDialogOpen = true;
            }
        };

        public MainWindowViewModel()
        {
            db = new SqliteHelper();
            db.Init();
        }

        #region Private Methods
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("Dialog Closed.");
        }
        #endregion
    }
}
