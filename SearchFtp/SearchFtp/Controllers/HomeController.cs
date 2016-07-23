using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearchFtp.Models;

namespace SearchFtp.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewData["ConList"] = ExcuteNonQuery("select top 15 * from Movie order by Time desc");
            return View();
        }
        public static List<ContentModels> ExcuteNonQuery(string sql)
        {
            string connStr = ConfigurationManager.ConnectionStrings["FtpContentContext"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql; //写SQL语句
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();//执行SQL语句，返回结果集
                //SqlDataAdapter dbAdapter = new SqlDataAdapter(cmd); //创建数据适配器对象,起连接数据库和数据集的作用
                //DataSet ds = new DataSet(); //创建数据集对象
                //dbAdapter.Fill(ds);//将结果填充到DataSet中
                List<ContentModels> con = new List<ContentModels>();
                while (reader.Read())
                {
                    ContentModels file = new ContentModels();
                    file.Name = reader["Name"].ToString();
                    file.Link = reader["Link"].ToString();
                    file.Size = reader["Size"].ToString();
                    file.Time = reader["Time"].ToString();
                    con.Add(file);
                }
                return con;
            }
        }
        public ActionResult Search(string text)
        {
            string[] words = System.Text.RegularExpressions.Regex.Split(text, @"\s+");
            string sql;
            sql = "select * from Movie where";
            if (words.Length == 1)
            {
                sql += ' ' + "name like '%" + text + "%'";
                ViewData["ConList"] = ExcuteNonQuery(sql);
            }
            else
            {
                sql += ' '+"name like '%" + words[0] + "%'";
                for (var i = 1; i < words.Length;i++ )
                {
                    sql += ' ' + "and name like '%" + words[i] + "%'";
                }
                ViewData["ConList"] = ExcuteNonQuery(sql);
            }
            return View("Index"); 
        }
    }
}