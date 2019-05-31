#region Header

//----------------------------------------------------------------------
// 
// Project is available at https://github.com/Hundhausen
// This Project is licensed under the GNU General Public License v3.0
//
// Date: 2019-05-25
// User: Hundhausen
//
//----------------------------------------------------------------------

#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using JPH_Library.Logger;
using JPH_Library.Updater;
using PC_Timer.Functions;
// ReSharper disable PossibleNullReferenceException

namespace PC_Timer.Form
{

    public partial class FrmPcTimerMain
    {
        #region var

        private static readonly ResourceManager _resManager = Languages.PC_Timer_lang.ResourceManager;
        private static readonly JPH_Logger _logger = new JPH_Logger("PC-Timer");

        #endregion

        #region Enum
        //required to set the option to prevent sleep
        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001,
        }
        #endregion

        public FrmPcTimerMain()
        {
            InitializeComponent();
            _logger.Info($"PC-Timer loaded | Lang: {App.Culture} |  20190526-12201220 ");

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (App.Culture)
            {
                //Unloading the event for this, prevents the Messagebox to show up.
                case "de-DE":
                    menRadio_de.Checked -= MenRadio_de_Checked;
                    menRadio_de.IsChecked = true;
                    menRadio_de.Checked += MenRadio_de_Checked;
                    break;
                case "en-US":
                    menRadio_en.Checked -= MenRadio_en_Checked;
                    menRadio_en.IsChecked = true;
                    menRadio_en.Checked += MenRadio_en_Checked;
                    break;
            }

            datetimepicker_date.Value = DateTime.Now;
        }

