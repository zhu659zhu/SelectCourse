using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelectCourse
{
    public partial class Form1 : Form
    {

        CookieContainer LoginCookie = new CookieContainer();

        string sid = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey softwareXXX = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("selectcourse",true);
            if (softwareXXX == null)
            {
                softwareXXX = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("selectcourse");
                softwareXXX.SetValue("username", textBox3.Text);
                softwareXXX.SetValue("password", textBox4.Text);
            }
            else
            {
                softwareXXX.SetValue("username", textBox3.Text);
                softwareXXX.SetValue("password", textBox4.Text);
            }

            textBox1.Text = Login(textBox3.Text, textBox4.Text);
            textBox3.Visible = false;
            textBox4.Visible = false;
            button1.Enabled = false;
        }

        public string Login(string username, string password)
        {
            string ret = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("http://sep.ucas.ac.cn/slogin"));
            request.Method = "POST";
            request.CookieContainer = LoginCookie;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36";
            request.Referer = "http://sep.ucas.ac.cn/";
            byte[] byteArray = Encoding.UTF8.GetBytes("userName="+System.Web.HttpUtility.UrlEncode(username)+"&pwd="+System.Web.HttpUtility.UrlEncode(password)+"&sb=sb&rememberMe=1"); //转化
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);//写入参数
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            LoginCookie = request.CookieContainer;

            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            ret = sr.ReadToEnd();
            
            WriteCookiesToDisk("cookie.txt", LoginCookie);
            response.Close();
            return ret;
        }

        

        public string PostData(string url, string datastr)
        {
            Console.WriteLine("POST页面" + url);
            string ret = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = "POST";
            //request.CookieContainer = LoginCookie;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36";
            request.Referer = "http://sep.ucas.ac.cn/";
            request.CookieContainer = LoginCookie;
            request.Timeout = 5000;
            byte[] byteArray = Encoding.UTF8.GetBytes(datastr); //转化
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);//写入参数
            newStream.Close();
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            ret = sr.ReadToEnd();
            LoginCookie = request.CookieContainer;
            response.Close();
            return ret;

        }//带登陆成功cookie发送数据

        private  string GetHtml(string url)
        {
            if (url == "")
                return "";
            Console.WriteLine("打开页面"+url);
            string ret = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Timeout = 5000;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            request.Referer = "http://sep.ucas.ac.cn/portal/site/226/821";
            request.CookieContainer = LoginCookie;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            LoginCookie = request.CookieContainer;
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            ret = sr.ReadToEnd();
            response.Close();
            
            return ret;
        }//带登陆成功cookie获取网页内容

        public string GetIdentity(string url)
        {
            if (url == "")
                return "";
            Console.WriteLine("打开页面" + url);
            string ret = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url + "?xxx=" + DateTime.Now.ToString("HHmmssfff")));
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Timeout = 5000;

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            request.CookieContainer = ReadCookiesFromDisk("cookie.txt");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            LoginCookie = request.CookieContainer;
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            ret = sr.ReadToEnd();
            response.Close();


            return ret;
        }


        public static void WriteCookiesToDisk(string file, CookieContainer cookieJar)
        {
            using (Stream stream = File.Create(file))
            {
                //try
                //{
                    Console.Out.Write("Writing cookies to disk... ");
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);
                    Console.Out.WriteLine("Done.");
                //}
                //catch (Exception e)
                //{
                //    Console.Out.WriteLine("Problem writing cookies to disk: " + e.GetType());
                //}
            }
        }//写cookie进文件

        public static CookieContainer ReadCookiesFromDisk(string file)
        {
            try
            {
                using (Stream stream = File.Open(file, FileMode.Open))
                {
                    Console.Out.Write("Reading cookies from disk... ");
                    BinaryFormatter formatter = new BinaryFormatter();
                    Console.Out.WriteLine("Done.");
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Problem reading cookies from disk: " + e.GetType());
                return new CookieContainer();
            }
        }//从文件中读cookie

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }  
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            Microsoft.Win32.RegistryKey softwareXXX = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("selectcourse");
            
            try
            {
                textBox3.Text = softwareXXX.GetValue("username").ToString();
                textBox4.Text = softwareXXX.GetValue("password").ToString();
                textBox8.Text = softwareXXX.GetValue("ccode").ToString();
                textBox2.Text = softwareXXX.GetValue("scode").ToString();
            }
            catch { }
            LoginCookie = ReadCookiesFromDisk("cookie.txt");
            comboBox3.SelectedIndex = 2;

        }



        private void button3_Click(object sender, EventArgs e)
        {

            //MessageBox.Show(ParseUrl(GetHtml("http://sep.ucas.ac.cn/portal/site/226")));
            try
            {
                string h = GetIdentity("http://sep.ucas.ac.cn/portal/site/226/821");
                string hurl = ParseUrl(h).Trim();
                textBox1.Text = GetHtml(hurl);
                if (textBox1.Text.IndexOf("退出系统") < 1)
                {
                    MessageBox.Show("用户失效...请重新登陆");
                    textBox3.Visible = true;
                    textBox4.Visible = true;
                    button1.Enabled = true;
                    return;
                }
                button4.Enabled = true;
                button5.Enabled = true;
                checkBox1.Enabled = true;
            }
            catch { MessageBox.Show("连接超时...请重试..."); }
            
            
        }

        public string ParseUrl(string html)
        {
            if (html == "")
                return "";
            string ret = "";
            Regex pattern = new Regex(@"content=""0;url=([\w\W]+?)"">");
            Match matchMode = pattern.Match(html);
            if (matchMode.Success)
            {
                ret = matchMode.Groups[1].Value;
            }
            
            return ret;
            //Regex pattern = new Regex(@"file_name"":""([\w\W]+?)"",""size"":""([\w\W]+?)""");
            //MatchCollection matchsMade = pattern.Matches(ret);
            //int i = 0;
            //foreach (Match item in matchsMade)
            //{
            //    i++;
            //    //MessageBox.Show(DeUnicode(item.Groups[1].Value));
            //    if (DeUnicode(item.Groups[1].Value).EndsWith(".mp4"))
            //    {
            //        moviemagnet = magnet;
            //        moviei = i;
            //        AddMagnet(magnet, i);
            //    }
            //}
        }

        public Course[] ParseSelectedCourse()
        {
            //http://jwxk.ucas.ac.cn/courseManage/selectedCourse
            string html = GetHtml("http://jwxk.ucas.ac.cn/courseManage/selectedCourse");
            
            Regex pattern = new Regex(@"<tr>
					<td>([\w\W]+?)</td>
					<td><a href=""/course/courseplan/([\w\W]+?)"" target=""_blank"">([\w\W]+?)</a></td>
					<td><a href=""/course/coursetime/[\w\W]+?"" target=""_blank"">([\w\W]+?)</a></td>
					<td>([\w\W]+?)</td>
					<td>([\w\W]+?)</td>
					<td>([\w\W]+?)</td>
                </tr>");
            MatchCollection matchsMade = pattern.Matches(html);
            Course[] courses = new Course[0];
            List<Course> clist = courses.ToList();  
            foreach (Match item in matchsMade)
            {
                string coursedetail = GetHtml("http://jwxk.ucas.ac.cn/course/coursetime/"+item.Groups[2].Value.Trim());

                Regex pattern2 = new Regex(@"<tr>
		   				<th>上课时间</th>
		   				<td>星期([\w\W]+?)： 第([\w\W]+?)节。</td>
		   			</tr>
		   			<tr>
		   				<th>上课地点</th>
		   				<td>([\w\W]+?)</td>
		   			</tr>
		   			<tr>
		   				<th>上课周次</th>
		   				<td>([\w\W]+?)</td>
		   			</tr>");
                MatchCollection matchsMade2 = pattern2.Matches(coursedetail);
                foreach (Match item2 in matchsMade2)
                {
                    Course c = new Course();
                    c.Course_ID = Convert.ToInt32(item.Groups[2].Value.Trim());
                    c.Course_Code = item.Groups[3].Value.Trim();
                    c.Course_Name = item.Groups[4].Value.Trim();
                    c.Course_Credit = Convert.ToDouble(item.Groups[5].Value.Trim());
                    c.Course_IsDegree = item.Groups[6].Value.Trim();
                    c.Course_Weekday = ConvertDay(item2.Groups[1].Value);
                    string[] sArray = Regex.Split(item2.Groups[2].Value, "、", RegexOptions.IgnoreCase);
                    int[] iArray = Array.ConvertAll<string, int>(sArray, s => int.Parse(s));
                    c.Course_DaySpan = iArray;
                    c.Course_TimeStr = "星期" + item2.Groups[1].Value + "： 第" + item2.Groups[2].Value + "节";
                    c.Course_Classroom = item2.Groups[3].Value;
                    sArray = Regex.Split(item2.Groups[4].Value, "、", RegexOptions.IgnoreCase);
                    iArray = Array.ConvertAll<string, int>(sArray, s => int.Parse(s));
                    c.Course_Weeks = iArray;
                    clist.Add(c);
                }
                
            }
            courses = clist.ToArray();  
            return courses;
        }



        public string GetPersonSchedule()
        {
            Course[] cs = ParseSelectedCourse();
            TimeSpan[,] ts = new TimeSpan[7, 11];
            for (int i = 0; i < cs.Length; i++)
            {
                TimeSpan t = new TimeSpan();
                t.Span_Text = cs[i].Course_Name + "<br />" + cs[i].Course_Classroom + "<br />" + cs[i].Course_Weeks[0] + "-" + cs[i].Course_Weeks[cs[i].Course_Weeks.Length-1]+"周";
                t.Span_Num =cs[i].Course_DaySpan.Length;
                ts[cs[i].Course_Weekday-1,cs[i].Course_DaySpan[0]-1]=t;
                for(int j=1;j<cs[i].Course_DaySpan.Length;j++)
                {
                    TimeSpan tempty = new TimeSpan();
                    tempty.Span_Num = -1;
                    ts[cs[i].Course_Weekday-1, cs[i].Course_DaySpan[j]-1] = tempty;
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(textBox7.Text);

            for (int i = 0; i < 11; i++)
            {
                sb.Append("<tr>");
                sb.Append("<th>" + (i + 1).ToString() + " " + GetTime(i+1) + "</th>");
                for (int j = 0; j < 7; j++)
                {
                    if (ts[j, i] == null)

                        sb.Append("<td></td>");
                    else
                    {
                        if (ts[j, i].Span_Num != -1)
                        {
                            if (ts[j, i].Span_Text != "")
                                sb.Append("<td " + "rowspan = '" + ts[j, i].Span_Num + "'>" + ts[j, i].Span_Text + "</td>");
                            else
                                sb.Append("<td></td>");
                        }
                    }
                }
                sb.Append("</tr>");
            }
            sb.Append(@"</tbody></table>
</body>
</html>");
            return sb.ToString();
        }

        public string GetTime(int i)
        {
            switch (i)
            {
                case 1:
                    return "[8:30-9:20]";
                case 2:
                    return "[9:20-10:10]";
                case 3:
                    return "[10:30-11:20]";
                case 4:
                    return "[11:20-12:10]";
                case 5:
                    return "[13:30-14:20]";
                case 6:
                    return "[14:20-15:10]";
                case 7:
                    return "[15:30-16:20]";
                case 8:
                    return "[16:20-17:10]";
                case 9:
                    return "[19:00-19:50]";
                case 10:
                    return "[19:50-20:40]";
                case 11:
                    return "[20:50-21:40]";
                default:
                    return "";
            }
        }

        public int ConvertDay(string temp)
        {
            if (temp == "一")
                return 1;
            else if (temp == "二")
                return 2;
            else if (temp == "三")
                return 3;
            else if (temp == "四")
                return 4;
            else if (temp == "五")
                return 5;
            else if (temp == "六")
                return 6;
            else
                return 7;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            string str = GetPersonSchedule();
            PersonSchedule p = new PersonSchedule(str);
            p.ShowDialog();
            try
            {
                

            }
            catch { MessageBox.Show("连接超时...请重试..."); }

        }

        public void CollegeQuery()
        {
            comboBox1.Items.Clear();
            string html = GetHtml("http://jwxk.ucas.ac.cn/courseManage/main");
            
            Regex pattern = new Regex(@"action=""/courseManage/selectCourse([\w\W]+?)""");
            Match matchMode = pattern.Match(html);
            if (matchMode.Success)
            {
                sid = matchMode.Groups[1].Value;
            }
            
            Regex patterns = new Regex(@"<div class=""span2""><input type=""checkbox"" name=""deptIds"" id=""id_[\w\W]+?"" value=""([\w\W]+?)""/> <label for=""id_[\w\W]+?"">([\w\W]+?)</label></div>");
            MatchCollection matchsMade = patterns.Matches(html);

            foreach (Match item in matchsMade)
            {
                comboBox1.Items.Add(item.Groups[2].Value + "-" + item.Groups[1].Value);
            }
            comboBox1.SelectedIndex = 0;

        }

        public void CourseQuery(string deptids)
        {
            comboBox2.Items.Clear();
            string coursedata = PostData("http://jwxk.ucas.ac.cn/courseManage/selectCourse" + sid, "deptIds=" + deptids + "&sb=0");
            textBox1.Text = coursedata;
            Regex pattern = new Regex(@"action=""/courseManage/selectCourse([\w\W]+?)""");
            Match matchMode = pattern.Match(coursedata);
            if (matchMode.Success)
            {
                sid = matchMode.Groups[1].Value;
            }

            Regex patterns = new Regex(@"<a href=""/course/coursetime/([\w\W]+?)"" target=""_blank"">([\w\W]+?)</a>");
            MatchCollection matchsMade = patterns.Matches(coursedata);

            foreach (Match item in matchsMade)
            {
                comboBox2.Items.Add(item.Groups[2].Value + "-" + item.Groups[1].Value);
            }
            if (comboBox2.Items.Count != 0)
            {
                comboBox2.SelectedIndex = 0;
                button2.Enabled = true;
            }
            else
                textBox1.Text="未找到课程...";

        }


        public void SelectCourse(string poststr)
        {
            string selectresult = PostData("http://jwxk.ucas.ac.cn/courseManage/saveCourse" + sid, poststr);
            //
            string ret = "";
            Regex pattern = new Regex(@"<div class=""head"">([\w\W]+?messageBoxSuccess[\w\W]+?</div>)");
            Match matchMode = pattern.Match(selectresult);
            if (matchMode.Success)
            {
                ret = matchMode.Groups[1].Value;
            }
            textBox1.Text = DateTime.Now+ "\r\n" + ret;
            if (ret.IndexOf("选课成功") > 0)
            {
                MessageBox.Show("选课成功~");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            try{
                if (comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择课程");
                    return;
                }
                if (checkBox2.Checked)
                    SelectCourse("deptIds=" + comboBox1.SelectedItem.ToString().Substring(comboBox1.SelectedItem.ToString().LastIndexOf("-") + 1) + "&sids=" + comboBox2.SelectedItem.ToString().Substring(comboBox2.SelectedItem.ToString().LastIndexOf("-") + 1) + "&did_" + comboBox2.SelectedItem.ToString().Substring(comboBox2.SelectedItem.ToString().LastIndexOf("-") + 1) + "=" + comboBox2.SelectedItem.ToString().Substring(comboBox2.SelectedItem.ToString().LastIndexOf("-") + 1));
                else
                    SelectCourse("deptIds=" + comboBox1.SelectedItem.ToString().Substring(comboBox1.SelectedItem.ToString().LastIndexOf("-") + 1) + "&sids=" + comboBox2.SelectedItem.ToString().Substring(comboBox2.SelectedItem.ToString().LastIndexOf("-") + 1));
            
            }
            catch { MessageBox.Show("连接超时...请重试..."); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try{
                CollegeQuery();
                button6.Enabled = true;
            }
            catch { MessageBox.Show("连接超时...请重试..."); }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                CourseQuery(comboBox1.SelectedItem.ToString().Substring(comboBox1.SelectedItem.ToString().LastIndexOf("-") + 1));
            }
            catch { MessageBox.Show("连接超时...请重试..."); }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                //多门课
                //deptIds=951&sids=125756&did_125756=125756&sids=125762&did_125762=125762
                try
                {
                    if (checkBox2.Checked)
                        SelectCourse("deptIds=" + textBox5.Text + "&sids=" + textBox6.Text + "&did_" + textBox6.Text + "=" + textBox6.Text);
                    else
                        SelectCourse("deptIds=" + textBox5.Text + "&sids=" + textBox6.Text);
                }
                catch { MessageBox.Show("连接超时...请重试..."); }
            }

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(comboBox3.SelectedItem.ToString());
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            comboBox1.Width = this.Width - comboBox1.Left - 15;
            comboBox2.Width = this.Width - comboBox2.Left - 15;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string str = GetHtml("http://jwxk.ucas.ac.cn/course/termSchedule");
            
            Regex reg = new Regex (@"<a href=""/course/courseplan/([\w\W]*?)"" target=""_blank"">([\w\W]*?)</a>[\w\W]*?<a href=""/course/coursetime/[\w\W]*?"" target=""_blank"">([\w\W]*?)</a>");
            MatchCollection matchsMade = reg.Matches(str);
            if (matchsMade.Count == 0)
            {
                MessageBox.Show("请先点击进入系统...");
                return;
            }
            FileStream fs = new FileStream("code.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            try
            {
                foreach (Match item in matchsMade)
                {
                    sw.Write(item.Groups[2].Value + " " + item.Groups[3].Value + " " + item.Groups[1].Value + "\r\n");
                }
                MessageBox.Show("读取完成...");
            }
            catch { MessageBox.Show("发生了一些错误..."); }
            sw.Close();
            fs.Close();
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("请先输入账号密码登陆...");
                textBox3.Visible = true;
                textBox4.Visible = true;
                button1.Enabled = true;
                return;
            }

            try
            {
                textBox1.Text = Login(textBox3.Text, textBox4.Text);
                string h = GetIdentity("http://sep.ucas.ac.cn/portal/site/226/821");
                string hurl = ParseUrl(h).Trim();
                textBox1.Text = GetHtml(hurl);

                Microsoft.Win32.RegistryKey softwareXXX = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("selectcourse", true);
                if (softwareXXX != null)
                {
                    softwareXXX.SetValue("ccode", textBox8.Text);
                    softwareXXX.SetValue("scode", textBox2.Text);
                }

                if (textBox1.Text.IndexOf("退出系统") < 1)
                {
                    MessageBox.Show("error...");
                }
                else
                {
                    CollegeQuery();
                    CourseQuery(textBox8.Text);
                    SelectCourse(textBox2.Text);
                }

            }
            catch { MessageBox.Show("连接超时...请重试..."); }
        }


    }
}
