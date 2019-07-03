using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rtspstream;
using GrpcStream;

namespace SampleClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ////const string host = "127.0.0.1";
            const string host = "localhost";
            const int Port = 1004;

            var channel = new Channel($"{host}:{Port}", ChannelCredentials.Insecure);
            var client = new GrpcStreamClient(new Rtspstream.Rtspstream.RtspstreamClient(channel));

            //client.GetStreaming(new AuthToken() { Token = "streamingtoken_1" }).Wait();
            client.GetStreaming(new AuthToken() { Token = 1 });

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    
    }

    
}
