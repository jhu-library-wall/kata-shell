﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
    
    public class WallConfig
    {
        public string Message { get; set; }
        public AppData[] Apps { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WallConfig wallConfig;
        Process _activeApp;

        public MainWindow()
        {
            InitializeComponent();

            string appDataFile = "apps.xml";
            //check for a local copy of this file (for debugging purposes) else use an app data copy
            if(!File.Exists(appDataFile))
                appDataFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/KataShell/" + appDataFile;
            var xs = new XmlSerializer(typeof(WallConfig));
            var reader = new StreamReader(appDataFile);

            wallConfig = (WallConfig)xs.Deserialize(reader);
            this.DataContext = wallConfig;
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
