#region Using Statements

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

#endregion Using Statements

namespace TUNExporter
{
    public partial class frmMain : Form
    {

        #region Variables

        private List<string> lstszFolders = new List<string>();
        private List<string> lstszFiles = new List<string>();
        private List<string> lstszAllReg = new List<string>();

        private string szOutputFolder = @"C:\Temp";
        private string szAllFilesFoldersName = "AllFilesFolders.txt";
        private string szAllRegistryValues = "AllRegistry.txt";
        private string szLogFile = "LogFile.txt";
        private string szFilePath = string.Empty;
        private string szLastTUNTXTFileFolder = string.Empty;

        private string szEX = "No Error";

        //private int intLineCount = 0;
        private int intPrg1 = 0;
        private int intErrorCount = 0;

        private bool blExitPressed = false;
        private bool blCancelPressed = false;
        //private bool blCopyingFileorCreatingFolder = false;
        private bool blExportStarted = false;

        private ProcessStartInfo procInfo;
        private int intRegFileCounter = 0;

        private System.Drawing.Color colorOriginalPicBackColor = System.Drawing.Color.Black;

        #endregion Variables 

        #region Functions

        #region Function - Constructor

        public frmMain()
        {
            InitializeComponent();
            colorOriginalPicBackColor = picBoxPRGMain.BackColor;
        }

        #endregion Function - Constructor

        #region Function - User Functions

        /// <summary>
        /// Using %Windir%\System32\reg.exe export a registry hive to a .reg file
        /// </summary>
        /// <param name="hivePath">Registry hive key to export in form: HKLM\Software\AMD\Config will export all values in the Config Key</param>
        /// <param name="dstDir">Directory in which to create .reg file</param>
        public void fnExportRegistryHive(string hivePath, string dstDir)
        {
            try
            {
                if (!Directory.Exists(dstDir))
                    Directory.CreateDirectory(dstDir);

                lblCurrent.Text = "Exporting: " + hivePath;
                Application.DoEvents();

                string fileName = fnValidateDirOrFileNameUsingRegEx(hivePath);
                string dstFile = Path.Combine(dstDir, fileName + "_" + intRegFileCounter.ToString() + ".reg");

                /* using switch /y (overwrite) with reg.exe was proving problematic so all .reg files are assigned an integer suffix that increments with every file so as long as there are less than 2^32 (SizeOf(int)) files all files will have a unique name*/
                intRegFileCounter++;
                procInfo = new ProcessStartInfo()
                {
                    FileName = "REG",
                    Arguments = $"EXPORT \"{hivePath}\" \"{dstFile}\" ",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(procInfo).WaitForExit();

                // Error checking and write result to log
                if (File.Exists(dstFile) == true)
                {
                    using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                    {
                        sw1.WriteLine("Registry data exported - " + hivePath);
                    }
                }
                else
                {
                    using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                    {
                        sw1.WriteLine("Registry data export failed - " + hivePath);
                    }
                }
            }
            catch (Exception ex)
            {
                szEX = ex.Message;
                intErrorCount++;
                try
                {
                    // Error checking and write result to log
                    using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                    {
                        if (ex.Message.Contains(hivePath) == true)
                        {
                            sw1.WriteLine("Error: " + ex.Message);
                        }
                        else
                        {
                            sw1.WriteLine("Error: " + ex.Message + " - Registry: " + hivePath);
                        }

                    }
                }
                catch
                {
                    // NO MessageBox.Show(); at error cos that's a pain in the arse
                }
            }

            // if Exit button pressed 
            if (blExitPressed == true)
            {
                procInfo = null;
            }
        }

        /// <summary>
        /// Replace characters that are not allowed on windows with a hyphen:
        /// < > : " / \ | ? 
        /// </summary>
        public string fnValidateDirOrFileNameUsingRegEx(string name)
        {
            return Regex.Replace(name, "<|>|:|\"|/|\\\\|\\||\\?|\\*", "-");
        }

        #endregion Function - User Functions

        #region Function - Event Functions

        private void btnInputFile_Click(object sender, EventArgs e)
        {
            prgMain.Value = 0;
            prgMain.Minimum = 0;

            // TUN Export files are .txt files and log files are .log files
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt|Log Files (*.log)|*.log|All Files (*.*)|*.*";
            ofd.Title = "Select TUN Export or Log File";
            ofd.InitialDirectory = @"D:\Temp\_VM\Latest";
            ofd.AddExtension = true;
            ofd.CheckFileExists = true;
            ofd.FilterIndex = 0; // Stat with .txt files
            ofd.Multiselect = false;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                szFilePath = ofd.FileName;
                txtInputFile.Text = ofd.FileName;
            }

        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            // TUN Export requires admin rights but ensure that you have write permissions to the output directory or TUNExporter will get upset
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select output folder for all files and registry entries";
            fbd.ShowNewFolderButton = true;
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                szOutputFolder = fbd.SelectedPath;
                txtOutput.Text = szOutputFolder;
            }

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Prevent pressing buttons again
            blExportStarted = true;
            btnExport.Enabled = false;
            btnInputFile.Enabled = false;
            btnOutput.Enabled = false;
            blCancelPressed = false;
            Application.DoEvents();

