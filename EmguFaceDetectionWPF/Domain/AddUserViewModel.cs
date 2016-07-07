using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using EmguFaceDetectionWPF.Helpers;
using EmguFaceDetectionWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EmguFaceDetectionWPF.Domain
{
    public class AddUserViewModel : ViewModelBase
    {
        private bool isError;
        private DelegateCommand addCommand;
        private DelegateCommand loadCameraCommand;

        private Image<Bgr, byte> _currentFrame;
        private Capture _capture;
        private CascadeClassifier _haarCascade;
        private Image<Gray, byte> _result, _TrainedFace = null;
        private DispatcherTimer _timer;
        private Mat _query = null;
        private SqliteHelper db;

        private MainWindow win;
        private MainWindowViewModel mvm;

        private UserModel user;

        public AddUserViewModel()
        {
            user = new UserModel();
            db = new SqliteHelper();

            win = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mvm = (MainWindowViewModel)win.DataContext;
        }

        public string Name
        {
            get { return user.Name; }
            set
            {
                user.Name = value;
                RaisePropertyChangedEvent();
                addCommand.RaiseCanExecuteChanged();
            }
        }

        public byte[] Face
        {
            get { return user.Face; }
            set
            {
                user.Face = value;
                RaisePropertyChangedEvent();
                addCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsError
        {
            get { return isError; }
            set
            {
                isError = value;
                RaisePropertyChangedEvent();
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return addCommand = new DelegateCommand
                {
                    CanExecuteDelegate = x =>
                    {
                        if (!string.IsNullOrEmpty(user.Name) && _result != null) return true;
                        return false;
                    },
                    ExecuteDelegate = x =>
                    {
                        if (_result != null)
                        {
                            _TrainedFace = _result;
                            string msg = string.Format("[{0}]: Add User. Log Message: {1}", DateTime.Now, db.InsertUser(user));
                            if (msg.ToLower().Contains("error"))
                            {

                            }
                            mvm.IsDialogOpen = false;
                        }
                    }
                };
            }
        }

        public ICommand LoadCameraCommand
        {
            get
            {
                return loadCameraCommand = new DelegateCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x =>
                    {
                        try
                        {
                            _capture = new Capture();
                            _haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
                            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.ApplicationIdle,
                                 (s, ev) => FrameGrabber(), Application.Current.Dispatcher);
                            _timer.Start();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(string.Format("Exception in {0}", ex.Message));
                            throw;
                        }
                    }
                };
            }
        }

        private void FrameGrabber()
        {
            if (_capture != null)
            {
                _query = _capture.QueryFrame();
                if (_query != null)
                {
                    IsError = false;
                    _currentFrame = _query.ToImage<Bgr, byte>();
                    if (_currentFrame != null)
                    {
                        Image<Gray, byte> grayFrame = _currentFrame.Convert<Gray, byte>();
                        var detectedFaces = _haarCascade.DetectMultiScale(grayFrame, 1.2, 10, System.Drawing.Size.Empty);
                        foreach (var face in detectedFaces)
                        {
                            _currentFrame.Draw(face, new Bgr(System.Drawing.Color.Red), 3);
                            _result = _currentFrame.Copy(face).Convert<Gray, byte>().Resize(100, 100, Inter.Cubic);
                        }
                        Face = _currentFrame.ToJpegData();
                    }
                }
                else
                {
                    CaptureStop();
                    IsError = true;
                }
            }
        }

        private void CaptureStop()
        {
            if (_capture != null && _timer != null)
            {
                _capture.Dispose();
                _timer.Stop();
            }
        }
    }
}
