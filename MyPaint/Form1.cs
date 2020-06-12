using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Drawing.Imaging;

namespace MyPaint
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        byte figura = 0; //1-line, 2 - circle, 3 - rectangle, 4 - pencil, 5 - eraser, 6 - filled circle, 7 - filled rectangle
        bool loaded, undo = false;
        string OName;
        Point p1, p2;
        Graphics g;
        Bitmap bmp;
        Pen myPen = new Pen(Brushes.Black, 1);
        Pen WhitePen = new Pen(Brushes.White, 1);
        Brush myBrush = new SolidBrush(Color.Black);
        Brush WhiteBrush = new SolidBrush(Color.White);
        
        
        internal class DrawnObj                                                  //CLASS OF ALL DRAWN OBJECTS 
        {
            private Point p1;
            public Point P1 { 
                get { return this.p1; }
                set { this.p1 = value; }
               }
            
            private Point p2; 
            public Point P2
            {
                get { return this.p2; }
                set { this.p2 = value; }
            }

            private byte f;
            public byte F
            {
                get { return this.f; }
                set { this.f = value; }
            }
            private Pen mypen;
            public Pen myPen
            {
                get { return this.mypen; }
                set { this.mypen = value; }
            }
            private Brush mybrush;
            public Brush myBrush
            {
                get { return this.mybrush; }
                set { this.mybrush = value; }
            }
            

        }
        List<DrawnObj> DrawnObjList = new List<DrawnObj>();                      //LIST OF ALL DRAWN OBJECTS
        List<DrawnObj> CopyObjList = new List<DrawnObj>();                      //LIST OF ALL DRAWN OBJECTS

        //MAIN PARAMETR CHANGE BY TOOLSTRIP BUTTONS
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            figura = 1;
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            figura = 2;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            figura = 3;
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            figura = 5;
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            figura = 4;
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            figura = 6;
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            figura = 7;
        }

        private void Form1_Load(object sender, EventArgs e)                             //FORM LOAD           
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);                                                       //Making image white by default
            pictureBox1.Image = bmp;                                                    //Loading to PictureBox
        }                                                                      

        private void pictureBox1_SizeChanged(object sender, EventArgs e)                //RESIZING PICTUREBOX 
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);                                                       //Making image white by default
            pictureBox1.Image = bmp;
            if (loaded) //redraw loaded image and draw on it
            {
                using (var image = new Bitmap(OName))
                    bmp = new Bitmap(image);
                pictureBox1.Image = new Bitmap(bmp);
                g = Graphics.FromImage(bmp);
            }
            ReDrawAll(DrawnObjList);
        }  

        private void toolStripButton6_Click(object sender, EventArgs e)                 //COLOR PICK          
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                myPen = new Pen(colorDialog1.Color);
                myBrush = new SolidBrush(colorDialog1.Color);
            }
        }     

        private void создатьToolStripButton_Click(object sender, EventArgs e)           //CLEAR / CREATE NEW  
        {
            DrawnObjList.Clear();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);                                                       //Making image white by default
            pictureBox1.Image = bmp;
            loaded = false;
        }

        private void открытьToolStripButton_Click(object sender, EventArgs e)           //OPEN                
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Windows Bitmap (*.bmp)|*.bmp";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                DrawnObjList.Clear();
                OName = openFileDialog.FileName;
                pictureBox1.Size = bmp.Size;                      // Changing size here making trouble when resizing form becouse of anchor properties
                using (var image = new Bitmap(OName))
                    bmp = new Bitmap(image);
                pictureBox1.Image = new Bitmap(bmp);
                g = Graphics.FromImage(bmp);
                loaded = true;
               }

        }

        private void toolStripButton9_Click(object sender, EventArgs e)                 //SAVE                
        {
                saveFileDialog1.InitialDirectory = Application.StartupPath + "\\Images";
                saveFileDialog1.Filter = "Windows Bitmap (*.bmp)|*.bmp";
                string SName;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                        SName = saveFileDialog1.FileName;
                        pictureBox1.Image.Save(SName, ImageFormat.Bmp);
                }
            
        }

        void ReDrawAll(List<DrawnObj> l)                                                //REDRAW FUNCTION     
        {
            if (l != null)
                foreach (var v in l)
                {
                    switch (v.F)
                    {
                        case 1:
                            g.DrawLine(v.myPen, v.P1, v.P2);
                            pictureBox1.Image = bmp;
                            break;
                        case 2:
                            g.DrawEllipse(v.myPen, v.P1.X, v.P1.Y, v.P2.X - v.P1.X, v.P2.Y - v.P1.Y);
                            pictureBox1.Image = bmp;
                            break;
                        case 3:
                            g.DrawRectangle(v.myPen, Math.Min(v.P2.X, v.P1.X), Math.Min(v.P2.Y, v.P1.Y), Math.Abs(v.P2.X - v.P1.X), Math.Abs(v.P2.Y - v.P1.Y));
                            pictureBox1.Image = bmp;
                            break;
                        case 4:
                            g.FillEllipse(v.myBrush, v.P2.X, v.P2.Y, 5, 5);
                            pictureBox1.Image = bmp;
                            break;
                        case 5:
                            g.FillRectangle(v.myBrush, v.P2.X, v.P2.Y, 20, 20);
                            pictureBox1.Image = bmp;
                           break;
                        case 6:
                            g.FillEllipse(v.myBrush, v.P1.X, v.P1.Y, v.P2.X - v.P1.X, v.P2.Y - v.P1.Y);
                            pictureBox1.Image = bmp;
                            break;
                        case 7:
                            g.FillRectangle(v.myBrush, Math.Min(v.P2.X, v.P1.X), Math.Min(v.P2.Y, v.P1.Y), Math.Abs(v.P2.X - v.P1.X), Math.Abs(v.P2.Y - v.P1.Y));
                            pictureBox1.Image = bmp;
                            break;
                    }

                }
        }

        private void toolStripButton10_Click(object sender, EventArgs e)                //UNDO
        {
            g.Clear(Color.White);                                                       //Making image white by default
            pictureBox1.Image = bmp;
            if (loaded) //redraw loaded image and draw on it
            {
                using (var image = new Bitmap(OName))
                    bmp = new Bitmap(image);
                pictureBox1.Image = new Bitmap(bmp);
                g = Graphics.FromImage(bmp);
            }
            if (DrawnObjList.Any()) //prevent IndexOutOfRangeException for empty list
            {
                CopyObjList = new List<DrawnObj>(DrawnObjList);
                CopyObjList.RemoveAt(CopyObjList.Count - 1);           // remove last drawn obj of this typo 
                ReDrawAll(CopyObjList);
                undo = true;
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)                //REDO
        {
            g.Clear(Color.White);                                                       //Making image white by default
            pictureBox1.Image = bmp;
            if (loaded) //redraw loaded image and draw on it
            {
                using (var image = new Bitmap(OName))
                    bmp = new Bitmap(image);
                pictureBox1.Image = new Bitmap(bmp);
                g = Graphics.FromImage(bmp);
            }
            if (DrawnObjList.Any()) //prevent IndexOutOfRangeException for empty list
                ReDrawAll(DrawnObjList);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)             //MOUSE DOWN ACTION   
        {
            p1 = e.Location;
        }
        
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)             //MOUSE MOVE DRAWING  
        {
            if (e.Button == MouseButtons.Left)
            {

                switch (figura)
                {
                       
            
                    case 1:
                        DrawnObj obj = new DrawnObj();

                        g.Clear(Color.White); // Clear all
                        if (loaded) //redraw loaded image and draw on it
                        {
                            using (var image = new Bitmap(OName))
                              bmp = new Bitmap(image);
                            pictureBox1.Image = new Bitmap(bmp);
                            g = Graphics.FromImage(bmp);
                        }

                        if (undo)
                        {
                            DrawnObjList.RemoveAt(DrawnObjList.Count - 1); 
                            ReDrawAll(DrawnObjList);
                            undo = false;                  
                        }

                        if (DrawnObjList.Any()) //prevent IndexOutOfRangeException for empty list
                        {
                            if(figura== DrawnObjList[DrawnObjList.Count-1].F && p1== DrawnObjList[DrawnObjList.Count - 1].P1) 
                                DrawnObjList.RemoveAt(DrawnObjList.Count-1);           // remove last drawn obj of this typo 
                            ReDrawAll(DrawnObjList);                                            //Redraw all exept last drawn line
                        }
                        p2 = e.Location;                // get coordinates of endpoint
                        g.DrawLine(myPen, p1, p2);      // нарисовать линию / draw line
                                  
                        obj.P1 = p1;
                        obj.P2 = p2;
                        obj.F = 1;
                        obj.myPen = myPen;

                        DrawnObjList.Add(obj);          //adding drawn obj


                        pictureBox1.Image = bmp; 
                        
                        obj = null;
                        break;

                    case 2:
                        DrawnObj Ellipse = new DrawnObj();
                      
                        g.Clear(Color.White);
                        if (loaded) 
                        {
                            using (var image = new Bitmap(OName))
                                bmp = new Bitmap(image);
                            pictureBox1.Image = new Bitmap(bmp);
                            g = Graphics.FromImage(bmp);
                        }

                        if (undo)
                        {
                            DrawnObjList.RemoveAt(DrawnObjList.Count - 1);
                            ReDrawAll(DrawnObjList);
                            undo = false;
                        }

                        if (DrawnObjList.Any()) 
                        {
                            if (figura == DrawnObjList[DrawnObjList.Count - 1].F && p1 == DrawnObjList[DrawnObjList.Count - 1].P1)
                                DrawnObjList.RemoveAt(DrawnObjList.Count - 1);           // remove last drawn obj of this typo 
                            ReDrawAll(DrawnObjList);                                            //redraw all
                        }

                        p2 = e.Location;

                        g.DrawEllipse(myPen, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);

                        Ellipse.P1 = p1;
                        Ellipse.P2 = p2;
                        Ellipse.F = 2;
                        Ellipse.myPen = myPen;

                        DrawnObjList.Add(Ellipse);

                        pictureBox1.Image = bmp;
                        Ellipse = null;
                        break;
                    case 3:
                        DrawnObj Rectangle = new DrawnObj();
                        g.Clear(Color.White);
                        if (loaded)
                        {
                            using (var image = new Bitmap(OName))
                                bmp = new Bitmap(image);
                            pictureBox1.Image = new Bitmap(bmp);
                            g = Graphics.FromImage(bmp);
                        }

                        if (undo)
                        {
                            DrawnObjList.RemoveAt(DrawnObjList.Count - 1);
                            ReDrawAll(DrawnObjList);
                            undo = false;
                        }

                        if (DrawnObjList.Any())
                        {
                            if (figura == DrawnObjList[DrawnObjList.Count - 1].F && p1 == DrawnObjList[DrawnObjList.Count - 1].P1)
                                DrawnObjList.RemoveAt(DrawnObjList.Count - 1);        
                            ReDrawAll(DrawnObjList);                                            
                        }

                        p2 = e.Location;

                        g.DrawRectangle(myPen, Math.Min(p2.X, p1.X), Math.Min(p2.Y, p1.Y), Math.Abs(p2.X - p1.X), Math.Abs(p2.Y - p1.Y));

                        Rectangle.P1 = p1;
                        Rectangle.P2 = p2;
                        Rectangle.F = 3;
                        Rectangle.myPen = myPen;

                        DrawnObjList.Add(Rectangle);
                        pictureBox1.Image = bmp;
                        Rectangle = null;
                        break;

                    case 4:
                        DrawnObj Pen = new DrawnObj();
                        if (loaded) //redraw loaded image and draw on it
                        {
                            using (var image = new Bitmap(OName))
                                bmp = new Bitmap(image);
                            pictureBox1.Image = new Bitmap(bmp);
                            g = Graphics.FromImage(bmp);
                            if (DrawnObjList.Any())
                                ReDrawAll(DrawnObjList);                                            //redraw all

                        }

                        if (undo)
                        {
                            DrawnObjList.RemoveAt(DrawnObjList.Count - 1);
                            ReDrawAll(DrawnObjList);
                            undo = false;
                        }

                        p2 = e.Location;


                        g.FillEllipse(myBrush, p2.X, p2.Y, 5,5);

                        Pen.P1 = p1;
                        Pen.P2 = p2;
                        Pen.F = 4;
                        Pen.myBrush = myBrush;

                        DrawnObjList.Add(Pen);
                        pictureBox1.Image = bmp;
                        Pen = null;
                        break;


                    case 5:
                        DrawnObj Eraser = new DrawnObj();
                        if (loaded) //redraw loaded image and draw on it
                        {
                            using (var image = new Bitmap(OName))
                                bmp = new Bitmap(image);
                            pictureBox1.Image = new Bitmap(bmp);
                            g = Graphics.FromImage(bmp);
                            if (DrawnObjList.Any())
                                ReDrawAll(DrawnObjList);                                            //redraw all

                        }

                        if (undo)
                        {
                            DrawnObjList.RemoveAt(DrawnObjList.Count - 1);
                            ReDrawAll(DrawnObjList);
                            undo = false;
                        }

                        p2 = e.Location;
                        g.FillRectangle(WhiteBrush, p2.X, p2.Y, 20, 20);

                        Eraser.P1 = p1;
                        Eraser.P2 = p2;
                        Eraser.F = 5;
                        Eraser.myBrush = WhiteBrush;

                        DrawnObjList.Add(Eraser);
                        pictureBox1.Image = bmp;
                        Eraser = null;
                        break;
                    case 6:
                        DrawnObj FillE = new DrawnObj();
                     
                        g.Clear(Color.White);
                        if (loaded)
                        {
                            using (var image = new Bitmap(OName))
                                bmp = new Bitmap(image);
                            pictureBox1.Image = new Bitmap(bmp);
                            g = Graphics.FromImage(bmp);
                        }

                        if (undo)
                        {
                            DrawnObjList.RemoveAt(DrawnObjList.Count - 1);
                            ReDrawAll(DrawnObjList);
                            undo = false;
                        }

                        if (DrawnObjList.Any())
                        {
                            if (figura == DrawnObjList[DrawnObjList.Count - 1].F && p1 == DrawnObjList[DrawnObjList.Count - 1].P1)
                                DrawnObjList.RemoveAt(DrawnObjList.Count - 1);
                            ReDrawAll(DrawnObjList);
                        }

                        p2 = e.Location;

                        g.FillEllipse(myBrush, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);

                        FillE.P1 = p1;
                        FillE.P2 = p2;
                        FillE.F = 6;
                        FillE.myBrush = myBrush;

                        DrawnObjList.Add(FillE);
                        pictureBox1.Image = bmp;
                        FillE = null;
                        break;

                    case 7:
                        DrawnObj FillR = new DrawnObj();


                        g.Clear(Color.White);
                        if (loaded)
                        {
                            using (var image = new Bitmap(OName))
                                bmp = new Bitmap(image);
                            pictureBox1.Image = new Bitmap(bmp);
                            g = Graphics.FromImage(bmp);
                        }

                        if (undo)
                        {
                            DrawnObjList.RemoveAt(DrawnObjList.Count - 1);
                            ReDrawAll(DrawnObjList);
                            undo = false;
                        }

                        if (DrawnObjList.Any())
                        {
                            if (figura == DrawnObjList[DrawnObjList.Count - 1].F && p1 == DrawnObjList[DrawnObjList.Count - 1].P1)
                                DrawnObjList.RemoveAt(DrawnObjList.Count - 1);
                            ReDrawAll(DrawnObjList);
                        }

                        p2 = e.Location;

                        g.FillRectangle(myBrush, Math.Min(p2.X, p1.X), Math.Min(p2.Y, p1.Y), Math.Abs(p2.X - p1.X), Math.Abs(p2.Y - p1.Y));

                        FillR.P1 = p1;
                        FillR.P2 = p2;
                        FillR.F = 7;
                        FillR.myBrush = myBrush;

                        DrawnObjList.Add(FillR);
                        pictureBox1.Image = bmp;

                        FillR = null;
                        break;

                }
            }
        }
        




     

     


       
      


    }
}


