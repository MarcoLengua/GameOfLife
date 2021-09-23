using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace GameOfLife
{
    class Program
    {
        static void Main(string[] args)
        {
            int width = 0;
            int height = 0;
            double probability = 0.5;
            List<List<int>> currentPopulation = new List<List<int>>();
            Console.WriteLine("Welcome to Game of Life!!!\nThis game is the evolution of Triells for Guenstiger.de");
            Console.WriteLine("If you want to skip forward evolution by X steps please press f.\nIf you want to save the game please press s.\nIf you want to load a world please press l.\nThe following grids are available by default:\nbee, space, pulsar, penta");
            Console.WriteLine("Please enter the grid width:");
            width = Convert.ToInt32(Console.ReadLine());
            // ich weiß dies und alle anderen user eingaben gehören in einen try catch block um falsche eingaben abzufangen, aber für alle eingaben, das jetzt noch zu machen bleibt keine zeit...
            Console.WriteLine("Please enter the grid height:");
            height = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter the probability that a cell is initialized as alive - input should between 0 and 100:");
            probability = Convert.ToDouble(Convert.ToInt16(Console.ReadLine())) / 100;
            Console.WriteLine("Your grid world is: " + width + " * " + height + " and the Probabilty that a cell is initially alive is " + probability * 100 + "%");
            Console.WriteLine("Press any key to start the game...");
            Console.ReadLine();
            Console.Clear();
            currentPopulation = initializePopulation(height, width, probability);
            printPopulation(currentPopulation);
            ConsoleKeyInfo cki;
            do
            {
                while (Console.KeyAvailable == false)
                {
                    Console.Clear();
                    currentPopulation = evolutionStep(currentPopulation, height, width);
                    printPopulation(currentPopulation);
                    Thread.Sleep(250);
                }
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.F)
                {
                    Console.WriteLine("Please enter the number of evolution Steps you want to skip forward:");
                    int epochs = Convert.ToInt32(Console.ReadLine());
                    currentPopulation = forEvolution(currentPopulation, height, width, epochs);
                    Console.Clear();
                    printPopulation(currentPopulation);
                    Console.WriteLine("Press any key to start the game again...");
                    Console.ReadLine();
                }
                else if (cki.Key == ConsoleKey.S)
                {
                    Console.WriteLine("Please enter the filename of the .txt that you want the current population to be saved to:");
                    string filename = Console.ReadLine() + ".txt";
                    savePopulation(currentPopulation, filename);
                    Console.WriteLine("Your current population has been saved to" + filename);
                    Console.WriteLine("Press any key to start the game again...");
                    Console.ReadLine();
                }
                else if (cki.Key == ConsoleKey.L)
                {
                    Console.WriteLine("Please enter the filename of the .txt that you want to load from:");
                    string filename = Console.ReadLine() + ".txt";
                    currentPopulation = loadPopulation(filename);
                    height = currentPopulation.Count;
                    width = currentPopulation[0].Count;
                    Console.WriteLine("Your population has been loaded.");
                    Console.WriteLine("Press any key to start the game again...");
                    Console.ReadLine();
                }
            } while (cki.Key != ConsoleKey.Q);
        }

        static List<List<int>> forEvolution(List<List<int>> currentPopulation, int height, int width, int epochs)
        {
            for (int i = 0; i < epochs; i++)
            {
                currentPopulation = evolutionStep(currentPopulation, height, width);
            }
            return currentPopulation;
        }
        static List<List<int>> initializePopulation(int height, int width, double probability)
        {
            List<List<int>> population = new List<List<int>>();
            for (int i = 0; i < height; i++)
            {
                population.Add(new List<int>());
                for (int j = 0; j < width; j++)
                {
                    Random random = new Random();
                    int alive = 0;
                    if (random.NextDouble() < probability)
                    {
                        alive = 1;
                    }
                    population[i].Add(alive);
                }
            }
            return population;
        }
        static void printPopulation(List<List<int>> population)
        {
            foreach (List<int> gridRow in population)
            {
                foreach (int gridValue in gridRow)
                {
                    char writeChar = ' ';
                    if (gridValue == 1)
                    {
                        writeChar = 'O';
                    }
                    Console.Write(writeChar);
                }
                Console.Write('\n');
            }
            Console.Write('\n');
        }
        static void savePopulation(List<List<int>> population, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false))
            {
                foreach (List<int> gridRow in population)
                {
                    foreach (int gridValue in gridRow)
                    {
                        char writeChar = 'X';
                        if (gridValue == 1)
                        {
                            writeChar = 'O';
                        }
                        writer.Write(writeChar);
                    }
                    writer.Write('\n');
                }
            }
        }
        static List<List<int>> loadPopulation(string filename)
        {
            List<List<int>> loadedPopulation = new List<List<int>>();
            int lineCount = 0;
            int charCount;
            using (StreamReader reader = new StreamReader(filename))
            {
                string charCountString = reader.ReadLine();
                charCount = charCountString.Length;
                while (reader.ReadLine() != null) { lineCount++; }
                lineCount += 1;
            }
            using (StreamReader reader = new StreamReader(filename))
            {
                for (int i = 0; i < lineCount; i++)
                {
                    loadedPopulation.Add(new List<int>());
                    for (int j = 0; j < charCount +1 ; j++)
                    {
                        char currentChar = (char)reader.Read();
                        if (currentChar == 'O')
                        {
                            loadedPopulation[i].Add(1);
                        }
                        else if (currentChar == 'X')
                        {
                            loadedPopulation[i].Add(0);
                        }
                    }
                }
            }
            return loadedPopulation;
        }
        //Diese methode könnte bestimmt schöner bzw. mit mehr modulo rechnungen gemacht werden, aber da modulo mit minus in c# leider bei minus
        //bleibt hatte ich auch hier keine Zeit mehr intensiver drüber nachzudenken. Python wäre leichter
        static List<List<int>> evolutionStep(List<List<int>> currentPopulation, int height, int width)
        {
            List<List<int>> futurePopulation = new List<List<int>>();
            for (int i = 0; i < height; i++)
            {
                futurePopulation.Add(new List<int>());
                for (int j = 0; j < width; j++)
                {
                    int pooling = 0;

                    if (i == 0 && j == 0)
                    {
                        pooling = currentPopulation[height - 1][width - 1] + currentPopulation[height - 1][j] + currentPopulation[height - 1][j + 1] + currentPopulation[i][width - 1] + currentPopulation[i][j + 1] + currentPopulation[i + 1][width - 1] + currentPopulation[i + 1][j] + currentPopulation[i + 1][j + 1];
                    }
                    else if (i == 0)
                    {
                        pooling = currentPopulation[height - 1][(j - 1) % width] + currentPopulation[height - 1][j % width] + currentPopulation[height - 1][(j + 1) % width] + currentPopulation[i][(j - 1) % width] + currentPopulation[i][(j + 1) % width] + currentPopulation[i + 1][(j - 1) % width] + currentPopulation[i + 1][j % width] + currentPopulation[i + 1][(j + 1) % width];
                    }
                    else if (j == 0)
                    {
                        pooling = currentPopulation[(i - 1) % height][width - 1] + currentPopulation[(i - 1) % height][j % width] + currentPopulation[(i - 1) % height][(j + 1) % width] + currentPopulation[i % height][width - 1] + currentPopulation[i % height][(j + 1) % width] + currentPopulation[(i + 1) % height][width - 1] + currentPopulation[(i + 1) % height][j % width] + currentPopulation[(i + 1) % height][(j + 1) % width];
                    }
                    else
                    {
                        pooling = currentPopulation[(i - 1) % height][(j - 1) % width] + currentPopulation[(i - 1) % height][j % width] + currentPopulation[(i - 1) % height][(j + 1) % width] + currentPopulation[i % height][(j - 1) % width] + currentPopulation[i % height][(j + 1) % width] + currentPopulation[(i + 1) % height][(j - 1) % width] + currentPopulation[(i + 1) % height][j % width] + currentPopulation[(i + 1) % height][(j + 1) % width];
                    }
                    int alive = 0;
                    if (((pooling == 2 || pooling == 3) && currentPopulation[i][j] == 1) || (pooling == 3 && currentPopulation[i][j] == 0))
                    {
                        alive = 1;
                    }
                    futurePopulation[i].Add(alive);
                }
            }
            return futurePopulation;
        }
    }
}
