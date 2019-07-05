using Grpc.Core;
using GrpcStream;
using Rtspstream;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SimpleRtspPlayer.GUI.Views
{
    /// <summary>
    /// Interaction logic for GrpcStreamView.xaml
    /// </summary>
    public partial class GrpcStreamView : UserControl
    {
        private int CHANNEL = 0;
        //const string HOST = "localhost";
        const string HOST = "192.168.101.200";
        //const string HOST = "223.171.38.179";
        const int PORT = 1004;

        const int FRAME_SLEEP = 1;   // 30

        GrpcStreamClient client;
        Thread thread;

        private bool canStream = false;

        public GrpcStreamView()
        {
            CHANNEL = ViewModelLocator.VIEW_INDEX;
            Console.WriteLine($"===== GrpcStreamView {ViewModelLocator.VIEW_INDEX++} =====");
            InitializeComponent();
            TxtHost.Text = HOST;
            TxtPort.Text = "" + PORT;
        }

        void StartStreaming()
        {
            string host = TxtHost.Text;
            string port = TxtPort.Text;
            var channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);
            client = new GrpcStreamClient(new Rtspstream.Rtspstream.RtspstreamClient(channel));

            GrpcStreamClient.StreamReceieved += (Object sender, StreamData streamData) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    bool isMyChannel = streamData.Channel == CHANNEL;
                    //Console.WriteLine($"ImageData received {CHANNEL} ===> token : {receivedToken}, isChannel : {isThisToken}");

                    if (streamData != null && streamData.Image.Length > 0 && canStream && isMyChannel)
                    {
                        var bitmapimg = new BitmapImage();
                        using (var mem = new MemoryStream(streamData.Image.ToByteArray()))
                        {
                            mem.Position = 0;
                            bitmapimg.BeginInit();
                            bitmapimg.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            bitmapimg.CacheOption = BitmapCacheOption.OnLoad;

                            bitmapimg.UriSource = null;
                            bitmapimg.StreamSource = mem;
                            bitmapimg.EndInit();
                        }
                        bitmapimg.Freeze();
                        if (bitmapimg != null)
                        {
                            Console.WriteLine($"     ===> received {CHANNEL} : len : {streamData.Image.Length},  width : {bitmapimg.PixelWidth}, hegith : {bitmapimg.PixelHeight}");
                            StreamImage.Source = bitmapimg;
                        }
                        else Console.WriteLine("Iamge received NULL");
                    }

                }));
            };

            thread = new Thread(() =>
            {             
                while (canStream)
                {
                    try
                    {
                        Console.WriteLine($"<=== Get streamData {CHANNEL}");
                        client.GetStreaming(new AuthToken() { Token = $"ct-" + DateTime.Now });
                        //Thread.Sleep(FRAME_SLEEP);
                    }
                    catch (TaskCanceledException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (ObjectDisposedException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

            });

            thread.Start();

            Console.WriteLine($"=====> StartStreaming [V_INDEX : {CHANNEL}], [CurrentThread : {Thread.CurrentThread.ManagedThreadId}], [Thread : {thread.ManagedThreadId}]");
        }

        private void StartButtonClicked(object sender, RoutedEventArgs e)
        {
            canStream = true;
            StartStreaming();
        }

        private void StopButtonClicked(object sender, RoutedEventArgs e)
        {
            canStream = false;
            if(thread != null) thread.Interrupt();
            
            Button b = sender as Button;
            if(b != null)
            {
                Console.WriteLine($"StopBtnClicked viNDEX : {CHANNEL}");

                DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)sender);
                if (parent == null)
                    parent = LogicalTreeHelper.GetParent((DependencyObject)sender);
                if (parent != null)
                    Console.WriteLine("Parent name : " + parent.ToString());
            }
        }

        public static int ParseStringToInt(string value)
        {
            int result = -1;
            return int.TryParse(value, out result) ? result : -1;
        }

        public static int ParseStringToInt(string value, int defaultValue)
        {
            int result = -1;
            return int.TryParse(value, out result) ? result : defaultValue;
        }
    }
}
