using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACTinportLog
{
    class Program
    {


        static void Main(string[] args)
        {
            string str = "[22:16:33.430] 25:40002D05::00002C42:21267101:28938440:10000:10000:0::99.99231:99.99231:1.192093E-07:-4.792213E-05:03E8:0:56BF:0:";

            string timestr = str.Substring(1, 12);
            string log = str.Substring(15);

            DateTime dTime = DateTime.Parse(timestr);


            Console.WriteLine("test");

            testPG testPG1= new testPG();

            List<string>  test = testPG1.textRead();

            foreach (var item in test)
            {
                string[] logs = item.Split(',');
                Thread.Sleep(int.Parse(logs[0]));

                ActGlobals.oFormActMain.ParseRawLogLine(true ,DateTime.Now, logs[1]);

                Console.WriteLine(item);
            }


        }


    }

    class testPG { 
        public List<string> textRead()
        {
             List<string> LogList = new List<string>();
            StreamReader sr = new StreamReader(@"C:\Users\tendo\Downloads\log\絶アレキの今日のlog2.txt", Encoding.GetEncoding("Shift_JIS"));
            DateTime dateTime2 = DateTime.Now;
            int cnt = 0;

            while (sr.Peek() != -1)
            {

                string str = sr.ReadLine();

                string timestr = str.Substring(1, 8);
                string log = str.Substring(15);

                DateTime dTime = DateTime.Parse(timestr);

                if (cnt == 0 )
                {
                    dateTime2 = dTime;
                    LogList.Add("0," + log);
                }
                else
                {
                    int sa = dTime.Second - dateTime2.Second;
                    dateTime2 = dTime;
                    if (0 > sa)
                    {
                        sa = 0;
                    }
                    LogList.Add(sa.ToString() + "," + log);
                }
                cnt++;
            }
            sr.Close();

            return LogList;
        }
    }
}
