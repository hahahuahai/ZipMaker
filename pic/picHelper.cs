using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using ZipMaker.log;

namespace ZipMaker.pic
{
    public class picHelper
    {
        /// <summary>
        /// 批量压缩图片（根据文件夹）
        /// </summary>
        /// <param name="originalPicFolderPath">存有原图的文件夹</param>
        /// <param name="savePicFolderPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void batchCompressPic(string originalPicFolderPath, string savePicFolderPath, int width, int height)
        {
            string filenameWithoutExtension = "";
            if (!Directory.Exists(savePicFolderPath))
            {
                Directory.CreateDirectory(savePicFolderPath);
            }

            List<string> fileFolderList = new List<string>();
            List<string> saveFolderList = new List<string>();
            List<string> fileList = new List<string>();

            DirectoryInfo theFolder = new DirectoryInfo(originalPicFolderPath);

            //获取文件夹下的文件夹
            foreach (DirectoryInfo nextFolder in theFolder.GetDirectories())
            {
                if (nextFolder.Name == "Index" || nextFolder.Name == "全景")
                    continue;

                fileFolderList.Add(nextFolder.FullName);
                saveFolderList.Add(savePicFolderPath + "\\" + nextFolder.Name);
            }
            //获取文件夹下的文件
            foreach (FileInfo nextFile in theFolder.GetFiles())
            {
                if (nextFile.Extension == ".jpg" || nextFile.Extension == ".JPG")
                {
                    filenameWithoutExtension = nextFile.Name.Replace(nextFile.Extension, "");
                    compressOnePic(originalPicFolderPath + "\\" + nextFile.Name, savePicFolderPath + "\\" + filenameWithoutExtension + "-1" + nextFile.Extension, width, height);
                }
            }
            ////压缩文件
            //foreach(string str in fileList)
            //{
            //    compressOnePic(str, str, width, height);
            //}
            //foreach (string str in fileFolderList)
            //{
            //    operateTheFolder(str);
            //}
            for (int i = 0; i < fileFolderList.Count; i++)
            {
                batchCompressPic(fileFolderList[i], saveFolderList[i], width, height);
            }
        }



        /// <summary>
        /// 压缩一张图片
        /// </summary>
        /// <param name="originalPicture">原图路径</param>
        /// <param name="compressedPicutre">压缩后图的路径</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void compressOnePic(string originalPicture, string compressedPicutre, int width, int height)
        {
            Image myImage = Image.FromFile(originalPicture);

            if (myImage.Width > myImage.Height)
                height = width * myImage.Height / myImage.Width;
            else
                width = height * myImage.Width / myImage.Height;

            Bitmap myBitmap = new Bitmap(width, height);

            //从Bitmap创建一个System.Drawing.Graphics对象，用来绘制高质量的缩小图。
            Graphics gr = Graphics.FromImage(myBitmap);

            try
            {

                //设置 System.Drawing.Graphics对象的SmoothingMode属性为HighQuality
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //下面这个也设成高质量
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                //下面这个设成High
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                //把原始图像绘制成上面所设置宽高的缩小图
                Rectangle orgRect = new Rectangle(0, 0, width, height);
                Rectangle saveRect = new Rectangle(0, 0, myImage.Width, myImage.Height);
                gr.DrawImage(myImage, orgRect, saveRect, GraphicsUnit.Pixel);

                //保存图像，大功告成！
                myBitmap.Save(compressedPicutre);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(picHelper), ex);
            }
            finally
            {
                gr.Clear(Color.Black);
                gr.Dispose();
                myBitmap.Dispose();
                myImage.Dispose();
            }

        }

    }
}
