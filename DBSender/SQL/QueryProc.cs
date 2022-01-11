using DBSender.UTIL;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBSender.SQL
{

    class QueryProc
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MySqlConnection sqlConnection = null;
        
        public void Connect(SQLConfig sqlConfig)
        {
            try
            {
                sqlConnection =
                    new MySqlConnection(
                        string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}",
                            sqlConfig.sqlHost, sqlConfig.sqlPort, sqlConfig.sqlDataBase, sqlConfig.sqlUserId, sqlConfig.sqlUserPw)
                    );

                //logger.Debug("SQL Connection Open Start");
                sqlConnection.Open();
                //logger.Debug("SQL Connection Open Finish");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public void Disconnect()
        {
            try
            {
                //logger.Debug("SQL Connection Close Start");
                sqlConnection.Close();
                //logger.Debug("SQL Connection Close Finish");
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public List<string[]> GetSearchData(string query)
        {
            List<string[]> result = new List<string[]>();

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, sqlConnection);
                MySqlDataReader dr = cmd.ExecuteReader();

                while(dr.Read())
                {
                    string[] fieldData = new string [dr.FieldCount];

                    //데이터 채워 넣기
                    for(int i = 0; i < dr.FieldCount; i++)
                        fieldData[i] = (dr[i] == null) ? "" : dr[i].ToString();

                    result.Add(fieldData);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }

            return result;
        }

        public int InsertData(string query, List<string[]> dataList)
        {
            MySqlCommand cmd;
            int result = 0;
            string cmdQuery = "";

            try
            {
                if(dataList != null)
                {
                    foreach(string[] data in dataList)
                    {
                        //Query문 만들어주기
                        cmdQuery = "";
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (cmdQuery != "")
                                cmdQuery += ",";

                            cmdQuery += "'" + data[i] + "'";
                        }
                        cmdQuery = query + " values (" + cmdQuery + ")";
                        cmd = new MySqlCommand(cmdQuery, sqlConnection);
                        result += cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error("Error Query Header : " + query);
                logger.Error("Error Query : " + cmdQuery);
            }

            logger.Info(string.Format("Insert Query Result : {0} / {1} ", result, dataList.Count));
            return result;
        }
    }
}
