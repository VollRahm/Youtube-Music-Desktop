using CefSharp;
using CefSharp.WinForms;
using DiscordRPC;
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
        public static Settings settings = new Settings();
        
        public CEFOutputDevice cefAudio;
        private DiscordRpcClient richPresence;
        private bool UpdateRichPresence = true;

        public SettingsForm(ChromiumWebBrowser wb)
        {
            InitializeComponent();
            cefAudio = new CEFOutputDevice(wb);
            LoadConfig();
        }

        private async void LoadConfig()
        {
            settings.Load(settingsPath);
            rpcCb.Checked = settings.EnableRichPresence;
            rpcCb.CheckedChanged += new EventHandler(rpcCb_CheckedChanged);
            var devices = await cefAudio.Enumerate();
            _ = SetupRichPresence(rpcCb.Checked);
            var deviceNames = devices.Select(x => x.Label).ToList();
            if (deviceNames.Contains(settings.OutputDevice))
            {
                await cefAudio.SetSink(devices.Where(x=>x.DeviceId == "default").FirstOrDefault());
                devices = await cefAudio.Enumerate();
                await cefAudio.SetSink(devices.Where(x => x.Label == settings.OutputDevice).FirstOrDefault());
            }
            
        }

        public async void SettingsForm_Shown(object sender, EventArgs e)
        {
            var devices = await cefAudio.Enumerate();
            var deviceNames = devices.Select(x => x.Label).ToList();
            outDvcCb.Items.Clear();
            outDvcCb.Items.AddRange(devices.ToArray());
            if (deviceNames.Contains(settings.OutputDevice))
            {
                outDvcCb.SelectedItem = devices.Where(x => x.Label == settings.OutputDevice).FirstOrDefault();
            }
            else
            {
                var defaultDevice = devices.Where(x => x.DeviceId == "default").FirstOrDefault();
                settings.OutputDevice = defaultDevice.Label;
                settings.Store(settingsPath);
                await cefAudio.SetSink(defaultDevice);
                outDvcCb.SelectedItem = defaultDevice;
            }
        }

        private async void outDvcCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var devices = await cefAudio.Enumerate();
            var device = (AudioOutput)outDvcCb.SelectedItem;
            settings.OutputDevice = device.Label;
            settings.Store(settingsPath);
            await cefAudio.SetSink(devices.Where(x=>x.Label == device.Label).FirstOrDefault());
        }
        private async Task SetupRichPresence(bool enable)
        {
            await Task.Delay(0);
            if (enable)
            {
                if(richPresence != null)
                if (richPresence.IsInitialized) return;
                richPresence = new DiscordRpcClient("733313957435277342", logger: new DiscordRPC.Logging.NullLogger());
                UpdateRichPresence = true;
                var success = richPresence.Initialize();
                richPresence.SetPresence(new RichPresence()
                {
                    State = "0:00/0:00",
                    Details = "Listening to nothing"
                });
                if (!success) return;
                _ = Task.Run(() => DoRPCUpdate());
            }
            else
            {
                if (richPresence == null) return;
                UpdateRichPresence = false;
                richPresence.ClearPresence();
                richPresence.Deinitialize();
                richPresence = null;
            }
        }

        private async Task DoRPCUpdate()
        {
            while (UpdateRichPresence)
            {
                var songName = await cefAudio.SongName();
                var songAuthor = await cefAudio.SongAuthor();
                TimeSpan playTime = TimeSpan.FromSeconds(await cefAudio.CurrentPlaybackTime());
                TimeSpan totalPlayTime = TimeSpan.FromSeconds(await cefAudio.PlaybackTime());

                if (totalPlayTime.TotalSeconds == 0 || songName == "")
                {
                    richPresence.SetPresence(new RichPresence()
                    {
                        Assets = new Assets() { LargeImageKey = "ytm", LargeImageText = "YouTube Music", SmallImageKey = "ytm", SmallImageText = "YouTube Music"},
                        State = "0:00/0:00",
                        Details = "Listening to nothing"
                    });
                    richPresence.Invoke();
                    richPresence.UpdateClearTime();
                    await Task.Delay(3000);
                    continue;
                }

                string timeString = string.Format("{0:D2}:{1:D2}", playTime.Minutes, playTime.Seconds) + "/" + string.Format("{0:D2}:{1:D2}", totalPlayTime.Minutes, totalPlayTime.Seconds);
                richPresence.SetPresence(new RichPresence()
                {
                    Assets = new Assets() { LargeImageKey = "ytm", LargeImageText = songName, SmallImageKey = "ytm", SmallImageText = "YouTube Music" },
                    Details = songName + " - " + songAuthor,
                    State = timeString
                });

                richPresence.Invoke();
                await Task.Delay(1000);
            }
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void rpcCb_CheckedChanged(object sender, EventArgs e)
        {
            settings.EnableRichPresence = rpcCb.Checked;
            settings.Store(settingsPath);
            _ = SetupRichPresence(rpcCb.Checked);
        }
    }
}
