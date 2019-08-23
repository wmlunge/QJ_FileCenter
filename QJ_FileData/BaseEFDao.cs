using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace QJFile.Data
{
    /// <summary>
    /// 公共接口
    /// </summary>
    public class BaseEFDao<T> : IBaseDao<T> where T : class, new()   //限制T为class
    {
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public SimpleClient<T> CurrentDb { get { return new SimpleClient<T>(Db); } }//用来处理T表的常用操作
        public static string ConnectionString = @"DataSource="+ AppDomain.CurrentDomain.BaseDirectory + "FileCenter.db";

        public BaseEFDao()
        {


            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = SqlSugar.DbType.Sqlite,
                InitKeyType = InitKeyType.SystemTable,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了

            });
            // 调式代码 用来打印SQL
            //Db.Aop.OnLogExecuting = (sql, pars) =>
            //{
            //    WriteLOG(sql + "\r\n" + Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            //};

        }

        public void CModel()
        {
            Db.DbFirst.CreateClassFile("E:\\Demo", "QJY.Data");
        }





        public virtual IEnumerable<T> GetALLEntities()
        {

            //AsNoTracking不记录数据变化状况
            return CurrentDb.GetList();

        }
        public virtual IEnumerable<T> GetEntities(Expression<Func<T, bool>> exp)
        {

            //AsNoTracking不记录数据变化状况
            return CurrentDb.GetList(exp).ToList();


        }





        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="exp">lambda查询条件where</param>
        /// <returns></returns>
        public virtual T GetEntity(Expression<Func<T, bool>> exp)
        {
            return CurrentDb.GetList(exp).SingleOrDefault();

        }

        /// <summary>
        /// 获取所有Entity(立即执行请使用ToList()
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <param name="objParams">可变参数</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntities(string CommandText)
        {
            return Db.SqlQueryable<T>("select * from " + typeof(T).Name + " where 1=1 and  " + CommandText).ToList();
        }


        /// <summary>
        /// 插入Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Insert(T entity)
        {
            entity = Db.Insertable<T>(entity).ExecuteReturnEntity();
            //int dataID = CurrentDb.InsertReturnIdentity(entity);
            //List<string> List = Db.DbMaintenance.GetIsIdentities(entity.GetType().Name);
            //if (List.Count > 0)
            //{
            //    //如果有自增,赋值
            //    entity.GetType().GetProperty(List[0].ToString()).SetValue(entity, dataID, null);
            //}
            return true;
        }

        /// <summary>
        /// 同时插入多个实体。
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual bool Insert(IEnumerable<T> entities)
        {
            int Return = 0;
            if (entities.Count() > 0)
            {
                Return = Db.Insertable(entities.ToArray()).ExecuteCommand();
            }
            return Return > 0;
        }


        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual bool Update(List<T> entities)
        {
            int Return = 0;
            if (entities.Count() > 0)
            {
                Return = Db.Updateable(entities).With(SqlWith.UpdLock).ExecuteCommand();
            }
            return Return > 0;

        }


        /// <summary>
        /// 更新Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Update(T entity)
        {
            return CurrentDb.Update(entity);

        }

        /// <summary>
        /// 删除Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Delete(T entity)
        {
            return CurrentDb.Delete(entity);

        }
        /// <summary>
        /// 批量删除Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Delete(Expression<Func<T, bool>> exp)
        {
            return CurrentDb.Delete(exp);
        }



        /// <summary>
        /// 根据条件查找
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <param name="objParams">可变参数</param>
        /// <returns></returns>
        public virtual DataTable GetDTByCommand(string CommandText)
        {
            return Db.Ado.GetDataTable(CovSQL(CommandText));
        }



        /// <summary>
        /// 获取链接字符串
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <param name="objParams">可变参数</param>
        /// <returns></returns>
        public string GetDBString()
        {
            return Db.CurrentConnectionConfig.ConnectionString;

        }


        //执行SQL语句
        public void ExsSql(string sql)
        {
            List<SugarParameter> parameters = null;
            Db.Ado.ExecuteCommand(CovSQL(sql), parameters);
        }

        public object ExsSclarSql(string sql)
        {
            List<SugarParameter> parameters = null;
            return Db.Ado.GetString(CovSQL(sql), parameters);
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        private string CovSQL(string strSQL)
        {
            if (Db.CurrentConnectionConfig.DbType == 0)//MYSQL数据库,SQLlite数据库
            {
                strSQL = strSQL.Replace("isnull", "ifnull").Replace("ISNULL", "IFNULL");
            }
            if (Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Sqlite)//MYSQL数据库,SQLlite数据库
            {
                strSQL = strSQL.Replace("isnull", "ifnull").Replace("ISNULL", "IFNULL");
            }
            return strSQL;
        }



        /// <summary>
        /// 数据分页
        /// </summary>
        /// <param name="viewName">表名</param>
        /// <param name="fieldName">字段</param>
        /// <param name="pageSize">默认20</param>
        /// <param name="pageNo">页数</param>
        /// <param name="orderString">排序</param>
        /// <param name="whereString">可选</param>
        /// <param name="recordTotal">总数</param>
        /// <returns></returns>
        public DataTable GetDataPager(string viewName, string fieldName, int pageSize, int pageNo, string orderString, string whereString, ref int recordTotal)
        {

            DataTable dt = Db.SqlQueryable<Object>("select  " + fieldName + "  from " + viewName + " where " + whereString).OrderBy(orderString).ToDataTablePage(pageNo, pageSize, ref recordTotal);
            return dt;
        }



        /// <summary>
        /// 行专列
        /// </summary>
        /// <param name="CommandText">Sql语句</param>
        /// <param name="objParams">可变参数</param>
        /// <returns></returns>
        public string GetDTHZL(string ExtendModes, string pdid)
        {
            string strSQL = "";
            if (Db.CurrentConnectionConfig.DbType == 0)//MYSQL数据库
            {
                string strTemp = "";
                foreach (string filename in ExtendModes.Split(','))
                {
                    strTemp = strTemp + " MAX(CASE ExFiledColumn WHEN '" + filename + "' THEN ExtendDataValue ELSE 0 END ) " + filename + ",";
                }
                strTemp = strTemp.TrimEnd(',');
                strSQL = " SELECT * FROM ( SELECT  uuid() AS ID, DataID,Yan_WF_PI.ISGD,Yan_WF_PI.BranchNo,Yan_WF_PI.BranchName,Yan_WF_PI.CRUser,Yan_WF_PI.CRUserName,  Yan_WF_PI.CRDate, " + strTemp + " FROM JH_Auth_ExtendData    INNER JOIN Yan_WF_PI  ON JH_Auth_ExtendData.DataID=Yan_WF_PI.ID  AND  JH_Auth_ExtendData.PDID='" + pdid + "' GROUP BY DataID,Yan_WF_PI.ISGD,Yan_WF_PI.BranchNo,Yan_WF_PI.BranchName,Yan_WF_PI.CRUser,Yan_WF_PI.CRUserName,  Yan_WF_PI.CRDate ) T  ";

            }
            else //sqlServer数据库
            {
                strSQL = " SELECT NEWID() AS ID, * FROM (  SELECT   Yan_WF_PI.ISGD,Yan_WF_PI.BranchNo,Yan_WF_PI.BranchName,JH_Auth_ExtendData.DataID, Yan_WF_PI.CRUser,Yan_WF_PI.CRUserName,  Yan_WF_PI.CRDate, JH_Auth_ExtendMode.TableFiledColumn,  ExtendDataValue  from JH_Auth_ExtendMode INNER JOIN JH_Auth_ExtendData ON JH_Auth_ExtendMode.PDID=JH_Auth_ExtendData.PDID and JH_Auth_ExtendMode.TableFiledColumn=JH_Auth_ExtendData.ExFiledColumn and JH_Auth_ExtendMode.PDID='" + pdid + "' INNER JOIN Yan_WF_PI  ON JH_Auth_ExtendData.DataID=Yan_WF_PI.ID  ) AS P PIVOT ( MAX(ExtendDataValue) FOR  p.TableFiledColumn  IN (" + ExtendModes + ") ) AS T ";

            }
            return strSQL;
        }

        public IEnumerable<T> GetEntities(Expression<Func<T, bool>> exp, string strComId)
        {
            throw new NotImplementedException();
        }
    }
}