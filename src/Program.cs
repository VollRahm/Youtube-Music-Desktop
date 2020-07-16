using System;
using System.Windows.Forms;
using Youtube_Music.Forms;

namespace Youtube_Music
{
    static class Program
    {
        /// <summary>
        /// The entry point of the application
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
