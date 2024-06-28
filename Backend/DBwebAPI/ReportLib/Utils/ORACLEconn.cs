using Oracle.ManagedDataAccess.Client;
using SqlSugar;

namespace ReportLib.Utils
{
    public class DBconn
    {
        public static string constr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=124.222.153.54)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xe)));Persist Security Info=True;User ID=oracle;Password=oracle;";
        public SqlSugarScope sqlORM = null;
        public bool Conn()
        {
            try
            {
                OracleConnection con = new OracleConnection(constr);
                con.Open();
                sqlORM = new SqlSugarScope(new ConnectionConfig()
                {
                    ConnectionString = constr,
                    DbType = DbType.Oracle,
                    IsAutoCloseConnection = true
                },
                db =>
                {
                    //调试SQL事件，可以删掉
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        //Console.WriteLine(sql);//输出sql,查看执行sql
                    };
                }

                );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public class ORACLEconn
    {
        public SqlSugarScope sqlORM = null;
        public bool getConn()
        {
            Console.WriteLine("get access!");

            //创建数据库连接
            DBconn dbconn = new DBconn();
            //打开ORM
            dbconn.Conn();

            sqlORM = dbconn.sqlORM;

            if (sqlORM != null)
            {
                Console.WriteLine("open success!"); return true;
            }
            else return false;
        }
    }
}
