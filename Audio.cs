using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;


namespace tris
{
    class Audio
    {
        public bool NoAudio = false;
        private Clock trackElapsed = new Clock();
        private string targetPath = "";
        private Dictionary<int, string> loadedPlugIns = new Dictionary<int, string>();
        public void Init(IntPtr win)
        {
            EvaluateTargetPath();

            if (targetPath.Length > 0)
            {
                Bass.LoadMe(targetPath);
                BassMix.LoadMe(targetPath);
                loadedPlugIns = Bass.BASS_PluginLoadDirectory(targetPath);
                Bass.BASS_Init(-1, 44100, 0, win);
                trackElapsed.interval = 50;
                trackElapsed.OnTick += new EventHandler(checkTracktick);
                trackElapsed.Start();

                string basePath = Program.resources.workingPath;
                if (basePath.Length == 0)
                {
                    foreach (string names in Program.resources.GetResourceNames())
                    {
                        string fn = Program.resources.GetResourcePath(names);
                        this.RegEvent(fn);
                    }
                }
                else
                {
                    string[] files = Directory.GetFiles(basePath);
                    foreach (string file in files)
                    {
                        this.RegEvent(file);
                    }
                }
            }
        }
        DateTime startTick = DateTime.MaxValue;
        long lastPos = 0;
        private void checkTracktick(object sender, EventArgs e)
        {
            if (currentMusicHandle == 0) { return; }
            long len=Bass.BASS_ChannelGetLength(currentMusicHandle);
            double secondsLen = Bass.BASS_ChannelBytes2Seconds(currentMusicHandle, len);
            if (secondsLen <= 120) { len = -1; }

            long current = Bass.BASS_ChannelGetPosition(currentMusicHandle);
            double seconds=Bass.BASS_ChannelBytes2Seconds(currentMusicHandle, current);
            
            TimeSpan dt = DateTime.Now.Subtract(startTick);
            bool elapsed = false;
            if (len == -1)
            {
                if (dt.TotalSeconds > 2*60)
                { 
                    elapsed = true; 
                }
            }

            if (len > -1)
            {
                if (current >= len)
                {
                    elapsed = true;
                }
            }

            if (len > -1) 
            { if (current < lastPos) { elapsed = true; } }
            lastPos = current;

            if ( elapsed )
            {
                string next=this.GetRandomTrack();
                this.SetMusic(next);
            }

        }

