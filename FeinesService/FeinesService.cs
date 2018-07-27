using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace Feines
{
    public class FeinesService : IFeinesService
    {
        ConnectionFactory factory;
        IConnection connection;
        IModel channel;

        public FeinesService()
        {


        }

        public void connecta(string host)
        {
            factory = new ConnectionFactory() { HostName = "172.99.0.2" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void desconnecta()
        {
            channel.Close();
            connection.Close();
        }

        public void creaCua(string nom)
        {
            channel.QueueDeclare(queue: nom,
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);
        }

        public void creaCua(string nom, int mida)
        {
            channel.QueueDeclare(queue: nom,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            // Això li he de passar
            // Evitar que li donin més d'una tasca alhora
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        public bool EnviaALaCua(string cua, string missatge)
        {
            var body = Encoding.UTF8.GetBytes(missatge);

            // Fer que els missatges siguin persistents a disc
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: cua,
                                 basicProperties: properties,
                                 body: body);
            return true;
        }

        public EventingBasicConsumer EsperaFeina()
        {
            return new EventingBasicConsumer(channel);
        }
        public void RebreDeLaCua(string cua, EventingBasicConsumer consumer)
        {
            // Auto ACK fa que els missatges es confirmin automàticament (el originador l'elimina de la cua
            // de tasques immediatament a menys que el defineixi com a false)
            channel.BasicConsume(queue: cua, autoAck: false, consumer: consumer);
        }

        public void FeinaAcabada(BasicDeliverEventArgs ea)
        {
            // Confirma que la tasca ha estat feta correctament (només cal si autoAck:false perquè llavors
            // el productor no l'ha eliminat de les tasques a fer)
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}