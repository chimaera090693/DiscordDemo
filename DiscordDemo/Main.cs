using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSharpPlus;

namespace DiscordDemo
{
    public partial class Main : Form
    {
        private DiscordThread parentRef;
        private int PingRowCounts = 0;
        public Main(DiscordThread thread)
        {
            InitializeComponent();
            PingRowCounts = 0;
            parentRef = thread;

            //parentRef.StartMain(new DiscordConfig)
        }

        private void LogReceived(object sender, DebugLogMessageEventArgs e)
        {
            AddStringToPingConsole(e.Timestamp.ToString("dd/MM/yyyy HH:mm:ss") + " : " + e.Message, true);

        }



        #region Form Event

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }


        private void btnInitial_Click(object sender, EventArgs e)
        {
            if (parentRef != null)
            {
                if (string.IsNullOrEmpty(tbToken.Text)) return;
                DiscordConfig cfg = new DiscordConfig();
                cfg = new DiscordConfig
                {
                    Token = tbToken.Text,
                    TokenType = TokenType.Bearer,
                    AutoReconnect = true,
                    LogLevel = LogLevel.Debug,
                    UseInternalLogHandler = true
                };
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                parentRef.InitialClient(cfg, LogReceived);
                parentRef.StartMain();

            }
        }

        private void btnDispose_Click(object sender, EventArgs e)
        {
            parentRef.Client.DisconnectAsync();
            btnDispose.Enabled = false;
            btnStart.Enabled = true;
            parentRef.Client.Dispose();
        }

        #endregion


        #region Console manager

        private void AddStringToPingConsole(string s, bool addLine = false)
        {
            var line = addLine ? "\r\n" : "";
            if (PingRowCounts > 200)
            {
                PingRowCounts = 0;
                tbConsole.Text = line;
                return;
            }
            tbConsole.Text += s + line;
            tbConsole.Select(tbConsole.Text.Length, 0);
            tbConsole.ScrollToCaret();
            PingRowCounts++;
        }

        #endregion

    }
}
