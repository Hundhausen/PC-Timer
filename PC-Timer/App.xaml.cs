#region Header

//----------------------------------------------------------------------
// 
// Project is available at https://github.com/Hundhausen
// This Project is licensed under the GNU General Public License v3.0
//
// Date: 2019-05-26
// User: Hundhausen
//
//----------------------------------------------------------------------

#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Xml;
using JPH_Library.Logger;

namespace PC_Timer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        #region var

        /// <summary>Path to the Config File of this App</summary>
        public static readonly string ConfigFile = "config.xml";

        /// <summary>Culture of this App</summary>
        public static string Culture { get; private set; }

        #endregion

        public App()
        {
            bool bNoFile = false;
            if (File.Exists(ConfigFile))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ConfigFile);

                    if (doc.DocumentElement != null)
                    {
                        JPH_Logger.PathToConfig = doc.DocumentElement.SelectSingleNode("/settings/logpath")?.InnerText;
                        Culture = (doc.DocumentElement.SelectSingleNode("/settings/language")?.InnerText);
                    }
                }
#pragma warning disable 168
                catch (Exception e)
#pragma warning restore 168
                {
#if DEBUG
                    // It shouldn't break here but when, maybe check out why. 
                    if (Debugger.IsAttached) Debugger.Break();
                    MessageBox.Show($"This should only be displayed, while the Program is in Debug Mode. \n{e.Message}", "XML Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                    // ignored
                }
            }
            else
            {
                bNoFile = true;
            }

            if (string.IsNullOrWhiteSpace(Culture)) Culture = Thread.CurrentThread.CurrentCulture.Name;

            switch (Culture)
            {
                case "de-DE":
                case "en-US":
                    break;
                default:
                    Culture = "en-US";
                    break;
            }
            if (bNoFile) Form.FrmPcTimerMain.write_lang_settings(Culture);
        }

    }
}

//----------------------------------------------------------------------
// Project is available at https://github.com/Hundhausen
//----------------------------------------------------------------------