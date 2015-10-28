using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace MostActiveFriend
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
           

        }

        private static long GetUserId(string url)
        {
            var getIdRegex = new Regex(@"vk.com/id(?<id>\d+)|(?<nick>(\w\d*))");
            var match = getIdRegex.Match(url);
            if (match.Groups["id"].Success)
                return Convert.ToInt64(match.Groups["id"].Value);
            else
                return ResolveName(match.Groups["nick"].Value);
        }

        public static long ResolveName(string p)
        {
            throw new NotImplementedException();
        }

    }
}

     