//{ прозрачный слой 
//bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
//bmp1.MakeTransparent(Color.White);
//g.DrawImage(bmp1, 0, 0, bmp.Size.Width, bmp.Size.Height);
//g = Graphics.FromImage(bmp1);
//pictureBox1.Image = null;
//pictureBox1.Image = bmp1;
//pictureBox1.Refresh();
//pictureBox1.Invalidate();
//pictureBox1.Invalidate();
//pictureBox1.Update();
//g.DrawLine(myPen, p1, p2);
//g.Clear(Color.White);
//   pictureBox1.Image = bmp1;
//   g = Graphics.FromImage(bmp1);
//g = Graphics.FromImage(bmp1);
//ControlPaint.DrawReversibleLine(p1, p2, myPen.Color);
//using (Pen clear_pen = new Pen(pictureBox1.BackColor, 1))
//{
//    clear_pen.StartCap = START_CAP;
//    clear_pen.EndCap = END_CAP;
//    g.DrawLine(clear_pen, mAnchorPoint, mPreviousPoint);
//}
//        p2 = e.Location;                
//         g.DrawLine(myPen, p1, p2);      // нарисовать линию
//         g.Dispose();
//          pictureBox1.Invalidate();
//g = Graphics.FromImage(bmp1);
//pictureBox1.Image = bmp1;
// pictureBox1.Image = bmp1;
//bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
//bmp1.MakeTransparent();
//g.DrawImage(bmp1, 0, 0, bmp.Size.Width, bmp.Size.Height);
//g = Graphics.FromImage(bmp);