            // validate Input file and output folder
            if (string.IsNullOrEmpty(txtInputFile.Text) == true)
            {
                MessageBox.Show("Please select a TUN input file?", "Input File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnExport.Enabled = true;
                btnInputFile.Enabled = true;
                btnOutput.Enabled = true;
                blExportStarted = false;

                return;
            }
            if (File.Exists(txtInputFile.Text) == false)
            {
                MessageBox.Show("The TUN input file does not appear to exist, please ensure that the path is correct", "Input File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnExport.Enabled = true;
                btnInputFile.Enabled = true;
                blExportStarted = false;
                btnOutput.Enabled = true;

                return;
            }
            if (string.IsNullOrEmpty(txtOutput.Text) == true)
            {
                MessageBox.Show("Please select a output directory", "Output Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnExport.Enabled = true;
                btnInputFile.Enabled = true;
                btnOutput.Enabled = true;
                blExportStarted = false;

                return;
            }
            if (Directory.Exists(txtOutput.Text) == false)
            {
                MessageBox.Show("The TUN output directory does not appear to exist, please ensure that the path is correct", "Input File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnExport.Enabled = true;
                btnInputFile.Enabled = true;
                btnOutput.Enabled = true;
                blExportStarted = false;

                return;
            }

            // Initialize vars
            szFilePath = txtInputFile.Text;
            szOutputFolder = txtOutput.Text;

            string szNextLine = string.Empty;
            string szFiletoCopy = string.Empty;
            string szFiletoCopyOldPath = string.Empty;

            if (File.Exists(szFilePath) == true)
            {

                #region Folders & Files

                // Scan input file for folders
                using (StreamReader sr = new StreamReader(szFilePath))
                {
                    bool blIsNullOrWhiteSpace = true;
                    string szLine1 = string.Empty;
                    while (blIsNullOrWhiteSpace == true) // Ignore blank lines
                    {
                        szLine1 = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(szLine1) == false) { blIsNullOrWhiteSpace = false; }
                    }
                    string szLine2 = sr.ReadLine();

                    // TUN Log File
                    if (szLine1.Contains("TUN log file") == true || szLine2.Contains("TUN log file") == true)
                    {

                        picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;
                        lblCurrent.Text = "Examining: " + szFilePath;
                        Application.DoEvents();

                        while (sr.EndOfStream == false)
                        {
                            // cancel or Exit
                            if (blExitPressed == true) { return; }
                            if (blCancelPressed == true) { return; }

                            try
                            {
                                szNextLine = sr.ReadLine();
                                if (szNextLine.Contains("DELETING FOLDER: "))
                                {
                                    // Found folder entry in log file
                                    lblStatusRight.Text = "Looking for FOLDER data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETING FOLDER: ", "");
                                    szNextLine = szNextLine.Replace(" ... ERROR (not empty)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have folder path only

                                    // Check for duplicate entries
                                    if (lstszFolders.Contains(szNextLine) == false)
                                    {
                                        lstszFolders.Add(szNextLine);
                                    }
                                }
                                else if (szNextLine.Contains("DELETING FILE: "))
                                {
                                    // Found file entry in log file
                                    lblStatusRight.Text = "Looking for FILE data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETING FILE: ", "");
                                    szNextLine = szNextLine.Replace(" ... ERROR (in use)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have file path only

                                    // Check for duplicate entries
                                    if (lstszFiles.Contains(szNextLine) == false)
                                    {
                                        lstszFiles.Add(szNextLine);
                                    }
                                }
                                else if (szNextLine.Contains("UPDATING SHARED FILE COUNTER: "))
                                {
                                    // Found file entry in log file
                                    lblStatusRight.Text = "Looking for FILE data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("UPDATING SHARED FILE COUNTER: ", "");
                                    szNextLine = szNextLine.Replace(" ... Ok (Skipped)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have file path only

                                    // Check for duplicate entries
                                    if (lstszFiles.Contains(szNextLine) == false)
                                    {
                                        lstszFiles.Add(szNextLine);
                                    }
                                }
                                else if (szNextLine.Contains("DELETE SHARED FILE: "))
                                {
                                    // Found file entry in log file
                                    lblStatusRight.Text = "Looking for FILE data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETE SHARED FILE: ", "");
                                    szNextLine = szNextLine.Replace(" ... Ok (Skipped)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have file path only

                                    // Check for duplicate entries
                                    if (lstszFiles.Contains(szNextLine) == false)
                                    {
                                        lstszFiles.Add(szNextLine);
                                    }
                                    //intLineCount++;
                                }
                                else if (szNextLine.Contains("DELETING REGISTRY KEY: "))
                                {
                                    // Found registry entry in log file
                                    lblStatusRight.Text = "Looking for REGISTRY KEY data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETING REGISTRY KEY: ", "");
                                    szNextLine = szNextLine.Replace(" ... FAILS (not found)", "");
                                    szNextLine = szNextLine.Replace(" ... ERROR(has sub - keys)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have hive path only

                                    // Check for duplicate entries
                                    if (lstszAllReg.Contains(szNextLine) == false)
                                    {
                                        lstszAllReg.Add(szNextLine);
                                    }
                                    //intLineCount++;
                                }
                                else if (szNextLine.Contains("DELETING REGISTRY VALUE: "))
                                {
                                    // Found registry entry in log file
                                    lblStatusRight.Text = "Looking for REGISTRY VALUE data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETING REGISTRY VALUE: ", "");
                                    szNextLine = szNextLine.Replace(" ... FAILS (not found)", "");
                                    szNextLine = szNextLine.Replace(" ... ERROR(has sub - keys)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have hive path only

                                    // Store last folder for add to file name
                                    string szTMP01 = szNextLine.Substring(0, szNextLine.LastIndexOf("\\"));

                                    // Check for duplicate entries
                                    if (lstszAllReg.Contains(szTMP01) == false)
                                    {
                                        lstszAllReg.Add(szTMP01);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                                picBoxPRGMain.BackColor = System.Drawing.Color.DarkRed;
                                Application.DoEvents();
                                szEX = ex.Message;
                            }

                        }

                        // remove any empty entries
                        for (int x = 0; x < lstszFolders.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszFolders[x]) == true || string.IsNullOrWhiteSpace(lstszFolders[x]) == true)
                            {
                                lstszFolders.RemoveAt(x);
                            }
                        }
                        for (int x = 0; x < lstszFiles.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszFiles[x]) == true || string.IsNullOrWhiteSpace(lstszFiles[x]) == true)
                            {
                                lstszFiles.RemoveAt(x);
                            }
                        }
                        for (int x = 0; x < lstszAllReg.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszAllReg[x]) == true || string.IsNullOrWhiteSpace(lstszAllReg[x]) == true)
                            {
                                lstszAllReg.RemoveAt(x);
                            }
                        }

                        // Update status info
                        prgMain.Maximum = (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count);
                        lblProg1.Text = "Progress: 0% - 0 of " + (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count).ToString();
                        lblFolders.Text = lstszFolders.Count.ToString() + " Folders Found";
                        lblFiles.Text = lstszFiles.Count.ToString() + " Files Found";
                        lblRegistry.Text = lstszAllReg.Count.ToString() + " Registry Entries Found";
                        lblCurrent.Text = "Examination complete: " + szFilePath;
                        Application.DoEvents();

                    } // TUN TXT File
                    else if (szLine1.Contains("Total Uninstall") == true || szLine2.Contains("Total Uninstall") == true)
                    {
                        picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;
                        lblCurrent.Text = "Examining: " + szFilePath;
                        Application.DoEvents();

                        while (sr.EndOfStream == false)
                        {
                            // cancel or Exit
                            if (blExitPressed == true) { return; }
                            if (blCancelPressed == true) { return; }
                            picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;

                            try
                            {
                                szNextLine = sr.ReadLine();
                                if (szNextLine.Contains("(FOLDER) ") == true)
                                {
                                    // Found folder entry in text file
                                    lblStatusRight.Text = "Looking for FOLDER data in TUN text file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    int intStart = szNextLine.IndexOf("(FOLDER) ") + ("(FOLDER) ").Length;
                                    int intLength = szNextLine.Length - intStart;
                                    szNextLine = szNextLine.Substring(intStart, intLength);
                                    szNextLine = szNextLine.Trim();
                                    // Now have folder path only

                                    // Store last folder for add to file name
                                    szLastTUNTXTFileFolder = szNextLine;

                                    // Check for duplicate entries
                                    if (lstszFolders.Contains(szNextLine) == false)
                                    {
                                        lstszFolders.Add(szNextLine);
                                    }
                                }
                                else if (szNextLine.Contains("(FILE)") == true)
                                {
                                    lblStatusRight.Text = "Looking for FILE data in TUN text file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    int intStart = szNextLine.IndexOf("(FILE) ") + ("(FILE) ").Length;
                                    int intLength = szNextLine.Length - intStart;
                                    szNextLine = szNextLine.Substring(intStart, intLength);

                                    szNextLine = szNextLine.Trim();
                                    szNextLine = szNextLine.Substring(0, szNextLine.IndexOf(" = "));
                                    szNextLine = szNextLine.Trim();
                                    // Now have file path only

                                    // Store last folder for add to file name
                                    szNextLine = szLastTUNTXTFileFolder + "\\" + szNextLine;

                                    // Check for duplicate entries
                                    if (lstszFiles.Contains(szNextLine) == false)
                                    {
                                        lstszFiles.Add(szNextLine);
                                    }
                                    //intLineCount++;
                                }
                                else if (szNextLine.Contains("(REG KEY) "))
                                {
                                    lblStatusRight.Text = "Looking for REGISTRY KEY data in TUN text file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    int intStart = szNextLine.IndexOf("(REG KEY) ") + ("(REG KEY) ").Length;
                                    int intLength = szNextLine.Length - intStart;
                                    szNextLine = szNextLine.Substring(intStart, intLength);
                                    szNextLine = szNextLine.Trim();
                                    // Now have hive path only

                                    // Check for duplicate entries
                                    if (lstszAllReg.Contains(szNextLine) == false)
                                    {
                                        lstszAllReg.Add(szNextLine);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                picBoxPRGMain.BackColor = System.Drawing.Color.DarkRed;
                                szEX = ex.Message;
                            }

                        }

                        // remove any empty entries
                        for (int x = 0; x < lstszFolders.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszFolders[x]) == true || string.IsNullOrWhiteSpace(lstszFolders[x]) == true)
                            {
                                lstszFolders.RemoveAt(x);
                            }
                        }
                        for (int x = 0; x < lstszFiles.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszFiles[x]) == true || string.IsNullOrWhiteSpace(lstszFiles[x]) == true)
                            {
                                lstszFiles.RemoveAt(x);
                            }
                        }
                        for (int x = 0; x < lstszAllReg.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszAllReg[x]) == true || string.IsNullOrWhiteSpace(lstszAllReg[x]) == true)
                            {
                                lstszAllReg.RemoveAt(x);
                            }
                        }

