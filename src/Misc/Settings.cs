using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Youtube_Music.Misc
{
    public class Settings
    {
        public string OutputDevice;
        public bool EnableRichPresence = true;

        public void Store(string path)
        {
            if (!File.Exists(path))
            {
                new FileInfo(path).Directory.Create();
            }
            try
            {
                var jsonString = JsonConvert.SerializeObject(this);
                File.WriteAllText(path, jsonString);
            }
            catch
            {
                MessageBox.Show("Unable to store config.");
            }
        }

        public void Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var jsonData = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
                    EnableRichPresence = jsonData.EnableRichPresence;
                    OutputDevice = jsonData.OutputDevice;
                }
                catch
                {
                    MessageBox.Show("Unable to load config.");
                }
            }
        }
    }
}
