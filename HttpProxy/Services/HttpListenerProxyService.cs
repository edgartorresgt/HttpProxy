using HttpProxy.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpProxy.Services
{
    public class HttpListenerProxyService : IHttpListenerProxy
    {
        private HttpListener _httpListener;
        private readonly ILogger<HttpListenerProxyService> _logger;

        public HttpListenerProxyService(ILoggerFactory loggerFactory)
        {

            _logger = loggerFactory.CreateLogger<HttpListenerProxyService>();
        }

        public void CreateProxy(int port)
        {
            _logger.LogInformation($"Initializing listener http://127.0.0.1:{port}/");
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://*:{port}/");
        }

        public Task StartProxy()
        {
            return Task.Run(() =>
            {
                try
                {

                    if (!HttpListener.IsSupported)
                    {
                        _logger.LogWarning("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                        return;
                    }

                    _httpListener.Start();
                    _logger.LogInformation("Start Listening…");
                    _logger.LogInformation($"Is listening: {_httpListener.IsListening}");
                    while (true)
                    {
                        //Waits for an incoming request and returns when one is received.
                        var context = _httpListener.GetContext();
                        var requestString = context.Request.Url;
                        var responseString = string.Empty;

                        _logger.LogInformation("Got request for " + requestString);

                        //Building response
                        foreach (var key in context.Request.Headers.AllKeys)
                        {
                            responseString += $"-(Header) {key}  = {context.Request.Headers[key]}" +
                                              Environment.NewLine;
                        }

                        _logger.LogInformation($"Log response to browser: {responseString}");

                        // Obtain a response object.
                        var response = context.Response;
                        // Construct a response.
                        var buffer = Encoding.UTF8.GetBytes(responseString.Replace(Environment.NewLine, "<br />"));
                        // Get a response stream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        var output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        // You must close the output stream.
                        output.Close();
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError("Listener Exception");
                    _logger.LogError("Source :{0} ", e.Source);
                    _logger.LogError("Message :{0} ", e.Message);
                }
                finally
                {
                    _logger.LogInformation("Stopping listener");
                    _httpListener.Stop();
                    
                }
            });
        }
    }

}