using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TORServices.Ext;

namespace CheckAndCutMP3
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(axWindowsMediaPlayer1_PlayStateChange);
        }
        int rowIndex = 0,oldRowIndex = 0;

        //https://markheath.net/post/how-to-get-media-file-duration-in-c
        private string GetAudioDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                TimeSpan ts = TimeSpan.FromTicks((long)t);
                string s = "";
                if (ts.TotalHours > 1)
                {
                    s = ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds;
                }
                else
                {
                    if (ts.TotalMinutes > 1)
                    {
                        s = ts.Minutes + ":" + ts.Seconds;
                    }
                    else
                    {

                        s =  "" + ts.Seconds;
                    }
                }
                return s;
            }
        }
     
        //https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        private String BytesToString(string filePath)
        {
            string[] suf = { " B", " KB", " MB", " GB", " TB", " PB", " EB" }; //Longs run out around EB
            long byteCount = new FileInfo(filePath).Length;
                
                if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Invoke(new Action(() => dataGridView1.Rows.Clear()));
                    textBox1.Invoke(new Action(() => textBox1.Text = folder.SelectedPath));
                    lblStatus.Invoke(new Action(() => lblStatus.Text = "Adding file"));
                    TORServices.Forms.Forms.frmWaitFormDialog frm = new TORServices.Forms.Forms.frmWaitFormDialog(() =>
                     {

                         //https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler?view=net-5.0
                         // Create a scheduler that uses two threads.
                         LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(30);
                        List<Task> tasks = new List<Task>();

                        // Create a TaskFactory and pass it our custom scheduler.
                        TaskFactory factory = new TaskFactory(lcts);
                        CancellationTokenSource cts = new CancellationTokenSource();

                        Directory.GetFiles(textBox1.Text, "*.mp3", SearchOption.AllDirectories).ToList<string>()
                         .ForEach(f =>
                         {
                             Task t = factory.StartNew(() => {
                              
                                 dataGridView1.Invoke(new Action(() => dataGridView1.Rows.Add(f, BytesToString(f), "")));//GetAudioDuration(f)
                                 lblStatus.Invoke(new  Action(()=>lblStatus.Text = "Add:" + f))  ;
                             }, cts.Token);
                             tasks.Add(t);
                         });
                        // Wait for the tasks to complete before displaying a completion message.
                        Task.WaitAll(tasks.ToArray());
                        cts.Dispose();
                          lblStatus.Invoke(new Action(() => lblStatus.Text = "Add file complete"));
                        axWindowsMediaPlayer1.URL = dataGridView1[0, 0].Value.ToString();



                            }) ;
                           frm.ShowDialog();
                    
                }
            }
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                string f = dataGridView1[0, e.RowIndex].Value.ToString();
                if (axWindowsMediaPlayer1.URL == f)
                {
                    if (e.RowIndex < dataGridView1.RowCount)
                    {
                        try
                        {
                            oldRowIndex = rowIndex;
                            rowIndex++;
                            RunFile();

                        }
                        catch { }
                    }
                   
                }
                try
                {
                  
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[1].Value = "Deleted";
                    File.Delete(f);
                }
                catch { }
            }
            
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex !=1)
            {
                try
                {
                    oldRowIndex = rowIndex;
                    rowIndex= e.RowIndex;
                    RunFile();
                }
                catch { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (rowIndex < dataGridView1.RowCount - 1)
            {

                if (e.newState == 1)
                {
                    this.BeginInvoke(new Action(() => {
                        oldRowIndex = rowIndex;
                        rowIndex++;
                        RunFile();
                    }));
                }
            }
        }
        void RunFile()
        {
            if (dataGridView1[0, oldRowIndex].Value.ToString() != "Deleted")
            {
                dataGridView1.Rows[oldRowIndex].DefaultCellStyle.BackColor = Color.Wheat;
                dataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Fuchsia;
            }
            axWindowsMediaPlayer1.URL = dataGridView1[0, rowIndex].Value.ToString();

            
        }
    }
}
