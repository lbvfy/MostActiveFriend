using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MostActiveFriend
{
    
    public partial class Form1 : Form
    {
        private string accessToken;
        private int userID;
        private int max;
        private int mostActiveFriendID;
        Form answer;
        public Form1()
        {
           
            InitializeComponent();
            accessToken = "";
            userID = 5086307;
            webBrowser1.Navigate(String.Format("http://api.vkontakte.ru/oauth/authorize?client_id={0}&display=popup&redirect_uri=https://oauth.vk.com/blank.html&scope=friends&response_type=token", userID));
            max = 0;
        }

        private  void button1_Click(object sender, EventArgs e)
        {
          
            string jsonStr = GET(String.Format("https://api.vk.com/method/friends.get?order=hints&fields=can_see_all_posts&access_token={0}", accessToken));//получаем друзей
                 
            JObject o = JObject.Parse(jsonStr);
            JArray array = (JArray)o["response"];
            var v = JsonConvert.DeserializeObject<Response>(array.First().ToString());
            RootObject friendList = new RootObject();
            
            foreach (JObject content in array.Children<JObject>())
            {
                friendList.response.Add(JsonConvert.DeserializeObject<Response>(content.ToString()));

            }

            foreach (Response response in friendList.response)
            {
                jsonStr = GET(String.Format("https://api.vk.com/method/wall.get?owner_id={0}&filter=owner",response.user_id));
                // костыль простоты ради
                string[] arrStr1 = jsonStr.Split(',');
                string[] arrStr2 = arrStr1[0].Split('[');
                try
                {
                    var countPosts = (Int32.Parse(arrStr2[1]));
                }
                catch (System.FormatException formatException)
                {
                    continue;
                }
                catch (System.IndexOutOfRangeException indexOutOfRangeException)
                {
                    continue;
                }
                //
                if (max < (Int32.Parse(arrStr2[1])))
                {
                    max = Int32.Parse(arrStr2[1]);
                    mostActiveFriendID = response.user_id;
                }
            }

           
            answer = new Form2(mostActiveFriendID,max);
            answer.Show();

        }
       
       private void webBrowser1_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().IndexOf("access_token") != -1)
            {
                //string accessToken = "";
                int userId = 0;
                Regex myReg = new Regex(@"(?<name>[\w\d\x5f]+)=(?<value>[^\x26\s]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in myReg.Matches(e.Url.ToString()))
                {
                    if (m.Groups["name"].Value == "access_token")
                    {
                        accessToken = m.Groups["value"].Value;
                    }
                    else if (m.Groups["name"].Value == "user_id")
                    {
                        userId = Convert.ToInt32(m.Groups["value"].Value);
                    }
                    // еще можно запомнить срок жизни access_token - expires_in,
                    // если нужно
                }
                button1.Visible = true;
                //MessageBox.Show(String.Format("Ключ доступа: {0}\nUserID: {1}", accessToken, userId));
            }
        }
       private string GET(string Url)
       {
           
           WebRequest req = WebRequest.Create(Url);
           WebResponse resp = req.GetResponse();
           Stream stream = resp.GetResponseStream();
           StreamReader sr = new StreamReader(stream);
           string Out = sr.ReadToEnd();
           sr.Close();
         // MessageBox.Show(Out);
           return Out;
       }

        
    }
    
    public class FriendPostsCount
    {
        public int user_id;
        public int count;
    }
    public class Response
    {
        public int uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int can_see_all_posts { get; set; }
        public int online { get; set; }
        public int user_id { get; set; }
        public List<int> lists { get; set; }
        public string deactivated { get; set; }
    }

    public class RootObject
    {
        public List<Response> response { get; set; }

        public RootObject()
        {
            this.response = new List<Response>();
        }
    }

}
