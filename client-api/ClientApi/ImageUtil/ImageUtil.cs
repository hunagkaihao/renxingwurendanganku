using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApi.ImageUtil
{
    public class ImageUtil
    {
        /// <summary>
        /// Image 转成 base64
        /// </summary>
        /// <param name="fileFullName"></param>
        public static string ImageToBase64(string fileFullName)
        {
            try
            {
                Bitmap bmp = new Bitmap(fileFullName);
                MemoryStream ms = new MemoryStream();
                var suffix = fileFullName.Substring(fileFullName.LastIndexOf('.') + 1,
                    fileFullName.Length - fileFullName.LastIndexOf('.') - 1).ToLower();
                var suffixName = suffix == "png"
                    ? ImageFormat.Png
                    : suffix == "jpg" || suffix == "jpeg"
                        ? ImageFormat.Jpeg
                        : suffix == "bmp"
                            ? ImageFormat.Bmp
                            : suffix == "gif"
                                ? ImageFormat.Gif
                                : ImageFormat.Jpeg;

                bmp.Save(ms, suffixName);
                byte[] arr = new byte[ms.Length]; ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length); ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static string WebImageToBase64(string fileFullName)
        {
            try
            {
                var webC = new System.Net.WebClient();
                Bitmap bmp = new Bitmap(webC.OpenRead(fileFullName));
                MemoryStream ms = new MemoryStream();
                var suffix = fileFullName.Substring(fileFullName.LastIndexOf('.') + 1,
                    fileFullName.Length - fileFullName.LastIndexOf('.') - 1).ToLower();
                var suffixName = suffix == "png"
                    ? ImageFormat.Png
                    : suffix == "jpg" || suffix == "jpeg"
                        ? ImageFormat.Jpeg
                        : suffix == "bmp"
                            ? ImageFormat.Bmp
                            : suffix == "gif"
                                ? ImageFormat.Gif
                                : ImageFormat.Jpeg;

                bmp.Save(ms, suffixName);
                byte[] arr = new byte[ms.Length]; ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length); ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <returns></returns>
        public static string ImageToBase64(Image image)
        {
            try
            {
                //    Bitmap bmp = new Bitmap(Form1.imageW, Form1.imageH, PixelFormat.Format32bppRgb);
                //    //这个矩形定义了，你将要在被截取的图像上要截取的图像区域的左顶点位置和截取的大小
                //    Rectangle rectSource = new Rectangle(Form1.imageX, Form1.imageY, Form1.imageW, Form1.imageH);

                //    //这个矩形定义了，你将要把 截取的图像区域 绘制到初始化的位图的位置和大小
                //    //我的定义，说明，我将把截取的区域，从位图左顶点开始绘制，绘制截取的区域原来大小
                //    Rectangle rectDest = new Rectangle(0, 0, Form1.imageW, Form1.imageH);

                ////第一个参数就是加载你要截取的图像对象，第二个和第三个参数及如上所说定义截取和绘制图像过程中的相关属性，第四个属性定义了属性值所使用的度量单位
                //    Graphics g = Graphics.FromImage(bmp);
                //    g.DrawImage(image, rectDest, rectSource, GraphicsUnit.Pixel);
                //    //e.Graphics.DrawImage(image, 0, 0, 336, 189);
                //    g.Dispose();

                Bitmap bmp = new Bitmap(image);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length]; ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length); ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static Bitmap GetImageFromBase64(string base64string)
        {
            byte[] b = Convert.FromBase64String(base64string);
            MemoryStream ms = new MemoryStream(b);
            Bitmap bitmap = new Bitmap(ms);
            ms.Close();
            return bitmap;
        }
        public static string MatToBase64(Mat image)
        {
            //try
            //{
            //    Bitmap bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            //    MemoryStream ms = new MemoryStream();
            //    bmp.Save(ms, ImageFormat.Jpeg);
            //    byte[] arr = new byte[ms.Length]; ms.Position = 0;
            //    ms.Read(arr, 0, (int)ms.Length); ms.Close();
            //    return Convert.ToBase64String(arr);
            //}
            //catch (Exception ex)
            //{
            return null;
            //}

        }

    }
}
