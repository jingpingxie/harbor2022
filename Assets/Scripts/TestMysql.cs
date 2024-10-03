using MySql.Data.MySqlClient;
using RoadArchitect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class TestMysql : MonoBehaviour
    {
        //建立连接语句
        //charset=utf8这句要写，不然可能会报错                                 
        string constr = "server=127.0.0.1;User Id=root;password=test123456;Database=harbor;charset=utf8";
        //建立连接
        public MySqlConnection mycon;

        void Start()
        {
            ConnectMysql();
            SearchMysql();
            //UpadteMysql();
        }

        private void ConnectMysql()
        {
            //建立连接
            mycon = new MySqlConnection(constr);
            //打开连接
            mycon.Open();

            bool isOK = mycon.Ping();
            if (isOK)
            {
                Debug.Log("数据库已连接");
            }
            else
            {
                Debug.Log("数据库连接错误");
            }
        }
        private void SearchMysql()
        {
            //查询数据
            string selstr = "select * from roads";
            MySqlCommand myselect = new MySqlCommand(selstr, mycon);

            DataSet ds = new DataSet();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(selstr, mycon);
                da.Fill(ds);
                Debug.Log("数据库数据:");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        Debug.Log(ds.Tables[0].Rows[i][j]);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("SQL:" + selstr + "\n" + e.Message.ToString());
            }
        }
        private void UpadteMysql()
        {
            //修改数据
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                cmd.Connection = mycon;
                cmd.CommandText = "UPDATE studentscores SET name = @name WHERE guid = @guid";


                Debug.Log("取出guid=1的元组，更改属性为name=C#Test");
                String name = "C#Test";
                String guid = "1";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@guid", guid);


                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                mycon.Close();
            }
        }
    }
}
