using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZipMaker.log;

namespace ZipMaker.IO
{
    public class IOHelper
    {
        /// <summary>
        /// 将文件夹内的所有文件复制到另一个文件夹中
        /// </summary>
        /// <param name="srcPath">原文件夹地址</param>
        /// <param name="aimPath">新文件夹地址</param>
        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                //检查目标目录是否以目录分割字符  
                //结束如果不是则添加之   
                if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                    aimPath += System.IO.Path.DirectorySeparatorChar;
                //判断目标目录是否存在如果不存在则新建之  
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                //得到源目录的文件列表,该里面是包含  
                //文件以及目录路径的一个数组    
                //如果你指向copy目标文件下面的文件    
                //而不包含目录请使用下面的方法    
                //string[]fileList=  Directory.GetFiles(srcPath);  
                string[] fileList =
                    Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录    
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个  
                    //目录就递归Copy该目录下面的文件    
                    if (Directory.Exists(file))
                        CopyDir(
                            file,
                            aimPath + System.IO.Path.GetFileName(file));
                    //否则直接Copy文件   
                    else
                        File.Copy(
                            file,
                            aimPath + System.IO.Path.GetFileName(file),
                            true);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(IOHelper), ex);
            }
        }
    }
}
