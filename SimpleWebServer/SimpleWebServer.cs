using System;
using System.IO;
using System.Net;

namespace MB.Web
{
    /// <summary>
    /// Simple web server. Serves the contents of a specified
    /// directory to an address plus port.
    /// </summary>
    public class SimpleWebServer
    {
        private string _rootDir;
        private SimpleHttpListener _listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebServer"/> class.
        /// Does not start the server automatically.
        /// If no file name is provided, index.html is returned (if it exists).
        /// </summary>
        /// <param name="prefix">The htpp address prefix.</param>
        /// <param name="rootDir">The root directory to serve the files from.</param>
        public SimpleWebServer(string prefix, string rootDir)
        {
            _rootDir = rootDir;

            _listener = new SimpleHttpListener(prefix)
            {
                OnReceivedRequest = OnReceivedRequest,
            };  
        }

        /// <summary>
        /// Starts the web server.
        /// </summary>
        public void Start()
        {
            _listener.Start();
        }

        private void OnReceivedRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var fileName = request.Url.AbsolutePath;

            Console.WriteLine("request: {0}", fileName);

            fileName = fileName.Substring(1); 
       
            // if no filename is given, use index.html
            if (string.IsNullOrEmpty(fileName))
                fileName = "index.html";
            else
            {
                var parts = fileName.Split('/');
                if (parts.Length > 0 && string.IsNullOrEmpty(parts[parts.Length - 1]))
                    fileName = Path.Combine(fileName, "index.html");
            }

            fileName = Path.Combine(_rootDir, fileName);

            response.ContentType = MimeHelper.GetMimeType(fileName);

            FileStream fileStream = null;

            try
            {
                fileStream = new FileStream(fileName, FileMode.Open);

                var buffer = new byte[1024 * 16];
                int nbytes;

                while ((nbytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    response.OutputStream.Write(buffer, 0, nbytes);
            }
            catch (Exception e)
            {
                Console.WriteLine("error serving file", e);
                response.StatusCode = 404;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                response.OutputStream.Close();
            }
        }
    }
}

