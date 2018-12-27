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
            Assert.AreEqual(selection.selectedGuardId*selection.selectedMinute,240);
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
            return new Selection{
                    selectedGuardId=selectedGuardId,
                    selectedMinute=selectedMinute.m}
            ;
        }

        private static int SelectGuard(List<GuardSleptMinute> guardSleptMinutes)
        {
            return guardSleptMinutes
                .GroupBy(g => g.Id, (i, minutes) => new {i, m = minutes.Sum(v => v.Minute)})
                .OrderByDescending(e => e.m).First().i;

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

    internal class Selection 
    {
        public int selectedGuardId { get; set; }
        public int selectedMinute { get; set; }
    }

    public class GuardSleeepMinute : IEquatable<GuardSleeepMinute>
    {
        public int Id { get; set; }
        public int Minute { get; set; }
        
        public bool Equals(GuardSleeepMinute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Minute == other.Minute;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GuardSleeepMinute) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ Minute;
            }
        }
    }

    public class GardRecordMinuteGenerator
    {
        readonly List<GuardSleptMinute> GuardSleptMinutes;

        public GardRecordMinuteGenerator()
        {
            GuardSleptMinutes = new List<GuardSleptMinute>();
        }
        public List<GuardSleptMinute> Generate(IEnumerable<GardRecord> records)
        {
            var orders = records.OrderBy(r => r.DateTime).ThenBy(r => r.Status);
            int gardId = -1;
            DateTime? sleptDate = null;
            foreach (GardRecord order in orders)
            {
                if (order.Status == GardRecordStatus.Start)
                {
                    gardId = order.Id;
                    sleptDate = null;
                }
                else
                {
                    order.Id = gardId;

                    if (order.Status == GardRecordStatus.Asleep)
                    {
                        sleptDate = order.DateTime;
                    }
                    else
                    {
                        if (order.Status == GardRecordStatus.WakeUp && sleptDate!= null)
                        {
                            ComputeSleepingMinutes(order.Id, sleptDate.Value, order.DateTime);
                            sleptDate = null;
                        }
                    }
                }
            }

            return GuardSleptMinutes;
        }

        private void ComputeSleepingMinutes(int guardId, DateTime SleepingDate, DateTime WakeUpDate)
        {
            int i = 0;
            while (true)
            {
                DateTime newDate = SleepingDate.AddMinutes(i);
                if (WakeUpDate <= newDate) break;
                i++;
                GuardSleptMinutes.Add(new GuardSleptMinute { Id =guardId, Minute = newDate.Minute });
            }
        }
    }

    public class GuardSleptMinute
    {
        public int Id { get; set; }
        public int Minute { get; set; }
    }

    public class GardRecordFactory
    {
        private static string AsleepPattern = @"(\d+-\d+-\d+ \d+:\d+)] falls asleep";
        private static string WakeUpPattern = @"(\d+-\d+-\d+ \d+:\d+)] wakes up";
        private static string StartPattern = @"(\d+-\d+-\d+ \d+:\d+)] Guard #(\d+) begins shift";

        public GardRecord Build(string input)
        {
            return buildStartinGardRecord(input) ??
                   buildAsleepRecord(input) ??
                   buildWakeUpRecord(input);
        }
        GardRecord buildStartinGardRecord(string input)
        {
            var regex = new Regex(StartPattern);
            foreach (Match match in regex.Matches(input))
            {
                if (match.Success)
                {
                    return new GardRecord
                    {
                        Id = Convert.ToInt32(match.Groups[2].Value),
                        DateTime = DateTime.ParseExact(match.Groups[1].Value,
                            "yyyy-MM-dd HH:mm",
                            CultureInfo.InvariantCulture),
                        Status = GardRecordStatus.Start
                    };
                }
            }

            return null;
        }
        GardRecord buildWakeUpRecord(string input)
        {
            var regex = new Regex(WakeUpPattern);
            foreach (Match match in regex.Matches(input))
            {
                if (match.Success)
                {
                    return new GardRecord
                    {
                        Id = -1,
                        DateTime = DateTime.ParseExact(match.Groups[1].Value,
                            "yyyy-MM-dd HH:mm",
                            CultureInfo.InvariantCulture),
                        Status = GardRecordStatus.WakeUp
                    };
                }
            }

            return null;
        }
        GardRecord buildAsleepRecord(string input)
        {
            var regex = new Regex(AsleepPattern);
            foreach (Match match in regex.Matches(input))
            {
                if (match.Success)
                {
                    return new GardRecord
                    {
                        Id = -1,
                        DateTime = DateTime.ParseExact(match.Groups[1].Value,
                            "yyyy-MM-dd HH:mm",
                            CultureInfo.InvariantCulture),
                        Status = GardRecordStatus.Asleep
                    };
                }
            }

            return null;
        }
    }
    public class GardRecord
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public GardRecordStatus Status { get; set; }
    }

    public enum GardRecordStatus
    {
        Start =1,
        WakeUp = 3,
        Asleep = 2
    }
}
