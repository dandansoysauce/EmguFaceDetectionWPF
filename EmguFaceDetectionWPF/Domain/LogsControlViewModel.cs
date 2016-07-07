using EmguFaceDetectionWPF.Helpers;
using EmguFaceDetectionWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EmguFaceDetectionWPF.Domain
{
    public class LogsControlViewModel : ViewModelBase
    {
        SqliteHelper db;

        private List<LogModel> _logs = new List<LogModel>();
        private List<LogModel> _filterLogs = new List<LogModel>();
        private ICollectionView _dataGridCollection;
        private string _filterString;

        public ICollectionView DataGridCollection
        {
            get { return _dataGridCollection; }
            set
            {
                _dataGridCollection = value;
                RaisePropertyChangedEvent();
            }
        }

        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                RaisePropertyChangedEvent();
                FilterCollection();
            }
        }

        public bool Filter(object obj)
        {
            var data = obj as LogModel;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(_filterString))
                {
                    return data.Name.Contains(_filterString) || data.Log.ToString().Contains(_filterString);
                }
                return true;
            }
            return false;
        }

        public LogsControlViewModel()
        {
            db = new SqliteHelper();
            LoadLogs();
            DataGridCollection = CollectionViewSource.GetDefaultView(_logs);
        }

        #region Private Methods
        private void FilterCollection()
        {
            if (_dataGridCollection != null)
            {
                _dataGridCollection.Refresh();
            }
        }

        private async void LoadLogs()
        {
            _logs = await db.GetLogs();
        }
        #endregion
    }
}
