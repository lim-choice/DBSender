using DBSender.CONFIG;
using DBSender.UTIL;
using DBSender.SQL;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DBSender
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static XmlConfig Config;
        private static List<TimerConfig> timerList;

        static void Main(string[] args)
        {
            Timer timer = null;

            try
            {
                //Load Config File
                Config =
                    new XmlConfig(
                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), "DBSenderConfig.xml")
                    );

                //Get TimerList
                timerList = Config.GetTimerList();

                if(timerList.Count > 0)
                {
                    //Set Timer
                    timer = new Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += Timer_Elapsed;
                    logger.Info(string.Format("Timer Start. [Count: {0}]", timerList.Count));
                    timer.Start();
                }
                else
                {
                    logger.Info("TimerList is empty.");
                }

                /*-- Read Key --*/
                Console.WriteLine("Press 'Q' key to Exit");
                while (Console.ReadKey().Key != ConsoleKey.Q) { }

                logger.Info("Timer Stop.");
                timer.Stop();

                logger.Info("Program Exit.");
            }
            catch (Exception ex)
            {
                if (timer != null)
                    timer.Stop();

                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            QueryProc SelectQueryProc = null;
            QueryProc InsertQueryProc = null;
            List<string[]> DataList = null;

            DateTime dt;
            bool procStart;

            try
            {
                foreach(TimerConfig timer in timerList)
                {
                    dt = DateTime.Now;

                    procStart = false;

                    //Tick 체크
                    if (timer.cron.tick >= 0)
                    {
                        if(++timer.cron.tickCount >= timer.cron.tick)
                            procStart |= true;
                    }

                    //Sec ~ Month 체크 [조건에 있는데 부합하면 false 처리]
                    if (timer.cron.sec >= 0 && timer.cron.sec != dt.Second)
                        procStart = false;
                    if (timer.cron.min >= 0 && timer.cron.min != dt.Minute)
                        procStart = false;
                    if (timer.cron.hour >= 0 && timer.cron.hour != dt.Hour)
                        procStart = false;
                    if (timer.cron.day >= 0 && timer.cron.day != dt.Day)
                        procStart = false;
                    if (timer.cron.month >= 0 && timer.cron.month != dt.Month)
                        procStart = false;

                    //쿼리 실행
                    if(procStart)
                    {
                        try
                        {
                            timer.cron.tickCount = 0;

                            //Select
                            logger.Info(string.Format("[Timer] {0} - Select Start ", timer.name));
                            SelectQueryProc = new QueryProc();
                            SelectQueryProc.Connect(timer.Select.SqlSetting);
                            DataList = SelectQueryProc.GetSearchData(timer.Select.QuerySetting.query);
                            SelectQueryProc.Disconnect();
                            logger.Info(string.Format("[Timer] {0} - Select Finish ", timer.name));

                            //Insert
                            logger.Info(string.Format("[Timer] {0} - Insert Start ", timer.name));
                            InsertQueryProc = new QueryProc();
                            InsertQueryProc.Connect(timer.Insert.SqlSetting);
                            InsertQueryProc.InsertData(timer.Insert.QuerySetting.query, DataList);
                            InsertQueryProc.Disconnect();
                            logger.Info(string.Format("[Timer] {0} - Insert Finish ", timer.name));
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                            logger.Error(ex.StackTrace);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }
        }
    }
}
