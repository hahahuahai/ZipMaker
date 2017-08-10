using System;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker.Model
{
    public class XMLfolderModel
    {
        public XMLfolderModel() { }

        /// <summary>
        /// XML中的文件夹名
        /// </summary>
        private string foldername;
        public string Foldername
        {
            get { return foldername; }
            set { foldername = value; }
        }

        /// <summary>
        /// XML中的文件夹是否存在
        /// </summary>
        private string exist;
        public string Exist
        {
            get { return exist; }
            set { exist = value; }
        }

    }
}
