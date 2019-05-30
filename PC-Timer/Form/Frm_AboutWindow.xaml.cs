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
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace PC_Timer.Form
{
    public partial class Frm_AboutWindow
    {
        #region var

        private static readonly ResourceManager _resManager = Languages.PC_Timer_lang.ResourceManager;

        #endregion

        #region ctor
        public Frm_AboutWindow()
        {
            InitializeComponent();

            label_version.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Used for Hyperlinks, to get open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void hyperlink_contrubitors_Click(object sender, RoutedEventArgs e)
        {
            textBlock_main.Inlines.Clear();
            Run head = new Run(_resManager.GetString("lable_contrubitors") + "\n\n") {FontWeight = FontWeights.Bold};
            textBlock_main.Inlines.Add(head);
            textBlock_main.Inlines.Add(_resManager.GetString("contrubitors_info_text") + "\n\n");

            //hundhausen
            Hyperlink hyperHundhausen = new Hyperlink(new Run("Jean-Pierre Hundhausen")) { NavigateUri = new Uri("https://github.com/Hundhausen") };
            hyperHundhausen.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(hyperHundhausen);
            textBlock_main.Inlines.Add(" " + _resManager.GetString("contrubitor_hundhausen") + "\n");

            //yasujizr
            Hyperlink hyperYasujizr = new Hyperlink(new Run("Yasujizr")) { NavigateUri = new Uri("https://github.com/Yasujizr") };
            hyperYasujizr.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(hyperYasujizr);
            textBlock_main.Inlines.Add(" " + _resManager.GetString("contrubitor_yasujizr") + "\n");

        }

        private void hyperlink_license_Click(object sender, RoutedEventArgs e)
        {
            textBlock_main.Inlines.Clear();
            Run head = new Run(_resManager.GetString("lable_license") + "\n\n") {FontWeight = FontWeights.Bold};
            textBlock_main.Inlines.Add(head);
            try
            {
                RichTextBox rtfBox = new RichTextBox();
                rtfBox.Selection.Load(new FileStream("License.rtf", FileMode.Open), DataFormats.Rtf);
                textBlock_main.Inlines.Add(rtfBox);
            }
            catch (Exception ex)
            {
                textBlock_main.Inlines.Add(_resManager.GetString("rtf_failed_load") + "\n\n\n" + ex.Message);
            }
        }

        private void hyperlink_libraries_Click(object sender, RoutedEventArgs e)
        {
            textBlock_main.Inlines.Clear();
            Run head = new Run(_resManager.GetString("lable_libraries") + "\n\n") {FontWeight = FontWeights.Bold};
            textBlock_main.Inlines.Add(head);

            //xceedsoftware - Extended WPF Toolkit
            Hyperlink hyperXceedsoftware = new Hyperlink(new Run(@"xceedsoftware")) { NavigateUri = new Uri("https://github.com/xceedsoftware") };
            hyperXceedsoftware.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(hyperXceedsoftware);
            textBlock_main.Inlines.Add(" - ");
            Hyperlink hyperExtendedWpfToolkit = new Hyperlink(new Run("Extended WPF Toolkit")) { NavigateUri = new Uri("https://github.com/xceedsoftware/wpftoolkit") };
            hyperExtendedWpfToolkit.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(hyperExtendedWpfToolkit);
            textBlock_main.Inlines.Add(" is licensed under the ");
            Hyperlink hyperExtendedWpfToolkitLicense = new Hyperlink(new Run("Microsoft Public License")) { NavigateUri = new Uri("https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md") };
            hyperExtendedWpfToolkitLicense.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(hyperExtendedWpfToolkitLicense);
            textBlock_main.Inlines.Add("\n");
            textBlock_main.Inlines.Add("\n");
            textBlock_main.Inlines.Add("\n");

            //NLog - NLog
            Hyperlink nlogUser = new Hyperlink(new Run(@"NLog")) { NavigateUri = new Uri("https://github.com/NLog") };
            nlogUser.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(nlogUser);
            textBlock_main.Inlines.Add(" - ");
            Hyperlink nlogRepo = new Hyperlink(new Run("NLog")) { NavigateUri = new Uri("https://github.com/NLog/NLog") };
            nlogRepo.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(nlogRepo);
            textBlock_main.Inlines.Add(" is licensed under the ");
            Hyperlink nlogLicense = new Hyperlink(new Run("BSD 3 - Clause \"New\" or \"Revised\" License")) { NavigateUri = new Uri("https://github.com/NLog/NLog/blob/dev/LICENSE.txt") };
            nlogLicense.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(nlogLicense);
            textBlock_main.Inlines.Add("\n");
            textBlock_main.Inlines.Add("\n");
            textBlock_main.Inlines.Add("\n");


            //NLog - NLog
            Hyperlink newtonsoftJsonUser = new Hyperlink(new Run(@"NLog")) { NavigateUri = new Uri("https://github.com/JamesNK") };
            nlogUser.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(newtonsoftJsonUser);
            textBlock_main.Inlines.Add(" - ");
            Hyperlink newtonsoftJsonRepo = new Hyperlink(new Run("NLog")) { NavigateUri = new Uri("https://github.com/JamesNK/Newtonsoft.Json") };
            nlogRepo.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(newtonsoftJsonRepo);
            textBlock_main.Inlines.Add(" is licensed under the ");
            Hyperlink newtonsoftJsonLicense = new Hyperlink(new Run("BSD 3 - Clause \"New\" or \"Revised\" License")) { NavigateUri = new Uri("https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md") };
            nlogLicense.RequestNavigate += Hyperlink_RequestNavigate;
            textBlock_main.Inlines.Add(newtonsoftJsonLicense);
            textBlock_main.Inlines.Add("\n");
            textBlock_main.Inlines.Add("\n");
            textBlock_main.Inlines.Add("\n");

        }

        #endregion
    }
}

//----------------------------------------------------------------------
// Project is available at https://github.com/Hundhausen
//----------------------------------------------------------------------