using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SunIRCLibrary
{
    class SettingsHandler
    {
        public string settingsFile;
        public SettingsHandler()
        {
            this.settingsFile = "SunIRCSettings.bin";
        }

        public void saveSettings()
        {
            try
            {
                Settings setting = new Settings();
                setting.downloaddir = SharedData.currentDownloadLocation;
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(settingsFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                formatter.Serialize(stream, setting);
                stream.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG: COULD NOT SAVE SETTINGS -> " + e.ToString());
            }

        }

        public void loadSettings()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();

                if (!File.Exists(settingsFile))
                {
                    saveSettings();
                }
                else
                {

                    Stream stream = new FileStream(settingsFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    Settings setting = (Settings)formatter.Deserialize(stream);
                    stream.Close();
                    SharedData.currentDownloadLocation = setting.downloaddir;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG: COULD NOT OPEN SETTINGS -> " + e.ToString());
                Debug.WriteLine("DEBUG: GENERATING NEW SETTINGS ");
               
            }

        }
    }
}
