using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace tris
{
    class ResourceHandler
    {
        public class ResArgs : EventArgs
        {
            public string key = "";
            public string fullFile = "";
            public int currentCount = 0;
            public int totalCount = 0;
            public ResArgs(string k, string full, int i, int total)
            {
                key = k;
                fullFile = full;
                currentCount = i;
                totalCount = total;
            }
        }
        public delegate void ResourceExtractHandler(object sender, ResArgs e);
        public event ResourceExtractHandler OnExtracted;
        public event EventHandler OnAllExtracted;

        Dictionary<string, string> extracted = new Dictionary<string, string>();
        public string workingPath = "";
        internal void Prepare()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            workingPath = Path.Combine(appdata, "Tris");
            try
            {
                Directory.CreateDirectory(workingPath);
            }
            catch (Exception exc)
            {
                Trace.WriteLine(exc.ToString());
            }

            Assembly a = Assembly.GetAssembly(typeof(tris.ResourceHandler));
            string[] names = a.GetManifestResourceNames();
            int nCount = names.Length;
            int nCurrent = 0;
            foreach (string name in names)
            {
                string resName=Path.Combine(workingPath,name);
                string ext = GetExtension(resName);
                if (ext == "RESOURCES") //kein Manuellstream
                {
                    nCount--;
                    continue;
                }
                
                using (Stream fs = a.GetManifestResourceStream(name))
                {
                    if (fs != null)
                    {
                        DeleteFile(resName);
                        using (FileStream outs = new FileStream(resName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            byte[] buffer = new byte[512];
                            int readsize = fs.Read(buffer, 0, buffer.Length);
                            while (readsize > 0)
                            {
                                outs.Write(buffer, 0, readsize);
                                readsize = fs.Read(buffer, 0, buffer.Length);
                            }
                            outs.Close();
                        }
                        fs.Close();



                        if (ext == "ZIP")
                        {
                            if (File.Exists(resName))
                            {
                                ZipLib zip = new ZipLib();
                                List<string> files = new List<string>();
                                zip.DecompressFile(resName, workingPath, ref files);
                                DeleteFile(resName);
                                nCount += files.Count;
                                foreach (string fn in files)
                                {
                                    string resKey = fn.Replace(workingPath, "");
                                    resKey = resKey.Replace("\\", ".").Trim('.');
                                    resKey = resKey.ToLower();
                                    extracted.Add(resKey, fn);
                                    nCurrent++;
                                    OnExtractedResource(nCount, nCurrent, fn);
                                }
                            }
                        }
                        else
                        {
                            string resKey = resName.Replace(workingPath, "");
                            resKey = resKey.Replace("\\", ".").Trim('.');
                            resKey = resKey.ToLower();
                            extracted.Add(resKey, resName);
                            nCurrent++;
                            OnExtractedResource(nCount, nCurrent, resName);
                        }
                    }
                }

                
            }
            if (OnAllExtracted != null) { OnAllExtracted(this, new EventArgs()); }
        }

        private void OnExtractedResource(int nCount, int nCurrent, string resName)
        {
            if (OnExtracted != null)
            {
                try
                {
                    OnExtracted(this, new ResArgs(Path.GetFileName(resName), resName, nCurrent, nCount));
                }
                catch (Exception exc)
                {
                    Trace.WriteLine(exc.ToString());
                }
            }
        }

        internal static string GetExtension(string resName)
        {
            string ext = Path.GetExtension(resName).Trim('.').ToUpper();
            return ext;
        }

        

        internal string GetResourcePath(string resourceName)
        {
            resourceName = resourceName.ToLower();
            if (extracted.ContainsKey(resourceName))
            { return extracted[resourceName]; }
            return "";
        }

        internal List<string> GetResourceNames()
        {
            List<string> lst=new List<string>( extracted.Keys );
            return lst;
        }

        internal void Free()
        {
            foreach (string key in extracted.Keys)
            {
                string fn = extracted[key];
                DeleteFile(fn);
            }
        }

        private static void DeleteFile(string fn)
        {
            try
            {
                string ext=GetExtension(fn);
                if (fn == "XML") //UserSettings, Highscorefile.. behalten
                { return; }
                if (File.Exists(fn))
                {
                    File.SetAttributes(fn, FileAttributes.Normal);
                    File.Delete(fn);
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(exc.ToString());
            }
        }



        private class ZipLib
        {

            private int mCompressionLevel = 6;

            public int CompressionLevel
            {
                get { return mCompressionLevel; }
                set { mCompressionLevel = value; }
            }

            /// <summary>
            /// Diese Funktion komprimiert alle Dateien in einem Ordner
            /// </summary>
            /// <param name="InputDir">Der Ordner der komprimiert werden soll</param>
            /// <param name="FileName">Gibt den Namen an nach dem die ZIP Datei benannt werden soll</param>
            /// <param name="OutputDir">Gibt das Ziel für die ZIP Datei an. Wenn kein Ziel übergeben wurde wird die Datei im Parent Ordner erstellt</param>
            public void CompressDirectory(string InputDir, string FileName, string OutputDir)
            {
                List<string> Files = new List<string>();
                string RelativePath = null;
                GetAllFiles(InputDir, ref Files);

                if (string.IsNullOrEmpty(OutputDir)) OutputDir = Path.GetDirectoryName(InputDir);
                if (Directory.Exists(OutputDir) == false) Directory.CreateDirectory(OutputDir);

                FileStream ZFS = new FileStream(OutputDir + "\\" + FileName, FileMode.Create);
                ICSharpCode.SharpZipLib.Zip.ZipOutputStream ZOut = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ZFS);

                ZOut.SetLevel(6);

                ICSharpCode.SharpZipLib.Zip.ZipEntry ZipEntry = default(ICSharpCode.SharpZipLib.Zip.ZipEntry);

                byte[] Buffer = new byte[4097];
                int ByteLen = 0;
                FileStream FS = null;

                int ParentDirLen = InputDir.Length + 1;
                for (int i = 0; i <= Files.Count - 1; i++)
                {
                    //Relativen Pfad für die Zip Datei erstellen
                    RelativePath = Files[i].Substring(ParentDirLen);

                    //ZipEntry erstellen
                    ZipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(RelativePath);
                    ZipEntry.DateTime = System.DateTime.Now;

                    //Eintrag hinzufügen
                    ZOut.PutNextEntry(ZipEntry);

                    //Datei in den Stream schreiben
                    FS = new FileStream(Files[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                    do
                    {
                        ByteLen = FS.Read(Buffer, 0, Buffer.Length);
                        ZOut.Write(Buffer, 0, ByteLen);
                    }
                    while (!(ByteLen <= 0));
                    FS.Close();
                }

                ZOut.Finish();
                ZOut.Close();
                ZFS.Close();
            }


            /// <summary>
            /// Diese Funktion komprimiert alle angegebenen Dateien die aus einem Ordner stammen und nicht aus unterschiedlichen Ordnern.
            /// Das hat den zweck falls man einen Ordner komprimieren will jedoch nicht mit allen Dateien sondern nur mit bestimmten Dateien.
            /// Dadurch bleibt die Ordnerstruktur erhalten wenn das Archiv erstellt wurde. Im Gegenstatz zur Funktion "CompressFiles" die keine Ordnerstrukuren erstellt
            /// </summary>
            /// <param name="InputFiles">Die Dateien die komprimiert werden sollen</param>
            /// <param name="FileName">Gibt den Namen an nach dem die ZIP Datei benannt werden soll</param>
            /// <param name="OutputDir">Gibt das Ziel für die ZIP Datei an. Wenn kein Ziel übergeben wurde wird die Datei im Parent Ordner erstellt</param>
            public void CompressDirectory(List<string> InputFiles, string FileName, string OutputDir)
            {
                string RelativePath = null;

                if (Directory.Exists(OutputDir) == false) Directory.CreateDirectory(OutputDir);

                FileStream ZFS = new FileStream(OutputDir + "\\" + FileName, FileMode.Create);
                ICSharpCode.SharpZipLib.Zip.ZipOutputStream ZOut = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ZFS);

                ZOut.SetLevel(6);

                ICSharpCode.SharpZipLib.Zip.ZipEntry ZipEntry = default(ICSharpCode.SharpZipLib.Zip.ZipEntry);

                byte[] Buffer = new byte[4097];
                int ByteLen = 0;
                FileStream FS = null;

                int ParentDirLen = Path.GetDirectoryName(InputFiles[0]).Length;
                for (int i = 0; i <= InputFiles.Count - 1; i++)
                {
                    //Relativen Pfad für die Zip Datei erstellen
                    RelativePath = InputFiles[i].Substring(ParentDirLen);

                    //ZipEntry erstellen
                    ZipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(RelativePath);
                    ZipEntry.DateTime = System.DateTime.Now;

                    //Eintrag hinzufügen
                    ZOut.PutNextEntry(ZipEntry);

                    //Datei in den Stream schreiben
                    FS = new FileStream(InputFiles[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                    do
                    {
                        ByteLen = FS.Read(Buffer, 0, Buffer.Length);
                        ZOut.Write(Buffer, 0, ByteLen);
                    }
                    while (!(ByteLen <= 0));
                    FS.Close();
                }

                ZOut.Finish();
                ZOut.Close();
                ZFS.Close();
            }

            /// <summary>
            /// Diese Funktion komprimiert Dateien zu einem ZIP-Archiv.
            /// </summary>
            /// <param name="InputFiles">Die Liste mit Dateien die komprimiert werden soll.</param>
            /// <param name="FileName">Der Dateiname der ZIP-Datei (ohne Pfad).</param>
            /// <param name="OutputDir">Das Ausgabeverzeichnis wo die ZIP Datei gespeichert werden soll.</param>
            /// <remarks></remarks>
            public void CompressFiles(List<string> InputFiles, string FileName, string OutputDir)
            {
                FileStream ZFS = new FileStream(OutputDir + "\\" + FileName, FileMode.Create);
                ICSharpCode.SharpZipLib.Zip.ZipOutputStream ZOut = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ZFS);

                ZOut.SetLevel(6);

                ICSharpCode.SharpZipLib.Zip.ZipEntry ZipEntry = default(ICSharpCode.SharpZipLib.Zip.ZipEntry);

                byte[] Buffer = new byte[4097];
                int ByteLen = 0;
                FileStream FS = null;


                for (int i = 0; i <= InputFiles.Count - 1; i++)
                {
                    //ZipEntry erstellen
                    ZipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(Path.GetFileName(InputFiles[i]));
                    ZipEntry.DateTime = System.DateTime.Now;

                    //Eintrag hinzufügen
                    ZOut.PutNextEntry(ZipEntry);

                    //Datei in den Stream schreiben
                    FS = new FileStream(InputFiles[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                    do
                    {
                        ByteLen = FS.Read(Buffer, 0, Buffer.Length);
                        ZOut.Write(Buffer, 0, ByteLen);
                    }
                    while (!(ByteLen <= 0));
                    FS.Close();
                }

                ZOut.Finish();
                ZOut.Close();
                ZFS.Close();
            }

            /// <summary>
            /// Diese Funktion dekomprimiert eine ZIP-Datei.
            /// </summary>
            /// <param name="FileName">Die Datei die dekomprimiert werden soll.</param>
            /// <param name="OutputDir">Das Verzeichnis in dem die Dateien dekomprimiert werden sollen.</param>
            public void DecompressFile(string FileName, string OutputDir,ref List<string> outputFiles)
            {
                if (!File.Exists(FileName)) { return; }
                FileStream ZFS = new FileStream(FileName, FileMode.Open);
                ICSharpCode.SharpZipLib.Zip.ZipInputStream ZIN = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(ZFS);
                
                ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = ZIN.GetNextEntry();
                while( zipEntry!=null )
                {
                    string inZipDirName = Path.GetDirectoryName(zipEntry.Name);
                    string inZipFileName = Path.GetFileName(zipEntry.Name);
                    
                    string absoluteOutDir = Path.Combine(OutputDir, inZipDirName);
                    if (!Directory.Exists(absoluteOutDir))
                    {
                        Directory.CreateDirectory(absoluteOutDir);
                    }
                    if (!ZIN.CanDecompressEntry)
                    {
                        Trace.WriteLine("Nicht entpackbar..");
                        return;
                    }
                    if (zipEntry.IsDirectory)
                    {
                        zipEntry = ZIN.GetNextEntry();
                        continue; 
                    }

                    string targetFileName = Path.Combine( absoluteOutDir ,inZipFileName );
                    if (inZipFileName.Length > 0)
                    {
                        byte[] Buffer = new byte[4097];
                        int ByteLen = 0;
                        FileStream FS = new FileStream(targetFileName, FileMode.Create);
                        do
                        {
                            ByteLen = ZIN.Read(Buffer, 0, Buffer.Length);
                            FS.Write(Buffer, 0, ByteLen);
                        }
                        while (!(ByteLen <= 0));
                        FS.Close();
                        outputFiles.Add(targetFileName);
                    }
                    zipEntry = ZIN.GetNextEntry();
                }
                ZIN.Close();
                ZFS.Close();
            }

            private void GetAllFiles(string Root, ref List<string> FileArray)
            {
                try
                {
                    string[] Files = System.IO.Directory.GetFiles(Root);
                    string[] Folders = System.IO.Directory.GetDirectories(Root);

                    for (int i = 0; i <= Files.Length - 1; i++)
                    {
                        FileArray.Add(Files[i].ToString());
                    }

                    for (int i = 0; i <= Folders.Length - 1; i++)
                    {
                        GetAllFiles(Folders[i], ref FileArray);
                    }
                }
                catch (Exception Ex)
                {
                    Trace.WriteLine(Ex);
                }
            }
        }

    }
}
