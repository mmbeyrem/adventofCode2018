using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode2018;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace adventofcode2018
{
    [TestClass]
    public class UnitTestDay3
    {
        [TestMethod]
        public void TestMethod1()
        {
            OverlapDetector od = new OverlapDetector();
            List<Claim> claims = new List<Claim>
            {
                new Claim
                {
                    Id =0,
                    XLength=1,
                    X =0,
                    Y=0
                },
                new Claim
                {
                    Id =0,
                    XLength=1,
                    X =1,
                    Y=0
                }
            };
            var overlapCount = od.Check(claims);
            Assert.AreEqual(0,overlapCount);
        }
        [TestMethod]
        public void TestMethod2()
        {
            OverlapDetector od = new OverlapDetector();
            List<Claim> claims = new List<Claim>
            {
                new Claim
                {
                    Id =0,
                    XLength=1,
                    X =0,
                    Y=0
                },
                new Claim
                {
                    Id =0,
                    XLength=2,
                    X =0,
                    Y=0
                }
            };
            var overlapCount = od.Check(claims);
            Assert.AreEqual(1,overlapCount);
        }
        [TestMethod]
        public void TestMethod3()
        {
            OverlapDetector od = new OverlapDetector();
            List<Claim> claims = new List<Claim>
            {
                new Claim
                {
                    Id =0,
                    XLength=4,
                    X =1,
                    Y=3
                },
                new Claim
                {
                    Id =0,
                    XLength=4,
                    X =3,
                    Y=1
                },
                new Claim
                {
                    Id =0,
                    XLength=2,
                    X =5,
                    Y=5
                }
            };
            var overlapCount = od.Check(claims);
            Assert.AreEqual(4,overlapCount);
        }

        [TestMethod]
        public void TestInputString()
        {
            string input = @"#3 @ 940,313: 27x11";
            string pattern = @"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)";
            var regex = new Regex(pattern);
            foreach (Match match in regex.Matches(input))
            {
               var c = new Claim
               {
                   Id = Convert.ToInt32(match.Groups[1].Value),
                   X  = Convert.ToInt32(match.Groups[2].Value),
                   Y = Convert.ToInt32(match.Groups[3].Value),
                   XLength = Convert.ToInt32(match.Groups[4].Value),
                   YLength = Convert.ToInt32(match.Groups[5].Value),

               };
            }
        }
        [TestMethod]
        public void TestDay1P1()
        {
            OverlapDetector od = new OverlapDetector();
            string pattern = @"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)";
            var points = File.ReadAllLines("dataset3.txt");
            var regex = new Regex(pattern);
            List<Claim> claims = new List<Claim>();
            foreach (string l in points)
            {
                foreach (Match match in regex.Matches(l))
                {
                    claims.Add(new Claim
                    {
                        Id = Convert.ToInt32(match.Groups[1].Value),
                        X  = Convert.ToInt32(match.Groups[2].Value),
                        Y = Convert.ToInt32(match.Groups[3].Value),
                        XLength = Convert.ToInt32(match.Groups[4].Value),
                        YLength = Convert.ToInt32(match.Groups[5].Value),

                    });
                } 
            }
            var overlapCount = od.Check(claims);
            Assert.AreEqual(0,overlapCount);
        }

    }

    public class Claim
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int XLength { get; set; }
        public int YLength { get; set; }
    }

    public class OverlapDetector
    {
        public int Check(List<Claim> claims)
        {
            HashSet<Point> allPoints = new HashSet<Point>();
            HashSet<Point> overlapPoints = new HashSet<Point>();
            List<int> ids = new List<int>();
            foreach (Claim claim in claims)
            {
                ids.Add(claim.Id);
                for (int i = 0; i < claim.XLength; i++)
                {
                    for (int j = 0; j < claim.YLength; j++)
                    {
                        Point point = new Point { X = claim.X + i, Y = claim.Y+j , Id = claim.Id};
                        AddPoint(allPoints, overlapPoints, point,ids); 
                    }
                }
            }

          
            return overlapPoints.Count;
        }

        private bool AddPoint(HashSet<Point> allPoints, HashSet<Point> OverlapPoints, Point px, List<int> ids)
        {
            var overlaped = false;
            if (!allPoints.Add(px))
            {
                OverlapPoints.Add(px);
                overlaped = true;
                ids.Remove(px.Id);
                Point origniPoint;
                allPoints.TryGetValue(px, out origniPoint);
                ids.Remove(origniPoint.Id);
            }


            return overlaped;
        }
    }
}
