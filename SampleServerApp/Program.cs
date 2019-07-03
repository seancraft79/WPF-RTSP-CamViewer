using Grpc.Core;
using Rtspstream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SampleServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServer();
        }

        class RtspStreamImpl : Rtspstream.Rtspstream.RtspstreamBase
        {
            public async override Task GetStreaming(AuthToken request, IServerStreamWriter<StreamData> responseStream, ServerCallContext context)
            {
                var projectPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\")));

                int index = 0;
                while(true)
                {
                    string path = Path.Combine(projectPath, "img", $"car{index++}.png");
                    if(index > 5) index = 0;
                    var imgData = await File.ReadAllBytesAsync(path);
                    var data = new StreamData();
                    data.Authtoken = new AuthToken { Token = "servertoken1" };
                    data.Filetype = new FileType { Type = 1 };
                    data.Image = Google.Protobuf.ByteString.CopyFrom(imgData);

                    await responseStream.WriteAsync(data);
                    Console.WriteLine($"Server sent => {path}");
                    Thread.Sleep(1000);
                }
                
            }

            public async override Task GetSimpleStream(AuthToken request, IServerStreamWriter<AuthToken> responseStream, ServerCallContext context)
            {
                List<AuthToken> tokens = new List<AuthToken>();
                for (int i = 0; i < 10; i++)
                {
                    var t = new AuthToken { Token = $"req:{request.Token}:res:servertoken_{i}" };
                    tokens.Add(t);
                }
                foreach(var token in tokens)
                {
                    await responseStream.WriteAsync(token);
                }
            }

            public override Task<AuthToken> GetSimple(AuthToken request, ServerCallContext context)
            {
                string token = $"req:{request.Token}:res:servertoken_s";
                
                return Task.FromResult(new AuthToken { Token = token});
            }
        }

        static void StartServer()
        {
            const int Port = 1004;

            Server server = new Server
            {
                Services = { Rtspstream.Rtspstream.BindService(new RtspStreamImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine($"=== Grpc server listening on port : {Port}");
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
