using CefSharp;
using CefSharp.WinForms;
using System;
using System.IO;
using System.Windows.Forms;
using Youtube_Music.Misc;

namespace Youtube_Music.Forms
{
    public partial class MainForm : Form
    {
        ChromiumWebBrowser cef;
        string lastAddress;
        SettingsForm settings;

        public MainForm()
        {
            InitializeComponent();
            SetupChrome();
            settings = new SettingsForm(cef);
            Native.waveOutGetVolume(IntPtr.Zero, out uint CurrVol);
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            volumeTrackBar.Value = CalcVol / (ushort.MaxValue / 100);
        }

        private void SetupChrome()
        {
            CefSettings settings = new CefSettings();
            settings.CefCommandLineArgs.Add("--user-agent", "Mozilla/5.0 Gecko/20100101 Firefox/70.0");
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CachePath = new DirectoryInfo("cache").FullName;
            Cef.Initialize(settings);
            cef = new ChromiumWebBrowser("https://music.youtube.com/");
            this.Controls.Add(cef);
            cef.Dock = DockStyle.Fill;
            cef.MenuHandler = new CustomContextMenu();
            cef.AddressChanged += new EventHandler<AddressChangedEventArgs>(OnAddressChanged);
            cef.LifeSpanHandler = new CustomPopupHandler();
        }

        private void OnAddressChanged(object sender, AddressChangedEventArgs e)
        {
            if (!e.Address.Contains("music.youtube.com"))
            {
                e.Browser.StopLoad();
                cef.Load(lastAddress);
            }
            lastAddress = e.Address;
        }

        private void volumeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int NewVolume = ((ushort.MaxValue / 100) * volumeTrackBar.Value);
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            Native.waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }

        bool firstTime = true;
        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            settings.Show();
            if(!firstTime)
            settings.SettingsForm_Shown(this, null);
            firstTime = false;
        }
    }
}
