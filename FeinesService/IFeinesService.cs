using RabbitMQ.Client.Events;

namespace Feines
{

    public interface IFeinesService
    {

        void connecta(string host);
        void desconnecta();
        void creaCua(string nom);
        void creaCua(string nom, int mida);
        bool EnviaALaCua(string cua, string text);

        EventingBasicConsumer EsperaFeina();
        void RebreDeLaCua(string cua, EventingBasicConsumer consumer);
        void FeinaAcabada(BasicDeliverEventArgs ea);
    }
}
