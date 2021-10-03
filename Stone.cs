using System;
using System.Collections.Generic;
using System.Drawing;

using System.Text;

namespace tris
{
    internal class Stone
    {
        internal static int shardCount = 4;  
        internal class Shard 
        {
            internal Shard next = null;
            internal int x = 0;
            internal int y = 0;
            internal Shard(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
        }
        Shard first = null;
        internal int Color = 1;
        internal Stone()
        {
            
        }

        internal void Build()
        {
            Color = Program.rnd.Next(1, 15);
            first = new Shard(0,0);
            Shard current = first;
            int shardSize = Program.rnd.Next(4, shardCount+1);
            for (int i = 1; i < shardSize; i++)
            {
                current.next = new Shard(current.x, current.y);
                current = current.next;
                
                bool invalid=true;
                while (invalid)
                {
                    int d = Program.rnd.Next(1, 5);
                    switch (d)
                    {
                        case (1): //up
                            current.y--;
                            break;
                        case (2): //down
                            current.y++;
                            break;
                        case (3): //left
                            current.x--;
                            break;
                        case (4): //right
                            current.x++;
                            break;
                        default:
                            break;
                    }

                    if (!AlreadyThere(current))
                    {
                        invalid = false;
                    }
                }
            }
        }

        private bool AlreadyThere(Shard current)
        {
            if (current == null) { return false; }
            Shard f = first;
            while (f!=null)
            {
                if (!f.Equals(current))
                {
                    if (f.x == current.x && f.y == current.y)
                    { return true; }
                }
                f = f.next;
            }
            return false;
        }

        private int GetRelativeDirection(Shard src, Shard next)
        {
            if (next == null) { return 0; }
            if (src.x < next.x) { return 1; } //rechts
            if (src.x > next.x) { return 2; } //links
            if (src.y < next.y) { return 3; } //unten
            if (src.y > next.y) { return 4; } //oben
            return 0;
        }

        internal Stone Translate(int x, int y)
        {
            Stone ghost = new Stone();
            ghost.Color = this.Color;
            ghost.first = new Shard(this.first.x+x, this.first.y+y);
            Shard current = first.next;
            Shard dst = ghost.first;
            while (current != null)
            {
                dst.next = new Shard(current.x+x,current.y+y);
                dst = dst.next;
                current = current.next;
            }
            return ghost;
        }

        internal Stone Rotate(int lr)
        {
            Stone ghost = new Stone();
            ghost.Color = this.Color;
            ghost.first = new Shard(this.first.x, this.first.y);
            Shard current = first.next;
            
            Shard dst = ghost.first;
            while (current != null)
            {
                dst.next = new Shard(current.x, current.y);
                if (lr == -1)
                {
                    dst.next.y *= -1;
                    int newY = dst.next.x;
                    dst.next.x = dst.next.y;
                    dst.next.y = newY;
                }
                if (lr == 1)
                {
                    dst.next.x *= -1;
                    int newY = dst.next.x;
                    dst.next.x = dst.next.y;
                    dst.next.y = newY;
                }
                dst = dst.next;
                current = current.next;
            }
            return ghost;
        }

       

        public void GetCoords(int x, int y, out Point[] shards)
        {
            List<Point> pts = new List<Point>();
            pts.Add(new Point(x, y));
            Shard curr = first;
            while (curr.next != null)
            {
                pts.Add(new Point(x + curr.next.x, y + curr.next.y));
                curr = curr.next;
            }

            shards = pts.ToArray();
        }
    }
}
