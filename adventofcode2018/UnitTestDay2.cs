using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace adventofcode2018
{
    [TestClass]
    public class UnitTestDay2
    {
        [TestMethod]
        public void ShoouldRetunZeroWhenInputIsEmptyOrNoDuplicate()
        {
            var i = string.Empty;
            DuplicateFounder duplicateFounder = new DuplicateFounder();
            Assert.IsTrue(duplicateFounder.FindOccurence(i,2) == 0);
            i =  "abc";
            Assert.IsTrue(duplicateFounder.FindOccurence(i,2) == 0);
        }

        [TestMethod]
        public void ShoouldRetun1When1DuplicateChar()
        {
            var i = "aadferbb";
            DuplicateFounder duplicateFounder = new DuplicateFounder();
            Assert.AreEqual(duplicateFounder.FindOccurence(i,2) , 2);
        }
        [TestMethod]
        public void ShoouldRetun1When1TripleChar()
        {
            var i = "aaadferb";
            DuplicateFounder duplicateFounder = new DuplicateFounder();
            Assert.AreEqual(duplicateFounder.FindOccurence(i,3) , 1);
        }

        [TestMethod]
        public void CumpyeCheckSum()
        {
            var i = File.ReadAllLines("dataset2.txt");
                
            DuplicateFounder duplicateFounder = new DuplicateFounder();
            Assert.AreEqual(duplicateFounder.CumputeCheckSum(i) , 12);
        }

        [TestMethod]
        public void EmptystringISSamaAsEmptyString()
        {
           
                
            DuplicateFounder duplicateFounder = new DuplicateFounder();
            Assert.IsFalse(duplicateFounder.Same(string.Empty,"a"));
            Assert.IsTrue(duplicateFounder.Same("a","a"));
            Assert.IsFalse(duplicateFounder.Same("a","b"));
            Assert.IsTrue(duplicateFounder.Same("ab","bc"));
            Assert.IsTrue(duplicateFounder.Same("abd","bcd"));

            var lines = File.ReadAllLines("dataset2.txt");
            for (var index = 0; index < lines.Length; index++)
            {
                string s = lines[index];
                for (var j = index + 1; j < lines.Length; j++)
                {
                    var d=  duplicateFounder.SameChar(s, lines[j]);
                    if (d.Count()>0)
                    {
                      Assert.AreEqual(new string(d.ToArray()), "");
                        break;
                    }
                }
            }
        }
        
    }

    public class DuplicateFounder
    {
        public int FindOccurence(string s, int occurence)
        {
            return s
                .Select(c => c)
                .GroupBy(c => c, (k, v) => new {k, c = Enumerable.Count<char>(v)})
                       .FirstOrDefault(c => c.c == occurence) == null? 0 : 1;

        }

        public int CumputeCheckSum(string[] s)
        {
            var os= s.Select(l => new
            {
                o2 = FindOccurence(l, 2),
                o3 = FindOccurence(l, 3)
            });
            return os.Sum(c => c.o2) *os.Sum(c => c.o3) ;
        }

        public bool Same(string a, string b)
        {
            return a.Length == b.Length &&
                   a.Select(c => c).Except(b.Select(c => c)).Count()
                   <=Math.Min(1,a.Length-1);
        }
        public IEnumerable<char> SameChar(string reference, string copy)
        {
            int diffIndex = -1;

            int i = 0;
            for ( ;i < reference.Length; i++)
            {
                if (reference[i] != copy[i])
                {
                    if (diffIndex != -1)
                    {
                        diffIndex = -2;
                        break;
                    }
                    diffIndex = i;
                }
            }

            if (diffIndex >0)
            {
                return reference.Select(c => c).Take(diffIndex).Concat(
                    reference.Select(c => c).Skip(diffIndex+1));
            }

            return Enumerable.Empty<char>();
        }
    }
}
