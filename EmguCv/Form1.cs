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
        private void btnOpen3_Click(object sender, EventArgs e)
        {
            Mat img;
            img = CvInvoke.Imread("images/tennisball02.jpg");
            string image_name = "images/tennisball02.jpg";
            int[] yellowLower = { 30, 150, 100 };
            int[] yellowUpper = { 60, 255, 255 };
            Mat rgb_image = read_rgb_image(image_name, true);
            Mat binary_image_mask = filter_color(rgb_image, yellowLower, yellowUpper);
            IInputArray contours = getContours(binary_image_mask);
            draw_ball_contour(binary_image_mask, read_rgb_image);

            //CvInvoke.NamedWindow("Image", Emgu.CV.CvEnum.WindowFlags.Normal);
            //CvInvoke.Imshow("Image", img);
            //CvInvoke.Imwrite("images/flower.jpg", img);
            CvInvoke.WaitKey(0);
        }

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

        static IInputArray getContours(Mat binary_image)
        {
            IInputArray contours = new Vector();
            CvInvoke.FindContours(binary_image, contours, Emgu.CV.CvEnum.RetrType.External , Emgu.CV.CvEnum.ContoursMatchType.I1);

            return contours;
        }
        static PointF get_contour_center(Vector<Point> countour)
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
        static void draw_ball_contour(Mat binary_image, Mat rgb_image, IInputArrayOfArrays contours)
        {
            Mat black_image = new Mat(binary_image.Rows, binary_image.Cols, Emgu.CV.CvEnum.DepthType.Cv8U,1);
            for (int c = 0; c < contours.size(); c++)
            {
                float area = CvInvoke.ContourArea(contours[c]);
                float perimeter = CvInvoke.ArcLength(contours[c], true);

                if (area > 300) {
                    PointF cxcy = get_contour_center(ctn);

                }
            }
            //Vector vector1 = new Vector() <vector<Point>> contours = CvInvoke.ContourArea(binary_image_mask);
        }
    }
}