                        // update status info
                        prgMain.Maximum = (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count);
                        lblProg1.Text = "Progress: 0% - 0 of " + (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count).ToString();
                        lblFolders.Text = lstszFolders.Count.ToString() + " Folders Found";
                        lblFiles.Text = lstszFiles.Count.ToString() + " Files Found";
                        lblRegistry.Text = lstszAllReg.Count.ToString() + " Registry Entries Found";
                        lblCurrent.Text = "Examination complete: " + szFilePath;
                        Application.DoEvents();
                    }
                    else
                    {
                        if (MessageBox.Show("This file does not appear to be a valid TUN file, would you like to attempt the export process anyway?", "Input Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                        {
                            btnExport.Enabled = true;
                            btnInputFile.Enabled = true;
                            btnOutput.Enabled = true;
                            blExportStarted = false;
                            return;
                        }
                        blCancelPressed = false;
                        blExitPressed = false;

                        // Attempt read as if TUN Log File
                        picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;
                        lblCurrent.Text = "Examining: " + szFilePath;
                        Application.DoEvents();

                        while (sr.EndOfStream == false)
                        {
                            // cancel or Exit
                            if (blExitPressed == true) { return; }
                            if (blCancelPressed == true) { return; }

                            try
                            {
                                szNextLine = sr.ReadLine();
                                if (szNextLine.Contains("DELETING FOLDER: "))
                                {
                                    // Found folder entry inlogfile
                                    lblStatusRight.Text = "Looking for FOLDER data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETING FOLDER: ", "");
                                    szNextLine = szNextLine.Replace(" ... ERROR (not empty)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have folder path only

                                    // Check for duplicate entries
                                    if (lstszFolders.Contains(szNextLine) == false)
                                    {
                                        lstszFolders.Add(szNextLine);
                                    }
                                }
                                else if (szNextLine.Contains("DELETING FILE: "))
                                {
                                    lblStatusRight.Text = "Looking for FILE data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETING FILE: ", "");
                                    szNextLine = szNextLine.Replace(" ... ERROR (in use)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have file path only

                                    // Check for duplicate entries
                                    if (lstszFiles.Contains(szNextLine) == false)
                                    {
                                        lstszFiles.Add(szNextLine);
                                    }
                                }
                                else if (szNextLine.Contains("UPDATING SHARED FILE COUNTER: "))
                                {
                                    lblStatusRight.Text = "Looking for FILE data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("UPDATING SHARED FILE COUNTER: ", "");
                                    szNextLine = szNextLine.Replace(" ... Ok (Skipped)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have file path only

                                    // Check for duplicate entries
                                    if (lstszFiles.Contains(szNextLine) == false)
                                    {
                                        lstszFiles.Add(szNextLine);
                                    }
                                }
                                else if (szNextLine.Contains("DELETE SHARED FILE: "))
                                {
                                    lblStatusRight.Text = "Looking for FILE data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETE SHARED FILE: ", "");
                                    szNextLine = szNextLine.Replace(" ... Ok (Skipped)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have file path only

                                    // Check for duplicate entries
                                    if (lstszFiles.Contains(szNextLine) == false)
                                    {
                                        lstszFiles.Add(szNextLine);
                                    }
                                    //intLineCount++;
                                }
                                else if (szNextLine.Contains("DELETING REGISTRY KEY: "))
                                {
                                    lblStatusRight.Text = "Looking for REGISTRY KEY data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETING REGISTRY KEY: ", "");
                                    szNextLine = szNextLine.Replace(" ... FAILS (not found)", "");
                                    szNextLine = szNextLine.Replace(" ... ERROR(has sub - keys)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have hive path only

                                    // Check for duplicate entries
                                    if (lstszAllReg.Contains(szNextLine) == false)
                                    {
                                        lstszAllReg.Add(szNextLine);
                                    }
                                }
                                else if (szNextLine.Contains("DELETING REGISTRY VALUE: "))
                                {
                                    lblStatusRight.Text = "Looking for REGISTRY VALUE data in TUN log file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    szNextLine = szNextLine.Replace("DELETING REGISTRY VALUE: ", "");
                                    szNextLine = szNextLine.Replace(" ... FAILS (not found)", "");
                                    szNextLine = szNextLine.Replace(" ... ERROR(has sub - keys)", "");
                                    szNextLine = szNextLine.Replace(" ... Ok", "");
                                    szNextLine = szNextLine.Trim();
                                    // Now have hive path only

                                    // Store last folder for add to file name
                                    string szTMP01 = szNextLine.Substring(0, szNextLine.LastIndexOf("\\"));

                                    // Check for duplicate entries
                                    if (lstszAllReg.Contains(szTMP01) == false)
                                    {
                                        lstszAllReg.Add(szTMP01);
                                    }

                                } // Attempt read as if TUN text File
                                else if (szNextLine.Contains("(FOLDER) ") == true)
                                {
                                    lblStatusRight.Text = "Looking for FOLDER data in TUN text file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    int intStart = szNextLine.IndexOf("(FOLDER) ") + ("(FOLDER) ").Length;
                                    int intLength = szNextLine.Length - intStart;
                                    szNextLine = szNextLine.Substring(intStart, intLength);
                                    szNextLine = szNextLine.Trim();
                                    // Now have folder path only

                                    // Store last folder for add to file name
                                    szLastTUNTXTFileFolder = szNextLine;

                                    // Check for duplicate entries
                                    if (lstszFolders.Contains(szNextLine) == false)
                                    {
                                        lstszFolders.Add(szNextLine);
                                    }

                                }
                                else if (szNextLine.Contains("(FILE)") == true)
                                {
                                    lblStatusRight.Text = "Looking for FILE data in TUN text file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    int intStart = szNextLine.IndexOf("(FILE) ") + ("(FILE) ").Length;
                                    int intLength = szNextLine.Length - intStart;
                                    szNextLine = szNextLine.Substring(intStart, intLength);

                                    szNextLine = szNextLine.Trim();
                                    szNextLine = szNextLine.Substring(0, szNextLine.IndexOf(" = "));
                                    szNextLine = szNextLine.Trim();
                                    // Now have file path only

                                    // Store last folder for add to file name
                                    szNextLine = szLastTUNTXTFileFolder + "\\" + szNextLine;

                                    // Check for duplicate entries
                                    if (lstszFiles.Contains(szNextLine) == false)
                                    {
                                        lstszFiles.Add(szNextLine);
                                    }

                                }
                                else if (szNextLine.Contains("(REG KEY) "))
                                {
                                    lblStatusRight.Text = "Looking for REGISTRY KEY data in TUN text file...";
                                    Application.DoEvents();

                                    // Remove unwanted text in line
                                    int intStart = szNextLine.IndexOf("(REG KEY) ") + ("(REG KEY) ").Length;
                                    int intLength = szNextLine.Length - intStart;
                                    szNextLine = szNextLine.Substring(intStart, intLength);
                                    szNextLine = szNextLine.Trim();
                                    // Now have hive path only

                                    // Check for duplicate entries
                                    if (lstszAllReg.Contains(szNextLine) == false)
                                    {
                                        lstszAllReg.Add(szNextLine);
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                picBoxPRGMain.BackColor = System.Drawing.Color.DarkRed;
                                Application.DoEvents();
                                szEX = ex.Message;
                            }

                        }

                        // remove any empty entries
                        for (int x = 0; x < lstszFolders.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszFolders[x]) == true || string.IsNullOrWhiteSpace(lstszFolders[x]) == true)
                            {
                                lstszFolders.RemoveAt(x);
                            }
                        }
                        for (int x = 0; x < lstszFiles.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszFiles[x]) == true || string.IsNullOrWhiteSpace(lstszFiles[x]) == true)
                            {
                                lstszFiles.RemoveAt(x);
                            }
                        }
                        for (int x = 0; x < lstszAllReg.Count; x++)
                        {
                            if (string.IsNullOrEmpty(lstszAllReg[x]) == true || string.IsNullOrWhiteSpace(lstszAllReg[x]) == true)
                            {
                                lstszAllReg.RemoveAt(x);
                            }
                        }

                        // Update status info with file, folder, registry data found in unknown input file
                        if (lstszFolders.Count > 0 || lstszFiles.Count > 0 || lstszAllReg.Count > 0)
                        {
                            prgMain.Maximum = (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count);
                            lblProg1.Text = "Progress: 0% - 0 of " + (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count).ToString();
                            lblFolders.Text = lstszFolders.Count.ToString() + " Folders Found";
                            lblFiles.Text = lstszFiles.Count.ToString() + " Files Found";
                            lblRegistry.Text = lstszAllReg.Count.ToString() + " Registry Entries Found";
                            lblCurrent.Text = "Examination complete: " + szFilePath;
                            Application.DoEvents();

                        }
                        else // No file, folder, registry data found in unknown input file 
                        {
                            MessageBox.Show("Sorry, unable to parse this file, therefore cannot continue with export process", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            btnExport.Enabled = true;
                            btnInputFile.Enabled = true;
                            btnOutput.Enabled = true;
                            blExportStarted = false;
                            picBoxPRGMain.BackColor = colorOriginalPicBackColor;

                            return;
                        }


                    }
                }

                // Continue with export
                lblStatusRight.Text = "Adding FILE and FOLDER DATA to " + szOutputFolder + "\\" + szAllFilesFoldersName;
                picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;
                Application.DoEvents();

                // Write Files to File 
                using (StreamWriter sw = new StreamWriter(szOutputFolder + "\\" + szAllFilesFoldersName, true))
                {
                    for (int y = 0; y < lstszFolders.Count; y++)
                    {
                        // cancel or Exit
                        if (blExitPressed == true) { return; }
                        if (blCancelPressed == true) { return; }

                        try
                        {
                            // Create and add to file of all folder data - AllFilesFolders.txt
                            lblCurrent.Text = "Writing folder data to " + szOutputFolder + "\\" + szAllFilesFoldersName;
                            Application.DoEvents();
                            sw.WriteLine("Folder - " + lstszFolders[y]);
                        }
                        catch (Exception ex)
                        {
                            // log entry for error
                            using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                            {
                                sw1.WriteLine("Error: " + ex.Message + " - for folder: " + lstszFolders[y]);

                            }
                            picBoxPRGMain.BackColor = System.Drawing.Color.IndianRed;
                            Application.DoEvents();
                            szEX = ex.Message;
                        }

                    }
                    for (int y = 0; y < lstszFiles.Count; y++)
                    {
                        // cancel or Exit
                        if (blExitPressed == true) { return; }
                        if (blCancelPressed == true) { return; }

                        try
                        {
                            // Create and add to file of all folder data - AllFilesFolders.txt
                            lblCurrent.Text = "Writing file data to " + szOutputFolder + "\\" + szAllFilesFoldersName;
                            Application.DoEvents();
                            sw.WriteLine("File - " + lstszFiles[y]);
                        }
                        catch (Exception ex)
                        {
                            // log entry for error
                            using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                            {
                                sw1.WriteLine("Error: " + ex.Message + " - for file: " + lstszFiles[y]);

                            }
                            picBoxPRGMain.BackColor = System.Drawing.Color.IndianRed;
                            Application.DoEvents();
                            szEX = ex.Message;
                        }

                    }
                }

                lblStatusRight.Text = "Adding REGISTRY DATA to " + szOutputFolder + "\\" + szAllRegistryValues;
                picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;
                Application.DoEvents();

                // Write registry to File 
                using (StreamWriter sw = new StreamWriter(szOutputFolder + "\\" + szAllRegistryValues, true))
                {
                    for (int y = 0; y < lstszAllReg.Count; y++)
                    {
                        // cancel or Exit
                        if (blExitPressed == true) { return; }
                        if (blCancelPressed == true) { return; }

                        try
                        {
                            // Create and add to file of all folder data - AllRegistry.txt
                            lblCurrent.Text = "Writing registry data to " + szOutputFolder + "\\" + szAllRegistryValues;
                            Application.DoEvents();
                            sw.WriteLine("Registry - " + lstszAllReg[y]);
                        }
                        catch (Exception ex)
                        {
                            // log entry for error
                            using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                            {
                                sw1.WriteLine("Error: " + ex.Message + " - for reg entry: " + lstszAllReg[y]);

                            }
                            picBoxPRGMain.BackColor = System.Drawing.Color.IndianRed;
                            Application.DoEvents();
                            szEX = ex.Message;
                        }

                    }
                    for (int y = 0; y < lstszAllReg.Count; y++)
                    {
                        // cancel or Exit
                        if (blExitPressed == true) { return; }
                        if (blCancelPressed == true) { return; }

                        try
                        {
                            // Create and add to file of all folder data - AllRegistry.txt
                            lblCurrent.Text = "Writing registry data to " + szOutputFolder + "\\" + szAllRegistryValues;
                            Application.DoEvents();
                            sw.WriteLine("Registry- " + lstszAllReg[y]);
                        }
                        catch (Exception ex)
                        {
                            // log entry for error
                            using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                            {
                                sw1.WriteLine("Error: " + ex.Message + " - for reg entry: " + lstszAllReg[y]);

                            }
                            picBoxPRGMain.BackColor = System.Drawing.Color.IndianRed;
                            Application.DoEvents();
                            szEX = ex.Message;
                        }

                    }
                }

                lblStatusRight.Text = "Copying all FILES and FOLDERS to " + szOutputFolder + "\\FileExport";
                picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;
                Application.DoEvents();

                // Attempt to copy all folders and files to output folder
                using (StreamReader sr = new StreamReader(szOutputFolder + "\\" + szAllFilesFoldersName))
                {
                    if (Directory.Exists(szOutputFolder + "\\" + szAllFilesFoldersName) == false)
                    {
                        try
                        {
                            Directory.CreateDirectory(szOutputFolder + "\\" + szAllFilesFoldersName);
                        }
                        catch
                        {
                            picBoxPRGMain.BackColor = System.Drawing.Color.IndianRed;
                            Application.DoEvents();

                        }
                    }
                    while (sr.EndOfStream == false)
                    {
                        // cancel or Exit
                        if (blExitPressed == true) { return; }
                        if (blCancelPressed == true) { return; }

                        szFiletoCopyOldPath = string.Empty;
                        szFiletoCopy = string.Empty;

                        picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;
                        Application.DoEvents();

                        try
                        {

                            // If it's a folder - CreateDirectory()
                            szFiletoCopy = sr.ReadLine();
                            if (szFiletoCopy.Contains("Folder - ") == true)
                            {
                                lblCurrent.Text = "Creating folder: " + szFiletoCopy;
                                Application.DoEvents();
                                szFiletoCopy = szFiletoCopy.Replace("Folder - ", "");
                                szFiletoCopy = szFiletoCopy.Trim();
                                szFiletoCopy = szFiletoCopy.Replace(Path.GetPathRoot(szFiletoCopy), szOutputFolder + "\\FileExport\\");
                                Directory.CreateDirectory(szFiletoCopy);
                                using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                                {
                                    sw1.WriteLine("Folder Created - " + szFiletoCopy);
                                }
                            }
                            else
                            {
                                // If it's a file- File.Copy()
                                szFiletoCopy = szFiletoCopy.Replace("File - ", "");
                                szFiletoCopyOldPath = szFiletoCopy;
                                szFiletoCopy = szFiletoCopy.Trim();
                                szFiletoCopy = szFiletoCopy.Replace(Path.GetPathRoot(szFiletoCopy), szOutputFolder + "\\FileExport\\");
                                lblCurrent.Text = "Exporting: " + szFiletoCopy;
                                Application.DoEvents();
                                Directory.CreateDirectory(Directory.GetParent(szFiletoCopy).FullName);
                                File.Copy(szFiletoCopyOldPath, szFiletoCopy, true);
                                using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                                {
                                    sw1.WriteLine("File Copied - " + szFiletoCopy);
                                }
                            }

                            prgMain.Increment(1);
                            intPrg1++;
                            lblProg1.Text = "Progress: " + ((intPrg1 * 100) / (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count)).ToString() + "% - " + intPrg1.ToString() + " of " + (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count).ToString();
                            picBoxPRGMain.BackColor = System.Drawing.Color.ForestGreen;
                            Application.DoEvents();

                        }
                        catch (Exception ex)
                        {
                            szEX = ex.Message;
                            intErrorCount++;

                            // log entry for file of older error
                            using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                            {
                                if (ex.Message.Contains(szFiletoCopy) == true)
                                {
                                    sw1.WriteLine("Error: " + ex.Message);
                                }
                                else
                                {
                                    sw1.WriteLine("Error: " + ex.Message + " - for file/folder: " + szFiletoCopy);
                                }

                            }
                        }
                    }
                }

                #endregion Folders & Files

                #region Registry Entries

                lblStatusRight.Text = "Exporting all REGISTRY DATA to " + szOutputFolder + @"\RegExport";
                Application.DoEvents();

                // Iterate through all registry entries
                for (int y = 0; y < lstszAllReg.Count; y++)
                {
                    // cancel or Exit
                    if (blExitPressed == true) { return; }
                    if (blCancelPressed == true) { return; }

                    // If reg entry is for HKEY_CLASSES_ROOT (HKCR)
                    if (lstszAllReg[y].Contains("HKEY_CLASSES_ROOT") || lstszAllReg[y].Contains("HKCR") == true)
                    {
                        lblStatusRight.Text = "Exporting HKCR DATA to " + szOutputFolder + @"\RegExport";
                        Application.DoEvents();

                        string szRegPathHKCR = lstszAllReg[y].Replace("HKEY_CLASSES_ROOT", "HKCR");
                        fnExportRegistryHive(szRegPathHKCR, szOutputFolder + @"\RegExport\HKEY_CLASSES_ROOT");
                    }
                    // If reg entry is for HKEY_CURRENT_USER (HKCU)
                    else if (lstszAllReg[y].Contains("HKEY_CURRENT_USER") || lstszAllReg[y].Contains("HKCU") == true)
                    {
                        lblStatusRight.Text = "Exporting HKCU DATA to " + szOutputFolder + @"\RegExport";
                        Application.DoEvents();

                        string szRegPathHKCU = lstszAllReg[y].Replace("HKEY_CURRENT_USER", "HKCU");
                        fnExportRegistryHive(szRegPathHKCU, szOutputFolder + @"\RegExport\HKEY_CURRENT_USER");
                    }
                    // If reg entry is for HKEY_LOCAL_MACHINE (HKLM)
                    else if (lstszAllReg[y].Contains("HKEY_LOCAL_MACHINE") || lstszAllReg[y].Contains("HKLM") == true)
                    {
                        lblStatusRight.Text = "Exporting HKLM DATA to " + szOutputFolder + @"\RegExport";
                        Application.DoEvents();

                        string szRegPathHKLM = lstszAllReg[y].Replace("HKEY_LOCAL_MACHINE", "HKLM");
                        fnExportRegistryHive(szRegPathHKLM, szOutputFolder + @"\RegExport\HKEY_LOCAL_MACHINE");
                    }
                    // If reg entry is for HKEY_USERS (HKU)
                    else if (lstszAllReg[y].Contains("HKEY_USERS") || lstszAllReg[y].Contains("HKU") == true)
                    {
                        lblStatusRight.Text = "Exporting HKU DATA to " + szOutputFolder + @"\RegExport";
                        Application.DoEvents();

                        string szRegPathHKU = lstszAllReg[y].Replace("HKEY_USERS", "HKU");
                        fnExportRegistryHive(szRegPathHKU, szOutputFolder + @"\RegExport\HKEY_USERS");
                    }
                    // If reg entry is for HKEY_CURRENT_CONFIG" (HKCC)
                    else if (lstszAllReg[y].Contains("HKEY_CURRENT_CONFIG") || lstszAllReg[y].Contains("HKCC") == true)
                    {
                        lblStatusRight.Text = "Exporting HKCC DATA to " + szOutputFolder + @"\RegExport";
                        Application.DoEvents();

                        string szRegPathHKCC = lstszAllReg[y].Replace("HKEY_CURRENT_CONFIG", "HKCC");
                        fnExportRegistryHive(szRegPathHKCC, szOutputFolder + @"\RegExport\HKEY_CURRENT_CONFIG");
                    }
                    // If reg entry is for any other registry value
                    else
                    {
                        lblStatusRight.Text = "Exporting Unidentified Registry DATA to " + szOutputFolder + @"\RegExport";
                        Application.DoEvents();

                        fnExportRegistryHive(lstszAllReg[y], szOutputFolder + @"\RegExport\ExtraRegData");
                    }

                    // Update status info
                    prgMain.Increment(1);
                    intPrg1++;
                    lblProg1.Text = "Progress: " + ((intPrg1 * 100) / (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count)).ToString() + "% - " + intPrg1.ToString() + " of " + (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count).ToString();
                    Application.DoEvents();
                }

                // Consolidate .reg files for each hive - HKCR, HKCU, HKLM, HKU, HKCC, ExtraReg
                lblStatusRight.Text = "Consolidating Registry DATA";
                Application.DoEvents();
                bool blAddregFileHeader = false;
                if (Directory.Exists(szOutputFolder + @"\RegExport") == true)
                {
                    foreach (string szdir in Directory.GetDirectories(szOutputFolder + @"\RegExport"))
                    {
                        string szDirName = szdir.Split('\\').Last();
                        lblStatusRight.Text = "Consolidating " + szDirName + " DATA to " + szDirName + ".reg";
                        Application.DoEvents();

                        foreach (string szRegFile in Directory.GetFiles(szdir))
                        {
                            lblCurrent.Text = "Adding " + szRegFile;
                            Application.DoEvents();

                            string szAllRegText = string.Empty;
                            try
                            {
                                using (StreamReader sr = new StreamReader(szRegFile))
                                {
                                    // Remove "Windows Registry Editor Version 5.00" and any preceding whitespace
                                    szAllRegText = sr.ReadToEnd();
                                    szAllRegText = szAllRegText.Replace("Windows Registry Editor Version 5.00", "");
                                    szAllRegText = szAllRegText.Trim();
                                }

                                // Create single consolidated .reg file
                                using (StreamWriter sw = new StreamWriter(Directory.GetParent(szRegFile) + "\\" + szDirName + ".reg", true))
                                {
                                    if (blAddregFileHeader == false)
                                    {
                                        sw.WriteLine("Windows Registry Editor Version 5.00" + Environment.NewLine);
                                        blAddregFileHeader = true;
                                    }
                                    sw.WriteLine();
                                    sw.Write(szAllRegText);
                                    sw.WriteLine();
                                    sw.Flush();
                                }

                                // Add log entry for last file
                                using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                                {
                                    sw1.WriteLine("Consolidating " + szDirName + " - Added: " + szRegFile);
                                }
                            }
                            catch (Exception ex)
                            {
                                using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                                {
                                    sw1.WriteLine("Consolidating " + szDirName + " - Error Adding: " + szRegFile + " - " + ex.Message);
                                }
                            }

                            // Attempt delete last .reg TMP file
                            try
                            {
                                File.Delete(szRegFile);
                                using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                                {
                                    sw1.WriteLine("Consolidating " + szDirName + " - Deletion Successful: " + szRegFile);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log error for deletion attempt
                                using (StreamWriter sw1 = new StreamWriter(szOutputFolder + "\\" + szLogFile, true))
                                {
                                    sw1.WriteLine("Consolidating " + szDirName + " - Error Deleting: " + szRegFile + " - " + ex.Message);
                                }
                            }

                        }
                        blAddregFileHeader = false;
                    }
                }
                else
                {
                    MessageBox.Show("Unable to locate: " + szOutputFolder + @"\RegExport\nConsolidation of REG files cannot continue", "TUNExport Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update status info with complete
                lblProg1.Text = "Progress: 100% - " + intPrg1.ToString() + " of " + (lstszFolders.Count + lstszFiles.Count + lstszAllReg.Count).ToString() + " with " + intErrorCount.ToString() + " Errors";
                prgMain.Value = prgMain.Maximum;
                lblStatusRight.Text = "Export complete";
                btnExport.Enabled = true;
                btnInputFile.Enabled = true;
                btnOutput.Enabled = true;
                blExportStarted = false;

                #endregion Registry Entries 

            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // TUNExporter isn't multi-threaded and doesn't use a background worker so btnExit_Click is a bit messy but it works
            if (blExportStarted == false) { Application.Exit(); }

            if (MessageBox.Show("Are you sure you wish to exit TUNExporter at this time?", "TUN Exporter MSG", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            lblStatusRight.Text = "Stopping current operation and exiting....";
            lblCurrent.Text = "Waiting for file copy or reg export to complete, forcing exit in 15s...";
            blExitPressed = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = false;
            picBoxPRGMain.BackColor = colorOriginalPicBackColor;
            Application.DoEvents();
            Application.Exit();
            Application.DoEvents();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // TUNExporter isn't multi-threaded and doesn't use a background worker so btnCancel_Click is a bit messy but it works
            if (blExportStarted == false) { MessageBox.Show("No export operation in progress", "TUN Export Message", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (MessageBox.Show("Are you sure you wish to cancel the export process at this time?", "TUN Exporter MSG", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            blCancelPressed = true;
            lblStatusRight.Text = "Stopping current operation....";
            picBoxPRGMain.BackColor = colorOriginalPicBackColor;
            lblStatusRight.Text = "Ready";
            lblCurrent.Text = "Current operation stopped";
            btnCancel.Enabled = true;
            btnExit.Enabled = true;
            btnExport.Enabled = true;
            btnInputFile.Enabled = true;
            btnOutput.Enabled = true;
            blExportStarted = false;
            prgMain.Value = 0;
            lblProg1.Text = "Progress: 0% - 0 of 0";
            lblFolders.Text = "0 Folders Found";
            lblFiles.Text = "0 Files Found";
            lblRegistry.Text = "0 Registry Entries Found";
            lblCurrent.Text = "";
            Application.DoEvents();
        }

        #endregion Function - Event Functions

        #endregion Functions

    }
}
