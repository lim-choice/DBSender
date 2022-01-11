using DBSender.UTIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSender.CONFIG
{
    class TimerConfig
    {
        public string name { get; set; }
        public CronConfig cron { get; set; }
        public TaskConfig Select { get; set; }
        public TaskConfig Insert { get; set; }

    }

    class CronConfig
    {
        public int tick { get; set; } = -1;
        public int sec { get; set; } = -1;
        public int min { get; set; } = -1;
        public int hour { get; set; } = -1;
        public int day { get; set; } = -1;
        public int month { get; set; } = -1;

        public int tickCount { get; set; } = 0;
    }

    class TaskConfig
    {
        public SQLConfig SqlSetting { get; set; }
        public QueryConfig QuerySetting { get; set; }
    }
}