//this.Invalidate();
//backP = new Pen(this.pictureBox1.BackColor);

//bmp1.MakeTransparent(Color.White);

//bmp1.MakeTransparent(this.pictureBox1.BackColor);
//bmp1.MakeTransparent(Color.White);
//g.DrawImage(bmp1, 0, 0, bmp.Size.Height, bmp.Size.Width);

/*// Create a Bitmap object from an image file.
Bitmap myBitmap = new Bitmap("Grapes.gif");

// Draw myBitmap to the screen.
e.Graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width,
    myBitmap.Height);

// Make the default transparent color transparent for myBitmap.
myBitmap.MakeTransparent();

// Draw the transparent bitmap to the screen.
e.Graphics.DrawImage(myBitmap, myBitmap.Width, 0,
    myBitmap.Width, myBitmap.Height); 
     
//Create a Bitmap object from an image file.
Bitmap myBitmap = new Bitmap("Grapes.gif");

// Draw myBitmap to the screen.
e.Graphics.DrawImage(
    myBitmap, 0, 0, myBitmap.Width, myBitmap.Height);

// Get the color of a background pixel.
Color backColor = myBitmap.GetPixel(1, 1);

// Make backColor transparent for myBitmap.
myBitmap.MakeTransparent(backColor);

// Draw the transparent bitmap to the screen.
e.Graphics.DrawImage(
    myBitmap, myBitmap.Width, 0, myBitmap.Width, myBitmap.Height);

 */

