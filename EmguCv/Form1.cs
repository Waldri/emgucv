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
using System.Numerics;
using Emgu.CV.Util;

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

        //detect img
        static Mat read_rgb_image(string image_name, bool show)
        {
            Mat image;
            image = CvInvoke.Imread(image_name);

            if (show)
            {
                CvInvoke.Imshow("Image", image);
            }
            return image;
        }

        static Mat filter_color(Mat rgb_image, int [] lower_bound_color, int [] upper_bound_color)
        {
            Mat hsv_image = new Mat();
            CvInvoke.CvtColor(rgb_image, hsv_image, Emgu.CV.CvEnum.ColorConversion.Rgb2Hsv);
            Mat mask = new Mat();

            CvInvoke.Imshow("hsv image", hsv_image);
            CvInvoke.InRange(hsv_image, new ScalarArray(new MCvScalar(lower_bound_color[0], lower_bound_color[1], lower_bound_color[2])), 
                                        new ScalarArray(new MCvScalar(upper_bound_color[0], upper_bound_color[1], upper_bound_color[2])), mask);

            return mask;
        }

        static VectorOfVectorOfPoint getContours(Mat binary_image)
        {
            IOutputArrayOfArrays contours = new VectorOfVectorOfPoint();
            IOutputArrayOfArrays hierarchy = null;
            CvInvoke.FindContours(binary_image, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            return (VectorOfVectorOfPoint)contours;
        }
        static PointF get_contour_center(VectorOfPoint countour)
        {
            Moments M = CvInvoke.Moments(countour, true);
            float cx = -1;
            float cy = -1;

            if(M.M00 != 0)
            {
                cx = Convert.ToInt32(M.M10 / M.M00);
                cy = Convert.ToInt32(M.M01 / M.M00);
            }
             
            PointF cx_cy = new PointF(cx, cy);

            return cx_cy;
        }
        
        static void draw_ball_contour(Mat binary_image, Mat read_rgb_image, VectorOfVectorOfPoint contours)
        {
            System.Diagnostics.Debug.WriteLine("draw_ball_contour: " + contours.Size.ToString());

            Mat black_image = new Mat(binary_image.Rows, binary_image.Cols, Emgu.CV.CvEnum.DepthType.Cv8U,1);
            for (int c = 0; c < contours.Size; c++)
            {
                double area = CvInvoke.ContourArea(contours[c]);
                double perimeter = CvInvoke.ArcLength(contours[c], true);

                System.Diagnostics.Debug.WriteLine("area: " + area.ToString() + " perimeter: " + perimeter.ToString());

                if (area > 100) {
                    PointF center = new PointF();
                    float radius = new float();
                    
                    CvInvoke.MinEnclosingCircle(contours[c]); // Radios Center ??
                    radius = CvInvoke.MinEnclosingCircle(contours[c]).Radius;
                    center = CvInvoke.MinEnclosingCircle(contours[c]).Center;

                    VectorOfPoint ctn = contours[c];
                    PointF cxcy = get_contour_center(ctn);
                    CvInvoke.Circle(black_image, Point.Round(cxcy), (int)(radius), new MCvScalar(0, 0, 255), 1, Emgu.CV.CvEnum.LineType.EightConnected);
                    CvInvoke.DrawContours(black_image, contours, c, new MCvScalar(150, 250, 150), 1);

                }
            }
            CvInvoke.Imshow("Black contours", black_image);

        }

        //Main
        private void btnOpen3_Click(object sender, EventArgs e)
        {
            Mat img;
            img = CvInvoke.Imread("images/tennisball02.jpg");
            string image_name = "images/tennisball02.jpg";
            int[] yellowLower = { 30, 150, 100 };
            int[] yellowUpper = { 60, 255, 255 };
            Mat rgb_image = read_rgb_image(image_name, true);
            Mat binary_image_mask = filter_color(rgb_image, yellowLower, yellowUpper);
            VectorOfVectorOfPoint contours = getContours(binary_image_mask);
            draw_ball_contour(binary_image_mask, rgb_image, contours);

            CvInvoke.WaitKey(0);
            CvInvoke.DestroyAllWindows();
            
        }
    }
}
