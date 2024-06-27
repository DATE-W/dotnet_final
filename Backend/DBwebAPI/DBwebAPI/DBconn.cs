
using Oracle.ManagedDataAccess.Client;
using SqlSugar;


namespace DBwebAPI
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


        //public SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        //{
        //    ConnectionString = constr,//连接符字串
        //    DbType = DbType.Oracle,//数据库类型
        //    IsAutoCloseConnection = true
        //},
        //db => {
        //    //调试SQL事件，可以删掉
        //    db.Aop.OnLogExecuting = (sql, pars) =>
        //    {
        //        //Console.WriteLine(sql);//输出sql,查看执行sql
        //    };
        //});


    }




    //public class SQLSugar
    //{


    //    public static string constr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));Persist Security Info=True;User ID=c##test;Password=021120;";

    //    public SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
    //    {
    //        ConnectionString = constr,
    //        DbType = DbType.Oracle,
    //        IsAutoCloseConnection = true
    //    });

    //}
}
