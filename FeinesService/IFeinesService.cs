using RabbitMQ.Client.Events;

namespace Feines
{

    public interface IFeinesService
    {

        void Connecta(string host);
        void Desconnecta();
        void CreaCua(string nom);
        void CreaCua(string nom, int mida);
        bool EnviaALaCua(string cua, string text);

        void CreaBroadcast(string exchange, string cua);
        bool EnviaBroadcast(string cua, string text);
        string SubscriuABroadcast(string exchange);

        EventingBasicConsumer EsperaMissatge(string cua);
        void MissatgeProcessat(BasicDeliverEventArgs ea);
    }
}
