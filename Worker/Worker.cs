﻿using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using Feines;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {

            IFeinesService broker = new FeinesService();
            broker.connecta("172.99.0.2");
            broker.creaCua("feina", 1);

            Console.WriteLine(" [*] Esperant feina.");

            var consumer = broker.EsperaFeina();
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [{0}] Fent la tasca: {1}", DateTime.Now.ToLongTimeString(), message);

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [{0}] Tasca acabada", DateTime.Now.ToLongTimeString());

                broker.FeinaAcabada(ea);

            };

            broker.RebreDeLaCua("feina", consumer);
            Console.WriteLine("Prem [enter] per sortir.");
            Console.ReadLine();
            broker.desconnecta();
        }
    }
}
