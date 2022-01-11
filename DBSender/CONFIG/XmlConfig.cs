using DBSender.CONFIG;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DBSender.UTIL
{
    class XmlConfig
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private XmlDocument XmlFile = null;
        private List<SQLConfig> sqlConfigList = null;
        private List<QueryConfig> queryConfigList = null;
        private List<TimerConfig> timerConfigList = null;

        public XmlConfig(string xmlPath)
        {
            try
            {
                sqlConfigList = new List<SQLConfig>();
                queryConfigList = new List<QueryConfig>();
                timerConfigList = new List<TimerConfig>();

                XmlFile = new XmlDocument();
                XmlFile.Load(xmlPath);

                //1. SQL List 가져오기
                try
                {
                    XmlNodeList xmlList = XmlFile.SelectNodes("/Root/Config/SqlList/Sql");
                    foreach (XmlNode node in xmlList)
                    {
                        if (node.NodeType == XmlNodeType.Comment)
                            continue;

                        if (node.Attributes["name"] != null)
                        {
                            SQLConfig sqlConfig = new SQLConfig();

                            sqlConfig.name = node.Attributes["name"].Value;

                            XmlNodeList childNodes = node.ChildNodes;
                            foreach (XmlNode xn in childNodes)
                            {
                                if (xn.NodeType == XmlNodeType.Comment)
                                    continue;

                                if (xn.Name == "host" && xn.Attributes["value"] != null)
                                    sqlConfig.sqlHost = xn.Attributes["value"].Value;
                                if (xn.Name == "port" && xn.Attributes["value"] != null)
                                    sqlConfig.sqlPort = xn.Attributes["value"].Value;
                                if (xn.Name == "db" && xn.Attributes["value"] != null)
                                    sqlConfig.sqlDataBase = xn.Attributes["value"].Value;
                                if (xn.Name == "uid" && xn.Attributes["value"] != null)
                                    sqlConfig.sqlUserId = xn.Attributes["value"].Value;
                                if (xn.Name == "pwd" && xn.Attributes["value"] != null)
                                    sqlConfig.sqlUserPw = xn.Attributes["value"].Value;
                            }

                            sqlConfigList.Add(sqlConfig);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    logger.Error(e.StackTrace);
                }

                //2. Query List 가져오기try
                try
                {
                    XmlNodeList xmlList = XmlFile.SelectNodes("/Root/Config/QueryList/Query");
                    foreach (XmlNode node in xmlList)
                    {
                        if (node.NodeType == XmlNodeType.Comment)
                            continue;

                        QueryConfig queryConfig = new QueryConfig();

                        if (node.Attributes["name"] != null)
                            queryConfig.name = node.Attributes["name"].Value;

                        queryConfig.query = node.InnerText.Replace("\r\n", "").Trim();
                        queryConfigList.Add(queryConfig);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    logger.Error(e.StackTrace);
                }

                //3. Timer List 가져오기
                try
                {
                    XmlNodeList xmlList = XmlFile.SelectNodes("/Root/Rule/TimerList/Timer");
                    foreach (XmlNode node in xmlList)
                    {
                        if (node.NodeType == XmlNodeType.Comment)
                            continue;

                        TimerConfig tmConfig = new TimerConfig();

                        if (node.Attributes["name"] != null)
                            tmConfig.name = node.Attributes["name"].Value;

                        if (node.Attributes["cron"] != null)
                        {
                            CronConfig cronConfig = new CronConfig();

                            string cronData = node.Attributes["cron"].Value;
                            string[] splitStr = cronData.Split(' ');

                            if (splitStr.Length >= 1 && splitStr[0] != "*")
                                cronConfig.tick = Convert.ToInt32(splitStr[0]);
                            if (splitStr.Length >= 2 && splitStr[1] != "*")
                                cronConfig.sec = Convert.ToInt32(splitStr[1]);
                            if (splitStr.Length >= 3 && splitStr[2] != "*")
                                cronConfig.min = Convert.ToInt32(splitStr[2]);
                            if (splitStr.Length >= 4 && splitStr[3] != "*")
                                cronConfig.hour = Convert.ToInt32(splitStr[3]);
                            if (splitStr.Length >= 5 && splitStr[4] != "*")
                                cronConfig.day = Convert.ToInt32(splitStr[4]);
                            if (splitStr.Length >= 6 && splitStr[5] != "*")
                                cronConfig.min = Convert.ToInt32(splitStr[5]);

                            tmConfig.cron = cronConfig;
                        }

                        TaskConfig taskConfig;
                        XmlNodeList childNodes = node.ChildNodes;
                        foreach (XmlNode xn in childNodes)
                        {
                            if (xn.NodeType == XmlNodeType.Comment)
                                continue;

                            taskConfig = new TaskConfig();

                            if (xn.Attributes["Sql"] != null)
                                taskConfig.SqlSetting = sqlConfigList.Find(x => x.name == xn.Attributes["Sql"].Value);

                            if (xn.Attributes["Query"] != null)
                                taskConfig.QuerySetting = queryConfigList.Find(x => x.name == xn.Attributes["Query"].Value);

                            if (xn.Name == "Select")
                                tmConfig.Select = taskConfig;
                            else if (xn.Name == "Insert")
                                tmConfig.Insert = taskConfig;
                        }

                        timerConfigList.Add(tmConfig);
                    }

                    logger.Info("Load Xml Config File.");
                    logger.Info("sqlConfigList : " + sqlConfigList.Count);
                    logger.Info("queryConfigList : " + queryConfigList.Count);
                    logger.Info("timerConfigList : " + timerConfigList.Count);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    logger.Error(e.StackTrace);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }
        }

        public List<TimerConfig> GetTimerList()
        {
            return timerConfigList;
        }
    }

}
