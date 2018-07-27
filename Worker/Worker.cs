using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using Feines;

namespace Worker
{
    class Program
    {
        private const string Host = "172.99.0.2";
        private const string NomCua = "feina";
        private const string MESSAGE_WAIT = " [*] Esperant feina.";
        private const string MESSAGE_WORK_START = " [{0}] Fent la tasca: {1}";
        private const string MESSAGE_WORK_END = " [{0}] Tasca acabada";
        private const string MESSAGE_END_PROGRAM = "Prem [enter] per sortir.";

        static void Main(string[] args)
        {

            IFeinesService broker = new FeinesService();
            broker.Connecta(Host);
            broker.CreaCua(NomCua, 1);

            Console.WriteLine(MESSAGE_WAIT);

            var consumer = broker.EsperaMissatge(NomCua);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(MESSAGE_WORK_START, DateTime.Now.ToLongTimeString(), message);

                // Fer veure que estem fent una tasca que dura un rato
                // ho fem segons els punts ...
                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(MESSAGE_WORK_END, DateTime.Now.ToLongTimeString());

                broker.MissatgeProcessat(ea);

            };

            Console.WriteLine(MESSAGE_END_PROGRAM);
            Console.ReadLine();
            broker.Desconnecta();
        }
    }
}
