﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FastBitmapLib;


namespace _3DModelViewer
{
    public class Camera
    {
        public Point3 Position = new Point3(3,3,3);

        public string DisplayName => $"Camera (X:{Position.X.ToString("f2")}, Y:{Position.Y.ToString("f2")}, Z:{Position.Z.ToString("f2")})";

        public Point3 Target = new Point3();

        public int FOV = 60;

        public Vector4 Normal
        {
            get
            {
                return Vector4.Normalize(Target.V - Position.V);
            }
        }

        public Vector4 Tangent
        {
            get
            {
                return Vector4.Normalize(Vector.Cross(Normal, Vector4.UnitY));                
            }
        }

        public Vector4 Binormal
        {
            get
            {
                return Vector4.Normalize(Vector.Cross(Normal, Tangent));
            }
        }

        public Camera() { }

        public Camera(Point3 pos)
        {
            Position = pos;
        }
        public Camera(Point3 pos, Point3 target)
        {
            Position = pos;
            Target = target;
        }
    }

    public static class Render
    {
        public static void Clear(Bitmap bitmap)
        {
            using (FastBitmap fast = bitmap.FastLock())
            {
                fast.Clear(Color.FromArgb(25,25,50));
            }
        }

        public static void DrawAxes(Bitmap bitmap, Point center, Point OX, Point OY, Point OZ)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawLine(new Pen(Color.Red, 3), center, OX);
                g.DrawLine(new Pen(Color.Green, 3), center, OY);
                g.DrawLine(new Pen(Color.Blue, 3), center, OZ);
            }
        }

        public static void Fill(Bitmap bitmap, Point[] poly, Color c, bool outline = false)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillPolygon(new SolidBrush(c), poly);
                if (outline) g.DrawPolygon(new Pen(Color.Black), poly);
            }
        }

        public static void FillTriangle(Bitmap bitmap, Triangle t, Color color)
        {
            List<Point3> points = new List<Point3>
            {
                t[0],
                t[1],
                t[2]
            };
            points.Sort((a, b) => b.Y.CompareTo(a.Y));
            Point3 A = points[0];
            Point3 B = points[1];
            Point3 C = points[2];

            float dx1;
            float dx2;
            float dx3;

            using (FastBitmap fast = bitmap.FastLock())
            {
                if (B.Y - A.Y > 0) dx1 = (B.X - A.X) / (B.Y - A.Y);
                else dx1 = 0;
                if (C.Y - A.Y > 0) dx2 = (C.X - A.X) / (C.Y - A.Y);
                else dx2 = 0;
                if (C.Y - B.Y > 0) dx3 = (C.X - B.X) / (C.Y - B.Y);
                else dx3 = 0;
                Point3 S = new Point3(A);
                Point3 E = new Point3(A);
                if (dx1 > dx2)
                {
                    for (; S.Y <= B.Y; S.Y++, E.Y++, S.X += dx2, E.X += dx1)
                        for (int i = (int)S.X; i < E.X; i++)
                        {
                            fast.SetPixel(i, (int)S.Y, color);
                        }
                    E = new Point3(B);
                    for (; S.Y <= C.Y; S.Y++, E.Y++, S.X += dx2, E.X += dx3)
                        for (int i = (int)S.X; i < E.X; i++)
                        {
                            fast.SetPixel(i, (int)S.Y, color);
                        }
                }
                else
                {
                    for (; S.Y <= B.Y; S.Y++, E.Y++, S.X += dx1, E.X += dx2)
                        for (int i = (int)S.X; i < E.X; i++)
                        {
                            fast.SetPixel(i, (int)S.Y, color);
                        }
                    S = new Point3(B);
                    for (; S.Y <= C.Y; S.Y++, E.Y++, S.X += dx3, E.X += dx2)
                        for (int i = (int)S.X; i < E.X; i++)
                        {
                            fast.SetPixel(i, (int)S.Y, color);
                        }
                }
            }
        }
    }
}
