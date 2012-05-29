using System;
using System.Net;
using System.Threading.Tasks;

namespace MB.Web
{
    /// <summary>
    /// Simple http server which listens at a provided address and port.
    /// </summary>
    public class SimpleHttpListener
    {
        private HttpListener _listener;
        private bool _isRunning;
        private bool _isStopping;
        private string _prefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleHttpListener"/> class.
        /// </summary>
        /// <param name="prefix">The htpp address prefix.</param>
        public SimpleHttpListener(string prefix)
        {
            _prefix = prefix;
        }

        /// <summary>
        /// Starts the http server. Internally, one async task is used for all requests.
        /// </summary>
        public void Start()
        {
            if (_isRunning) return;

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("ERROR! Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            if (_prefix == null)
            {
                Console.WriteLine("ERROR! prefix missing");
                return;
            }

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add(_prefix);
                _listener.Start();

                _isRunning = true;
            }
            catch (HttpListenerException ex) 
            {
                Console.WriteLine("ERROR! http listener at {0} could not be started", _prefix);
                Console.WriteLine("make sure the user has the rights for this port or run as administrator");
                Console.WriteLine(ex);
                
                _isRunning = false;
            }

            if (_isRunning)
            {
                Console.WriteLine("started at " + _prefix);

                Task.Factory.StartNew(() =>
                {
                    while (!_isStopping)
                    {
                        var context = _listener.GetContext();
                        var request = context.Request;
                        var response = context.Response;                        

                        //Console.WriteLine("KeepAlive: {0}", request.KeepAlive);
                        //Console.WriteLine("Local end point: {0}", request.LocalEndPoint.ToString());
                        //Console.WriteLine("Remote end point: {0}", request.RemoteEndPoint.ToString());
                        //Console.WriteLine("Is local? {0}", request.IsLocal);
                        //Console.WriteLine("HTTP method: {0}", request.HttpMethod);
                        //Console.WriteLine("Protocol version: {0}", request.ProtocolVersion);
                        //Console.WriteLine("Is authenticated: {0}", request.IsAuthenticated);
                        //Console.WriteLine("Is secure: {0}", request.IsSecureConnection);                        

                        if (OnReceivedRequest != null)
                            OnReceivedRequest(request, response);
                    }

                    _isRunning = false;
                    _isStopping = false;
                    _listener.Close();

                    Console.WriteLine("stopped");

                    if (OnStopped != null)
                        OnStopped();
                });
            }
        }

        /// <summary>
        /// Stops the http server.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning || _isStopping) return;
            _isStopping = true;
        }

        /// <summary>
        /// Gets or sets the on OnReceivedRequest callback action.
        /// </summary>
        public Action<HttpListenerRequest, HttpListenerResponse> OnReceivedRequest 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the OnStopped callback action.
        /// </summary>
        public Action OnStopped 
        { 
            get; 
            set;
        }
    }
}

