using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSender.UTIL
{
    class SQLConfig
    {
        public string name { get; set; }
        public string sqlHost { get; set; }
        public string sqlPort { get; set; }
        public string sqlDataBase { get; set; }
        public string sqlUserId { get; set; }
        public string sqlUserPw { get; set; }
    }

    class QueryConfig
    {
        public string name { get; set; }

        public string query { get; set; }
    }
}
