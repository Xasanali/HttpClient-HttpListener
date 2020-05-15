using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientSide
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var tasks = new List<Task>();
            string url = "http://localhost:8888/";
            SendRequest(client, url).GetAwaiter().GetResult();
        }

        private static async Task SendRequest(HttpClient client, string url)
        {
            try
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                var path1 = Directory.GetParent(System.IO.Directory.GetCurrentDirectory());
                var path3 = AppDomain.CurrentDomain.FriendlyName;
                var parentPath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
                string path = Path.Combine($"{parentPath}", "App_Data", "movies.mp4");

                using (FileStream fstream = File.OpenRead($"{path}"))
                {
                    byte[] array = new byte[fstream.Length];
                    var file = new ByteArrayContent(array, 0, array.Length);
                    content.Add(file, "test", "movies.mp4");

                    // await Upload(url, "test", fstream, array);

                    var s = fstream.Read(array, 0, array.Length);
                }

                using (client = new HttpClient())
                {
                    var result = await client.PostAsync(url, content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    Console.WriteLine(resultContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task<Stream> Upload(string actionUrl, string paramString, FileStream paramFileStream, byte[] paramFileBytes)
        {
            HttpContent stringContent = new StringContent(paramString);
            HttpContent fileStreamContent = new StreamContent(paramFileStream);
            HttpContent bytesContent = new ByteArrayContent(paramFileBytes);
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(stringContent, "param1", "param1");
                formData.Add(fileStreamContent, "file1", "file1");
                formData.Add(bytesContent, "file2", "file2");
                var response = await client.PostAsync(actionUrl, formData);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                return await response.Content.ReadAsStreamAsync();
            }
        }

    }
}
