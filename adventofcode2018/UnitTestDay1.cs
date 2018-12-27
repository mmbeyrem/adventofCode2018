using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace adventofcode2018
{
    [TestClass]
    public class UnitTestDay1
    {
        private FrequencyComputer _fc; 
        [TestInitialize]
        public void SetUp()
        {
            _fc  = new FrequencyComputer();
        }
        [TestMethod]
        public void WhenNullReturnEmtyList()
        {
            string rawFrequency = null;
            int frequenciesSum = _fc.Computer(rawFrequency);
            Assert.IsTrue(frequenciesSum == 0);
        }

        [TestMethod]
        public void WhenEmptyReturnEmtyList()
        {
            string rawFrequency = String.Empty;
           int frequenciesSum = _fc.Computer(rawFrequency);
           
            Assert.IsTrue(frequenciesSum == 0);
        }

        [TestMethod]
        public void WhenInvalidformatReturnEmtyList()
        {
            string rawFrequency = "a";
            var frequencies = _fc.Computer(rawFrequency);
            Assert.IsTrue(frequencies ==0);
        }
        [TestMethod]
        public void WhenvalidPositifValuesformatReturnEmtyList()
        {
            string rawFrequency = "1,2,3,4,5,6,6";
            var frequencies = _fc.Computer(rawFrequency);
            Assert.IsTrue(frequencies==27);
        }

        [TestMethod]
        public void WhenvalidNegatifValuesformatReturnEmtyList()
        {
            string rawFrequency = "-1,-2,-3,-4,-5,-6,-6";
            var frequencies = _fc.Computer(rawFrequency);
            Assert.IsTrue(frequencies==-27);
        }
        [TestMethod]
        public void readFromFile()
        {
            string rawFrequency = File.ReadAllText("dataset.txt")
                .Replace('\n',',');
            var frequencies = _fc.Computer(rawFrequency);
            Assert.AreEqual(frequencies,-27);
        }
         
        [TestMethod]
        public void CheckTwiceFrequency()
        {
            string rawFrequency ="+1, -1";
            int frequence = _fc.GetTwiceFrequency(rawFrequency);
            Assert.AreEqual(frequence,0);

            rawFrequency ="+3, +3, +4, -2, -4";
            frequence = _fc.GetTwiceFrequency(rawFrequency);
            Assert.AreEqual(frequence,10);

            rawFrequency ="-6, +3, +8, +5, -6";
            frequence = _fc.GetTwiceFrequency(rawFrequency);
            Assert.AreEqual(frequence,5);

            rawFrequency ="+7, +7, -2, -7, -4";
            frequence = _fc.GetTwiceFrequency(rawFrequency);
            Assert.AreEqual(frequence,14);

            rawFrequency = File.ReadAllText("dataset.txt")
                .Replace('\n',',');
            frequence = _fc.GetTwiceFrequency(rawFrequency);
            Assert.AreEqual(frequence,14);
        }
        
    }

    public class FrequencyComputer
    {
        public int Computer(string rawFrequency)
        {
            return Parse(rawFrequency).Sum();
        }
        private IEnumerable<int> Parse(string rawFrequency)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawFrequency)) return Enumerable.Empty<int>();
                return rawFrequency.Split(',')
                    .Select(int.Parse).ToList();

            }
            catch (Exception ex)
            {
                return Enumerable.Empty<int>();
            }
        }

        public int GetTwiceFrequency(string rawFrequency)
        {
            var values = Parse(rawFrequency).ToList();
            int s = 0;
            IList<int> computedFrequencies = new List<int>();
            computedFrequencies.Add(s);
            for (int i =0; i<values.Count;)
            {
                s += values[i];
                if (computedFrequencies.Contains(s))
                    return s;
                
                computedFrequencies.Add(s);
                i = (i + 1) % values.Count;
            }
               
            return 0;
        }
    }
}
