using CefSharp;

namespace Youtube_Music.Misc
{
    public class CustomContextMenu : CefSharp.IContextMenuHandler
    {
        private readonly int GoToHome = 2175;

        public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
            model.AddItem((CefMenuCommand)GoToHome, "Go to Home Page");
        }

        public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if((int)commandId == GoToHome)
            {
                chromiumWebBrowser.Load("https://music.youtube.com");
            }
            return true;
        }

        public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
            
        }

        public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }

        
    }
}
