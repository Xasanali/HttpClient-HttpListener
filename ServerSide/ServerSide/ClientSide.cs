using HttpMultipartParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    public class ClientSide
    {
        public async Task StartAsClient()
        {
            var client = new HttpClient();
            var tasks = new List<Task>();
            string serverUrl = "http://localhost:8888/";
            await SendRequest(client, serverUrl);

            //Task.WaitAll(tasks.ToArray());
        }

        private async Task SendRequest(HttpClient client, string url)
        {
            try
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                var path1 = Directory.GetParent(System.IO.Directory.GetCurrentDirectory());
                var path3 = AppDomain.CurrentDomain.FriendlyName;
                var parentPath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
                string path = Path.Combine($"{parentPath}", "App_Data/ClientSide", "MoviesForServer.mp4");

                using (FileStream fstream = File.OpenRead($"{path}"))
                {
                    byte[] array = new byte[fstream.Length];
                    var file = new ByteArrayContent(array, 0, array.Length);
                    content.Add(file, "test", "MoviesForServer.mp4");

                    // await Upload(url, "test", fstream, array);

                    var s = fstream.Read(array, 0, array.Length);
                }

                using (client = new HttpClient())
                {
                    var response = await client.PostAsync(url, content);
                    await SaveResponseData(response);
                    //string resultContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(resultContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task SaveResponseData(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("No client data was sent with the request.");
                return;
            }

            Console.WriteLine("Start of save server data:");

            Stream body = await response.Content.ReadAsStreamAsync();
            byte[] bodyByteArray = await response.Content.ReadAsByteArrayAsync();
            string bodyString = await response.Content.ReadAsStringAsync();
            //Encoding encoding = (Encoding)response.Content.Headers.ContentEncoding;
            StreamReader reader = new StreamReader(body);

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var path1 = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName;
            var path2 = AppDomain.CurrentDomain.FriendlyName;

            //Stream data = body;
            var parser = new MultipartFormDataParser(body);
            foreach (var file in parser.Files)
            {
                Stream data = file.Data;
                path = Path.Combine($"{path}", "App_Data/ClientSide", $"{Guid.NewGuid()}-{file.FileName}");
                using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
                {
                    await data.CopyToAsync(outputFileStream);
                }
            }

            //using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
            //{
            //    await data.CopyToAsync(outputFileStream);
            //}

            body.Close();
            reader.Close();
        }

        private async Task<Stream> Upload(string actionUrl, string paramString, FileStream paramFileStream, byte[] paramFileBytes)
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
