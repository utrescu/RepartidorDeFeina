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
        private const string PARAM_ERROR = "S'ha de passar UN sol paràmetre amb el nom del fitxer";
        private const string FILE_ERROR = "El fitxer de missatges '{0}' no existeix";
        private const string MESSAGE_SEND = "[{0}]  Enviant: {1}";

        private static string getFileFromArgs(string[] args)
        {

            if (args.Length == 0)
            {
                return FitxerPerDefecte;
            }

            if (args.Length != 1)
            {
                throw new ApplicationException(PARAM_ERROR);
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

                broker.Connecta(Host);
                broker.CreaCua(NomCua);

                string missatge;


                System.IO.StreamReader file = new System.IO.StreamReader(fitxer);
                while ((missatge = file.ReadLine()) != null)
                {
                    Console.WriteLine(MESSAGE_SEND, DateTime.Now.ToLongTimeString(), missatge);
                    broker.EnviaALaCua(NomCua, missatge);
                }

                file.Close();
                broker.Desconnecta();
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine(FILE_ERROR, args[0]);
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
