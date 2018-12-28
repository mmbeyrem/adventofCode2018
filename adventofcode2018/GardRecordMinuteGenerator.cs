using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace adventofcode2018
{
    public class GardRecordMinuteGenerator
    {
        readonly List<GuardSleptMinute> GuardSleptMinutes;

        public GardRecordMinuteGenerator()
        {
            GuardSleptMinutes = new List<GuardSleptMinute>();
        }
        public List<GuardSleptMinute> Generate(IEnumerable<GardRecord> records)
        {
            StringBuilder b = new StringBuilder();
            var orders = records.OrderBy(r => r.DateTime);
            int gardId = -1;
            DateTime? sleptDate = null;
            int minutes = 0;
            foreach (GardRecord order in orders)
            {
                minutes = 0;
                switch (order.Status)
                {
                    case GardRecordStatus.Start:
                        gardId = order.Id;
                        sleptDate = null;
                        break;
                    case GardRecordStatus.Asleep:
                        order.Id = gardId;
                        sleptDate = order.DateTime;
                        break;
                    case GardRecordStatus.WakeUp:
                        order.Id = gardId;
                   minutes =ComputeSleepingMinutes(order.Id, sleptDate.Value, order.DateTime);
                        sleptDate = null;
                        break;
                }
                b.Append($"{order.DateTime},{order.Id},{order.Status},{minutes}\n");
            }
            File.WriteAllText("day4.txt",b.ToString());
            
            return GuardSleptMinutes;
        }

        private int ComputeSleepingMinutes(int guardId, DateTime SleepingDate, DateTime WakeUpDate)
        {
            int i = 0;
            while (true)
            {
                DateTime newDate = SleepingDate.AddMinutes(i);
                if(newDate.Hour !=0) continue;
                if (WakeUpDate == newDate) break;
                i++;
                GuardSleptMinutes.Add(new GuardSleptMinute { Id =guardId, Minute = newDate.Minute });
            }
            return i;
        }
    }
}