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
using Youtube_Music.Misc;

namespace Youtube_Music.Forms
{
    public partial class PopupWindow : Form
    {
        ChromiumWebBrowser cef;
        string lastAddress;

        public PopupWindow(string url, Size sz)
        {
            InitializeComponent();
            this.Size = sz;
            cef = new ChromiumWebBrowser(url);
            this.Controls.Add(cef);
            cef.Dock = DockStyle.Fill;
            cef.MenuHandler = new CustomContextMenu();
            cef.AddressChanged += new EventHandler<AddressChangedEventArgs>(OnAddressChanged);
            cef.TitleChanged += new EventHandler<TitleChangedEventArgs>((o, ev) => Text = ev.Title);
            
        }

        private void OnAddressChanged(object sender, AddressChangedEventArgs e)
        {
            if (!e.Address.Contains("youtube.com") || e.Address.EndsWith("youtube.com/"))
            {
                e.Browser.StopLoad();
                cef.Load(lastAddress);
            }
            lastAddress = e.Address;
        }

        private void PopupWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            cef.Dispose();
        }
    }
}
