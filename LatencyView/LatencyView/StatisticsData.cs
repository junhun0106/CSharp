using System;
using System.Collections.Concurrent;
using System.Linq;

namespace LatencyView
{
    public class WebApiInfo
    {
        public string url = string.Empty;
        public string method = string.Empty;
        public int count;
        public int failedCount;
        public TimeSpan timeMin;
        public TimeSpan timeMax;
        public TimeSpan timeAvg;

        /// <summary>
        /// 절사평균
        /// </summary>
        public TimeSpan TrimmedMean {
            get {
                if (count < 3) {
                    return timeAvg;
                }

                var min = timeMin.Ticks;
                var max = timeMax.Ticks;
                var total = timeAvg.Ticks * count;

                var value = (total - min - max) / (count - 2);
                return new TimeSpan(value);
            }
        }
    }

    public class StatsModel
    {
        public WebApiInfo[] WebApiInfos;
    }

    public class StatisticsData
    {
        private static readonly ConcurrentDictionary<string, WebApiInfo> _statisticsData = new ConcurrentDictionary<string, WebApiInfo>(StringComparer.Ordinal);

        public WebApiInfo[] Data => _statisticsData.Values.OrderByDescending(x => x.timeAvg).ToArray();

        public void Add(string url, string method, bool isStatusOk, TimeSpan elapsed)
        {
            _statisticsData.AddOrUpdate(
                $"{url}-{method}",
                new WebApiInfo {
                    url = url,
                    method = method,
                    count = 1,
                    failedCount = !isStatusOk ? 1 : 0,
                    timeMin = elapsed,
                    timeMax = elapsed,
                    timeAvg = elapsed,
                },
                (_, value) => {
                    if (!isStatusOk) value.failedCount++;
                    if (value.timeMin > elapsed) value.timeMin = elapsed;
                    if (value.timeMax < elapsed) value.timeMax = elapsed;
                    value.timeAvg = GetAvg(value.timeAvg, elapsed, value.count);
                    value.count++;
                    return value;
                }
            );
        }

        private static TimeSpan GetAvg(TimeSpan prevAvg, TimeSpan x, int n)
        {
            return new TimeSpan(GetAvg(prevAvg.Ticks, x.Ticks, n));
        }

        private static long GetAvg(long prevAvg, long x, int n)
        {
            return ((prevAvg * n) + x) / (n + 1);
        }

        public void Clear()
        {
            _statisticsData.Clear();
        }
    }
}
