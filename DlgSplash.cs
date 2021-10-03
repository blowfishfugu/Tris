using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace tris
{
    public partial class DlgSplash : Form
    {
        public DlgSplash()
        {
            InitializeComponent();
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, 0);
            progressCanvas.mZone = new DropZone();
            progressCanvas.mZone.InitZone(37, 19);
            progressCanvas.renderImage = false;
            progressCanvas.TriggerResize();
        }

        internal void Extracted(object sender, ResourceHandler.ResArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new ResourceHandler.ResourceExtractHandler(Extracted), sender, e);
                return;
            }

            double i = e.currentCount;
            double n = e.totalCount;
            double factor = i / n;
            double criticalHeight = (double)progressCanvas.mZone.height * factor;
            int ymax = int.Parse(Math.Round(criticalHeight, 0).ToString());
            for (int y = 0; y < progressCanvas.mZone.height; y++)
            {
                for (int x = 0; x < progressCanvas.mZone.width; x++)
                {
                    if (y > ymax)
                    {
                        progressCanvas.mZone.area[y][x] = 0;
                    }
                    else
                    {
                        if (progressCanvas.mZone.area[y][x] == 0)
                        {
                            progressCanvas.mZone.area[y][x] = Program.rnd.Next(0, 16);
                        }
                    }
                }
            }
            progressCanvas.Redraw(this, new EventArgs());
        }

        internal void AllExtracted(object sender, EventArgs e)
        {
            if (InvokeRequired) { this.Invoke(new EventHandler(AllExtracted), sender, e); return; }

            List<int> completed = progressCanvas.mZone.CompletedRows();
            if (completed.Count > 0)
            {
                
                progressCanvas.Redraw(this, new EventArgs());
                Application.DoEvents(); //force redraw
                Thread.Sleep(100);
                progressCanvas.mZone.RemoveCompleted();
                progressCanvas.ColorSwap();
                progressCanvas.Redraw(this, new EventArgs());
                Application.DoEvents(); //force redraw
                Thread.Sleep(100);
                
            }
        }
    }
}
