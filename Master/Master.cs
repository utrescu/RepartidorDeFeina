using System;
using RabbitMQ.Client;
using System.Text;
using Feines;
using System.IO;

namespace newTask
{
    class Program
    {
        private const string NomCua = "feina";
        private const string Host = "172.99.0.2";
        private const string FitxerPerDefecte = "missatges.txt";

        private static string getFileFromArgs(string[] args)
        {

            if (args.Length == 0)
            {
                return FitxerPerDefecte;
            }

            if (args.Length != 1)
            {
                throw new ApplicationException("S'ha de passar UN sol paràmetre amb el nom del fitxer");
            }

            var fitxer = args[0];

            if (!File.Exists(fitxer))
            {
                throw new FileNotFoundException();
            }

            return fitxer;
        }

        static void Main(string[] args)
        {

            try
            {
                var fitxer = getFileFromArgs(args);


                IFeinesService broker = new FeinesService();

                broker.connecta(Host);
                broker.creaCua(NomCua);

                string missatge;


                System.IO.StreamReader file = new System.IO.StreamReader(fitxer);
                while ((missatge = file.ReadLine()) != null)
                {
                    Console.WriteLine("[{0}]  Enviant: {1}", DateTime.Now.ToLongTimeString(), missatge);
                    broker.EnviaALaCua(NomCua, missatge);
                }

                file.Close();
                broker.desconnecta();
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("El fitxer de missatges '{0}' no existeix", args[0]);
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
