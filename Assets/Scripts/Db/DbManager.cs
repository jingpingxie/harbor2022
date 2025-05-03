using MySql.Data.MySqlClient;
using System.Data;


namespace Assets.Scripts.Db
{

    public static class DbManager
    {
        private static DbConnection dbConnection;


        public static void Initialize()
        {
            dbConnection = new DbConnection();
        }

        public static DataTable ExecuteQuery(string query)
        {
            using (MySqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string query)
        {
            using (MySqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    int result = cmd.ExecuteNonQuery(); // 返回影响的行数或自动生成的ID（对于INSERT操作）
                    return result;
                }
            }
        }
    }
}

//使用例
//DbManager dbManager = new DbManager();
//// 查询操作示例：获取所有记录的列表。假设表名为`users`，有两列`id`和`name`。
//string query = "SELECT * FROM users"; // 查询语句，可以根据需要进行修改。
//DataTable resultTable = dbManager.ExecuteQuery(query); // 执行查询操作。
//foreach (DataRow row in resultTable.Rows) // 遍历结果集。 
//{
//}