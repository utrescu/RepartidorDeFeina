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

        public void Connecta(string host)
        {
            factory = new ConnectionFactory() { HostName = "172.99.0.2" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Desconnecta()
        {
            channel.Close();
            connection.Close();
        }

        public void CreaCua(string nom)
        {
            channel.QueueDeclare(queue: nom,
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);
        }

        public void CreaCua(string nom, int mida)
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

        ///
        /// Envia un missatge directe a la cua
        ///
        public bool EnviaALaCua(string cua, string missatge)
        {
            Envia("", cua, missatge);
            return true;
        }

        public EventingBasicConsumer EsperaMissatge(string cua)
        {
            var consumer = new EventingBasicConsumer(channel);
            // Auto ACK fa que els missatges es confirmin automàticament (el originador l'elimina de la cua
            // de tasques immediatament a menys que el defineixi com a false)
            channel.BasicConsume(queue: cua, autoAck: false, consumer: consumer);
            return consumer;
        }

        public void MissatgeProcessat(BasicDeliverEventArgs ea)
        {
            // Confirma que la tasca ha estat feta correctament (només cal si autoAck:false perquè llavors
            // el productor no l'ha eliminat de les tasques a fer)
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        // Broadcast ------

        public void CreaBroadcast(string exchange, string cua)
        {
            // Amb els exchange de tipus fanout s'envia tot el rebut a tothom
            // que l'escolti
            channel.ExchangeDeclare(exchange, "fanout");

        }

        public bool EnviaBroadcast(string exchange, string missatge)
        {
            Envia(exchange, "", missatge);
            return true;
        }

        public string SubscriuABroadcast(string exchange)
        {
            channel.ExchangeDeclare(exchange, "fanout");

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: exchange,
                              routingKey: "");
            return queueName;
        }

        private void Envia(string exchange, string cua, string missatge)
        {

            var body = Encoding.UTF8.GetBytes(missatge);

            // Fer que els missatges siguin persistents a disc
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: exchange,
                                 routingKey: cua,
                                 basicProperties: properties,
                                 body: body);
        }
    }
}