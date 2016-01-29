using ContactManager.Controller;
using ContactManager.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ContactManager.Forms
{
    public partial class Main : Form
    {
        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;
        private readonly ContactController _dc;
        private readonly ILog _log;
        static readonly Timer SaveTimer = new Timer();
        private string _filterText;

        public Main(ILog log, int autoSaveInMinutes)
        {
            _log = log; //program level log file
            _dc = new ContactController(_log, Program.SavePath);
            InitializeComponent();
            ConfigureSysTray();
            ConfigureAutoSave(autoSaveInMinutes);
            ConfigureAutoComplete();
            cmbState.Text = "FL";
            //dataGridView1.AutoGenerateColumns = false;
            LoadData();
            LoadTestData();
        }

        private void LoadTestData()
        {

            txtFirstName.Text = "chris";
            txtLastName.Text = "hudson";
            txtEmail.Text = "chudson121@hotmail.com";
            txtPhone.Text = "8133804342";
            txtStreet.Text = "4122 e 97th ave";
            txtCity.Text = "Tampa"; 
            txtZip.Text = "33617";
            
        }

        private void LoadData(string filter = "")
        {
            //dataGridView1.DataSource = null;
            //dataGridView1.Refresh();
            //dataGridView1.AutoGenerateColumns = true;
            //dataGridView1.Rows.Clear();
            //dataGridView1.Columns.Clear();
            dataGridView1.DataSource = _dc.GetAll(filter);
            //AddColumns();
            //dataGridView1.Refresh();
        }



        private void ConfigureAutoComplete()
        {
            var autoComplete = new AutoCompleteStringCollection();
            //autoComplete.AddRange(_dc.EntryEvents.ToArray());

            //txtEventName.AutoCompleteMode = AutoCompleteMode.Suggest;
            //txtEventName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //txtEventName.AutoCompleteCustomSource = autoComplete;

        }

        private void ConfigureAutoSave(int saveIntervalinMinutes)
        {
            _log.Info(string.Format("AutoSave Interval = {0}", saveIntervalinMinutes));
            SaveTimer.Interval = (saveIntervalinMinutes * 60 * 1000);
            /* Adds the event and the event handler for the method that will process the timer event to the timer. */
            SaveTimer.Tick += TimerEventProcessor;
            SaveTimer.Start();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {

            if (string.IsNullOrEmpty(txtFilter.Text))
                return;

            _log.Info("AutoSaveing Entry");
            //Save current entry
            //SaveEntry(txtFilter.Text, txtEventName.Text);
        }

        private void SaveEntry()
        {
           // _log.Info(string.Format("Saving Entry = {0} EventType = {1}", msg, eventType));
            //if (!string.IsNullOrEmpty(msg))
            //{
                //_dc.AddEntry(msg, eventType);
            //}

            ClearOldText();
        }

        private void ClearOldText()
        {
            txtFilter.Text = "";
            ClearControls(groupBox1);
            //clear all other contact info boxes

            //reload form
            LoadData(_filterText);
        }

        private void ClearControls(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                }
                
                if (c is CheckBox)
                {

                    ((CheckBox)c).Checked = false;
                }

                if (c is RadioButton)
                {
                    ((RadioButton)c).Checked = false;
                }

                if (c.HasChildren)
                {
                    ClearControls(c);
                }
            }
        }

        private void ConfigureSysTray()
        {
            // Create a simple tray menu with only one item.
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("E&xit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            _trayIcon = new NotifyIcon
            {
                Text = Assembly.GetExecutingAssembly().GetName().Name,
                Icon = ContactManager.Properties.Resources.AppInfo,
                ContextMenu = _trayMenu,
                Visible = true
            };

            // Add menu to tray icon and show it.

            _trayIcon.DoubleClick += trayIcon_DoubleClick;
            _trayIcon.Click += trayIcon_Click;
        }

        private void trayIcon_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        

        private void ShowForm()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
            BringToFront();
            Focus();
            txtFilter.Focus();
        }
        
        private void HideForm()
        {
            Hide();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // SaveEntry(txtFilter.Text, txtEventName.Text);
            Application.Exit();
        }

     
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool exitonClose;
            if (!bool.TryParse(ConfigurationManager.AppSettings["ExitOnClose"], out exitonClose)) return;
            if (e.CloseReason != CloseReason.UserClosing) return;
            if (exitonClose) return;

            e.Cancel = true; //I'm sorry Dave, I'm afraid I can't do that.
            HideForm();
        }

        private void clearAllToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //var dialogResult = MessageBox.Show(Resources.MSG_ArchiveFile, Resources.M_ArchiveQuestionTitle, MessageBoxButtons.YesNo);
            //switch (dialogResult)
            //{
            //    case DialogResult.Yes:
            //        _dc.ArchiveFile();
            //        LoadData(_filterText);
            //        break;
            //    case DialogResult.No:
            //        break;
            //}
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveEntry();
            Application.Exit();
        }


   

        private void btnFilter_Click(object sender, EventArgs e)
        {
            _filterText = txtFilter.Text;

            LoadData(_filterText);
        }

        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            
            Contact c = new Contact();
            c.FirstName = txtFirstName.Text;
            c.LastName = txtLastName.Text;
            c.EmailAddress = txtEmail.Text;
            c.Phone = txtPhone.Text;
            c.Address1 = new Address(txtStreet.Text, string.Empty, txtCity.Text, cmbState.Text, txtZip.Text);
            c.MembershipDate = Convert.ToDateTime(dtpMembership.Text);

            _dc.Add(c);
        }

        private void monthlyAdditionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Pull all users for the previous month

        }
    }
}

