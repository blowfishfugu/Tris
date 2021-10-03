using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace tris
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(params string[] cmdline)
        {
            try
            {
                 try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    //Application.Run(new TrisForm());
                    new MyApp().Run(cmdline);
                }
                catch (Exception ex1)
                {
                    Trace.WriteLine(ex1);
                }
               
                audio.Leave();
                resources.Free();
               
            }
            catch (Exception exc)
            {
                Trace.WriteLine(exc.ToString());
            }
            
        }

        public static Audio audio = new Audio();
        public static ResourceHandler resources = new ResourceHandler();
        public static Random rnd = new Random((int)DateTime.Now.Ticks);

        public class MyApp : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
        {
            protected override void OnCreateSplashScreen()
            {
                DlgSplash splash = new DlgSplash();
                resources.OnExtracted += splash.Extracted;
                resources.OnAllExtracted += splash.AllExtracted;
                this.SplashScreen = splash;
            }

            

            protected override void OnCreateMainForm()
            {
                resources.Prepare();
                audio.Init(IntPtr.Zero);
              
                this.MainForm = new TrisForm();
            }
        }
    }
}
