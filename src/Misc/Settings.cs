using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinimalJson;

namespace Youtube_Music.Misc
{
    public class Settings
    {
        public string outputDevice;

        public void Store(string path)
        {
            if (!File.Exists(path))
            {
                new FileInfo(path).Directory.Create();
            }
            try
            {
                var json = new JsonObject();
                json.set("outputDevice", outputDevice);
                File.WriteAllText(path, json.ToString());
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
                    var json = JsonObject.readFrom(File.ReadAllText(path));
                    outputDevice = json.get("outputDevice").asString();
                }
                catch
                {
                    MessageBox.Show("Unable to load config.");
                }
            }
        }
    }
}
