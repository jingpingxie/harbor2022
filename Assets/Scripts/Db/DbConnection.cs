using MySql.Data.MySqlClient;
using System;


namespace Assets.Scripts.Db
{


    public class DbConnection
    {
        //server=127.0.0.1;User Id=root;password=test123456;Database=harbor;charset=utf8";
        private string server = "127.0.0.1";// 或你的服务器地址
        private string database = "harbor";// 你的数据库名
        private string uid = "root";// 你的数据库用户名
        private string password = "test123456";// 你的数据库密码
        private string connectionString;

        public DbConnection()
        {
            Initialize();
        }

        private void Initialize()
        {
            string port = "3306"; // MySQL默认端口是3306，如果不是默认，需要修改这里
            string sslMode = "none"; // 根据需要设置SSL模式，例如：none, preferred, required, verify-ca, verify-full
            string additionalOptions = "Allow User Variables=True"; // 根据需要添加额外的选项
            string format = "Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode={5}; {6};charset=utf8";
            connectionString = String.Format(format, server, port, database, uid, password, sslMode, additionalOptions);
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
