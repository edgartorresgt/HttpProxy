using System.Threading.Tasks;

namespace HttpProxy.Interfaces
{
    public interface IHttpListenerProxy
    {
        void CreateProxy(int port);
        Task StartProxy();
    }
}