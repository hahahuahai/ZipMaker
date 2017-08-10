using System;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker.Model
{
    public class XMLfileModel
    {
        public XMLfileModel() { }

        /// <summary>
        /// XML中的文件名
        /// </summary>
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// XML中的文件是否存在
        /// </summary>
        private string exist;

        public string Exist
        {
            get { return exist; }
            set { exist = value; }
        }
    }
}
