using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;

using System.Text;

using System.Windows.Forms;

namespace tris
{
    public partial class BlockCanvas : Control
    {
        Brush back = Brushes.Black;
        Brush front = Brushes.SteelBlue;
        Pen gridPen = null;
        Pen[] blockPens = null;
        Brush[] blockBrushs = null;

        Color backColor = Color.White;
        Color foreColor = Color.Black;
        Bitmap bgImage = null;

        public DropZone mZone = null;
        public int alpha = 210;
           
        public BlockCanvas()
        {
            InitializeComponent();
            InitMirrorMatrix();

            gridPen = new Pen(front, 1.0f/200.0f);
            InitPensAndBrushs();
        }

        private void InitPensAndBrushs()
        {
            blockPens = new Pen[16];
            blockBrushs = new Brush[16];

            blockBrushs[0] = new SolidBrush(Color.FromArgb(alpha, Color.White));

            blockBrushs[1] = new SolidBrush(Color.FromArgb(220, Color.Red));
            blockBrushs[2] = new SolidBrush(Color.FromArgb(220, Color.Blue));
            blockBrushs[3] = new SolidBrush(Color.FromArgb(220, Color.Green));
            blockBrushs[4] = new SolidBrush(Color.FromArgb(220, Color.Purple));
            blockBrushs[5] = new SolidBrush(Color.FromArgb(220, Color.Yellow));
            blockBrushs[6] = new SolidBrush(Color.FromArgb(220, Color.DeepPink));
            blockBrushs[7] = new SolidBrush(Color.FromArgb(220, Color.YellowGreen));
            blockBrushs[8] = new SolidBrush(Color.FromArgb(255, Color.DarkBlue));
            blockBrushs[9] = new SolidBrush(Color.FromArgb(255, Color.DarkMagenta));
            blockBrushs[10] = new SolidBrush(Color.FromArgb(255, Color.Red));
            blockBrushs[11] = new SolidBrush(Color.FromArgb(255, Color.Green));
            blockBrushs[12] = new SolidBrush(Color.FromArgb(255, Color.Turquoise));
            blockBrushs[13] = new SolidBrush(Color.FromArgb(255, Color.Yellow));
            blockBrushs[14] = new SolidBrush(Color.FromArgb(255, Color.Green));

            blockBrushs[15] = new SolidBrush(Color.FromArgb(55, Color.Black));

            for (int i = 0; i < blockPens.Length; i++)
            {
                blockPens[i] = new Pen(blockBrushs[blockBrushs.Length - i - 1], gridPen.Width);
            }
        }

        bool whiteback = true;
        public void ColorSwap()
        {
            whiteback = !whiteback;
            if (whiteback)
            {
                blockBrushs[0] = new SolidBrush(Color.FromArgb(alpha, Color.White));
                blockBrushs[15] = new SolidBrush(Color.FromArgb(55, Color.Black));
                backColor=Color.FromArgb(alpha, Color.White);
                foreColor = Color.FromArgb(alpha, Color.Black);
            }
            else
            {
                blockBrushs[0] = new SolidBrush(Color.FromArgb(alpha,Color.Black));
                blockBrushs[15] = new SolidBrush(Color.FromArgb(55, Color.White));
                backColor = Color.FromArgb(alpha, Color.Black);
                foreColor = Color.FromArgb(alpha, Color.White);
            }
            bgImage = null;
        }

        Bitmap b = null;
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (b != null)
            { 
                pevent.Graphics.DrawImage(b, this.ClientRectangle); 
            }
            else
            {
                OnPaint(pevent);
            }
        }

        public bool renderImage = false;
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics dst = pe.Graphics;
            if (b == null)
            {
                b = new Bitmap(this.Width + 2, this.Height + 2);
            }

            if (bgImage == null)
            {
                if (renderImage || DesignMode)
                {
                    this.bgImage = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                    Graphics bg = Graphics.FromImage(this.bgImage);
                    Point topleft = new Point(ClientRectangle.Left, ClientRectangle.Top);
                    Point bottomleft = new Point(ClientRectangle.Left, Bottom);
                    Brush gradient = new LinearGradientBrush(topleft, bottomleft, backColor, foreColor);
                    bg.FillRectangle(gradient, ClientRectangle);
                }
            }

