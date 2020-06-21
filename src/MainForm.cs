using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Youtube_Music
{
    public partial class MainForm : Form
    {
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        ChromiumWebBrowser cef;
        string lastAddress;

        public MainForm()
        {
            InitializeComponent();
            //AllocConsole();
            CefSettings settings = new CefSettings();
            settings.UserAgent = "VollRahms YT Music for Desktop";
            settings.CefCommandLineArgs.Add("--user-agent", "Mozilla/5.0 Gecko/20100101 Firefox/70.0");
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            settings.CachePath = "cache";
            Cef.Initialize(settings);
            cef = new ChromiumWebBrowser("https://music.youtube.com/");
            this.Controls.Add(cef);
            cef.Dock = DockStyle.Fill;
            cef.MenuHandler = new CustomContextMenu();
            cef.AddressChanged += new EventHandler<AddressChangedEventArgs>(OnAddressChanged);

            waveOutGetVolume(IntPtr.Zero, out uint CurrVol);
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            volumeTrackBar.Value = CalcVol / (ushort.MaxValue / 100);
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
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }
    }
}
