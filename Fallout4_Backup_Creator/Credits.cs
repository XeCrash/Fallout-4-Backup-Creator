using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fallout4_Backup_Creator
{
    public partial class Credits : Form
    {
        public Credits()
        {
            InitializeComponent();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/user/TheGLAMGASM");
        }

        private void label9_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("XeCrash#1389");
            MessageBox.Show("''XeCrash#1389'' has been copied to your clipboard just paste this into discords friend search bar and add me up!");
        }

        private void label10_Click(object sender, EventArgs e)
        {
            Process.Start("https://twitter.com/XeCrashDev");
        }

        private void label11_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.instagram.com/teh_crash/");
        }
    }
}
