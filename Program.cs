using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace MSI
{
    class Program
    {
        static void Main(string[] args)
        {
            string defaultPath = "../../../conf.xml";
            Configuration conf;
            try
            {
                if (args.Length < 2)
                {
                    conf = GetConfiguration(defaultPath);
                }
                else
                {
                    conf = GetConfiguration(args[1]);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Nie znaleziono podanego pliku!");
                return;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Ścieżka do pliku nie jest poprawna");
                return;
            }
            Strips strips = new Strips();
            strips.LoadFromFile(conf.PathToProblem);
            GA gA = new GA(strips, conf.PopulationSize, 10);
            gA.InitializePopulation();
            for (int i = 0; i < conf.MaxIterations; i++)
            {
                gA.DoIteration();
                if (gA.GetSolutionIfFound() != null)
                {
                    strips.SaveToFile(conf.OutputPath, gA.GetSolutionIfFound());
                    Console.WriteLine("Rozwiązanie zapisano do pliku wyjściowego");
                    return;
                }
            }
            Console.WriteLine("Nie znaleziono rozwiązania!");
        }
        public static Configuration GetConfiguration(string path)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Configuration));
            StreamReader rd = new StreamReader(path);
            return (Configuration)xml.Deserialize(rd);
        }
    }
}
