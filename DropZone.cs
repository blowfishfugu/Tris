using System;
using System.Collections.Generic;
using System.Drawing;

using System.Text;


namespace tris
{
    public class DropZone
    {
        public int[][] area = null;
        public int width=0;
        public int height=0;
        public void InitZone(int w, int h)
        {
            movingStone = null;
            nextStone = null;
            width=w;
            height=h;
            area = new int[h][];
            for (int y = 0; y < h; y++)
            {
                ClearRow(y);
            }
        }

        public void ResizeZone(int wn, int hn)
        {
            int xmin = 0; 
            int xmax = width;
            if (width < wn) 
            {
                int dt = wn - width;
                dt /= 2;
                xmin += dt;
            }
            int[][] newArea = new int[hn][];
            for (int y = 0; y < hn; y++)
            {
                newArea[y] = new int[wn];
                for (int x = 0; x < wn; x++)
                {
                    newArea[y][x] = 0;
                    int relX=x - xmin;
                    if (relX >= 0 && relX < width)
                    {
                        if (y < height)
                        {
                            newArea[y][x] = area[y][relX];
                        }
                    }
                }
            }
            
            this.width = wn;
            this.height = hn;
            
            area = newArea;
        }

        private void ClearRow(int y)
        {
            area[y] = new int[width];
            for (int x = 0; x < width; x++)
            {
                area[y][x] = 0;
            }
        }

        internal Stone movingStone = null;
        internal Stone nextStone = null;
        public int xPos = 0;
        public int yPos = 0;
        public event EventHandler OnNewStone = null;
        public event EventHandler OnStoneLocked = null;
        public event EventHandler OnStoneMovedDown = null;
        public event EventHandler OnStoneRotated = null;

        internal void NewStone()
        {
            if (nextStone != null) 
            { 
                movingStone = nextStone; 
            }
            else 
            {
                movingStone = new Stone();
                movingStone.Build();
            }
            nextStone = new Stone();
            nextStone.Build();
            xPos = width / 2;
            yPos = height - 1;
            if (OnNewStone != null)
            {
                OnNewStone(this, new EventArgs());
            }
        }

        internal void MoveDown()
        {
            if (movingStone == null) 
            { NewStone(); }

            yPos--;
            if (Collision())
            {
                yPos++;
                LockStone();
                NewStone();
            }
            else
            {
                if (OnStoneMovedDown != null)
                {
                    this.OnStoneMovedDown(this, new EventArgs());
                }
            }
        }

        private void LockStone()
        {
            Point[] coords=null;
            movingStone.GetCoords(xPos, yPos, out coords);
            foreach (Point pt in coords)
            {
                if (pt.Y >= 0 && pt.Y < height)
                {
                    if (pt.X >= 0 && pt.X < width)
                    {
                        area[pt.Y][pt.X] = movingStone.Color;
                    }
                }
            }
            if (OnStoneLocked != null)
            {
                OnStoneLocked(this, new EventArgs());
            }
        }

        private bool Collision()
        {
            if (movingStone == null) { return false; }
            int leftEdge = xPos;
            int rightEdge = xPos;
            int bottom = yPos;
            int top = yPos;
            Point[] coords = null;
            movingStone.GetCoords(xPos, yPos, out coords);
            foreach (Point pt in coords)
            {
                if (pt.X < leftEdge) { leftEdge = pt.X; }
                if (pt.X > rightEdge) { rightEdge = pt.X; }
                if (pt.Y < bottom) { bottom = pt.Y; }
                if (pt.Y > top) { top = pt.Y; }
            }
            //Rahmen
            if (leftEdge < 0) { return true; }
            if (rightEdge >= width) { return true; }
            if (bottom < 0) { return true; }

             //andere Steine
            foreach (Point pt in coords)
            {
                if (pt.Y >= 0 && pt.Y < height)
                {
                   if (area[pt.Y][pt.X] > 0 )
                    { return true; }

                }
            }
           
            return false;
        }

        internal void RotateLeft()
        {
            movingStone = movingStone.Rotate(1);
            AlignToBorder();
            if (Collision())
            {
                movingStone = movingStone.Rotate(-1);
            }
            else
            {
                if (OnStoneRotated != null)
                {
                    OnStoneRotated(this, new EventArgs());
                }
            }
        }

        private void AlignToBorder()
        {
            Point[] coords = null;
            movingStone.GetCoords(xPos, yPos, out coords);
            int leftEdge = xPos;
            int rightEdge = xPos;
            foreach (Point pt in coords)
            {
                if (pt.X < leftEdge) { leftEdge = pt.X; }
                if (pt.X > rightEdge) { rightEdge = pt.X; }
            }
            if (leftEdge < 0)
            {
                xPos -= leftEdge;
            }
            else if (rightEdge >= width)
            {
                xPos -= (rightEdge - leftEdge);
            }
        }

        internal void RotateRight()
        {
            movingStone = movingStone.Rotate(-1);
            AlignToBorder();
            if (Collision())
            {
                movingStone = movingStone.Rotate(1);
            }
            else
            {
                if (OnStoneRotated != null)
                {
                    OnStoneRotated(this, new EventArgs());
                }
            }
        }

        internal void MoveRight()
        {
            xPos++;
            if (Collision()) { xPos--; }
            
        }

        internal void MoveLeft()
        {
            xPos--;
            if (Collision()) { xPos++; }
            
        }

        internal bool IsFull()
        {
            return Collision();
        }

        internal void RemoveCompleted()
        {
            for (int y = 0; y < (height-1); y++)
            {
                if (IsRowComplete(y))
                {
                    for (int z = y; z < (height - 1); z++)
                    {
                        area[z] = area[z + 1];
                    }
                    ClearRow(height - 1);
                    y--; //recheck an y ist jetzt reihe von eins drüber
                }
            }
        }

        internal List<int> CompletedRows()
        {
            List<int> complete = new List<int>();
            for (int y = 0; y < height; y++)
            {
                bool isComplete = IsRowComplete(y);
                if (isComplete)
                {
                    complete.Add(y);
                }
            }
            return complete;
        }

        private bool IsRowComplete(int y)
        {
            bool isComplete = true;
            for (int x = 0; x < width; x++)
            {
                if (area[y][x] == 0)
                { 
                    isComplete = false;
                    break;
                }
            }
            if (isComplete)
            {
                for (int x = 0; x < width; x++)
                {
                    area[y][x] = 1;
                }
            }
            return isComplete;
        }
    }
}
