using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode2018;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ploeh.AutoFixture;

namespace adventofcode2018
{
    [TestClass]
    public class UnitTestDay4
    {
        [TestMethod]
        public void TestExtractGuardStratRoutine()
        {
            var pattern = @"(\d+-\d+-\d+ \d+:\d+)] Guard #(\d+) begins shift";
            var regex = new Regex(pattern);
            string input = "[1518-11-01 00:00] Guard #10 begins shift";
            foreach (Match match in regex.Matches(input))
            {
                if (match.Success)
                {
                    var c = new GardRecord
                    {
                        Id = Convert.ToInt32(match.Groups[2].Value),
                        DateTime = DateTime.ParseExact(match.Groups[1].Value,
                            "yyyy-MM-dd HH:mm",
                            CultureInfo.InvariantCulture),
                        Status = GardRecordStatus.Start
                    };
                }
            }
        }

        [TestMethod]
        public void TestExtractGuardwakes()
        {
            var pattern = @"(\d+-\d+-\d+ \d+:\d+)] wakes up";
            var regex = new Regex(pattern);
            string input = "[1518-11-01 00:25] wakes up";
            foreach (Match match in regex.Matches(input))
            {
                if (match.Success)
                {
                    var c = new GardRecord
                    {
                        Id = -1,
                        DateTime = DateTime.ParseExact(match.Groups[1].Value,
                            "yyyy-MM-dd HH:mm",
                            CultureInfo.InvariantCulture),
                        Status = GardRecordStatus.WakeUp
                    };
                }
            }
        }

        [TestMethod]
        public void TestExtractGuardAsleep()
        {
            var pattern = @"(\d+-\d+-\d+ \d+:\d+)] falls asleep";
            var regex = new Regex(pattern);
            string input = "[1518-11-01 00:25] falls asleep";
            foreach (Match match in regex.Matches(input))
            {
                if (match.Success)
                {
                    var c = new GardRecord
                    {
                        Id = -1,
                        DateTime = DateTime.ParseExact(match.Groups[1].Value,
                            "yyyy-MM-dd HH:mm",
                            CultureInfo.InvariantCulture),
                        Status = GardRecordStatus.Asleep
                    };
                }
            }
        }

        [TestMethod]
        public void TestOrderGuardRecordByTime()
        {
            string[] rawRecords = File.ReadAllLines("dataset4.txt");
            var selection = GetGuardsMinutesAsleep(rawRecords);
            Assert.AreEqual(selection.selectedGuardId * selection.selectedMinute
                ,240);
        }

        private static Selection GetGuardsMinutesAsleep(string[] rawRecords)
        {
            var gardRecordMinuteGenerator = new GardRecordMinuteGenerator();
            List<GardRecord> allrecords = ExtractGuardRawRecords(rawRecords).ToList();
            var guardSleptMinutes = gardRecordMinuteGenerator.Generate(allrecords);
            int selectedGuardId = SelectGuard(guardSleptMinutes);


            var selectedMinute = guardSleptMinutes.Where(g => g.Id == selectedGuardId)
                .Select(s => s.Minute)
                .GroupBy(m => m, (m, minutes) => new {m, c = minutes.Count()})
                .OrderByDescending(e => e.c).First();
            return new Selection
                {
                    selectedGuardId = selectedGuardId,
                    selectedMinute = selectedMinute.m
                }
                ;
        }

        private static int SelectGuard(List<GuardSleptMinute> guardSleptMinutes)
        {
            var ddd =
                guardSleptMinutes
                    .GroupBy(g => g.Id, (id, minutes) =>
                        new {id, minutes = minutes.Sum(v => v.Minute)})
                    .OrderByDescending(e => e.minutes).ToList();
            return guardSleptMinutes
                .GroupBy(g => g.Id, (id, minutes) =>
                    new {id, minutes = minutes.Sum(v => v.Minute)})
                .OrderByDescending(e => e.minutes).First().id;
        }

        private static IEnumerable<GardRecord> ExtractGuardRawRecords(string[] rawRecords)
        {
            var factory = new GardRecordFactory();

            foreach (string rawRecord in rawRecords)
            {
                yield return factory.Build(rawRecord);
            }
        }
    }
}
