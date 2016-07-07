using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Threading;
using Emgu.CV.CvEnum;
using EmguFaceDetectionWPF.Helpers;
using EmguFaceDetectionWPF.Domain;

namespace EmguFaceDetectionWPF.Contents
{
    /// <summary>
    /// Interaction logic for AddUserDialog.xaml
    /// </summary>
    public partial class AddUserDialog : UserControl
    {
        public AddUserDialog()
        {
            InitializeComponent();
        }
    }
}
