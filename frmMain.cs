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

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

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
                            axWindowsMediaPlayer1.URL = dataGridView1[0, e.RowIndex + 1].Value.ToString();
                            
                        }
                        catch { }
                    }
                   
                }
                try
                {
                  
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
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
                    axWindowsMediaPlayer1.URL = dataGridView1[0, e.RowIndex].Value.ToString();
                }
                catch { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
