using SimpleRtspPlayer.GUI.Models;
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
using System.Windows.Shapes;

namespace SimpleRtspPlayer.GUI.Views
{
    /// <summary>
    /// Interaction logic for GrpcStreamWindow.xaml
    /// </summary>
    public partial class GrpcStreamWindow : Window
    {
        public GrpcStreamWindow()
        {
            InitializeComponent();
            GrpcWindowModel grpcWindowModel = new GrpcWindowModel();
            this.DataContext = grpcWindowModel;
        }
        
    }
}
