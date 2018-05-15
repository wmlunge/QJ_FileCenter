using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QJFile.Data
{
    public class DocumentB : BaseEFDao<Document>
    {

    }
    public class QycodeB : BaseEFDao<Qycode>
    {

    }
    public class userB : BaseEFDao<user>
    {
        /// <summary>
        /// 判断是否具有管理权限
        /// </summary>
        /// <param name="strUser"></param>
        /// <param name="strpasd"></param>
        /// <returns></returns>
        public bool isAuth(string strUser, string strpasd)
        {
            var users = new userB().GetEntities(d => d.username == strUser && d.pasd == strpasd);
            return users.Count() == 1;
        }
    }

    public class userlogB : BaseEFDao<userlog>
    { }
}
