using System;
using System.Linq;
using MazeSolver;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace MazeSolverTest
{
    [TestFixture]
    public class TestGenome
    {
        [Test]
        public void TestMutate()
        {
            BitArray ba = new BitArray(new bool[] { false, false, false, false, false, false, false, false });
            Genome g = new Genome(ba);

            for(int i = 0; i < ba.Count; ++i)
            {
                Assert.IsFalse(g.Genes[i]);
            }

            g.Mutate(0.0);

            for(int i = 0; i < ba.Count; ++i)
            {
                Assert.IsFalse(g.Genes[i]);
            }

            g.Mutate(1.0);
            for(int i = 0; i < g.Genes.Count; ++i)
            {
                Assert.IsTrue(g.Genes[i]);
            }
        }

        [Test]
        public void TestMutateZeroSizeGenome()
        {
            Genome g = new Genome(new BitArray(0));
            Assert.DoesNotThrow(() => g.Mutate(0.50));
        }

        [Test]
        public void TestCrossover()
        {
            BitArray ba = new BitArray(new bool[] { false, false, false, false, false, false, false, false });
            BitArray bb = new BitArray(new bool[] { true, true, true, true, true, true, true, true});
            Genome mom = new Genome(ba);
            Genome dad = new Genome(bb);

            Genome baby = mom.CrossOver(dad);
            bool prev = baby.Genes[0];
            bool passed = false;
            for (int i = 1; i < baby.Genes.Count; ++i)
            {
                bool next = baby.Genes[i];
                if(prev != next)
                {
                    passed = true;
                    break;
                }
                prev = next;
            }
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestDecode()
        {
            BitArray ba = new BitArray(new bool[] { false, false, false, true, true, false, true, true });
            Genome mom = new Genome(ba);
            IList<Direction> steps = mom.Decode();
            var expected_directions = new []{ Direction.NORTH, Direction.SOUTH, Direction.EAST, Direction.WEST };
            Assert.AreEqual(ba.Count / 2, steps.Count);

            var results= steps.Zip(expected_directions, (actual, expected) => {
                Assert.IsTrue(expected == actual);
                return expected == actual;
            });
        }
    }
}