using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using Feines;

namespace Spectator
{
    class Program
    {
        private const string Host = "172.99.0.2";
        private const string NomExchange = "resultats";
        private const string MESSAGE_WAIT = " [*] Esperant informes.";
        private const string MESSAGE_END_PROGRAM = "Prem [enter] per sortir.";

        static void Main(string[] args)
        {

            IFeinesService broker = new FeinesService();
            broker.Connecta(Host);
            string cua = broker.SubscriuABroadcast(NomExchange);
            Console.WriteLine(MESSAGE_WAIT);

            var consumer = broker.EsperaMissatge(cua);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("{0} Feina Acabada: {1}", DateTime.Now.ToLongTimeString(), message);
            };

            Console.WriteLine(MESSAGE_END_PROGRAM);
            Console.ReadLine();
            broker.Desconnecta();
        }
    }
}