            Graphics g = Graphics.FromImage(b);
            
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            if (bgImage!=null)
            {
                g.DrawImage(bgImage, this.ClientRectangle);
            }
            
            g.DrawRectangle(gridPen, this.ClientRectangle);

            

            CalcViewMatrix();
            g.MultiplyTransform(viewMatrix);
            int zHeight = 20;
            int zWidth = 10;
            if (mZone != null)
            {
                zHeight = mZone.height;
                zWidth = mZone.width;
            }
            
            if (mZone != null)
            {
                for (int y = 0; y < mZone.height; y++)
                {
                    for (int x = 0; x < mZone.width; x++)
                    {
                        int color = mZone.area[y][x];
                        if (!renderImage || color > 0)
                        {
                            g.FillRectangle(blockBrushs[color], new Rectangle(x, y, 1, 1));
                        }
                        g.DrawRectangle(blockPens[color], new Rectangle(x, y, 1, 1));
                    }
                }

               DrawCurrentBlock(mZone, g);
            }
            
            dst.DrawImage(b, this.ClientRectangle);
        }

        private void DrawCurrentBlock(DropZone mZone, Graphics g)
        {
            if (mZone == null) { return; }
            if (mZone.movingStone == null) { return; }
            Stone st = mZone.movingStone;
            Point[] coords = null;
            st.GetCoords(mZone.xPos, mZone.yPos, out coords);
            foreach (Point pt in coords)
            {
                g.FillRectangle(blockBrushs[st.Color], new Rectangle(pt.X, pt.Y, 1, 1));
                g.DrawRectangle(blockPens[st.Color], new Rectangle(pt.X, pt.Y, 1, 1));
            }

            int leftEdge = mZone.xPos;
            int rightEdge = mZone.xPos;
            foreach (Point pt in coords)
            {
                if (pt.X < leftEdge) { leftEdge = pt.X; }
                if (pt.X > rightEdge) { rightEdge = pt.X; }
            }

            g.DrawLine(blockPens[5], new Point(leftEdge, 0), new Point(leftEdge, mZone.yPos));
            g.DrawLine(blockPens[5], new Point(rightEdge+1, 0), new Point(rightEdge+1, mZone.yPos));

        }

        Matrix mirrorMatrix = new Matrix();
        Matrix viewMatrix = null;
        private void InitMirrorMatrix()
        {
            mirrorMatrix = new Matrix(   1.0f, 0.0f, 0.0f,
                                        -1.0f, 0.0f, 0.0f); 
        }

        public void TriggerResize()
        {
            this.OnSizeChanged(new EventArgs());
        }

        public void TriggerRecalibrate()
        {
            InitPensAndBrushs();
            this.OnSizeChanged(new EventArgs());
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            b = null;
            bgImage = null;
            viewMatrix = null;
            this.Invalidate();
        }

        bool inverted = false;
        public void Invert()
        {
            inverted = !inverted;
            TriggerResize();
        }


        private Matrix CalcViewMatrix()
        {
            float zHeight = 20.0f;
            float zWidth = 10.0f;
            if (mZone != null)
            {
                zHeight = mZone.height;
                zWidth = mZone.width;
            }
            if (viewMatrix == null)
            {
                viewMatrix = new Matrix();
                viewMatrix.Scale(this.Width / zWidth, this.Height / zHeight);

                if (!inverted)
                {
                    viewMatrix.Multiply(mirrorMatrix);
                    viewMatrix.Translate(0.0f, -(zHeight));
                }
            }
            return viewMatrix;
        }

        internal void Redraw(object trisForm, EventArgs eventArgs)
        {
            if (this.InvokeRequired)
            {
                this.Invoke( new EventHandler(Redraw), this, eventArgs);
                return;
            }
            this.Invalidate();
        }

    }
}
