using System.Collections.Generic;

namespace TelemetryLive.Dto
{
    public class StatisticsRo
    {
        public Dictionary<string, StatisticsDictValue> StatisticValues { get; set; }
        public StatisticsRo(Dictionary<string, StatisticsDictValue> statisticsValues)
        {
            this.StatisticValues = statisticsValues;
        }
        public StatisticsRo()
        {
            this.StatisticValues = new Dictionary<string, StatisticsDictValue>();
        }
    }
}
