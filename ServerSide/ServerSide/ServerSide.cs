using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using HttpMultipartParser;
using System.Text;
using System.Net.Http;

namespace ServerSide
{
    public class ServerSide
    {

        public async Task StartAsServer()
        {
            await Listen();
        }

        private async Task Listen()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8888/");
            listener.Start();
            Console.WriteLine("Waiting connecting...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                await SaveRequestData(request);

                //await ResponseToClientString(response);
                // or
                await ResponseToClientStream(response);
            }
        }

        public async Task SaveRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return;
            }

            if (request.ContentType != null)
            {
                Console.WriteLine("Client data content type {0}", request.ContentType);
            }
            Console.WriteLine("Client data content length {0}", request.ContentLength64);

            Console.WriteLine("Start of client data:");

            Stream body = request.InputStream;
            Encoding encoding = request.ContentEncoding;
            StreamReader reader = new StreamReader(body, encoding);

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var path1 = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName;
            var path2 = AppDomain.CurrentDomain.FriendlyName;

            var parser = new MultipartFormDataParser(body);
            foreach (var file in parser.Files)
            {
                Stream data = file.Data;
                path = Path.Combine($"{path}", "App_Data/ServerSide", $"{file.FileName}");
                using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
                {
                    await data.CopyToAsync(outputFileStream);
                }
            }
            body.Close();
            reader.Close();
        }


        public async Task ResponseToClientString(HttpListenerResponse response)
        {
            string responseString = "<html><head><meta charset='utf8'></head><body>Hello World!</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
        }

        public async Task ResponseToClientStream(HttpListenerResponse response)
        {

            try
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                var path1 = Directory.GetParent(System.IO.Directory.GetCurrentDirectory());
                var path3 = AppDomain.CurrentDomain.FriendlyName;
                var parentPath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
                string path = Path.Combine($"{parentPath}", "App_Data/ServerSide", "VideoForClient.mp4");

                using (FileStream fstream = File.OpenRead($"{path}"))
                {
                    byte[] array = new byte[fstream.Length];
                    var file = new ByteArrayContent(array, 0, array.Length);
                    content.Add(file, "test", "VideoForClient.mp4");

                    // await Upload(url, "test", fstream, array);

                    //var s = fstream.Read(array, 0, array.Length);

                    //To Do this
                    //response.ContentType
                    //response.ContentEncoding
                    //response.ContentLength64
                    response.ContentLength64 = array.Length;
                    response.ContentType = "application/octet-stream";

                    await response.OutputStream.WriteAsync(array, 0, array.Length);

                    response.OutputStream.Close();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
