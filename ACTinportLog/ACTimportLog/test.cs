using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.IO;
using System.Threading;

namespace ACTimportLog
{
    public partial class test : UserControl, IActPluginV1
    {
        public test()
        {
            InitializeComponent();
        }

        public void DeInitPlugin()
        {
        }

        List<string> vs = new List<string>();

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {

            //lbStatus = pluginStatusText;   // Hand the status label's reference to our local var
            pluginScreenSpace.Controls.Add(this);   // Add this UserControl to the tab ACT provides
            this.Dock = DockStyle.Fill; // Expand the UserControl to fill the tab's client space
                                        // MultiProject.BasePlugin.xmlSettings = new SettingsSerializer(this); // Create a new settings serializer and pass it this instance

            pluginScreenSpace.Text = "testtest";
            pluginStatusText.Text = "testestsetsetsetste";



            string str = "[22:16:33.430] 25:40002D05::00002C42:21267101:28938440:10000:10000:0::99.99231:99.99231:1.192093E-07:-4.792213E-05:03E8:0:56BF:0:";

            string timestr = str.Substring(1, 12);
            string log = str.Substring(15);

            DateTime dTime = DateTime.Parse(timestr);


            Console.WriteLine("test");


            vs = textRead();

            Task.Run(() => Checklog());

            ActGlobals.oFormActMain.OnLogLineRead += OFormActMain_OnLogLineRead;

        }

        private void OFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            string str = "";
        }

        private void Checklog()
        {
            Thread.Sleep(10000);
            foreach (var item in vs)
            {
                string[] logs = item.Split(',');
                Thread.Sleep(int.Parse(logs[0]));

                label1.Text = logs[0] + logs[1];

                ActGlobals.oFormActMain.ParseRawLogLine(true, DateTime.Now, logs[1]);

                Console.WriteLine(item);
            }
        }


        public List<string> textRead()
        {
            List<string> LogList = new List<string>();
            StreamReader sr = new StreamReader(@"C:\Users\tendo\Downloads\log\絶アレキの今日のlog2.txt", Encoding.GetEncoding("Shift_JIS"));
            DateTime dateTime2 = DateTime.Now;
            int cnt = 0;

            while (sr.Peek() != -1)
            {

                string str = sr.ReadLine();

                string timestr = str.Substring(1, 12);
                string log = str.Substring(15);

                DateTime dTime = DateTime.Parse(timestr);

                if (cnt == 0)
                {
                    dateTime2 = dTime;
                    LogList.Add("0," + log);
                }
                else
                {
                    int sa = ((dTime.Second * 1000) + dTime.Millisecond) - ((dateTime2.Second * 1000) + dateTime2.Millisecond);
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
