using RabbitMQ.Client.Events;

namespace Feines
{

    public interface IFeinesService
    {

        void connecta(string host);
        void desconnecta();
        void creaCua(string nom);
        void creaCua(string nom, int mida);
        void creaCuaBroadcast(string exchange, string cua);
        bool EnviaALaCua(string cua, string text);
        bool EnviaBroadcast(string cua, string text);

        EventingBasicConsumer EsperaMissatge(string cua);
        void MissatgeProcessat(BasicDeliverEventArgs ea);
    }
}