//public void LayerImage(System.Drawing.Image Current, int LayerOpacity)
//{
//    Bitmap bitmap = new Bitmap(Current);
//    int h = bitmap.Height;
//    int w = bitmap.Width;
//    Bitmap backg = new Bitmap(w, h + 20);
//    Graphics g = null;
//    try
//    {
//        g = Graphics.FromImage(backg);
//        g.Clear(Color.White);
//        Font font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
//        RectangleF rectf = new RectangleF(70, 90, 90, 50);
//        Color color = Color.FromArgb(255, 128, 128, 128);
//        Point atpoint = new Point(backg.Width / 2, backg.Height - 10);
//        SolidBrush brush = new SolidBrush(color);
//        StringFormat sf = new StringFormat();
//        sf.Alignment = StringAlignment.Center;
//        sf.LineAlignment = StringAlignment.Center;
//        g.DrawString("BRAND AMBASSADOR", font, brush, atpoint, sf);
//        g.Dispose();
//        MemoryStream m = new MemoryStream();
//        backg.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
//    }
//    catch { }

//    Color pixel = new Color();

//    for (int x = 0; x < bitmap.Width; x++)
//    {
//        for (int y = 0; y < bitmap.Height; y++)
//        {
//            pixel = bitmap.GetPixel(x, y);
//            backg.SetPixel(x, y, Color.FromArgb(LayerOpacity, pixel));
//        }
//    }
//    MemoryStream m1 = new MemoryStream();
//    backg.Save(m1, System.Drawing.Imaging.ImageFormat.Jpeg);
//    m1.WriteTo(Response.OutputStream);
//    m1.Dispose();
//    base.Dispose();
//}





//List<Bitmap> images = new List<Bitmap>();
//Bitmap finalImage = new Bitmap(640, 480);
////For each layer, I transform the data into a Bitmap (doesn't matter what kind of
////data, in this question) and add it to the images list
//for (int i = 0; i<nLayers; ++i)
//{
//    Bitmap bitmap = new Bitmap(layerBitmapData[i]));
//    images.Add(bitmap);
//}

//using (Graphics g = Graphics.FromImage(finalImage))
//{
//    //set background color
//    g.Clear(Color.Black);

//    //go through each image and draw it on the final image (Notice the offset; since I want to overlay the images i won't have any offset between the images in the finalImage)
//    int offset = 0;
//    foreach (Bitmap image in images)
//    {
//        g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
//    }   
//}
////Draw the final image in the pictureBox
//this.layersBox.Image = finalImage;
////In my case I clear the List because i run this in a cycle and the number of layers is not fixed 
//images.Clear();