using SqlSugar;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace QJFile.Data
{
    public class DocumentB : BaseEFDao<Document>
    {

    }
    public class QycodeB : BaseEFDao<Qycode>
    {

    }

    public class appsetingB : BaseEFDao<appseting>
    {
        public static string GetValueByKey(string strKey, string strDefault = "")
        {
            string strValue = "";
            var keys = new appsetingB().GetEntities(d => d.Key == strKey).ToList();
            if (keys.Count() > 0)
            {
                strValue = keys[0].Value;
            }
            else
            {
                strValue = strDefault;
            }
            
            return strValue;
        }
    }
    public class JH_Auth_UserB : BaseEFDao<user>
    {
        /// <summary>
        /// 判断是否具有管理权限
        /// </summary>
        /// <param name="strUser"></param>
        /// <param name="strpasd"></param>
        /// <returns></returns>
        public bool isAuth(string strUser, string strpasd)
        {
            var users = new JH_Auth_UserB().GetEntities(d => d.username == strUser && d.pasd == strpasd);
            return users.Count() == 1;
        }

        public class UserInfo
        {
            public user User;
            public string UserRoleCode;
            public Qycode Qycode;
            public JH_Auth_QY QYinfo;

        }


        public UserInfo GetUserInfo(string strUser, string strpasd)
        {
            UserInfo UserInfo = new UserInfo(); ;
            var users = new JH_Auth_UserB().GetEntities(d => d.username == strUser && d.pasd == strpasd).ToList();
            if (users.Count() > 0)
            {
                UserInfo.User = users[0];
                UserInfo.Qycode = new QycodeB().GetALLEntities().FirstOrDefault();
            }
            return UserInfo;
        }



    }

    public class userlogB : BaseEFDao<userlog>
    { }

    public class JH_Auth_QY
    {
        public string QYCode { get; set; }
        public string FileServerUrl { get; set; }


    }

    #region 文档管理模块
    public class FT_FolderB : BaseEFDao<FT_Folder>
    {


        public FoldFile GetWDTREE(int FolderID, ref List<FoldFileItem> ListID, string strUserName = "")
        {
            List<FT_Folder> ListAll = new FT_FolderB().GetALLEntities().ToList();
            FT_Folder Folder = new FT_FolderB().GetEntity(d => d.ID == FolderID);
            FT_FolderB.FoldFile Model = new FT_FolderB.FoldFile();
            Model.Name = Folder.Name;
            Model.FolderID = Folder.ID;
            Model.CRUser = Folder.CRUser;
            Model.PFolderID = Folder.PFolderID.Value;
            ListID.Add(new FoldFileItem() { ID = Folder.ID, Type = "folder" });
            if (strUserName != "")
            {
                Model.SubFileS = new FT_FileB().GetEntities(d => d.FolderID == Folder.ID && d.CRUser == strUserName).ToList();
            }
            else
            {
                Model.SubFileS = new FT_FileB().GetEntities(d => d.FolderID == Folder.ID).ToList();
            }
            foreach (var item in Model.SubFileS)
            {
                ListID.Add(new FoldFileItem() { ID = item.ID, Type = "file" });

            }
            Model.SubFolder = new FT_FolderB().GETNEXTFLODER(Folder.ID, ListAll, ref ListID, strUserName);
            return Model;
        }


        private List<FoldFile> GETNEXTFLODER(int FolderID, List<FT_Folder> ListAll, ref List<FoldFileItem> ListID, string strUserName = "")
        {
            List<FoldFile> ListData = new List<FoldFile>();
            var list = ListAll.Where(d => d.PFolderID == FolderID);
            if (strUserName != "")
            {
                list = list.Where(d => d.CRUser == strUserName);
            }
            foreach (var item in list)
            {
                FoldFile FolderNew = new FoldFile();
                FolderNew.FolderID = item.ID;
                FolderNew.Name = item.Name;
                FolderNew.CRUser = item.CRUser;
                FolderNew.PFolderID = item.PFolderID.Value;
                if (strUserName != "")
                {
                    FolderNew.SubFileS = new FT_FileB().GetEntities(d => d.FolderID == item.ID && d.CRUser == strUserName).ToList();
                }
                else
                {
                    FolderNew.SubFileS = new FT_FileB().GetEntities(d => d.FolderID == item.ID).ToList();
                }
                foreach (var SubFile in FolderNew.SubFileS)
                {
                    ListID.Add(new FoldFileItem() { ID = SubFile.ID, Type = "file" });
                }
                FolderNew.SubFolder = GETNEXTFLODER(item.ID, ListAll, ref ListID, strUserName);
                ListData.Add(FolderNew);
                ListID.Add(new FoldFileItem() { ID = item.ID, Type = "folder" });
            }
            return ListData;

        }



        /// <summary>
        /// 获取指定文件夹下得所有文件夹
        /// </summary>
        /// <param name="FolderID"></param>
        /// <returns></returns>
        public List<FT_Folder> GetChiFolder(int FolderID)
        {
            string strQuery = FolderID.ToString() + "-";
            return new FT_FolderB().GetEntities(d => d.Remark.Contains(strQuery)).ToList();
        }

        /// <summary>
        /// 复制树状结构
        /// </summary>
        /// <param name="FloderID"></param>
        /// <param name="PID"></param>
        public void CopyFloderTree(int FloderID, int PID)
        {
            List<FoldFileItem> ListID = new List<FoldFileItem>();
            FoldFile Model = new FT_FolderB().GetWDTREE(FloderID, ref ListID);
            FT_Folder Folder = new FT_FolderB().GetEntity(d => d.ID == Model.FolderID);
            Folder.PFolderID = PID;
            new FT_FolderB().Insert(Folder);

            //更新文件夹路径Code
            FT_Folder PFolder = new FT_FolderB().GetEntity(d => d.ID == PID);
            Folder.Remark = PFolder.Remark + "-" + Folder.ID;
            new FT_FolderB().Update(Folder);

            foreach (FT_File file in Model.SubFileS)
            {
                file.FolderID = Folder.ID;
                new FT_FileB().Insert(file);
            }
            GreateWDTree(Model.SubFolder, Folder.ID);
        }

        /// <summary>
        /// 根据父ID创建树装结构文档
        /// </summary>
        /// <param name="ListFoldFile"></param>
        private void GreateWDTree(List<FoldFile> ListFoldFile, int newfolderid)
        {

            foreach (FoldFile item in ListFoldFile)
            {

                FT_Folder PModel = new FT_FolderB().GetEntity(d => d.ID == item.FolderID);
                PModel.PFolderID = newfolderid;
                new FT_FolderB().Insert(PModel);

                //更新文件夹路径Code
                FT_Folder PFolder = new FT_FolderB().GetEntity(d => d.ID == newfolderid);
                PModel.Remark = PFolder.Remark + "-" + PModel.ID;
                new FT_FolderB().Update(PModel);

                foreach (FT_File file in item.SubFileS)
                {
                    file.FolderID = PModel.ID;
                    new FT_FileB().Insert(file);
                }

                GreateWDTree(item.SubFolder, PModel.ID);



            }
        }



        public void DelWDTree(int FolderID)
        {
            List<FoldFileItem> ListID = new List<FoldFileItem>();
            new FT_FolderB().GetWDTREE(FolderID, ref ListID);
            foreach (FoldFileItem listitem in ListID)
            {
                if (listitem.Type == "file")
                {
                    new FT_FileB().Delete(d => d.ID == listitem.ID);
                }
                else
                {
                    new FT_FolderB().Delete(d => d.ID == listitem.ID);
                }
            }

        }



        public class FoldFile
        {
            public int FolderID { get; set; }
            public string Name { get; set; }
            public string CRUser { get; set; }
            public int PFolderID { get; set; }
            public string Remark { get; set; }

            public List<FoldFile> SubFolder { get; set; }
            public List<FT_File> SubFileS { get; set; }

        }
        public class FoldFileItem
        {
            public int ID { get; set; }
            public string Type { get; set; }

        }
    }
    public class FT_FileB : BaseEFDao<FT_File>
    {
        public void AddVersion(FT_File oldmodel, string strMD5, string strSIZE)
        {
            FT_File_Vesion Vseion = new FT_File_Vesion();
            Vseion.FileMD5 = oldmodel.FileMD5;
            Vseion.RFileID = oldmodel.ID;
            new FT_File_VesionB().Insert(Vseion);
            //添加新版本

            oldmodel.FileVersin = oldmodel.FileVersin + 1;
            oldmodel.FileMD5 = strMD5;
            oldmodel.FileSize = strSIZE;
            new FT_FileB().Update(oldmodel);
            //修改原版本

        }


        public List<FT_File> getDT(string strWhere, int page, int pagecount, ref int total)
        {
            var dt = new FT_FileB().Db.Queryable<FT_File>().Where(strWhere).OrderBy(it => it.CRDate, OrderByType.Desc).ToPageList(page, pagecount, ref total);
            return dt;

        }

        /// <summary>
        /// 判断同一目录下是否有相同文件(不判断应用文件夹)
        /// </summary>
        /// <param name="strMD5"></param>
        /// <param name="strFileName"></param>
        /// <param name="FolderID"></param>
        /// <returns></returns>
        public FT_File GetSameFile(string strFileName, string strkzname, int FolderID)
        {
            int[] folders = { 1, 2, 3 };
            if (!folders.Contains(FolderID))
            {
                return new FT_FileB().GetEntities(d => d.Name == strFileName && d.FileExtendName == strkzname && d.FolderID == FolderID).FirstOrDefault();
            }
            return null;

        }

        /// <summary>
        /// 获取文件在服务器上的预览文件路径
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>


        /// <summary>
        /// 更新企业空间占用
        /// </summary>
        /// <param name="FileSize"></param>
        /// <returns></returns>
        public int AddSpace(string strUserName, int FileSize)
        {
            user qymodel = new JH_Auth_UserB().GetEntity(d => d.username == strUserName);
            if (qymodel != null)
            {
                qymodel.Space = qymodel.Space + FileSize;
            }
            new JH_Auth_UserB().Update(qymodel);
            return int.Parse(qymodel.Space.ToString());
        }






    }



    public class FT_File_DownhistoryB : BaseEFDao<FT_File_Downhistory>
    {

    }


    public class FT_File_ShareB : BaseEFDao<FT_File_Share>
    {

    }


    public class FT_File_UserAuthB : BaseEFDao<FT_File_UserAuth>
    {

    }


    public class FT_File_UserTagB : BaseEFDao<FT_File_UserTag>
    {

    }

    public class FT_File_VesionB : BaseEFDao<FT_File_Vesion>
    {

    }

    public class helpdataB : BaseEFDao<helpdata>
    {

    }

    #endregion
}
