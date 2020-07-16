using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private const string CHECKPLAYER_SCRIPT = @"(function(){
                    return document.getElementsByTagName('video')[0].getDuration();
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
            object playerState = null;
            while(playerState == null)
            {
                await Task.Delay(10);
                playerState = (await cef.EvaluateScriptAsync(CHECKPLAYER_SCRIPT)).Result;
            }
            while (playerState.ToString() == "0")
            {
                await Task.Delay(5);
                playerState = (await cef.EvaluateScriptAsync(CHECKPLAYER_SCRIPT)).Result;
            }
            var result =await cef.EvaluateScriptAsync(SETSINK_SCRIPT.Replace("%SINK%", sink.DeviceId));
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
