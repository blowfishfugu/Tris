using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace tris
{

    public partial class TrisForm : Form
    {
        
        
        class KeyEvent : EventArgs
        {
            internal Keys keys;
            internal KeyEvent(Keys k)
            {
                keys = k;
            }
        }
        const int cw = 10;
        const int ch = 20;
        Keys speedKey = Keys.S;
        Keys speedKey2 = Keys.NumPad5;
        
        public TrisForm()
        {
            InitializeComponent();

           
            keyHandlers.Add(Keys.Left, new EventHandler<KeyEvent>(MoveLeft));
            keyHandlers.Add(Keys.A, new EventHandler<KeyEvent>(MoveLeft));
            keyHandlers.Add(Keys.NumPad4, new EventHandler<KeyEvent>(MoveLeft));

            keyHandlers.Add(Keys.Right, new EventHandler<KeyEvent>(MoveRight));
            keyHandlers.Add(Keys.D, new EventHandler<KeyEvent>(MoveRight));
            keyHandlers.Add(Keys.NumPad6, new EventHandler<KeyEvent>(MoveRight));
            
            keyHandlers.Add(Keys.Up, new EventHandler<KeyEvent>(RotateRight));
            keyHandlers.Add(Keys.Q, new EventHandler<KeyEvent>(RotateRight));
            keyHandlers.Add(Keys.W, new EventHandler<KeyEvent>(RotateRight));
            keyHandlers.Add(Keys.NumPad8, new EventHandler<KeyEvent>(RotateRight));

            keyHandlers.Add(Keys.Down, new EventHandler<KeyEvent>(RotateLeft));
            keyHandlers.Add(Keys.X, new EventHandler<KeyEvent>(RotateLeft));
            keyHandlers.Add(Keys.E, new EventHandler<KeyEvent>(RotateLeft));
            keyHandlers.Add(Keys.NumPad2, new EventHandler<KeyEvent>(RotateLeft));

            keyHandlers.Add(Keys.NumPad9, new EventHandler<KeyEvent>(NextTrack));
            keyHandlers.Add(Keys.M, new EventHandler<KeyEvent>(NextTrack));

            gravity.interval = 1000.0;
            gravity.OnTick += new EventHandler(MoveDown);
            gravity.OnSubTick += new EventHandler(SubProcessing);
            
            gameField.mZone = dropZone;
            dropZone.InitZone(cw, ch);
            dropZone.OnNewStone += dropZone_OnNewStone;
            dropZone.OnStoneLocked += dropZone_OnStoneLocked;
            dropZone.OnStoneMovedDown += dropZone_OnStoneMovedDown;
            dropZone.OnStoneRotated += dropZone_OnStoneRotated;
            chkSurprises.Checked = SurpriseEnabled;
            gameField.Focus();
            gameField.TriggerResize();
        }

        private void NextTrack(object sender, KeyEvent e)
        {
            this.btnMusic_Click(sender, e);
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 0x1;
            const int HTCAPTION = 0x2;

            if (m.Msg == WM_NCHITTEST)
            {
                if ((int)m.Result == HTCLIENT)
                {
                    m.Result = (IntPtr)HTCAPTION;
                }
            }
        }

        private void lblStatus_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }

        }

        void dropZone_OnStoneMovedDown(object sender, EventArgs e)
        {
            Program.audio.PlayEvent("DOWN");
        }

        void dropZone_OnStoneLocked(object sender, EventArgs e)
        {
            Program.audio.PlayEvent("LOCK");
        }

        void dropZone_OnStoneRotated(object sender, EventArgs e)
        {
            Program.audio.PlayEvent("ROT");
        }

       

        void dropZone_OnNewStone(object sender, EventArgs e)
        {
            if (stillDown.Contains(speedKey) 
                || stillDown.Contains(speedKey2)
                )
            {
                if (prevInterval != 0)
                {
                    gravity.interval = prevInterval;
                }
            }

            if (previewCanvas.mZone == null)
            {
                previewCanvas.mZone = new DropZone();
                previewCanvas.alpha = 255;
                previewCanvas.renderImage = false;
                previewCanvas.TriggerRecalibrate();
            }

            int sc = Stone.shardCount * 2;
            if (previewCanvas.mZone.width != sc)
            {
                previewCanvas.mZone.InitZone(sc, sc);
                previewCanvas.mZone.yPos = sc/2;
                previewCanvas.mZone.xPos = sc/2;
                previewCanvas.TriggerResize();
            }
            Program.audio.PlayEvent("NEW");
        }

        public bool SurpriseEnabled = true;
       
        private Clock gravity = new Clock();
        private DropZone dropZone = new DropZone();
        private Dictionary<Keys, EventHandler<KeyEvent>> keyHandlers = new Dictionary<Keys, EventHandler<KeyEvent>>();
        private bool gameRunning = false;
        List<Keys> stillDown = new List<Keys>();
        private long ranking = 0;
        private long clearedRows = 0;
        private long clearCount = 0;
        
        private bool restart = true;
        private void MoveDown(object sender, EventArgs e)
        {
            if (!gameRunning) { return; }
            if (InvokeRequired)
            {
                this.Invoke( new EventHandler(MoveDown), this, new EventArgs() ) ;
                return;
            }
            dropZone.MoveDown();
            previewCanvas.mZone.movingStone = dropZone.nextStone;
            previewCanvas.Redraw(this,new EventArgs());

            Render(dropZone);
            CheckCompletedRows();
            if (dropZone.IsFull())
            {
                GameLost();
            }
            
        }

        List<Keys> smooth = new List<Keys>();
        private void SubProcessing(object sender, EventArgs e)
        {
            if (InvokeRequired) 
            {
                this.Invoke(new EventHandler(SubProcessing), this, new EventArgs());
                return;
            }
            if (stillDown.Count > 0)
            {
                foreach (Keys k in stillDown)
                {
                    if (keyHandlers.ContainsKey(k))
                    {
                        if (smooth.Contains(k))
                        {
                            EventHandler<KeyEvent> ev = keyHandlers[k];
                            if (ev != null)
                            {
                                if (ev != RotateLeft && ev != RotateRight)
                                {
                                    ev(this, new KeyEvent(k));
                                }
                            }
                        }
                        else { smooth.Add(k); }
                    }
                }
                
            }
            Render(dropZone);
        }

        private void CheckCompletedRows()
        {
            if (!gameRunning) { return; }  
            List<int> completed = dropZone.CompletedRows();
            if (completed.Count > 0)
            {
                gameRunning = false;
                
                RenderCompleted(dropZone, completed);
                Application.DoEvents(); //force redraw
                Thread.Sleep( int.Parse(Math.Round(gravity.interval/4.0,0).ToString()));
                dropZone.RemoveCompleted();
                Render(dropZone);


                if (prevInterval != 0)
                {
                    gravity.interval = prevInterval;
                }

                long lastScore = long.Parse((Math.Pow(2.0, completed.Count+1) / 2.0).ToString());
                lastScore *= dropZone.width;
                ranking += lastScore;
                clearedRows += completed.Count;
                clearCount++;
                if ((clearCount / 10) > 0 && clearCount != 0)
                {
                    gravity.interval = gravity.interval * Math.Pow(0.97, clearCount / 10);
                    if (SurpriseEnabled)
                    {
                        if (gravity.interval < 150 ) 
                        {
                            if (dropZone.width < 30)
                            {
                                gravity.interval = 150;
                            }
                            else if (gravity.interval < 100)
                            {
                                gravity.interval = 100;
                            }
                        }
                    }
                    else
                    {
                        if (gravity.interval < 220) { gravity.interval = 220; }
                    }
                }
                if ((clearedRows / 20) > 0 && clearedRows != 0)
                {
                    if (SurpriseEnabled)
                    {
                        int newShardCount = 4 + ((int)clearedRows / 20);
                        if (newShardCount != Stone.shardCount)
                        {
                            
                            if ((newShardCount > dropZone.width / 3) && dropZone.width<30 )
                            {
                                dropZone.ResizeZone(newShardCount * 3, newShardCount * 6);
                                gameField.TriggerResize();
                            }
                            if (dropZone.height > 40 && gameField.alpha != 170)
                            {
                                gameField.alpha = 170;
                                gameField.TriggerRecalibrate();
                            }

                            int inv = Program.rnd.Next(0, 2);
                            if (inv > 0)
                            {
                                gameField.Invert();
                                previewCanvas.Invert();
                            }

                            int colswap = Program.rnd.Next(0, 2);
                            if (colswap > 0)
                            {
                                gameField.ColorSwap();
                                previewCanvas.ColorSwap();
                            }

                            int dynbg = Program.rnd.Next(0, 2);
                            gameField.renderImage = (dynbg>0);
                            previewCanvas.renderImage = (dynbg>0);
                            
                            if (newShardCount > 5)
                            {
                                Stone.shardCount = 5;
                            }
                        }
                    }
                }
                lblStatus.Text = string.Format("Pt: {0}  Timing: ({1}ms/step)  Clearedrows: {5}{2}{3}->+{4}",
                    ranking.ToString(), gravity.interval.ToString("###.0"),
                    Environment.NewLine,
                    completed.Count, lastScore.ToString(), clearedRows.ToString());
                Program.audio.PlayEvent(completed.Count.ToString());

                gameRunning = true;
               
            }
            
        }

        

        private void GameLost()
        {
            Program.audio.PlayEvent("LOST");
            gravity.Stop();
            lblStatus.Text=("Game Over: " + ranking.ToString() + " points achieved");
            btnStart.Text = ">";
            gameRunning = false;
            ranking = 0;
            clearedRows = 0;
            clearCount = 0;
            restart = true;
        }

        private void Render(DropZone dropZone)
        {
           gameField.Redraw(this, new EventArgs());
        }
        
        private void RenderCompleted(DropZone dropZone, List<int> completed)
        {
            for (int i=1;i<completed.Count;i++) 
            {
                gameField.ColorSwap();
                gameField.Redraw(this, new EventArgs());
                Application.DoEvents();
                gameField.ColorSwap();
                gameField.Redraw(this, new EventArgs());
                Application.DoEvents();
            }
            gameField.Redraw(this, new EventArgs());
        }
       
        private void RotateLeft(object sender, KeyEvent e)
        {
            if (!gameRunning) { return; }
            dropZone.RotateLeft();
            
        }

        private void RotateRight(object sender, KeyEvent e)
        {
            if (!gameRunning) { return; }
            dropZone.RotateRight();
           
        }

        private void MoveRight(object sender, KeyEvent e)
        {
            if (!gameRunning) { return; }
            dropZone.MoveRight();
           
        }

        private void MoveLeft(object sender, KeyEvent e)
        {
            if (!gameRunning) { return; }
            dropZone.MoveLeft();
           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            gameRunning = false;
            gravity.Stop();
            this.Close();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == speedKey || keyData==speedKey2 )
            {
                return true;
            }
            if (keyHandlers.ContainsKey(keyData))
            { return true; }
            return base.IsInputKey(keyData);
        }

        double prevInterval = 0.0;
        private void TrisForm_KeyDown(object sender, KeyEventArgs e)
        {
             if (stillDown.Contains(e.KeyCode)) 
                { return; }

                stillDown.Add(e.KeyCode);
                
            if (e.KeyCode == speedKey || e.KeyCode==speedKey2 )
            {
                prevInterval = gravity.interval;
                gravity.interval = 20.0;
                return;
            }

            if (keyHandlers.ContainsKey(e.KeyCode))
            {
               EventHandler<KeyEvent> ev = keyHandlers[e.KeyCode];
                if (ev != null)
                {
                    ev(this, new KeyEvent(e.KeyCode));
                    Render(dropZone);
                }
            }
        }

        private void TrisForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (smooth.Contains(e.KeyCode))
            {
                smooth.Remove(e.KeyCode);
            }
            if (stillDown.Contains(e.KeyCode))
            {
                stillDown.Remove(e.KeyCode);
            }
            if (e.KeyCode == speedKey || e.KeyCode==speedKey2 )
            {
                if (prevInterval != 0)
                {
                    gravity.interval = prevInterval;
                    prevInterval = 0;
                }
            }
            
        }
       
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!gameRunning) 
            {
                if (restart) 
                {
                    SurpriseEnabled = chkSurprises.Checked;
                    this.BackColor = Color.Black;
                    this.ForeColor = Color.White;
                    gameField.alpha = 255;
                    if (SurpriseEnabled)
                    { gameField.alpha = 220; }

                    Stone.shardCount = 4;
                    dropZone.InitZone(cw, ch);
                    gameField.TriggerRecalibrate();
                    previewCanvas.TriggerRecalibrate();
                    restart = false;

                    this.btnMusic_Click(this, new EventArgs());

                }
                gravity.Start(); 
                btnStart.Text = "||"; 
                Render(dropZone); 
            }
            else
            {
                gravity.Stop(); btnStart.Text = ">"; 
                
            }
            gameRunning = !gameRunning;
            
        }

        private void lblStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (IsInputKey(e.KeyCode))
            {
                e.IsInputKey = true;
            }
        }

        private void btnMusic_Click(object sender, EventArgs e)
        {
            string track = Program.audio.GetRandomTrack();
            if (chkOff.Checked) { track = ""; }
            Program.audio.SetMusic(track);
        }

        private void chkOff_CheckedChanged(object sender, EventArgs e)
        {
            btnMusic_Click(sender, e);
        }

        
       

        
    }
}
