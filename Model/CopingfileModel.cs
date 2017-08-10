using System;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker.Model
{
    /// <summary>
    /// 待复制的文件/文件夹的数据模型
    /// </summary>
    public class CopingfileModel
    {
        #region file的数据结构，当property为file时用
        public CopingfileModel() { }

        /// <summary>
        /// 源文件的完整路径
        /// </summary>
        private string sourcefilePath;
        public string SourcefilePath
        {
            get { return sourcefilePath; }
            set { sourcefilePath = value; }
        }

        /// <summary>
        /// 源文件的文件名+扩展名,文件名不包含路径。
        /// </summary>
        private string sourcefileSafeFileName;
        public string SourcefileSafeFileName
        {
            get { return sourcefileSafeFileName; }
            set { sourcefileSafeFileName = value; }
        }

        /// <summary>
        /// 源文件的文件名，文件名不包含路径。
        /// </summary>
        private string sourcefileFilename;
        public string SourcefileFilename
        {
            get { return sourcefileFilename; }
            set { sourcefileFilename = value; }
        }

        /// <summary>
        /// 待复制进去的目标文件夹路径
        /// </summary>
        private string destFolderpath;
        public string DestFolderpath
        {
            get { return destFolderpath; }
            set { destFolderpath = value; }
        }
        #endregion

        #region folder的数据结构，当property为folder时用
        //源文件夹路径
        private string oldFolderpath;
        public string OldFolderpath
        {
            get { return oldFolderpath; }
            set { oldFolderpath = value; }
        }
        //目标文件夹路径
        private string newFolderpath;
        public string NewFolderpath
        {
            get { return newFolderpath; }
            set { newFolderpath = value; }
        }

        #endregion
        /// <summary>
        /// 属性，file还是folder
        /// </summary>
        private string property;
        public string Property
        {
            get { return property; }
            set { property = value; }
        } 

    }
}
