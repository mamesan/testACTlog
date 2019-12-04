using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.IO;
using System.Threading;

namespace ACTLogActionChecker
{
    public partial class ActionChecker : UserControl, IActPluginV1
    {
        private SettingsSerializer xmlSettings;
        List<string> vs = new List<string>();

        public ActionChecker()
        {
            InitializeComponent();
        }

        public void DeInitPlugin()
        {
            ACTInitSetting.SaveSettings(this.xmlSettings);
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            //lbStatus = pluginStatusText;   // Hand the status label's reference to our local var
            pluginScreenSpace.Controls.Add(this);   // Add this UserControl to the tab ACT provides
            Dock = DockStyle.Fill; // Expand the UserControl to fill the tab's client space
                                   // MultiProject.BasePlugin.xmlSettings = new SettingsSerializer(this); // Create a new settings serializer and pass it this instance

            pluginScreenSpace.Text = "LogActionChecker";
            pluginStatusText.Text = "ACTLogActionCheckerStart";

            // インターフェイス情報を格納する
            this.xmlSettings = new SettingsSerializer(this);

            // コントロール情報を取得する
            Control[] ct = ACTInitSetting.GetAllControls(this);

            // 取得したコントロール情報を全て回し、初期表示用の情報を格納する
            foreach (Control tempct in ct)
            {
                if (tempct.Name.IndexOf("_init") > 0)
                {
                    // コントロールリストの情報を格納する
                    this.xmlSettings.AddControlSetting(tempct.Name, tempct);
                }
            }

            // 設定ファイルを読み込む
            ACTInitSetting.LoadSettings(xmlSettings);


        }

        /// <summary>
        /// logファイルを読み込む
        /// </summary>
        /// <returns>ディレイを含んだlogのList</returns>
        public List<string> textRead()
        {
            List<string> LogList = new List<string>();
            StreamReader sr = new StreamReader(@imputLog_init.Text, Encoding.GetEncoding("UTF-8"));
            DateTime dateTime2 = DateTime.Now;
            int cnt = 0;
            while (sr.Peek() != -1)
            {
                string str = sr.ReadLine();
                string log = str.Substring(15);
                DateTime dTime = DateTime.Parse(str.Substring(1, 12));
                int sa = 0;
                if (cnt != 0)
                {
                    sa = ((dTime.Second * 1000) + dTime.Millisecond) - ((dateTime2.Second * 1000) + dateTime2.Millisecond);
                    if (0 > sa)
                    {
                        sa = 0;
                    }
                }
                LogList.Add(sa.ToString() + "," + log);
                dateTime2 = dTime;
                cnt++;
            }
            sr.Close();
            return LogList;
        }

        /// <summary>
        /// ファイル選択ダイアログ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.InitialDirectory = @"C:\";
            ofd.Filter = "テキストファイル(*.txt)|*.txt";
            ofd.FilterIndex = 2;
            ofd.Title = "開くファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                imputLog_init.Text = ofd.FileName;
            }
        }

        /// <summary>
        /// log読み込み開始処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try { 
            vs = textRead();
            Task.Run(() => Checklog());
            MessageBox.Show("5秒後に処理が開始します。");
            } 
            catch (Exception ex)
            {
                MessageBox.Show("何らかのエラーで、処理が中断しました。\r\n\r\n" + ex);
            }
        }

        /// <summary>
        /// logをACTに読み込ませる処理
        /// </summary>
        private void Checklog()
        {
            try
            {
                Thread.Sleep(5000);
                foreach (var item in vs)
                {
                    string[] logs = item.Split(',');
                    Thread.Sleep(int.Parse(logs[0]));
                    label1.Text = logs[0] + logs[1];
                    // ACTにlogを送り付ける
                    ActGlobals.oFormActMain.ParseRawLogLine(true, DateTime.Now, logs[1]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("何らかのエラーで、処理が中断しました。\r\n\r\n" + ex);
            }
        }
    }
}
