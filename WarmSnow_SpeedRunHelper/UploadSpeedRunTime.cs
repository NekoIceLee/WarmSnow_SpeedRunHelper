using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WarmSnow_SpeedRunHelper
{
    internal class UploadSpeedRunTime
    {
        public static UploadSpeedRunTime Instance { get; } = new UploadSpeedRunTime();
        public string PlayerName { get; set; }
        readonly string SQLServerAddr = "gz-cdb-7erg1rwj.sql.tencentcdb.com";
        readonly int SQLServerPort = 63972;
        readonly string ClientUser = "";
        readonly string ClientPasswd = "";
        MySqlConnection _connection;
        
        UploadSpeedRunTime()
        {
            _connection = new MySqlConnection($"server={SQLServerAddr},{SQLServerPort};userid={ClientUser};password={ClientPasswd};database=application;");
        }

    }
}
