using System;
using RabbitMQ.Client;
using System.Text;
using Feines;
using System.Collections.Generic;

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
            List<string> missatges = new List<string> {
                "Construir un avió de paper....",
                "Pintar el paper........",
                "Agafar el barret......",
                "Mirar la hora....",
                "Prendre la Bastilla..................",
                "Menjar ........ ",
                "Mirar si plou.........."
            };

            IFeinesService broker = new FeinesService();

            broker.connecta("172.99.0.2");
            broker.creaCua("feina");

            foreach (var missatge in missatges)
            {
                Console.WriteLine("[*] Enviant: " + missatge);
                broker.EnviaFeina("feina", missatge);
            }

            broker.desconnecta();
        }

    }
}
