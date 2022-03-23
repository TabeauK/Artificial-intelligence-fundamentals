using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSI
{
    public class GA
    {
        private Strips strips;
        private int PopulationSize { get; }
        private int ChromosomeMaxSize { get;  }
        private double probabilityOfMutation = 0.05;
        private double probabilityOfAddNewGeneInMutation = 0.2;
        private double probabilityOfCrossing = 0.5;
        private double probabilityOfCompression = 0.05;
        private int iterationCounter;

        private List<List<int>> chromosomes;

        public GA(Strips strips, int populationSize, int chromosomeMaxSize)
        {
            PopulationSize = populationSize;
            ChromosomeMaxSize = chromosomeMaxSize;
            this.strips = strips;
            iterationCounter = 0;
        }

        public void InitializePopulation() 
        {
            Random random = new Random();
            chromosomes = new List<List<int>>();
            for (int i = 0; i < PopulationSize; ++i)
            {
                chromosomes.Add(new List<int>());
            }

            foreach (var chromosome in chromosomes)
            {
                int chromosomeSize = random.Next(ChromosomeMaxSize + 1);
                for (int i = 0; i < chromosomeSize; ++i)
                    chromosome.Add(random.Next(strips.GetMaxActionCode));
            }
        }

        public void DoIteration()
        {
            iterationCounter++;
            Compression();
            Crossing();
            Mutation();
            Select();
            Console.WriteLine($"Iteracja {iterationCounter}, najlepszy wynik: {FitnessFunction(chromosomes[0])}, ostatni wynik: {FitnessFunction(chromosomes[chromosomes.Count - 1])}, średni wynik: {chromosomes.Select(c => FitnessFunction(c)).Average()}");
        }

        private void Compression()
        {
            Random random = new Random();
            List<List<int>> newChromosomes = new List<List<int>>();
            List<int> indexesOfCompressedChromosomes = new List<int>();
            var index = 0;

            chromosomes.ForEach(ch => {
                var randomNumber = random.Next(101);
                if (randomNumber <= (probabilityOfCompression * 100))
                {
                    List<int> newChromosome = new List<int>();
                    List<int> forwardExecutabilityVector = strips.GetForwardExecutabilityVector(ch);

                    for (int i = 0; i < forwardExecutabilityVector.Count; ++i)
                    {
                        if (forwardExecutabilityVector[i] == 1)
                        {
                            newChromosome.Add(ch[i]);
                        }

                    }
                    indexesOfCompressedChromosomes.Add(index);                   
                    newChromosomes.Add(newChromosome);
                }
                index++;
            });

            for (int i=0; i<newChromosomes.Count; i++)
            { //replace old chromosomes by new ones
                chromosomes.RemoveAt(indexesOfCompressedChromosomes[i]);
                chromosomes.Insert(indexesOfCompressedChromosomes[i], newChromosomes[i]);
            }
        }

        private void Select()
        {
            List<(int index, double fitness)> chromosomesFitness = chromosomes.Select((c, i) => (i, FitnessFunction(c))).ToList();
            chromosomesFitness.Sort((x, y) => y.fitness.CompareTo(x.fitness));
            List<List<int>> newChromosomes = new List<List<int>>();
            for (int i = 0; i< PopulationSize; ++i)
            {
                newChromosomes.Add(chromosomes[chromosomesFitness[i].index]);
            }
            chromosomes = newChromosomes;
        }

        private void Crossing()
        {
            Random random = new Random();
            List<List<int>> chromosomesToCrossing = chromosomes.Where(c => random.Next(101) <= (probabilityOfCrossing * 100)).OrderBy(x => random.Next(chromosomes.Count * 10)).ToList();

            for (int i=0; i<chromosomesToCrossing.Count - 1; i+=2)
            { //crossig chromosomes
                List<int> chromosome1 = chromosomesToCrossing[i];
                List<int> chromosome2 = chromosomesToCrossing[i + 1];

                if (chromosome1.Count != 0 && chromosome2.Count != 0)
                {
                    int index = random.Next(Math.Min(chromosome1.Count, chromosome2.Count));

                    List<int> newChromosome1 = chromosome1.GetRange(0, index);
                    newChromosome1.AddRange(chromosome2.GetRange(index, chromosome2.Count - index));

                    List<int> newChromosome2 = chromosome2.GetRange(0, index);
                    newChromosome2.AddRange(chromosome1.GetRange(index, chromosome1.Count - index));

                    chromosomes.Add(newChromosome1);
                    chromosomes.Add(newChromosome2);
                }
            }
        }

        private void Mutation()
        {
            Random random = new Random();
            List<List<int>> newChromosomes = new List<List<int>>();

            chromosomes.ForEach(ch => {
                var randomNumber = random.Next(101);
                if (randomNumber <= (probabilityOfMutation * 100))
                {
                    List<int> newChromosome = ch;
                    if (random.Next(101) < (probabilityOfAddNewGeneInMutation * 100))
                    { //add new gene at the end of chromosome
                        
                        newChromosome.Add(random.Next(strips.GetMaxActionCode));
                    }
                    else
                    { //change one gene in chromosome
                        if (newChromosome.Count != 0)
                        {
                            newChromosome[random.Next(ch.Count)] = random.Next(strips.GetMaxActionCode);
                        }
                    }
                    newChromosomes.Add(newChromosome);
                }
            });

            chromosomes.AddRange(newChromosomes);
        }

        public List<int> GetSolutionIfFound() 
        {
            if (strips.IsSolution(chromosomes[0]))
            {
                return chromosomes[0];
            }
            return null;
        }

        public double FitnessFunction(List<int> chromosome)
        {
            //maxvalue - 2
            double c1 = chromosome.Count != 0 ? strips.GetForwardExecutabilityVector(chromosome).Sum() / (double) chromosome.Count : 0,
                c2 = chromosome.Count != 0 ? strips.GetBackwardExecutabilityVector(chromosome).Sum() / (double) chromosome.Count : 0,
                c3 = strips.CompareStates(strips.ChromosomeToStateForward(chromosome), strips.End),
                c4 = strips.CompareStates(strips.ChromosomeToStateBackward(chromosome), strips.Start);
            double fitness = 0.2 * c1 + 0.2 * c2 + 0.8 * c3 + 0.8 * c4;
            return fitness;
        }
    }
}
