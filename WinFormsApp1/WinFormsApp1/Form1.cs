using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Width = 1200;
            this.Height = 700;
            bm = new Bitmap(picture.Width, picture.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            picture.Image = bm;
            p.Width = (float)brushSize.Value;
            erase.Width = (float)brushSize.Value;
        }

        private void picture_Click(object sender, EventArgs e)
        {

        }

        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 3);
        Pen erase = new Pen(Color.White, 10);
        int index;
        int x, y, sX, sY, cX, cY;

        ColorDialog cd = new ColorDialog();
        Color newColor;

        private void picture_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            py = e.Location;

            cX = e.X;
            cY = e.Y;
        }

        private void picture_MouseMove(object sender, MouseEventArgs e)
        {
            if(paint)
            {
                if(index == 1)
                {
                    px = e.Location;
                    g.DrawLine(p, px, py);
                    py = px;
                }
            }
            if (paint)
            {
                if (index == 2)
                {
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                }
            }
            picture.Refresh();

            x = e.X;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - cY;
        }

        private void picture_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;

            sX = x - cX;
            sY = y - cY;

            if(index == 3)
            {
                g.DrawEllipse(p, cX, cY, sX, sY);
            }

            if(index == 4)
            {
                g.DrawRectangle(p, cX, cY, sX, sY);
            }

            if(index == 5)
            {
                g.DrawLine(p, cX, cY, x, y);
            }
        }

        private void btnPencil_Click(object sender, EventArgs e)
        {
            index = 1;
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            index = 3;
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            index = 4;
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            index = 5;
        }


        private void picture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if(paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }

                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }

                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            picture.Image = bm;
            index = 0;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            newColor = cd.Color;
            pickColor.BackColor = cd.Color;
            p.Color = cd.Color;
        }

        static Point setPoint(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y*pY));
        }

        private void brushSize_ValueChanged(object sender, EventArgs e)
        {
            p.Width = (float)brushSize.Value;
            erase.Width = (float)brushSize.Value;
        }

        private void colorPicker_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = setPoint(colorPicker, e.Location);
            pickColor.BackColor = ((Bitmap)colorPicker.Image).GetPixel(point.X, point.Y);
            newColor = pickColor.BackColor;
            p.Color = pickColor.BackColor;
        }

        private void validate(Bitmap bm, Stack<Point>sp, int x, int y, Color oldColor, Color newColor)
        {
            Color cx = bm.GetPixel(x, y);
            if(cx == oldColor)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, newColor);
            }
        }

        public void Fill(Bitmap bm, int x, int y, Color newClr)
        {
            Color oldColor = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, newClr);
            if (oldColor == newColor) return;

            while(pixel.Count>0)
            {
                Point pt = (Point) pixel.Pop();
                if(pt.X > 0 && pt.Y > 0 && pt.X < bm.Width - 1 && pt.Y < bm.Height - 1)
                {
                    validate(bm, pixel, pt.X - 1, pt.Y, oldColor, newClr);
                    validate(bm, pixel, pt.X, pt.Y - 1, oldColor, newClr);
                    validate(bm, pixel, pt.X + 1, pt.Y, oldColor, newClr);
                    validate(bm, pixel, pt.X, pt.Y + 1, oldColor, newClr);
                }
            }
        }

        private void picture_MouseClick(object sender, MouseEventArgs e)
        {
            if(index == 7)
            {
                Point point = setPoint(picture, e.Location);
                Fill(bm, point.X, point.Y, newColor);
            }
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            index = 7;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Image(*.jpg) | *.jpg|(*.* | *.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, picture.Width, picture.Height), bm.PixelFormat);
                btm.Save(sfd.FileName, ImageFormat.Jpeg);

                MessageBox.Show("Bilden har sparats, väldigt bra jobbat!");
            }
        }
    }
}
