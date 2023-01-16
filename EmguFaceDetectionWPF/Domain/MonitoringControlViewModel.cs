using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using EmguFaceDetectionWPF.Constants;
using EmguFaceDetectionWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EmguFaceDetectionWPF.Domain
{
    public class MonitoringControlViewModel : ViewModelBase
    {
        private VideoCapture _capture;
        private CascadeClassifier _haarCascade;
        private Image<Bgr, byte> _currentFrame;
        private Image<Gray, byte> _result;
        private RecognizerEngine _engine;
        private DispatcherTimer _timer;
        private DispatcherTimer _scanner;
        private SqliteHelper db;

        private byte[] monitoringImage;
        private byte[] recognizedImage;

        public byte[] MonitoringImage
        {
            get { return monitoringImage; }
            set
            {
                monitoringImage = value;
                RaisePropertyChangedEvent();
            }
        }

        public byte[] RecognizedImage
        {
            get { return recognizedImage; }
            set
            {
                recognizedImage = value;
                RaisePropertyChangedEvent();
            }
        }

        public MonitoringControlViewModel()
        {
            db = new SqliteHelper();
            string recognizerPath = AppDomain.CurrentDomain.BaseDirectory + "recognizer.YAML";
            _engine = new RecognizerEngine(recognizerPath);
            _engine.TrainRecognizer();
        }

        public ICommand LoadCameraCommand => new DelegateCommand
        {
            CanExecuteDelegate = x => true,
            ExecuteDelegate = x =>
            {
                try
                {
                    _capture = new VideoCapture();
                    _haarCascade = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
                    _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.ApplicationIdle,
                         (s, ev) => FrameGrabber(), Application.Current.Dispatcher);
                    _timer.Start();

                    _scanner = new DispatcherTimer(TimeSpan.FromSeconds(3), DispatcherPriority.ApplicationIdle,
                         (s, ev) => FaceScanner(), Application.Current.Dispatcher);
                    _scanner.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Poop, " + ex.Message);
                }
            }
        };

        #region Private Methods
        private void FaceScanner()
        {
            if (_capture != null)
            {
                Mat query = _capture.QueryFrame();
                if (query != null)
                {
                    _currentFrame = query.ToImage<Bgr, byte>();
                    if (_currentFrame != null)
                    {
                        Image<Gray, byte> grayFrame = _currentFrame.Convert<Gray, byte>();
                        var detectedFaces = _haarCascade.DetectMultiScale(grayFrame, 1.2, 10, System.Drawing.Size.Empty);
                        foreach (var face in detectedFaces)
                        {
                            _result = _currentFrame.Copy(face).Convert<Gray, byte>().Resize(100, 100, Inter.Cubic);
                            if (_engine != null)
                            {
                                int user = _engine.RecognizeUser(_result);
                                Console.WriteLine(string.Format("User: {0}", user));
                                if (user > 0)
                                {
                                    _currentFrame.Draw(face, new Bgr(System.Drawing.Color.LightGreen), 3);
                                    RecognizedImage = _currentFrame.ToJpegData();
                                    db.InsertLog(user, StringConstants.KEY_LOG, DateTime.Now);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FrameGrabber()
        {
            if (_capture != null)
            {
                Mat query = _capture.QueryFrame();
                if (query != null)
                {
                    _currentFrame = query.ToImage<Bgr, byte>();
                    if (_currentFrame != null)
                    {
                        Image<Gray, byte> grayFrame = _currentFrame.Convert<Gray, byte>();
                        var detectedFaces = _haarCascade.DetectMultiScale(grayFrame, 1.2, 10, System.Drawing.Size.Empty);
                        foreach (var face in detectedFaces)
                        {
                            _currentFrame.Draw(face, new Bgr(System.Drawing.Color.Red), 3);
                            _result = _currentFrame.Copy(face).Convert<Gray, byte>().Resize(100, 100, Inter.Cubic);
                        }

                        MonitoringImage = _currentFrame.ToJpegData();
                    }
                }
            }
        }
        #endregion
    }
}
