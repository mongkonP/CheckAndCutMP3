using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckAndCutMP3
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        int rowIndex = 0,oldRowIndex = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = folder.SelectedPath;
                    Directory.GetFiles(textBox1.Text, "*.mp3", SearchOption.AllDirectories).ToList<string>()
                    .ForEach(f =>
                    {
                        dataGridView1.Rows.Add(f);
                    });
                    axWindowsMediaPlayer1.URL = dataGridView1[0, 0].Value.ToString();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
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

        private void axWindowsMediaPlayer1_EndOfStream(object sender, AxWMPLib._WMPOCXEvents_EndOfStreamEvent e)
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
