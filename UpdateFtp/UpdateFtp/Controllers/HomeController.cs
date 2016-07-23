using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using UpdateFtp.Models;

namespace UpdateFtp.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        string ftpServerIP;
        string ftpRemotePath;
        string ftpUserID;
        string ftpPassword;
        string ftpURI;

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="FtpServerIP"></param>
        /// <param name="FtpRemotePath"></param>
        /// <param name="FtpUserID"></param>
        /// <param name="FtpPassword"></param>
        public void FtpWeb(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            //FtpRemotePath = TransChinese(FtpRemotePath);
            ftpServerIP = FtpServerIP;
            ftpRemotePath = FtpRemotePath;
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath;
        }

        /// <summary>
        /// 从ftp服务器上获得文件列表
        /// </summary>
        /// <param name="RequedstPath">服务器下的相对路径</param>
        /// <returns></returns>
        public string[] GetFile(string RequedstPath)
        {
            string[] downloadFile;
            try
            {
                StringBuilder result = new StringBuilder();
                string uri = ftpURI + '/'+ RequedstPath;   //目标路径 path为服务器地址
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);//中文文件名

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Substring(0,1) != 'd'.ToString())
                    {
                        result.Append(line);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                downloadFile = null;
                Console.WriteLine("获取文件出错：" + ex.Message);
                return downloadFile;
            }
        }

        /// <summary>
        /// 从ftp服务器上获得文件夹列表
        /// </summary>
        /// <param name="RequedstPath">服务器下的相对路径</param>
        /// <returns></returns>
        public string[] GetDirctory(string RequedstPath)
        {
            string[] downloadDirctory;
            try
            {
                StringBuilder result = new StringBuilder();
                string uri = ftpURI + '/' + RequedstPath;   //目标路径 path为服务器地址
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);//中文文件名

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Substring(0, 1) == 'd'.ToString() && line.Substring(line.Length-1) != '.'.ToString())
                    {
                        result.Append(line);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                downloadDirctory = null;
                Console.WriteLine("获取文件出错：" + ex.Message);
                return downloadDirctory;
            }
        }

        /// <summary>
        /// 取文件名
        /// </summary>
        /// <param name="con">本次遍历的文件</param>
        /// <returns></returns>
        public static string[] SelectName(string[] con)
        {
            var l = con.Length;
            string[] info = new string[l];
            for (var i = 0; i < l; i++)
            {
                string[] c = System.Text.RegularExpressions.Regex.Split(con[i].ToString(), @"\s+");
                for (var n = 8; n < c.Length; n++)
                {
                    info[i] = info[i] + c[n] + ' ';
                }
            }
            return info;
        }
        /// <summary>
        /// 取更新时间
        /// </summary>
        /// <param name="con">本次遍历的文件</param>
        /// <returns></returns>
        public static string[] SelectTime(string[] con)
        {
            var l = con.Length;
            string[] info = new string[l];
            for (var i = 0; i < l; i++)
            {
                string[] c = System.Text.RegularExpressions.Regex.Split(con[i].ToString(), @"\s+");
                info[i] = c[5] + ' ' + c[6] + ' ' + c[7];
            }
            return info;
        }

        /// <summary>
        /// 获取指定文件大小
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public string GetFileSize(string filepath)
        {
            FtpWebRequest FTP;
            long fileSize = 0;
            string finalSize = null;
            try
            {
                FTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + filepath));
                FTP.UseBinary = true;
                FTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FTP.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)FTP.GetResponse();

                fileSize = response.ContentLength;
                if (fileSize == 0)
                    finalSize = null;
                else if (fileSize < 1024.00)
                    finalSize = fileSize.ToString("F2") + " Byte";
                else if (fileSize >= 1024.00 && fileSize < 1048576)
                    finalSize = (fileSize / 1024.00).ToString("F2") + " K";
                else if (fileSize >= 1048576 && fileSize < 1073741824)
                    finalSize = (fileSize / 1024.00 / 1024.00).ToString("F2") + " M";
                else if (fileSize >= 1073741824)
                    finalSize = (fileSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
                //return finalSize;  
                response.Close();
                return finalSize;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取文件出错：" + ex.Message);
                return finalSize;
            }
        }

        /// <summary>
        /// 添加此目录所有文件
        /// </summary>
        /// <param name="all"></param>
        /// <param name="con"></param>
        /// <param name="files"></param>
        /// <param name="now"></param>
        public void addcon(HomeController all, List<ContentModels> con, string[] files, string now)
        {
            var file_l = files.Length;
            var name = SelectName(files);
            var time = SelectTime(files);
            string link = null;//中文路径
            string url = null;//编码路径
            string a = null;
            for (var i = 0; i < file_l; i++)
            {
                link = all.ftpURI + now + name[i];
                foreach (var letter in link)
                {
                    if ((int)letter > 127)
                    {
                        a = System.Web.HttpUtility.UrlEncode(letter.ToString(), System.Text.Encoding.GetEncoding("GB2312"));
                        url += a;
                    }
                    else
                    {
                        url += letter;
                    }
                }
                
                con.Add(new ContentModels()
                {
                    Name = name[i],
                    Link = url,
                    Size = all.GetFileSize(now + name[i]),
                    Time = time[i]
                });
            }
        }

        /// <summary>
        /// 从ftp取得所有文件
        /// </summary>
        /// <param name="ftp">ftpIP地址</param>
        /// <returns></returns>
        public static List<ContentModels> GetAll(string ftp)
        {
            List<ContentModels> con = new List<ContentModels>();
            HomeController all = new HomeController();
            all.FtpWeb(ftp,null,null,null);
            Stack<string> dir = new Stack<string>();
            dir.Push(null);
            string[] dirctory;
            string[] dir_name;
            string[] file;
            string path;
            while (dir.Count > 0)
            {
                path = dir.Pop();
                file = all.GetFile(path);
                if (file != null)
                    all.addcon(all, con, file, path);
                dirctory = all.GetDirctory(path);
                if (dirctory != null)
                {
                    dir_name = SelectName(dirctory);
                    foreach (var d in dir_name)
                    {
                        //dir.Push(path + TransChinese(d) + '/');
                        dir.Push(path + d + '/');
                    }
                }
            }
            return con;
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="con">从ftp取得的数据</param>
        /// <returns></returns>
        public static void ExcuteNonQuery(List<ContentModels> con)
        {
            string connStr = ConfigurationManager.ConnectionStrings["FtpContentContext"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "truncate table movie";
                cmd.ExecuteNonQuery();
                foreach (var c in con)
                {
                    cmd.CommandText = "insert into movie (name,link,size,time) values('" + c.Name + "','" + c.Link + "','" + c.Size + "','" + c.Time + "')";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        public void Timer_Elapsed(object sender, ElapsedEventArgs e)

    {
        while (true)
            {
                DateTime now = System.DateTime.Now;
                int h = now.Hour;
                //int m = now.Minute;
                if (h == 0)
                {
                    string ftp;
                    ftp = "192.168.123.39:22";
                    List<ContentModels> con = GetAll(ftp);
                    if (con != null)
                        ExcuteNonQuery(con);
                    ftp = "192.168.123.39";
                    con = GetAll(ftp);
                    if (con != null)
                        ExcuteNonQuery(con);
                }
                else break;
            }
    }
        public ActionResult Index()
        {
            System.Timers.Timer _timer;
            int _Interval=2400000;
            _timer = new System.Timers.Timer();
            _timer.Enabled = true;
            _timer.Interval = _Interval;
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            return null;
        }
    }
}