using HttpProxy.Interfaces;
using HttpProxy.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HttpProxy
{
    class Program
    {
        private const int Port = 8888;
        private static IHttpListenerProxy _httpListenerProxy;

        static void Main()
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddLogging(config => config
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug))
                .AddSingleton<IHttpListenerProxy, HttpListenerProxyService>()
                .BuildServiceProvider();

            //configure console logging
            serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            //configure Proxy Server
            _httpListenerProxy = serviceProvider.GetService<IHttpListenerProxy>();

            //Initializing listener 
            _httpListenerProxy?.CreateProxy(Port);
            var startListenerProxyTask = Task.Run(() =>
                _httpListenerProxy?.StartProxy()
            );
            Task.WaitAll(startListenerProxyTask);
        }
    }
}