using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "172.99.0.2" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "feina",
                            durable: true, // defineix si la cua perdura a les aturades del servidor
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                    Console.WriteLine(" [*] Esperant feina.");

                    // Evitar que li donin més d'una tasca alhora
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Fent la tasca: {0}", message);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);

                        Console.WriteLine(" [x] Tasca acabada");

                        // Confirma que la tasca ha estat feta correctament (només cal si autoAck:false perquè llavors
                        // el productor no l'ha eliminat de les tasques a fer)
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    // Auto ACK fa que els missatges es confirmin automàticament (el originador l'elimina de la cua
                    // de tasques immediatament a menys que el defineixi com a false)
                    channel.BasicConsume(queue: "feina", autoAck: false, consumer: consumer);

                    Console.WriteLine("Prem [enter] per sortir.");
                    Console.ReadLine();
                }
            }
        }
    }
}
