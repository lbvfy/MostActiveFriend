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
    public partial class Form2 : Form
    {
        public Form2(int user_ID,int postsValue)
        {
            InitializeComponent();
            string answer = GET(String.Format("https://api.vk.com/method/users.get?user_ids={0}",user_ID));
            
            // repeated from Form1
            JObject o = JObject.Parse(answer);
            JArray array = (JArray)o["response"];
            var v = JsonConvert.DeserializeObject<Response>(array.First().ToString());
            UserObject friendList = new UserObject();

            foreach (JObject content in array.Children<JObject>())
            {
                friendList.userResponse.Add(JsonConvert.DeserializeObject<UserResponse>(content.ToString()));

            }
            textBox1.Text = friendList.userResponse[0].first_name;
            textBox2.Text = friendList.userResponse[0].last_name;
            textBox3.Text = postsValue.ToString();
        }

        private string GET(string Url)
     {
           
           WebRequest req = WebRequest.Create(Url);
           WebResponse resp = req.GetResponse();
           Stream stream = resp.GetResponseStream();
           StreamReader sr = new StreamReader(stream);
           string Out = sr.ReadToEnd();
           sr.Close();
           return Out;
      }
    }
    public class UserResponse
    {
        public int uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class UserObject
    {
        public List<UserResponse> userResponse { get; set; }
        public UserObject()
        {
            this.userResponse = new List<UserResponse>();
        }
    }
     
}
