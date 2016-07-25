/**
 * This file contains the implementation of the Genetic Algorithm to solve the Maze traveler problem
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace MazeSolver
{
    static public class MAP_SIZE
    {
        public const int HEIGHT = 10;
        public const int WIDTH = 15;
    }

    /**
     * Direciton enum to control the direction of travel.
     */
    public enum Direction
    {
        NORTH = 0,
        EAST,
        SOUTH,
        WEST,
        MAX_VAL
    }

    public class Genome
    {
        private readonly BitArray m_genes;
        private static Random RANDOM = new Random(DateTime.Now.Millisecond);

        public Genome(BitArray genes)
        {
            m_genes = genes;
        }

        public Genome(int num_bits)
        {
            m_genes = RandomGenes(num_bits);
        }

        public static BitArray RandomGenes(int num_bits)
        {
            var genes = new BitArray(num_bits);
            for (int i = 0; i < num_bits; ++i)
            {
                genes[i] = RANDOM.Next(0, 2) == 1;
            }
            return genes;
        }

        public Genome CrossOver(Genome other)
        {
            int crossover_point = RANDOM.Next(m_genes.Count);
            Func<int, bool> gene_picker = (index => index < crossover_point ? m_genes[index] : other.m_genes[index]);
            var crossed = (from i in Enumerable.Range(0, m_genes.Count) select gene_picker(i)).ToArray();
            var baby = new BitArray(crossed);
            return new Genome(baby);
        }

        public void Mutate(double mutation_rate)
        {
            for(int i = 0; i < m_genes.Count; ++i)
            {
                if(RANDOM.NextDouble() < mutation_rate)
                {
                    m_genes[i] = !m_genes[i];
                }
            }
        }

        public IList<Direction> Decode()
        {
            Func<bool, bool, Direction> bitsToInt = (first, second) =>
            {
                int lowBit = Convert.ToInt16(first);
                int highBit = Convert.ToInt16(second);
                int val = lowBit | (highBit << 1);
                return (Direction)val;
            };

            IList<Direction> directions = new List<Direction>();
            for (int i = 0; i < Genes.Count - 1; i += 2)
            {
                directions.Add(bitsToInt(Genes[i], Genes[i + 1]));
            }
            return directions;
        }

        public double Fitness
        {
            get; set;
        }

        public BitArray Genes
        {
            get { return m_genes; }
        }

        public override string ToString()
        {
            var output = (from i in Enumerable.Range(0, m_genes.Count) select Convert.ToInt16(m_genes[i]).ToString()).ToArray();
            return String.Join("", output);
        }
    }

    public class GenAlg
    {
        public const double MUTATION_RATE = 0.01;
        public const double CROSSOVER_RATE = 0.7;
        public const int CHROMO_LENGTH = 70;
        public const int POPULATION_SIZE = 140;

        private Random m_Random;
        private IList<Genome> m_Genomes;
        private int m_PopulationSize;
        private Genome m_FittestGenome;
        private double m_BestFitnessScore;
        private double m_TotalFitnessScore;
        private int m_Generation;
        private MazeMap m_MazeMap;
        private MazeMap m_Brain;
        private bool m_Busy;

        private Genome RouletteWheelSelection()
        {
            double slice = m_Random.NextDouble() * m_TotalFitnessScore;
            double spin_total = 0;
            Genome chosen_one = null;
            foreach(Genome g in m_Genomes)
            {
                spin_total += g.Fitness;
                if(spin_total > slice)
                {
                    chosen_one = g;
                    break;
                }
            }
            return chosen_one;
        }

        private void UpdateFitnessScores()
        {
            var temp_map = new MazeMap();
            m_FittestGenome = null;
            m_BestFitnessScore = 0;
            m_TotalFitnessScore = 0;

            foreach(Genome g in m_Genomes)
            {
                IList<Direction> directions = g.Decode();
                g.Fitness = m_MazeMap.ScoreRoute(directions, ref temp_map);
                m_TotalFitnessScore += g.Fitness;

                if(g.Fitness > m_BestFitnessScore)
                {
                    m_BestFitnessScore = g.Fitness;
                    m_FittestGenome = g;
                    m_Brain = temp_map;
                    temp_map = new MazeMap();

                    if(g.Fitness == 1)
                    {
                        m_Busy = false;
                        break;
                    }
                }
                temp_map.ResetMemory();
            }
        }

        private void CreateStartPopulation()
        {
            for(int i = 0; i < m_PopulationSize; ++i)
            {
                m_Genomes.Add(new Genome(GenAlg.CHROMO_LENGTH));
            }
        }

        public GenAlg(int pop_size)
        {
            m_PopulationSize = pop_size;
            m_Random = new Random(DateTime.Now.Millisecond);
            m_MazeMap = new MazeMap();
            m_Genomes = new List<Genome>();
            CreateStartPopulation();
        }

        public void Epoch()
        {
            UpdateFitnessScores();

            int new_babies = 0;
            IList<Genome> babies = new List<Genome>();
            while (new_babies < m_PopulationSize)
            {
                Genome mum = RouletteWheelSelection();
                Genome dad = RouletteWheelSelection();
                Genome baby1, baby2;
                if(m_Random.NextDouble() > GenAlg.CROSSOVER_RATE || mum == dad)
                {
                    baby1 = mum;
                    baby2 = dad;
                }
                else
                {
                    baby1 = mum.CrossOver(dad);
                    baby2 = dad.CrossOver(mum);
                    baby1.Mutate(GenAlg.MUTATION_RATE);
                    baby2.Mutate(GenAlg.MUTATION_RATE);
                }
                babies.Add(baby1);
                babies.Add(baby2);
                new_babies += 2;
            }

            m_Genomes = babies;
            ++m_Generation;
        }

        public MazeMap BestRun
        {
            get { return m_Brain; }
        }

        public int Generation
        {
            get { return m_Generation; }
        }

        public Genome FittestGenome
        {
            get { return m_FittestGenome; }
        }

        public bool Searching
        {
            get { return m_Busy; }
            set { m_Busy = value; }
        }

        public void Stop()
        {
            m_Busy = false;
        }
    }

    public class MazeMap
    {
        private int[,] m_Map;
        private int m_StartX, m_StartY;
        private int m_EndX, m_EndY;
        private int[,] m_Memory;

        public MazeMap()
        {
            m_Map = new int[,] {
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                {1, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1},
                {8, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1},
                {1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1},
                {1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1},
                {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1},
                {1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 5},
                {1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}};
            m_Memory = new int[MAP_SIZE.HEIGHT, MAP_SIZE.WIDTH];
            m_StartX = 14;
            m_StartY = 7;
            m_EndX = 0;
            m_EndY = 2;
        }

        public int[,] Memory
        {
            get { return m_Memory; }
        }

        public int[,] Map
        {
            get { return m_Map; }
        }

        public double ScoreRoute(IList<Direction> path, ref MazeMap memory)
        {
            int pos_x = m_StartX;
            int pos_y = m_StartY;
            foreach (Direction step in path)
            {
                if(step == Direction.EAST && pos_x < MAP_SIZE.WIDTH-1 && m_Map[pos_y, pos_x+1] == 0)
                {
                    ++pos_x;
                }
                else if(step == Direction.NORTH && pos_y > 0 && m_Map[pos_y-1, pos_x] == 0)
                {
                    --pos_y;
                }
                else if(step == Direction.WEST && pos_x > 0 && (m_Map[pos_y, pos_x-1] == 0 || m_Map[pos_y,pos_x-1] == 8))
                {
                    --pos_x;
                }
                else if(step == Direction.SOUTH  && pos_y < MAP_SIZE.HEIGHT-1 && m_Map[pos_y+1, pos_x] == 0)
                {
                    ++pos_y;
                }
                memory.Memory[pos_y, pos_x] = 1;
            }
            int diff_x = Math.Abs(pos_x - m_EndX);
            int diff_y = Math.Abs(pos_y - m_EndY);
            return 1 / (double)(diff_x + diff_y + 1);
        }

        public void ResetMemory()
        {
            m_Memory = new int[MAP_SIZE.HEIGHT, MAP_SIZE.WIDTH];
        }
    }
}
