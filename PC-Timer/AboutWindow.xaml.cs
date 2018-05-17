using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace PC_Timer {
    public partial class AboutWindow :Window {
        public AboutWindow(string config_file) {
            InitializeComponent();
            
            //reads the Culture from file
            string culture;
            if(!File.Exists(config_file)) {
                culture = Thread.CurrentThread.CurrentCulture.Name;
                }
            else {
                XmlDocument doc = new XmlDocument();
                doc.Load(config_file);
                XmlNode node = doc.DocumentElement.SelectSingleNode("/settings/language");
                culture = (node.InnerText);
                }
            SetLanguageDictionary(new CultureInfo(culture));

            label_version.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

        /// <summary>
        /// Changes the Language depending on the localisation. Default is en-US
        /// </summary>
        /// <param name="newCulture"></param>
        private void SetLanguageDictionary(CultureInfo newCulture) {
            CultureInfo.DefaultThreadCurrentCulture = newCulture;
            CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            ResourceDictionary dict = new ResourceDictionary();
            switch(Thread.CurrentThread.CurrentCulture.ToString()) {
                case "en-US":
                    dict.Source = new Uri("..\\Resources\\Dictionary_en-US.xaml", UriKind.Relative);
                    break;
                case "de-DE":
                    dict.Source = new Uri("..\\Resources\\Dictionary_de-DE.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\Dictionary_en-US.xaml", UriKind.Relative);
                    break;
                }
            this.Resources.MergedDictionaries.Add(dict);
            Thread.CurrentThread.CurrentUICulture = newCulture;
            }

        /// <summary>
        /// Used for Hyperlinks, to get opend
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
            }

        private void hyperlink_contrubitors_Click(object sender, RoutedEventArgs e) {
            textBlock_main.Inlines.Clear();
            Run head = new Run(Application.Current.FindResource("lable_contrubitors").ToString() + "\n\n");
            head.FontWeight = FontWeights.Bold;
            textBlock_main.Inlines.Add(head);
            textBlock_main.Inlines.Add(Application.Current.FindResource("contrubitors_info_text").ToString()+"\n\n");

            //hundhausen
            Hyperlink hyper_hundhausen = new Hyperlink(new Run("Jean-Pierre Hundhausen")) { NavigateUri = new Uri("https://github.com/Hundhausen") };
            hyper_hundhausen.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
            textBlock_main.Inlines.Add(hyper_hundhausen);
            textBlock_main.Inlines.Add(" " + Application.Current.FindResource("hundhausen").ToString() + "\n");

            //yasujizr
            Hyperlink hyper_yasujizr = new Hyperlink(new Run("Yasujizr")) { NavigateUri = new Uri("https://github.com/Yasujizr") };
            hyper_yasujizr.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
            textBlock_main.Inlines.Add(hyper_yasujizr);
            textBlock_main.Inlines.Add(" " + Application.Current.FindResource("yasujizr").ToString() + "\n");

            }

        private void hyperlink_license_Click(object sender, RoutedEventArgs e) {
            textBlock_main.Inlines.Clear();
            Run head = new Run(Application.Current.FindResource("lable_license").ToString() + "\n\n");
            head.FontWeight = FontWeights.Bold;
            textBlock_main.Inlines.Add(head);
            try {
                    RichTextBox rtfBox = new RichTextBox();
                    rtfBox.Selection.Load(new FileStream("License.rtf", FileMode.Open), DataFormats.Rtf);
                    textBlock_main.Inlines.Add(rtfBox);
                }
            catch(Exception ex) {
                textBlock_main.Inlines.Add(Application.Current.FindResource("rtf_failed_load").ToString() + "\n\n\n" + ex.Message);
                }
            }

        private void hyperlink_libraries_Click(object sender, RoutedEventArgs e) {
            textBlock_main.Inlines.Clear();
            Run head = new Run(Application.Current.FindResource("lable_libraries").ToString() + "\n\n");
            head.FontWeight = FontWeights.Bold;
            textBlock_main.Inlines.Add(head);

            //xceedsoftware - Extended WPF Toolkit
            Hyperlink hyper_xceedsoftware = new Hyperlink(new Run("xceedsoftware")) { NavigateUri = new Uri("https://github.com/xceedsoftware") };
            hyper_xceedsoftware.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
            textBlock_main.Inlines.Add(hyper_xceedsoftware);
            textBlock_main.Inlines.Add(" - ");
            Hyperlink hyper_Extended_WPF_Toolkit = new Hyperlink(new Run("Extended WPF Toolkit")) { NavigateUri = new Uri("https://github.com/xceedsoftware/wpftoolkit") };
            hyper_Extended_WPF_Toolkit.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
            textBlock_main.Inlines.Add(hyper_Extended_WPF_Toolkit);
            textBlock_main.Inlines.Add("\n");
            }
        }
    }
