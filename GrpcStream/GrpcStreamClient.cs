using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Rtspstream;

namespace GrpcStream
{
    public class GrpcStreamClient
    {
        Rtspstream.Rtspstream.RtspstreamClient client;
        public static EventHandler<StreamData> StreamReceieved { get;set; }
        public GrpcStreamClient(Rtspstream.Rtspstream.RtspstreamClient client)
        {
            this.client = client;
        }

        //private StreamData GetStreamData(AuthToken token)
        //{
        //    using (var call = client.GetStreaming(token))
        //    {
        //        var responseStream = call.ResponseStream;
        //        return responseStream.Current;
        //    }
        //}

        //public async Task GetStreaming(AuthToken token)
        //{
        //    try
        //    {
        //        using (var call = client.GetStreaming(token))
        //        {
        //            var responseStream = call.ResponseStream;

        //            while (await responseStream.MoveNext())
        //            {
        //                var streamData = responseStream.Current;
        //                //Console.WriteLine($"StreamData => {streamData.Image.Length}");
        //                StreamReceieved?.Invoke(this, streamData);
        //            }
        //        }
        //    }
        //    catch (RpcException e)
        //    {
        //        Console.WriteLine("GetStreamingRPC failed : " + e.Message);
        //    }
        //}

        public void GetStreaming(AuthToken token)
        {
            var streamData = client.GetStreaming(token);
            if (streamData != null)
            {
                StreamReceieved?.Invoke(this, streamData);
            }
            else
            {
                Console.WriteLine("GetSimple token is NULL");
            }
        }
        
        //public async Task GetSimpleStream(AuthToken token)
        //{
        //    try
        //    {
        //        using (var call = client.GetSimpleStream(token))
        //        {
        //            var responseStream = call.ResponseStream;
        //            StringBuilder responseLog = new StringBuilder("Result => ");

        //            while (await responseStream.MoveNext())
        //            {
        //                var streamData = responseStream.Current;
        //                responseLog.Append("\n" + streamData.ToString());
        //            }
        //            Console.WriteLine(responseLog.ToString());
        //        }
        //    }
        //    catch (RpcException e)
        //    {
        //        Console.WriteLine("GetSimpleStreamRPC failed : " + e.Message);
        //    }
        //}

        //public async Task GetSimple()
        //{
        //    var result = await client.GetSimpleAsync(new AuthToken { Token = "clienttoken2" });
        //    if (result != null)
        //    {
        //        Console.WriteLine($"GetSimple token is {result.Token}");
        //    }
        //    else
        //    {
        //        Console.WriteLine("GetSimple token is NULL");
        //    }
        //}
    }
}
