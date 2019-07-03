using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace RtspClientSharp.Openalpr
{
    public class OpenAlpr
    {
        const string SECRET_KEY = "sk_ec3130ea716adc676f68876e";

        public static async Task<string> GetAlprInfo(string image_path)
        {
            Byte[] bytes = File.ReadAllBytes(image_path);
            return await GetAlprInfo(bytes);
        }

        public static async Task<string> GetAlprInfo(Byte[] bytes)
        {
            string imagebase64 = Convert.ToBase64String(bytes);

            var content = new StringContent(imagebase64);

            HttpClient client = new HttpClient();

            var response = await client.PostAsync("https://api.openalpr.com/v2/recognize_bytes?recognize_vehicle=1&country=kr&secret_key=" + SECRET_KEY, content).ConfigureAwait(false);

            var buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var byteArray = buffer.ToArray();
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            return responseString;
        }
    }
}
