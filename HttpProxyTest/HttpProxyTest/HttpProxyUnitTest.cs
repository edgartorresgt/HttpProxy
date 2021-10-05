using HttpProxy.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HttpProxyTest
{
    public class HttpProxyUnitTest
    {
        [Fact]
        public void integrationTest_ShouldReturnResponse_OK()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<HttpListenerProxyService>>();
            mockLogger.Setup(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()));

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => mockLogger.Object);

            var listenerProxyService = new HttpListenerProxyService(mockLoggerFactory.Object);
            listenerProxyService.CreateProxy(8888);
            
            //act
            Task.Run(() =>
                {
                    using var task = listenerProxyService.StartProxy();
                    using var httpClient = new HttpClient();
                    var response = httpClient.GetAsync("http://google.com").Result;
                    var actualContent = response.Content.ReadAsStringAsync().Result;

                    //Assert
                    Assert.Equal("-(Header) Host  = google.com<br />", actualContent);
                }
            );
        }

    }
}
