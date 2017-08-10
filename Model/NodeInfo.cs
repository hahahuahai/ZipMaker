
namespace ZipMaker
{
    /// <summary>
    /// 结点类型
    /// </summary>
    public enum NodeType
    {
        ntRoot,
        ntFolder,
        ntBuildingFolder,
        ntJPGFile,
        ntFloorJPGFile,//分层平面图
        ntCompanyTxtFile,//公司介绍
        ntParcelFlvFile,
        ntBuildingCADFile,
        ntUnknown
    };

    /// <summary>
    /// 结点信息，作为TreeNode的Tag对象
    /// </summary>
    public class NodeInfo
    {

        public NodeType nodeType;
        public string nodeName;
        public bool existedInServer;
        public string newPath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nodeType">结点类型</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="existedInServer">是否在服务器存在</param>
        /// <param name="newPath">新关联文件路径</param>
        public NodeInfo( NodeType nodeType, string nodeName, bool existedInServer , string newFile )
        {
            this.nodeType = nodeType;
            this.nodeName = nodeName;
            this.existedInServer = existedInServer;
            this.newPath = newFile;
        }

        public NodeInfo( NodeType nodeType, string nodeName, bool existedInServer):this( nodeType, nodeName, existedInServer, "") { }

        public NodeInfo(NodeType nodeType, string nodeName) : this(nodeType, nodeName, false) { }

        public NodeInfo(NodeType nodeType) : this(nodeType, "") { }

        public NodeInfo() : this(NodeType.ntUnknown) { }
        /// <summary>
        /// 根据类型字符串获取结点类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NodeType GetNodeType(string type)
        {
            switch (type.ToLower())
            {
                case "jpg":
                    return NodeType.ntJPGFile;
                case "f-jpg":
                    return NodeType.ntFloorJPGFile;
                case "flv":
                    return NodeType.ntParcelFlvFile;
                case "txt":
                    return NodeType.ntCompanyTxtFile;
                case "cad":
                    return NodeType.ntBuildingCADFile;
            }
            return NodeType.ntUnknown;
        }


        /// <summary>
        /// 在打开文件对话框时获取文件结点类型对应的扩展名字符串
        /// </summary>
        /// <param name="nt"></param>
        /// <returns></returns>
        public string GetFilterString()
        {
            switch ( nodeType )
            {
                case NodeType.ntJPGFile:
                case NodeType.ntFloorJPGFile:
                    return "JPG文件 (*.jpg)|*.jpg";
                case NodeType.ntCompanyTxtFile:
                    return "文本文件 (*.txt)|*.txt";
                case NodeType.ntParcelFlvFile:
                    return "视频文件 (*.flv)|*.flv";
                case NodeType.ntBuildingCADFile:
                    return "cad文件 (*.rar)|*.rar";
            }
            return "所有文件 (*.*)|*.*";
        }

    }
}