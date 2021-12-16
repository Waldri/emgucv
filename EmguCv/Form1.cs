using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;

namespace EmguCv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpFile = new OpenFileDialog();
            OpFile.Filter = "Image File (*.bmp, *.png) | *.bmp;*.png";
            if(DialogResult.OK == OpFile.ShowDialog())
            {
                this.picOrign.Image = new Bitmap(OpFile.FileName);
            }
        }

        private void btnCam_Click(object sender, EventArgs e)
        {
            ImageViewer viewer = new ImageViewer(); //create an image viewer
            VideoCapture capture = new VideoCapture(); //create a camera captue
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                viewer.Image = capture.QueryFrame(); //draw the image obtained from camera
            });
            viewer.ShowDialog(); //show the image viewer
        }

        private void btnGray_Click(object sender, EventArgs e)
        {
            Bitmap ggg = new Bitmap((Bitmap)this.picOrign.Image);
            Method.ConvertToGray(ggg);
            this.picResize.Image = ggg;
            
        }

        private void btnOpen2_Click(object sender, EventArgs e)
        {
            Mat img;
            img = CvInvoke.Imread("images/flower.jpg");
            CvInvoke.NamedWindow("Image", Emgu.CV.CvEnum.WindowFlags.Normal);
            CvInvoke.Imshow("Image", img);
            CvInvoke.Imwrite("images/flower.jpg", img);
            CvInvoke.WaitKey(0);
        }
    }
}
