using System.Configuration;

namespace QJFile.Data
{
    public class SqlHelp
    {
        /// <summary>
        /// 将SQLserver的连接字符串语法转为Mysql的语法
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static string concat(string strConSQL)
        {
            string strReturn = strConSQL;
            if (new QycodeB().Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Sqlite)
            {
                strReturn = strReturn.Replace("+", "||");
            }
            if (new QycodeB().Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.MySql)
            {
                strReturn = "CONCAT(" + strReturn.Replace('+', ',') + ")";
            }
            return strReturn;
        }
        /// <summary>
        /// 处理链接字符串废弃
        /// </summary>
        /// <returns></returns>
        public static string concatold(params string[] objs)
        {
            string strReturn = "";
            string strDbType = GetConfig("DBType");
            if (strDbType == "0")
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    strReturn = strReturn + ",";
                }
                strReturn = " CONCAT(" + strReturn.TrimEnd(',') + " ) ";
            }
            if (strDbType == "1")
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    strReturn = objs[i].ToString() + "+";
                }
                strReturn.TrimEnd('+');
            }

            return strReturn;
        }



        public static string GetConfig(string strKey, string strDefault = "")
        {
            return ConfigurationManager.AppSettings[strKey] ?? strDefault;
        }

    }



}
