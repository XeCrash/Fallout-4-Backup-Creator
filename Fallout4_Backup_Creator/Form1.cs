using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Fallout4_Backup_Creator
{
    public partial class Form1 : Form
    {
        private string Build = "1.0.0.0";
        private object OSBuild = Environment.OSVersion;
        FolderBrowserDialog folder = new FolderBrowserDialog();
        bool isBusy;
        int FileCount = 0;
        bool cancelFlag = false;
        bool GotCanceled = false;
        public Form1()
        {
            base.Closing += new CancelEventHandler(Form1_Closing);
            InitializeComponent();
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.WorkerReportsProgress = true;
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            bool isBusy = backgroundWorker1.IsBusy;
            if (isBusy)
            {
                backgroundWorker1.CancelAsync();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                Text = $"Fallout 4 Backup Creator | Build: {Build} | OS: {OSBuild.ToString()} 64bit";
            }
            else
            {
                Text = $"Fallout 4 Backup Creator | Build: {Build} | OS: {OSBuild.ToString()} 32bit OS";
            }
            tb_SourcePath.Text = GrabPath().ToString();
            tb_InstalledPath.Text = tb_SourcePath.Text;
            try
            {
                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", "Overseer (TrueType)", "").ToString() == "Overseer.otf")
                {
                    Console.WriteLine("Font is already installed");
                }
                else
                {
                    if (File.Exists(Directory.GetCurrentDirectory() + @"\Resources\Overseer.otf"))
                    {
                        RegisterFont("Overseer.otf");
                        Console.WriteLine("Font Installed");
                    }
                    else
                    {

                    }
                }
            }
            catch(Exception error)
            {
                
            }
        }

        private object GrabPath()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                    object DefaultInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Bethesda Softworks\Fallout4", "Installed Path", "Unable to retrieve file path to Fallout 4. Please select the path where Fallout 4 is installed.");
                    return DefaultInstallPath.ToString();
                }
                catch
                {
                    MessageBox.Show("it seems that Fallout 4 isnt installed.", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "";
                }
            }
            else
            {
                try
                {
                    object DefaultInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bethesda Softworks\Fallout4", "Installed Path", "Unable to retrieve file path to Fallout 4. Please select the path where Fallout 4 is installed.");
                    return DefaultInstallPath.ToString();
                }
                catch
                {
                    MessageBox.Show("it seems that Fallout 4 isnt installed.", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folder.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(folder.SelectedPath + @"\Fallout4.exe") | File.Exists(folder.SelectedPath + @"\Fallout4Launcher.exe"))
                {
                    tb_SourcePath.Clear();
                    tb_SourcePath.Text = folder.SelectedPath;
                }
                else
                {
                    MessageBox.Show("Selected path doesn't seem to contain Fallout 4.\n(Tip: The path that contains Fallout 4 has Fallout4.exe and Fallout4Launcher.exe inside of it.)", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folder.ShowDialog() == DialogResult.OK)
            {
                tb_DestinationPath.Clear();
                tb_DestinationPath.Text = folder.SelectedPath + "\\";
            }
        }

        #region Installing Font
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        public static extern int AddFontResourceA(string lpFileName);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int AddFontResource(string lpszFilename);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int CreateScalableFontResource(uint fdwHidden, string
        lpszFontRes, string lpszFontFile, string lpszCurrentPath);

        /// <summary>
        /// Installs font on the user's system and adds it to the registry so it's available on the next session
        /// Your font must be included in your project with its build path set to 'Content' and its Copy property
        /// set to 'Copy Always'
        /// </summary>
        /// <param name="contentFontName">Your font to be passed as a resource (i.e. "myfont.tff")</param>
        private static void RegisterFont(string contentFontName)
        {
            // Creates the full path where your font will be installed
            var fontDestination = Path.Combine(System.Environment.GetFolderPath
                                              (System.Environment.SpecialFolder.Fonts), contentFontName);

            if (!File.Exists(fontDestination))
            {
                // Copies font to destination
                System.IO.File.Copy(Path.Combine(System.IO.Directory.GetCurrentDirectory() + "\\Resources\\", contentFontName), fontDestination);

                // Retrieves font name
                // Makes sure you reference System.Drawing
                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(fontDestination);
                var actualFontName = fontCol.Families[0].Name;

                //Add font
                AddFontResource(fontDestination);
                //Add registry entry  
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",
                actualFontName, contentFontName, RegistryValueKind.String);
            }
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                if (tb_SourcePath.Text != string.Empty || tb_DestinationPath.Text != string.Empty)
                {
                    if (tb_DestinationPath.Text != string.Empty)
                    {
                        backgroundWorker1.RunWorkerAsync();
                        button3.Text = "Cancel Backup Process";
                        isBusy = true;
                        cancelFlag = false;
                    }
                }
                else
                {
                    if (tb_SourcePath.Text == string.Empty && tb_DestinationPath.Text == string.Empty)
                    {
                        MessageBox.Show("The source & destination path can not be empty.", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if(tb_SourcePath.Text == string.Empty)
                    {
                        MessageBox.Show("The source path can not be empty.", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if(tb_DestinationPath.Text == string.Empty)
                    {
                        MessageBox.Show("The destination path can not be empty.", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                backgroundWorker1.CancelAsync();
                button3.Text = "One moment while we clean things up for cancelation...";
                isBusy = false;
                cancelFlag = true;
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo source = new DirectoryInfo(tb_SourcePath.Text);
            DirectoryInfo destination = new DirectoryInfo(tb_DestinationPath.Text);
            if (!backgroundWorker1.CancellationPending)
            {
                CopyAll(source, destination);
            }
            else
            {
               
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (GotCanceled == false)
            {
                button3.Text = "Begin Backup Process";
                isBusy = false;
                MessageBox.Show("Backup has been completed!");
                FileCount = 0;
                progressBar1.Value = 0;
                label3.Text = "00.00 %";
                label4.Text = " All Files have been successfully copied!";
                label5.Text = "Currently Copying: Process Finished";
                GotCanceled = false;
            }
            else
            {
                MessageBox.Show("The backup process has successfully been canceled!", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FileCount = 0;
                button3.Text = "Begin Backup Process";
                label3.Text = "00.00 %";
                label4.Text = "Copying File: 0 out of 0";
                label5.Text = "Currently Copying: Process Canceled";
                progressBar1.Value = 0;
                var AllFiles = GetAllFiles(tb_DestinationPath.Text);
                foreach (var fi in AllFiles)
                {
                    var attr = File.GetAttributes(fi);
                    attr = attr & ~FileAttributes.ReadOnly;
                    File.SetAttributes(fi, attr);
                }
                Directory.Delete(tb_DestinationPath.Text, true);
                tb_DestinationPath.Text = "";
                
            }
        }

        public static List<String> GetAllFiles(String directory)
        {
            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).ToList();
        }

        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            DirectoryInfo dInfo = new DirectoryInfo(target.FullName);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
            int fCount = Directory.GetFiles(tb_SourcePath.Text, "*", SearchOption.AllDirectories).Length;
            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                if (!cancelFlag)
                {
                    FileCount++;
                    double Progress = ((double)FileCount / (double)fCount) * 100;
                    var IntValue = Math.Round(Progress);
                    var AllFiles = GetAllFiles(tb_DestinationPath.Text);
                    foreach (var Files in AllFiles)
                    {
                        var attr = File.GetAttributes(Files);
                        attr = attr & ~FileAttributes.ReadOnly;
                        File.SetAttributes(Files, attr);
                    }
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                    label3.Invoke((MethodInvoker)delegate { label3.Text = String.Format("{0:P2}", ((double)FileCount / (double)fCount)); });
                    label4.Invoke((MethodInvoker)delegate { label4.Text = String.Format(@"Copying File: {0} out of {1}", FileCount, fCount); });
                    label5.Invoke((MethodInvoker)delegate { label5.Text = String.Format(@"Now Copying: {0} ({1})", fi.Name, FormatBytes(fi.Length)); });
                    backgroundWorker1.ReportProgress((int)IntValue);
                }
                else
                {
                    cancelFlag = true;
                    GotCanceled = true;
                    break;
                }
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                if (!cancelFlag)
                {
                    DirectoryInfo nextTargetSubDir =
                        target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir);
                }
                else
                {
                    cancelFlag = true;
                    GotCanceled = true;
                    break;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Once you complete the backup phase you will need to change the registry path from the main game's path to the backup games path to be able to launch from the backgames folder and load its files. If we don't do this even if we launch from the backup games folder the files from the main game will still be the ones that get loaded.", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (folder.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(folder.SelectedPath + @"\Fallout4.exe") | File.Exists(folder.SelectedPath + @"\Fallout4Launcher.exe"))
                {
                    tb_SourcePath.Clear();
                    tb_SourcePath.Text = folder.SelectedPath;
                }
                else
                {
                    MessageBox.Show("Selected path doesn't seem to contain Fallout 4.\n(Tip: The path that contains Fallout 4 has Fallout4.exe and Fallout4Launcher.exe inside of it.)", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                if (tb_InstalledPath.Text != String.Empty)
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Bethesda Softworks\Fallout4", "Installed Path", tb_InstalledPath.Text);
                    MessageBox.Show("The Installed path has been successfully changed in the Registry!", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tb_SourcePath.Text = GrabPath().ToString();
                    tb_InstalledPath.Text = GrabPath().ToString();
                }
                else
                {
                    MessageBox.Show("The install path can not be empty.", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (tb_InstalledPath.Text != String.Empty)
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bethesda Softworks\Fallout4", "Installed Path", tb_InstalledPath.Text);
                    MessageBox.Show("The Installed path has been successfully changed in the Registry!", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tb_SourcePath.Text = GrabPath().ToString();
                    tb_InstalledPath.Text = GrabPath().ToString();
                }
                else
                {
                    MessageBox.Show("The install path can not be empty.", "Fallout 4 Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            new Credits().Show();
        }
    }
}
