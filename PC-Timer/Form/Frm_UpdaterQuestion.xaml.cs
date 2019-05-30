#region Header

//----------------------------------------------------------------------
// 
// Project is available at https://github.com/Hundhausen
// This Project is licensed under the GNU General Public License v3.0
//
// Date: 2019-05-28
// User: Hundhausen
//
//----------------------------------------------------------------------

#endregion

using System;
using System.Resources;
using JPH_Library.Logger;

namespace PC_Timer.Form
{
    /// <summary>Window asks if the Program should search for updates automatically</summary>
    /// <seealso cref="System.Windows.Window" />
    public partial class Frm_UpdaterQuestion
    {
        public enum eResult
        {
            Yes,
            No,
            Later
        }

        #region var

        private static readonly ResourceManager _resManager = Languages.PC_Timer_lang.ResourceManager;
        private static readonly JPH_Logger _logger = new JPH_Logger("UpdaterQuestion");
        public eResult? Result { get; private set; }

        public int Interval
        {
            get
            {
                int interval;
                switch (cb_interval.SelectedIndex)
                {
                    case 0:
                        interval = 0;
                        break;
                    case 1:
                        interval = 1;
                        break;
                    case 2:
                        interval = 5;
                        break;
                    case 3:
                        interval = 7;
                        break;
                    case 4:
                        interval = 14;
                        break;
                    case 5:
                        interval = 30;
                        break;
                    case 6:
                        interval = 60;
                        break;
                    case 7:
                        interval = 90;
                        break;
                    default:
                        _logger.Fatal($"cb_interval ArgumentOutOfRangeException | Selected Index {cb_interval.SelectedIndex} |  20190528-21520128 ");
                        throw new ArgumentOutOfRangeException();
                }

                return interval;
            }
        }

        #endregion


        public Frm_UpdaterQuestion()
        {
            InitializeComponent();

            #region Setting Lang

            cbi_0.Content = _resManager.GetString("updater_never");
            cbi_1.Content += " " + _resManager.GetString("updater_day");
            cbi_5.Content += " " + _resManager.GetString("updater_days");
            cbi_7.Content += " " + _resManager.GetString("updater_days");
            cbi_14.Content += " " + _resManager.GetString("updater_days");
            cbi_30.Content += " " + _resManager.GetString("updater_days");
            cbi_60.Content += " " + _resManager.GetString("updater_days");
            cbi_90.Content += " " + _resManager.GetString("updater_days");

            #endregion
        }

        #region Form Events
        private void Btn_yes_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Result = eResult.Yes;
            DialogResult = true;
            Close();
        }

        private void Btn_no_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Result = eResult.No;
            DialogResult = true;
            Close();
        }

        private void Btn_later_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Result = eResult.Later;
            DialogResult = true;
            Close();
        }
        #endregion

        private void Frm_updateQuestion_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Result == null)
            {
                Result = eResult.Later;
                DialogResult = true;
            }
        }
    }
}

//----------------------------------------------------------------------
// Project is available at https://github.com/Hundhausen
//----------------------------------------------------------------------