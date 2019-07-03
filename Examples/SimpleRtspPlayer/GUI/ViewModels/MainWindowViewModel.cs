using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using Grpc.Core;
using GrpcStream;
using RtspClientSharp;
using RtspClientSharp.Openalpr;
using Rtspstream;
using SimpleRtspPlayer.GUI.Models;

namespace SimpleRtspPlayer.GUI.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string RtspPrefix = "rtsp://";
        private const string HttpPrefix = "http://";

        private string _status = string.Empty;
        private readonly IMainWindowModel _mainWindowModel;
        private bool _startButtonEnabled = true;
        private bool _stopButtonEnabled;

        //public string DeviceAddress { get; set; } = "rtsp://184.72.239.149/vod/mp4:BigBuckBunny_175k.mov";  // Example
        //public string DeviceAddress { get; set; } = "rtsp://david:123456@vandilab.iptime.org:554//h264Preview_01_main";
        public string DeviceAddress { get; set; } = "rtsp://vandilab.iptime.org:554//h264Preview_01_main";

        public string Login { get; set; } = "david";
        public string Password { get; set; } = "123456";

        public IVideoSource VideoSource => _mainWindowModel.VideoSource;

        /// <summary>
        /// Image of Right Top
        /// </summary>
        private ImageSource grpcImageRT;
        public ImageSource ImageRT
        {
            get { return grpcImageRT; }
            set
            {
                if(grpcImageRT != value)
                {
                    grpcImageRT = value;
                    OnPropertyChanged("ImageRT");
                }
            }
        }

        /// <summary>
        /// Image of Left Bottom
        /// </summary>
        private ImageSource grpcImageLB;
        public ImageSource ImageLB
        {
            get { return grpcImageLB; }
            set
            {
                if (grpcImageLB != value)
                {
                    grpcImageLB = value;
                    OnPropertyChanged("ImageLB");
                }
            }
        }
        
        public RelayCommand StartClickCommand { get; }
        public RelayCommand StopClickCommand { get; }
        public RelayCommand<CancelEventArgs> ClosingCommand { get; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(IMainWindowModel mainWindowModel)
        {
            _mainWindowModel = mainWindowModel ?? throw new ArgumentNullException(nameof(mainWindowModel));
            
            StartClickCommand = new RelayCommand(OnStartButtonClick, () => _startButtonEnabled);
            StopClickCommand = new RelayCommand(OnStopButtonClick, () => _stopButtonEnabled);
            ClosingCommand = new RelayCommand<CancelEventArgs>(OnClosing);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnStartButtonClick()
        {
            string address = DeviceAddress;

            if (!address.StartsWith(RtspPrefix) && !address.StartsWith(HttpPrefix))
                address = RtspPrefix + address;

            if (!Uri.TryCreate(address, UriKind.Absolute, out Uri deviceUri))
            {
                MessageBox.Show("Invalid device address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var credential = new NetworkCredential(Login, Password);

            var connectionParameters = !string.IsNullOrEmpty(deviceUri.UserInfo) ? new ConnectionParameters(deviceUri) :
                new ConnectionParameters(deviceUri, credential);
            //var connectionParameters = new ConnectionParameters(deviceUri, credential);

            //connectionParameters.RtpTransport = RtpTransportProtocol.UDP;
            connectionParameters.CancelTimeout = TimeSpan.FromSeconds(1);

            _mainWindowModel.Start(connectionParameters);
            _mainWindowModel.StatusChanged += MainWindowModelOnStatusChanged;

            _startButtonEnabled = false;
            StartClickCommand.RaiseCanExecuteChanged();
            _stopButtonEnabled = true;
            StopClickCommand.RaiseCanExecuteChanged();
        }

        private void OnStopButtonClick()
        {
            _mainWindowModel.Stop();
            _mainWindowModel.StatusChanged -= MainWindowModelOnStatusChanged;

            _stopButtonEnabled = false;
            StopClickCommand.RaiseCanExecuteChanged();
            _startButtonEnabled = true;
            StartClickCommand.RaiseCanExecuteChanged();
            Status = string.Empty;
        }

        private void MainWindowModelOnStatusChanged(object sender, string s)
        {
            Application.Current.Dispatcher.Invoke(() => Status = s);
        }

        private void OnClosing(CancelEventArgs args)
        {
            _mainWindowModel.Stop();
        }
    }
}