using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZipMaker.log;

namespace ZipMaker.Zip
{
    public class ZipHelper
    {


        List<string> files = new List<string>();    //all files

        List<string> emptyFolders = new List<string>();    //all empty folders

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetFile"></param>
        public  void ZipFolder(string source, string targetFile)
        {
            GetAllDirectories(source);

            while (source.LastIndexOf("\\") + 1 == source.Length)//check if the path endwith "\\"
            {
                source = source.Substring(0, source.Length - 1);//remove "\\"
            }

            string rootMark = source.Substring(0, source.LastIndexOf("\\") + 1);

            Crc32 crc = new Crc32();

            ZipOutputStream outPutStream = new ZipOutputStream(File.Create(targetFile));
            outPutStream.SetLevel(9); // 0 - store only to 9 - means best compression
            foreach (string file in files)
            {
                FileStream fileStream = File.OpenRead(file);
                byte[] buffer = new byte[fileStream.Length];

                fileStream.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(file.Replace(rootMark, string.Empty));

                entry.DateTime = DateTime.Now;

                // set Size and the crc, because the information

                // about the size and crc should be stored in the header

                // if it is not set it is automatically written in the footer.

                // (in this case size == crc == -1 in the header)

                // Some ZIP programs have problems with zip files that don't store

                // the size and crc in the header.

                entry.Size = fileStream.Length;
                fileStream.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                outPutStream.PutNextEntry(entry);
                outPutStream.Write(buffer, 0, buffer.Length);
            }

            foreach (string emptyPath in emptyFolders)
            {
                ZipEntry entry = new ZipEntry(emptyPath.Replace(rootMark, string.Empty) + "\\");
                outPutStream.PutNextEntry(entry);
            }

            outPutStream.Finish();
            outPutStream.Close();
        }
        private void GetAllDirectories(string rootPath)
        {
            string[] subPaths = Directory.GetDirectories(rootPath);
            foreach (string path in subPaths)
            {
                GetAllDirectories(path);
            }

            string[] filesArray = Directory.GetFiles(rootPath);

            foreach (string file in filesArray)
            {
                this.files.Add(file); //put all files in current folder into list
            }
            if (subPaths.Length == filesArray.Length && filesArray.Length == 0) //if it is an empty folder
            {
                this.emptyFolders.Add(rootPath);//add it to the list
            }
        }



        ///// <summary>
        ///// 压缩
        ///// </summary>
        ///// <param name="filesPath">待压缩文件夹路径</param>
        ///// <param name="zipFilePath">zip压缩文件夹路径</param>
        //public static void CreateZipFile(string filesPath, string zipFilePath)
        //{

        //    if (!Directory.Exists(filesPath))
        //    {
        //        Console.WriteLine("Cannot find directory '{0}'", filesPath);
        //        return;
        //    }

        //    try
        //    {
        //        string[] filenames = Directory.GetFiles(filesPath);
        //        using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
        //        {

        //            s.SetLevel(9); // 压缩级别 0-9
        //            //s.Password = "123"; //Zip压缩文件密码
        //            byte[] buffer = new byte[4096]; //缓冲区大小
        //            foreach (string file in filenames)
        //            {
        //                ZipEntry entry = new ZipEntry(Path.GetFileName(file));
        //                entry.DateTime = DateTime.Now;
        //                s.PutNextEntry(entry);
        //                using (FileStream fs = File.OpenRead(file))
        //                {
        //                    int sourceBytes;
        //                    do
        //                    {
        //                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
        //                        s.Write(buffer, 0, sourceBytes);
        //                    } while (sourceBytes > 0);
        //                }
        //            }
        //            s.Finish();
        //            s.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteLog(typeof(ZipHelper), ex); ;
        //        Console.WriteLine("Exception during processing {0}", ex);
        //    }
        //}

        ///// <summary>
        ///// 解压
        ///// </summary>
        ///// <param name="zipFilePath"></param>
        //public static void UnZipFile(string zipFilePath)
        //{
        //    if (!File.Exists(zipFilePath))
        //    {
        //        Console.WriteLine("Cannot find file '{0}'", zipFilePath);
        //        return;
        //    }
        //    try
        //    {
        //        using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
        //        {

        //            ZipEntry theEntry;
        //            while ((theEntry = s.GetNextEntry()) != null)
        //            {

        //                Console.WriteLine(theEntry.Name);

        //                string directoryName = Path.GetDirectoryName(theEntry.Name);
        //                string fileName = Path.GetFileName(theEntry.Name);

        //                // create directory
        //                if (directoryName.Length > 0)
        //                {
        //                    Directory.CreateDirectory(directoryName);
        //                }

        //                if (fileName != String.Empty)
        //                {
        //                    using (FileStream streamWriter = File.Create(theEntry.Name))
        //                    {

        //                        int size = 2048;
        //                        byte[] data = new byte[2048];
        //                        while (true)
        //                        {
        //                            size = s.Read(data, 0, data.Length);
        //                            if (size > 0)
        //                            {
        //                                streamWriter.Write(data, 0, size);
        //                            }
        //                            else
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogHelper.WriteLog(typeof(ZipHelper), ex);
        //    }

        //}


        ///// <summary>
        ///// 压缩文件
        ///// </summary>
        ///// <param name="sourceFilePath"></param>
        ///// <param name="destinationZipFilePath"></param>
        //public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
        //{
        //    if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
        //        sourceFilePath += System.IO.Path.DirectorySeparatorChar;
        //    ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
        //    zipStream.SetLevel(6);  // 压缩级别 0-9
        //    CreateZipFiles(sourceFilePath, zipStream);
        //    zipStream.Finish();
        //    zipStream.Close();
        //}
        ///// <summary>
        ///// 递归压缩文件
        ///// </summary>
        ///// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
        ///// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名</param>
        ///// <param name="staticFile"></param>
        //private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream)
        //{
        //    Crc32 crc = new Crc32();
        //    string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
        //    foreach (string file in filesArray)
        //    {
        //        if (Directory.Exists(file))                     //如果当前是文件夹，递归
        //        {
        //            CreateZipFiles(file, zipStream);
        //        }
        //        else                                            //如果是文件，开始压缩
        //        {
        //            FileStream fileStream = File.OpenRead(file);
        //            byte[] buffer = new byte[fileStream.Length];
        //            fileStream.Read(buffer, 0, buffer.Length);
        //            string tempFile = file.Substring(sourceFilePath.LastIndexOf("\\") + 1);
        //            ZipEntry entry = new ZipEntry(tempFile);
        //            entry.DateTime = DateTime.Now;
        //            entry.Size = fileStream.Length;
        //            fileStream.Close();
        //            crc.Reset();
        //            crc.Update(buffer);
        //            entry.Crc = crc.Value;
        //            zipStream.PutNextEntry(entry);
        //            zipStream.Write(buffer, 0, buffer.Length);
        //        }
        //    }
        //}


 
    }
}