        #region Functions
        public static void write_lang_settings(string lang)
        {
            if (File.Exists(App.ConfigFile))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(App.ConfigFile);
                    doc.DocumentElement.SelectSingleNode("/settings/language").InnerText = lang;
                    doc.Save(App.ConfigFile);
                }
                catch (Exception e)
                {
                    _logger.Fatal($"XML File is broken! |  20190526-12101055 | {e.Message}");
                    throw;
                }
            }
            else
            {
                new XDocument(new XElement("settings", new XElement("language", lang))).Save(App.ConfigFile);
            }

        }

        #endregion



        #region Form Events

        private void MenRadio_en_Checked(object sender, RoutedEventArgs e)
        {
            write_lang_settings("en-US");
            MessageBox.Show(_resManager.GetString("msgbox_lang_changed"),
                _resManager.GetString("msgbox_lang_changed_title"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenRadio_de_Checked(object sender, RoutedEventArgs e)
        {
            write_lang_settings("de-DE");
            MessageBox.Show(_resManager.GetString("msgbox_lang_changed"),
                _resManager.GetString("msgbox_lang_changed_title"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenItem_en_Click(object sender, RoutedEventArgs e)
        {
            menRadio_en.IsChecked = true;
        }

        private void MenItem_de_Click(object sender, RoutedEventArgs e)
        {
            menRadio_de.IsChecked = true;
        }

        private void MenCode_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Hundhausen/PC-Timer");
        }

        private void MenError_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Hundhausen/PC-Timer/issues");
        }

        /// <summary>
        /// Opens the About Window after clicking the Menu Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenInfo_Click(object sender, RoutedEventArgs e)
        {
            Frm_AboutWindow aw = new Frm_AboutWindow();
            aw.Show();
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            int time = 0;
            int threshold = 60; //threshold in sec, where timer ask before start
            if (tab_time.IsSelected)
            {
                int sec;
                int min;
                int hour;
                try
                {
                    //failsafe. if nothing is set in a textbox, vaule = 0. When not an integer can be parse into an integer, it should crash
                    hour = string.IsNullOrEmpty(txtbox_hour.Text) ? 0 : int.Parse(txtbox_hour.Text);
                    min = string.IsNullOrEmpty(txtbox_min.Text) ? 0 : int.Parse(txtbox_min.Text);
                    sec = string.IsNullOrEmpty(txtbox_sec.Text) ? 0 : int.Parse(txtbox_sec.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show(_resManager.GetString("msgbox_noint_txt") + "\n\n" + e, _resManager.GetString("msgbox_fail_head"), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                time = sec + (min * 60) + (hour * 60 * 60);
            }
            else if (tab_date.IsSelected)
            {
                if (!DateTime.TryParse(datetimepicker_date.Text, out DateTime selDate))
                {
                    MessageBox.Show(_resManager.GetString("msgbox_fail_txt"), _resManager.GetString("msgbox_fail_head"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                DateTime curDate = DateTime.Now;
                if (selDate < curDate)
                {
                    MessageBox.Show(_resManager.GetString("msgbox_dateoutofrange_txt"), _resManager.GetString("msgbox_fail_head"), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    TimeSpan span = selDate.Subtract(curDate);
                    time = (int)span.TotalSeconds;
                }
            }
            else
            {
                MessageBox.Show(_resManager.GetString("msgbox_fail_txt"), _resManager.GetString("msgbox_fail_head"), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //case is depending on the position and default is only a fail safe. 0 is shutdown, 1 restart and so on
            if (checkBox_sleep.IsEnabled)
            {
                if (checkBox_display.IsEnabled)
                {
                    NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
                }
                else
                {
                    NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
                }
            }

            //when set time is under threshold, user gets asked
            if (time < threshold)
            {
                if (MessageBox.Show(_resManager.GetString("msgbox_ask_start") + "\n" + time + " " + _resManager.GetString("msgbox_ask_time"), "PC-Timer", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }
            }

            switch (combo_art.SelectedIndex)
            {
                case 0:
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/C shutdown /s /f /t " + time
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    MessageBox.Show(_resManager.GetString("msgbox_start_shutodown_txt"), _resManager.GetString("msgbox_start_head"), MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 1:
                    Process process1 = new Process();
                    ProcessStartInfo startInfo1 = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/C shutdown /r /f /t " + time
                    };
                    process1.StartInfo = startInfo1;
                    process1.Start();
                    MessageBox.Show(_resManager.GetString("msgbox_start_restart_txt"), _resManager.GetString("msgbox_start_head"), MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 2:
                    Process process2 = new Process();
                    ProcessStartInfo startInfo2 = new ProcessStartInfo();
                    startInfo2.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo2.FileName = "cmd.exe";
                    startInfo2.Arguments = "/C shutdown /h /f /t " + time;
                    process2.StartInfo = startInfo2;
                    process2.Start();
                    MessageBox.Show(_resManager.GetString("msgbox_start_suspend_txt"), _resManager.GetString("msgbox_start_head"), MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    Process process3 = new Process();
                    ProcessStartInfo startInfo3 = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/C shutdown /l /f /t " + time
                    };
                    process3.StartInfo = startInfo3;
                    process3.Start();
                    MessageBox.Show(_resManager.GetString("msgbox_start_logout_txt"), _resManager.GetString("msgbox_start_head"), MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    MessageBox.Show(_resManager.GetString("msgbox_fail_txt"), _resManager.GetString("msgbox_fail_head"), MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

            }

        }

        private void checkBox_display_Checked(object sender, RoutedEventArgs e)
        {
            //Display Checkbox is not allowed to be checked alone because then the code will not be executed and also it makes no sense to me to keep the display active when the pc is allowed to go into sleep
            checkBox_sleep.IsChecked = true;
        }

        private void checkBox_sleep_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBox_display.IsChecked = false;
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C shutdown /a"
            };
            process.StartInfo = startInfo;
            process.Start();
            MessageBox.Show(_resManager.GetString("msgbox_abort_txt"), _resManager.GetString("msgbox_abort_head"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            if (File.Exists(App.ConfigFile))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(App.ConfigFile);

                    bool? bLookForUpdates = null;
                    bool? bPreReleases = null;
                    DateTime? lastCheck = null;
                    uint? unCheckInterval = null;

                    if (doc.DocumentElement != null)
                    {
                        XmlNode node = doc.DocumentElement.SelectSingleNode("/settings/updater/lookforupdates");
                        if (node != null) bLookForUpdates = Convert.ToBoolean(node.InnerText);

                        node = doc.DocumentElement.SelectSingleNode("/settings/updater/prereleases");
                        if (node != null) bPreReleases = Convert.ToBoolean(node.InnerText);

                        node = doc.DocumentElement.SelectSingleNode("/settings/updater/lastcheck");
                        if (node != null) lastCheck = Convert.ToDateTime(node.InnerText);

                        node = doc.DocumentElement.SelectSingleNode("/settings/updater/checkinterval");
                        if (node != null) unCheckInterval = Convert.ToUInt32(node.InnerText);
                    }

                    if (bLookForUpdates == null || bPreReleases == null || lastCheck == null || unCheckInterval == null)
                    {
                        Frm_UpdaterQuestion frmUpdaterQuestion = new Frm_UpdaterQuestion();
                        frmUpdaterQuestion.ShowDialog();
                        Frm_UpdaterQuestion.eResult? result = frmUpdaterQuestion.Result;
                        switch (result)
                        {
                            case Frm_UpdaterQuestion.eResult.Yes:
                                {
                                    bLookForUpdates = true;
                                    unCheckInterval = (uint?)frmUpdaterQuestion.Interval;
                                    lastCheck = DateTime.UtcNow.Subtract(
                                        TimeSpan.FromDays((double)(unCheckInterval + 10)));
                                    bPreReleases = frmUpdaterQuestion.chb_prerelease.IsChecked;

                                    XmlNode elementUpdater = doc.CreateNode(XmlNodeType.Element, "updater", null);

                                    XmlNode elementupdates = doc.CreateElement("lookforupdates");
                                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                    elementupdates.InnerText = bLookForUpdates.ToString();

                                    XmlNode elementInterval = doc.CreateElement("checkinterval");
                                    elementInterval.InnerText = unCheckInterval.ToString();

                                    XmlNode elementCheck = doc.CreateElement("lastcheck");
                                    elementCheck.InnerText = lastCheck.Value.ToString("o");

                                    XmlNode elementPrerelease = doc.CreateElement("prereleases");
                                    elementPrerelease.InnerText = bPreReleases.ToString();

                                    elementUpdater.AppendChild(elementupdates);
                                    elementUpdater.AppendChild(elementInterval);
                                    elementUpdater.AppendChild(elementCheck);
                                    elementUpdater.AppendChild(elementPrerelease);

                                    doc.DocumentElement.AppendChild(elementUpdater);
                                    doc.Save(App.ConfigFile);

                                    break;
                                }
                            case Frm_UpdaterQuestion.eResult.No:
                                {
                                    bLookForUpdates = false;
                                    unCheckInterval = (uint?)frmUpdaterQuestion.Interval;
                                    lastCheck = DateTime.UtcNow.Subtract(
                                        TimeSpan.FromDays((double)(unCheckInterval + 10)));
                                    bPreReleases = frmUpdaterQuestion.chb_prerelease.IsChecked;

                                    XmlNode elementUpdater = doc.CreateNode(XmlNodeType.Element, "updater", null);

                                    XmlNode elementupdates = doc.CreateElement("lookforupdates");
                                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                    elementupdates.InnerText = bLookForUpdates.ToString();

                                    XmlNode elementInterval = doc.CreateElement("checkinterval");
                                    elementInterval.InnerText = unCheckInterval.ToString();

                                    XmlNode elementCheck = doc.CreateElement("lastcheck");
                                    elementCheck.InnerText = lastCheck.Value.ToString("o");

                                    XmlNode elementPrerelease = doc.CreateElement("prereleases");
                                    elementPrerelease.InnerText = bPreReleases.ToString();

                                    elementUpdater.AppendChild(elementupdates);
                                    elementUpdater.AppendChild(elementInterval);
                                    elementUpdater.AppendChild(elementCheck);
                                    elementUpdater.AppendChild(elementPrerelease);

                                    doc.DocumentElement.AppendChild(elementUpdater);
                                    doc.Save(App.ConfigFile);
                                    break;
                                }
                            case Frm_UpdaterQuestion.eResult.Later:
                                return;
                            default:
                                _logger.Fatal("Frm_UpdaterQuestion.eResult ArgumentOutOfRangeException |  20190528-21422434 ");
                                throw new ArgumentOutOfRangeException($"{result} ArgumentOutOfRangeException |  20190528-21422434 ");
                        }
                    }

                    if ((bool)bLookForUpdates)
                    {
                        DateTime utcNow = DateTime.UtcNow;
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        lastCheck.Value.AddDays((double)unCheckInterval);
                        if (utcNow > lastCheck)
                        {
                            Version version = Assembly.GetExecutingAssembly().GetName().Version;
                            List<int> lstVersion = new List<int> { version.Major, version.Minor, version.Build, version.Revision }; //Why is Build and Revision switched? Thanks MS
                            // ReSharper disable once PossibleInvalidOperationException
                            if (Updater.NewVersionAvaible("Hundhausen", "PC-Timer", lstVersion, (bool)bPreReleases, out List<GithubRelease> lstGithubReleases))
                            {
                                Style style = new Style();
                                style.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.YesButtonContentProperty, _resManager.GetString("updater_download")));
                                style.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.NoButtonContentProperty, _resManager.GetString("updater_visit")));
                                // ReSharper disable once AssignNullToNotNullAttribute
                                style.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.CancelButtonContentProperty, string.Format(_resManager.GetString("updater_later"), unCheckInterval)));

                                // ReSharper disable once AssignNullToNotNullAttribute
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(String.Format(_resManager.GetString("updater_newversion"), lstGithubReleases[0].Tag), _resManager.GetString("updater_newversion_Title"), MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Cancel, style);
                                switch (result)
                                {
                                    case MessageBoxResult.Yes:
                                        using (WebClient client = new WebClient())
                                        {
                                            client.DownloadFile(lstGithubReleases[0].Assets[0].DownloadUrl, lstGithubReleases[0].Assets[0].Name);
                                            Process.Start(lstGithubReleases[0].Assets[0].Name);
                                            doc.Load(App.ConfigFile);
                                            doc.DocumentElement.SelectSingleNode("/settings/updater/lastcheck").InnerText = DateTime.UtcNow.ToString("o");
                                            doc.Save(App.ConfigFile);
                                            Application.Current.Shutdown();
                                        }
                                        break;
                                    case MessageBoxResult.No:
                                        Process.Start(lstGithubReleases[0].Url);
                                        doc.Load(App.ConfigFile);

                                        doc.DocumentElement.SelectSingleNode("/settings/updater/lastcheck").InnerText = DateTime.UtcNow.ToString("o");
                                        doc.Save(App.ConfigFile);
                                        break;
                                    case MessageBoxResult.Cancel:
                                        doc.Load(App.ConfigFile);
                                        doc.DocumentElement.SelectSingleNode("/settings/updater/lastcheck").InnerText = DateTime.UtcNow.ToString("o");
                                        doc.Save(App.ConfigFile);
                                        break;
                                    default:
                                        _logger.Fatal("MessageBoxResult ArgumentOutOfRangeException  |  20190529-21400324 ");
                                        throw new ArgumentOutOfRangeException($"MessageBoxResult ArgumentOutOfRangeException  |  20190529-21400324");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Fatal($"XML File is broken! |  20190526-12101055 | {e.Message}");
                    throw;
                }
            }
        }

        #endregion
    }
}
//----------------------------------------------------------------------
// Project is available at https://github.com/Hundhausen
//----------------------------------------------------------------------