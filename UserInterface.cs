using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fiddler;

namespace FPDDiger
{
    class UserInterface : UserControl
    {
        private TabPage tabPage;
        private CheckBox chkb_Enabled;
        private TextBox textBox_Result;
        private Button btn_Clear;
        private LinkLabel linkLabel;
        public bool bEnabled;
        public delegate void Delegate_AddResult(string strUrl);
        public UserInterface()
        {
            this.bEnabled = false;
            this.InitializeUI();
            FiddlerApplication.UI.tabsViews.TabPages.Add(this.tabPage); 
        }

        public void InitializeUI()
        {
            this.tabPage = new TabPage("爆路径检测");
            this.tabPage.AutoScroll = true;

            this.chkb_Enabled = new CheckBox();
            this.chkb_Enabled.Top = 10;
            this.chkb_Enabled.Left = 20;
            this.chkb_Enabled.Text = "Enable";
            this.chkb_Enabled.Checked = false;

            this.btn_Clear = new Button();
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.Left = 200;
            this.btn_Clear.Top = 10;
            this.Enabled = false;

            this.textBox_Result = new TextBox();
            this.textBox_Result.Top = 50;
            this.textBox_Result.Left = 20;
            this.textBox_Result.Width = 1000;
            this.textBox_Result.Height = 600;
            this.textBox_Result.ReadOnly = true;
            this.textBox_Result.Multiline = true;
            this.textBox_Result.ScrollBars = ScrollBars.Vertical;

            this.linkLabel = new LinkLabel();
            this.linkLabel.Text = "Her0in Team";
            this.linkLabel.Top = this.textBox_Result.Bottom + 20;
            this. linkLabel.Left = 500;
            this.linkLabel.AutoSize = true;
            this.linkLabel.TabStop = true;

            this.tabPage.Controls.Add(this.chkb_Enabled);
            this.tabPage.Controls.Add(this.btn_Clear);
            this.tabPage.Controls.Add(this.linkLabel);
            this.tabPage.Controls.Add(this.textBox_Result);

            this.chkb_Enabled.CheckedChanged += new EventHandler(this.chkb_Enabled_CheckedChanged);
            this.btn_Clear.Click += new EventHandler(this.btn_Clear_Clicked);
            this.linkLabel.Click += new EventHandler(this.linkLabel_Clicked);
        }

        private void linkLabel_Clicked(object obj, EventArgs args)
        {
            System.Diagnostics.Process.Start("http://www.her0in.org/");
        }
        private void btn_Clear_Clicked(object obj, EventArgs args)
        {
            this.textBox_Result.Text = "";
        }
        private void chkb_Enabled_CheckedChanged(object obj, EventArgs args)
        {
            this.SuspendLayout();
            this.bEnabled = this.chkb_Enabled.Checked;
            this.btn_Clear.Enabled = this.bEnabled;
            this.ResumeLayout();
        }
        public void AddResult(string strUrl)
        {
            if (!this.textBox_Result.InvokeRequired)
                this.textBox_Result.AppendText(strUrl + "\r\n");
            else
            {
                Delegate_AddResult delegate_addresult = new Delegate_AddResult(this.AddResult);
                this.textBox_Result.Invoke(delegate_addresult, strUrl);
            }
        }

    }
}
