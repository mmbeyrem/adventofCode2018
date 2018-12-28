using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
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
}