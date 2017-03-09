using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace KataShell
{
    [Serializable]
    public class AppData
    {
        public string Name { get; set; }
        public string ImageFile { get; set; }
        public string Filename { get; set; }
        public string Arguments { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<AppData> Apps { get; } = new List<AppData>();

        Process _activeApp;

        public MainWindow()
        {
            InitializeComponent();

            string appDataFilename = "apps.xml";
            string appDataPath = appDataFilename;
            //check for a local copy of this file (for debugging purposes) else use a hard coded path copy
            if (!File.Exists(appDataPath))
                appDataPath = "D:/" + appDataFilename;
            //Didn't find that, check for an app data copy
            if (!File.Exists(appDataPath))
                appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/KataShell/" + appDataFilename;
            if (!File.Exists(appDataPath))
            {
                MessageBox.Show("Unable to load app list.", "App List Error");
            }

            var xs = new XmlSerializer(typeof(List<AppData>));
            var reader = new StreamReader(appDataPath);
            Apps = (List<AppData>)xs.Deserialize(reader);

            this.DataContext = Apps;
        }

        private void KillActiveApp()
        {
            if (!_activeApp?.HasExited == true)
            {
                _activeApp.Kill(); //sorry!
                _activeApp = null;
            }
        }

        private void AppButton_Click(object sender, RoutedEventArgs e)
        {
            KillActiveApp();

            var btn = (Button)sender;
            var appData = (AppData)btn.Tag;
            var psi = new ProcessStartInfo((string)appData.Filename);
            if (string.IsNullOrEmpty(psi.FileName))
                return;
            psi.Arguments = appData.Arguments;
            psi.UseShellExecute = true;

            try
            {
                _activeApp = Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to start app: " + ex.Message, "App Launch Error");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KillActiveApp();
        }
    }
}
