using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Youtube_Music.Misc;

namespace Youtube_Music.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly string settingsPath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName,"cache\\settings.json");
        private Settings settings = new Settings();
        
        private CEFOutputDevice cefAudio;

        public SettingsForm(ChromiumWebBrowser wb)
        {
            InitializeComponent();
            cefAudio = new CEFOutputDevice(wb);
            LoadConfig();
        }

        private async void LoadConfig()
        {
            settings.Load(settingsPath);
            var devices = await cefAudio.Enumerate();
            var deviceNames = devices.Select(x => x.Label).ToList();
            if (deviceNames.Contains(settings.outputDevice))
            {
                await cefAudio.SetSink(devices.Where(x=>x.DeviceId == "default").FirstOrDefault());
                devices = await cefAudio.Enumerate();
                await cefAudio.SetSink(devices.Where(x => x.Label == settings.outputDevice).FirstOrDefault());
            }
        }

        public async void SettingsForm_Shown(object sender, EventArgs e)
        {
            var devices = await cefAudio.Enumerate();
            var deviceNames = devices.Select(x => x.Label).ToList();
            outDvcCb.Items.Clear();
            outDvcCb.Items.AddRange(devices.ToArray());
            if (deviceNames.Contains(settings.outputDevice))
            {
                outDvcCb.SelectedItem = devices.Where(x => x.Label == settings.outputDevice).FirstOrDefault();
            }
            else
            {
                var defaultDevice = devices.Where(x => x.DeviceId == "default").FirstOrDefault();
                settings.outputDevice = defaultDevice.Label;
                settings.Store(settingsPath);
                await cefAudio.SetSink(defaultDevice);
                outDvcCb.SelectedItem = defaultDevice;
            }
        }

        private async void outDvcCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var devices = await cefAudio.Enumerate();
            var device = (AudioOutput)outDvcCb.SelectedItem;
            await cefAudio.SetSink(devices.Where(x=>x.Label == device.Label).FirstOrDefault());
            settings.outputDevice = device.Label;
            settings.Store(settingsPath);
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