        private void EvaluateTargetPath()
        {
            try
            {
                if (Utils.Is64Bit)
                {
                    targetPath = Path.Combine(Application.StartupPath, "x64");
                }
                else
                {
                    targetPath = Path.Combine(Application.StartupPath, "x86");
                }

                if (!File.Exists(Path.Combine(targetPath, "bass.dll")))
                {
                    string exFile = "";
                    if (Utils.Is64Bit)
                    {
                        exFile = Program.resources.GetResourcePath("x64.bass.dll");
                    }
                    else
                    {
                        exFile = Program.resources.GetResourcePath("x86.bass.dll");
                    }
                    if (File.Exists(exFile))
                    {
                        targetPath = Path.GetDirectoryName(exFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                targetPath = "";
                NoAudio = true;
            }
        }

        public void Leave()
        {
            trackElapsed.Stop();
            if (NoAudio) { return; }

            FreeHandles();


            Bass.FreeMe();
            BassMix.FreeMe();

            if (loadedPlugIns != null)
            {
                foreach (int plugin in loadedPlugIns.Keys)
                {
                    Bass.BASS_PluginFree(plugin);
                }
            }
            Bass.BASS_Free();
        }

        private void FreeHandles()
        {
            FreeMusicHandle();
            FreeBlockHandle(ref newblock);
            FreeBlockHandle(ref lockblock);
            FreeBlockHandle(ref downblock);
            FreeBlockHandle(ref rotblock);

            foreach (int key in completed.Keys)
            {
                List<int> handles = completed[key];
                foreach (int handle in handles)
                {
                    if (handle != 0)
                    {
                        Bass.BASS_StreamFree(handle);
                    }
                }
                handles.Clear();
            }
        }

        private static void FreeBlockHandle(ref int handle)
        {
            if (handle != 0)
            {
                Bass.BASS_StreamFree(handle);
            }
            handle = 0;
        }

        List<int> lastOnes = new List<int>();
        internal string GetRandomTrack()
        {
            if( knownMusicFiles.Count==0 ){ return ""; }
            if (lastOnes.Count == knownMusicFiles.Count)
            {
                lastOnes.RemoveRange(0, lastOnes.Count - 1);
            }
            int next=Program.rnd.Next(0, knownMusicFiles.Count);
            if (knownMusicFiles.Count > 1)
            {
                while (lastOnes.Contains(next))
                {
                    next = Program.rnd.Next(0, knownMusicFiles.Count);
                }
                lastOnes.Add(next);
            }
            return knownMusicFiles[next];
        }

        int currentMusicHandle = 0;
        int currentMusicType = 0;

        Dictionary<int, int> moveHandles = new Dictionary<int, int>();
        public string CurrentMusicTitle = "";
        public void SetMusic(string fn)
        {
            CurrentMusicTitle = "";
            if (NoAudio) { return; }
            FreeMusicHandle();

            if (fn == "") { return; }

            string ext = ResourceHandler.GetExtension(fn);
            if (ext == "S3M" || ext=="XM" || ext=="MOD" )
            {
                currentMusicType = 1;
                currentMusicHandle = Bass.BASS_MusicLoad(fn, 0, 0, BASSFlag.BASS_DEFAULT| BASSFlag.BASS_MUSIC_LOOP|BASSFlag.BASS_MUSIC_PRESCAN, 44100);
            }
            else if (ext == "MP3" || ext == "WAV" || ext=="OGG")
            {
                currentMusicType = 2;
                currentMusicHandle = Bass.BASS_StreamCreateFile(fn, 0, 0, BASSFlag.BASS_DEFAULT| BASSFlag.BASS_MUSIC_LOOP |BASSFlag.BASS_MUSIC_PRESCAN);
            }
            if( currentMusicHandle!=0 )
            {
                startTick = DateTime.Now;
                lastPos = 0;
                Bass.BASS_ChannelSetAttribute(currentMusicHandle, BASSAttribute.BASS_ATTRIB_VOL, 0.6f);
                Bass.BASS_ChannelPlay(currentMusicHandle, true);
                
            }
        }

        private void FreeMusicHandle()
        {
            if (currentMusicHandle != 0)
            {
                if (currentMusicType == 1)
                {
                    Bass.BASS_ChannelStop(currentMusicHandle);
                    currentMusicHandle = 0;
                }
                else if (currentMusicType == 2)
                {
                    Bass.BASS_StreamFree(currentMusicHandle);
                    currentMusicHandle = 0;
                }
            }
        }

        List<string> knownMusicFiles = new List<string>();

        int newblock = 0;
        int lockblock = 0;
        int downblock = 0;
        int rotblock = 0;
        int lost = 0;
        public void PlayEvent(string name)
        {
            if (NoAudio) { return; }
            int handle = 0;
            switch (name)
            {
                case ("NEW"):
                    handle = newblock;
                    break;
                case ("LOCK"):
                    handle = lockblock;
                    break;
                case ("DOWN"):
                    handle = downblock;
                    break;
                case ("ROT"):
                    handle = rotblock;
                    break;
                case("LOST"):
                    handle = lost;
                    FreeMusicHandle();
                    break;
                default:
                    {
                        int level=0;
                        if (int.TryParse(name, out level))
                        {
                            if (completed.ContainsKey(level))
                            {
                                List<int> lst = this.completed[level];
                                if (lst != null && lst.Count > 0)
                                {
                                    int pos = Program.rnd.Next(0, lst.Count);
                                    handle = lst[pos];
                                }
                            }
                        }
                    }
                    break;
            }
            if (handle != 0)
            {
                Bass.BASS_ChannelPlay(handle, true);
            }
        }

        Dictionary<int, List<int>> completed = new Dictionary<int, List<int>>();

        internal void RegEvent(string fn)
        {
            if (NoAudio) { return; }
            string ext=ResourceHandler.GetExtension(fn);
            switch (ext)
            { 
                case("S3M"):
                case("MP3"):
                case("WAV"):
                case("OGG"):
                case("XM"):
                case("MOD"):
                    {
                        string fonly = Path.GetFileName(fn);
                        if (fonly.Length > 4)
                        {
                            //new,locked,moved,down,rotate
                            string cmd = fonly.Substring(0, 4);
                            switch (cmd)
                            { 
                                case("NEW_"):
                                    this.newblock = Bass.BASS_StreamCreateFile(fn, 0, 0, 0);
                                    break;
                                case("LOCK"):
                                    this.lockblock = Bass.BASS_StreamCreateFile(fn, 0, 0, 0);
                                    break;
                                case("DOWN"):
                                    this.downblock = Bass.BASS_StreamCreateFile(fn, 0, 0, 0);
                                    break;
                                case("ROT_"):
                                    this.rotblock = Bass.BASS_StreamCreateFile(fn, 0, 0, 0);
                                    break;
                                case("LOST"):
                                    this.lost = Bass.BASS_StreamCreateFile(fn, 0, 0, 0);
                                    break;
                                case ("CMP_"):
                                    {

                                        fonly = fonly.Remove(0, "CMP_".Length);
                                        string first = fonly.Substring(0, 1);
                                        int level = 0;
                                        if (int.TryParse(first, out level))
                                        {
                                            if (level >= 1)
                                            {
                                                int handle = Bass.BASS_StreamCreateFile(fn, 0, 0, 0);
                                                if (handle != 0)
                                                {
                                                    if (!completed.ContainsKey(level))
                                                    {
                                                        completed.Add(level, new List<int>());
                                                    }
                                                    completed[level].Add(handle);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    if (!knownMusicFiles.Contains(fn))
                                    {
                                        knownMusicFiles.Add(fn);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
