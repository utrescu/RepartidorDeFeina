using System;
using RabbitMQ.Client;
using System.Text;
using Feines;

namespace newTask
{
    class Program
    {

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "No facis res!");
        }

        static void Main(string[] args)
        {
            IFeinesService broker = new FeinesService();

            broker.connecta("172.99.0.2");
            broker.creaCua("feina");

            string missatge;

            System.IO.StreamReader file = new System.IO.StreamReader(@"missatges.txt");
            while ((missatge = file.ReadLine()) != null)
            {
                Console.WriteLine("[{0}]  Enviant: {1}", DateTime.Now.ToLongTimeString(), missatge);
                broker.EnviaALaCua("feina", missatge);
            }

            file.Close();

            broker.desconnecta();
        }

    }
}
