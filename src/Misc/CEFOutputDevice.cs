using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Youtube_Music.Forms;

namespace Youtube_Music.Misc
{
    public class CEFOutputDevice
    {
        public ChromiumWebBrowser cef;

        private static bool RecievedCallback = false;
        private static dynamic CallbackData = null;

        public static async void EvaluateCallback(dynamic data)
        {
            while (CallbackData != null) await Task.Delay(10);
            CallbackData = data;
            RecievedCallback = true;
        }

        private const string ENUMERATE_SCRIPT = @"(async function()
                        {	
	                    await CefSharp.BindObjectAsync('jscallback', 'jscallback');
                        var devices = await navigator.mediaDevices.enumerateDevices();
	                    await jscallback.send(devices);
                        })();";

        private const string SETSINK_SCRIPT = @"
            (async function(){
                var ytmPlayer = document.getElementsByTagName('video')[0];
                await ytmPlayer.setSinkId('%SINK%');
            })();";

        private const string PLAYERTIME_SCRIPT = @"(function(){
                    return document.getElementsByTagName('video')[0].getDuration();
            })();";

        private const string CURRENTPLAYERTIME_SCRIPT = @"(function(){
                    return document.getElementsByTagName('video')[0].getCurrentTime();
            })();";

        private const string SONGNAME_SCRIPT = @"(function(){
                    return document.getElementsByClassName('title style-scope ytmusic-player-bar')[0].innerHTML;
            })();";

        private const string AUTHOR_SCRIPT = @"(function(){
                    return document.getElementsByClassName('byline style-scope ytmusic-player-bar complex-string')[0].children[0].innerHTML;
            })();";

        private const string SETVOLUME_SCRIPT = @"(function(){
                   document.getElementById('volume-slider').value = %%VALUE%%;
            })();";

        private const string GETVOLUME_SCRIPT = @"(function(){
                   return document.getElementById('volume-slider').value;
            })();";

        public CEFOutputDevice(ChromiumWebBrowser cef)
        {
            this.cef = cef;
        }

        public async Task<IReadOnlyList<AudioOutput>> Enumerate()
        {
            while (!cef.CanExecuteJavascriptInMainFrame) await Task.Delay(10);
            try { cef.JavascriptObjectRepository.Register("jscallback", new JSAsyncCallback(), true, new BindingOptions() { CamelCaseJavascriptNames = false }); } catch { }
            cef.ExecuteScriptAsync(ENUMERATE_SCRIPT);
            while (!RecievedCallback) await Task.Delay(2);
            var data = CallbackData;
            CallbackData = null;
            RecievedCallback = false;

            List<AudioOutput> devices = new List<AudioOutput>();
            foreach(dynamic d in data)
            {
                if(d.kind == "audiooutput")
                {
                    devices.Add(new AudioOutput(d.deviceId, d.label, d.groupId));
                }
            }

            return devices.AsReadOnly();
        }

        public async Task SetSink(AudioOutput sink)
        {
            while (await PlaybackTime() == 0)
                await Task.Delay(5);

            await cef.EvaluateScriptAsync(SETSINK_SCRIPT.Replace("%SINK%", sink.DeviceId));
        }

        public async Task<float> PlaybackTime()
        {
            object playerState = (await cef.EvaluateScriptAsync(PLAYERTIME_SCRIPT)).Result;
            if (playerState == null) return 0;
            if (float.TryParse(playerState.ToString(), out float time))
                return time;
            else return 0;
        }

        public async Task<float> CurrentPlaybackTime()
        {
            object playerState = (await cef.EvaluateScriptAsync(CURRENTPLAYERTIME_SCRIPT)).Result;
            if (playerState == null) return 0;
            if (float.TryParse(playerState.ToString(), out float time))
                return time;
            else return 0;
        }

        public async Task<string> SongName()
        {
            if (await PlaybackTime() == 0) return "";
            object playerState = (await cef.EvaluateScriptAsync(SONGNAME_SCRIPT)).Result;
            if (playerState == null) return "";
            return playerState.ToString();
        }

        public async Task<string> SongAuthor()
        {
            if (await PlaybackTime() == 0) return "";
            object playerState = (await cef.EvaluateScriptAsync(AUTHOR_SCRIPT)).Result;
            if (playerState == null) return "";
            return playerState.ToString();
        }

        public async Task SetVolume(int volume)
        {
            await cef.EvaluateScriptAsync(SETVOLUME_SCRIPT.Replace("%%VALUE%%", volume.ToString()));
        }

        public async Task<int> GetVolume()
        {
            object playerState = (await cef.EvaluateScriptAsync(GETVOLUME_SCRIPT)).Result;
            if (playerState == null) return -1;
            if (int.TryParse(playerState.ToString(), out int time))
                return time;
            else return -1;
        }
        
    }

    public class AudioOutput
    {
        public string DeviceId;
        public string Label;
        public string GroupId;

        public AudioOutput(string deviceId, string label, string groupId)
        {
            DeviceId = deviceId;
            Label = label;
            GroupId = groupId;
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public class JSAsyncCallback
    {
        public void send(dynamic data)
        {
            CEFOutputDevice.EvaluateCallback(data);
        }
    }
}
