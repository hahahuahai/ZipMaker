using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ZipMaker.IO;
using ZipMaker.log;
using ZipMaker.Model;
using ZipMaker.Zip;

namespace ZipMaker
{

    public partial class zipmaker : Form
    {
        string folderpath = "";//  公司/宗地文件夹路径
        List<CopingfileModel> lstCopingfileModel = new List<CopingfileModel>();//所有待复制的文件集合

        public zipmaker()
        {
            InitializeComponent();

            treeView.AllowDrop = true;
        }

        #region 事件

        /// <summary>
        /// 选择BUTTON事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_chooseXMLpath_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "宗地模板文件(*.parcel)|*.parcel|公司模板文件(*.company)|*.company";
                openFileDialog1.ValidateNames = true;
                openFileDialog1.CheckPathExists = true;
                openFileDialog1.CheckFileExists = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox_XMLpath.Text = openFileDialog1.FileName;

                    if (createEmptyFolder(Path.GetFileNameWithoutExtension(textBox_XMLpath.Text))) //创建文件夹成功才继续下一步
                        showTemplateInfo(textBox_XMLpath.Text);

                }
                else
                {
                    textBox_XMLpath.Text = "";
                    return;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }
        }

        /// <summary>
        /// 建树
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showTemplateInfo(string template)
        {
            try
            {
                //if (radioButton_company.Checked == false && radioButton_parcel.Checked == false)
                //{
                //    MessageBox.Show("请先选择XML类型！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                treeView.Nodes.Clear();
                string ext = Path.GetExtension(template);
                if (ext.ToLower() == ".company") createTreeBYxml_company(template);//根据xml文件构建公司树
                else if (ext.ToLower() == ".parcel") createTreeBYxml_parcel(template);//根据xml文件构建宗地树
                treeView.ExpandAll();
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }
        }

        /// <summary>
        /// 创建数据文件夹
        /// </summary>
        /// <param name="name"></param>
        private bool createEmptyFolder(string name)
        {
            try
            {
                FolderBrowserDialog folderbrowserdialog1 = new FolderBrowserDialog();
                folderbrowserdialog1.Description = "所有需要更新的文件会被复制到临时产生的工作文件夹，请选择工作文件夹存放的位置";
                folderbrowserdialog1.ShowNewFolderButton = false;
                if (folderbrowserdialog1.ShowDialog() == DialogResult.OK)
                {
                    folderpath = Path.Combine(folderbrowserdialog1.SelectedPath, name);
                    if (Directory.Exists(folderpath) == false)
                    {
                        Directory.CreateDirectory(folderpath);
                        MessageBox.Show("文件夹创建成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("文件夹已存在，请删除文件夹中内容！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }
            return false;
        }

        /// <summary>
        /// 压缩BUTTON事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_createZip_Click(object sender, EventArgs e)
        {
            try
            {
                //首先，让用户选择是否真的要进行zip压缩
                DialogResult dr = MessageBox.Show("确定生成zip文件吗？", "提示信息", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.Cancel) return;
                //然后，判断文件夹路径是否存在
                if (folderpath == "")
                {
                    MessageBox.Show("请先创建数据文件夹！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (Directory.Exists(folderpath) == false)
                {
                    MessageBox.Show("文件夹不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".zip";
                dlg.Filter = "zip文件|*.zip";
                dlg.Title = "压缩文件存放路径";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //然后，选择zip存储路径
                    string zipFilePath = dlg.FileName;
                    //FolderBrowserDialog folderbrowserdialog1 = new FolderBrowserDialog();
                    //folderbrowserdialog1.ShowNewFolderButton = false;
                    //if (folderbrowserdialog1.ShowDialog() == DialogResult.OK)
                    //{
                    //    zipFilePath = folderbrowserdialog1.SelectedPath + "\\" + treeView1.Nodes[0].Text + ".zip";
                    //}

                    //然后，将所有文件复制进目标文件夹
                    copyFiles();

                    //然后，生成列表.txt
                    createTXT();

                    // 产生缩略图文件
                    pic.picHelper.batchCompressPic(folderpath, folderpath, 400, 300);

                    //最后，进行压缩
                    (new ZipHelper()).ZipFolder(folderpath, zipFilePath);
                    MessageBox.Show("压缩成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        /// <summary>
        /// 树控件节点的双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (folderpath == "")
                {
                    MessageBox.Show("请先创建数据文件夹！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                TreeNode nodeSelected = e.Node;
                NodeInfo nodeInfo = nodeSelected.Tag as NodeInfo;
                NodeType nodeType = nodeInfo.nodeType;
                if (nodeType == NodeType.ntRoot) return;//根节点
                if (nodeType == NodeType.ntFolder || nodeType == NodeType.ntBuildingFolder) return;//文件夹

                TreeNode parentnode = nodeSelected.Parent;
                NodeType parentNodeType = GetNodeType(parentnode);

                //直属于宗地文件夹的文件导入
                if (parentNodeType == NodeType.ntRoot && nodeSelected.Text != "全景")//
                {
                    //选择待上传的文件并复制到宗地文件夹的相应位置
                    OpenFileDialog openfiledialog = new OpenFileDialog();
                    openfiledialog.Filter = nodeInfo.GetFilterString();
                    openfiledialog.RestoreDirectory = true;
                    if (openfiledialog.ShowDialog() == DialogResult.OK)
                    {
                        string filepath = openfiledialog.FileName;
                        //File.Copy(filepath, Path.Combine( folderpath , openfiledialog.SafeFileName), true);

                        //把待复制的文件存入lstCopingfileModel里面，以便最后集中复制进目标文件夹
                        CopingfileModel Copingfile = new CopingfileModel();
                        Copingfile.SourcefilePath = filepath;
                        Copingfile.SourcefileFilename = nodeSelected.Text;
                        Copingfile.SourcefileSafeFileName = openfiledialog.SafeFileName;
                        Copingfile.DestFolderpath = folderpath;
                        Copingfile.Property = "file";
                        lstCopingfileModel.Add(Copingfile);

                        //treeView.SelectedNode.BackColor = Color.Green;//节点变为绿色，表示该节点的文件已经复制到相应地方。
                        nodeInfo.newPath = filepath;
                    }
                }
                //单独考虑“全景”文件
                else if (nodeSelected.Text == "全景")
                {
                    //在宗地文件夹中创建一个“全景”文件夹
                    string Panoramafolderpath_new = Path.Combine(folderpath, nodeSelected.Text);
                    if (Directory.Exists(Panoramafolderpath_new) == false)
                    {
                        Directory.CreateDirectory(Panoramafolderpath_new);
                        //MessageBox.Show("文件夹创建成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    //选择待复制的“全景”文件夹
                    FolderBrowserDialog folderbrowserdialog1 = new FolderBrowserDialog();
                    folderbrowserdialog1.ShowNewFolderButton = false;
                    folderbrowserdialog1.Description = "请选择待复制的全景文件夹";
                    if (folderbrowserdialog1.ShowDialog() == DialogResult.OK)
                    {
                        string Panoramafolderpath_old = folderbrowserdialog1.SelectedPath;
                        //IOHelper.CopyDir(Panoramafolderpath_old, Panoramafolderpath_new);//将全景文件夹更新到宗地文件夹相应地方
                        CopingfileModel copingfile = new CopingfileModel();
                        copingfile.OldFolderpath = Panoramafolderpath_old;
                        copingfile.NewFolderpath = Panoramafolderpath_new;
                        copingfile.Property = "folder";
                        lstCopingfileModel.Add(copingfile);
                        //nodeSelected.BackColor = Color.Green;//节点变为绿色，表示该节点的文件已经复制到相应地方。
                        nodeInfo.newPath = Panoramafolderpath_old;
                    }
                }
                //非直属于宗地文件夹的文件导入
                else
                {
                    //TreeNode pparentnode = parentnode.Parent;
                    string parentnodefolderPath = Path.Combine(folderpath, parentnode.Text);
                    if (Directory.Exists(parentnodefolderPath) == false)
                    {
                        Directory.CreateDirectory(parentnodefolderPath);
                        //MessageBox.Show("文件夹创建成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    //选择待上传的文件并复制到宗地文件夹的相应位置
                    OpenFileDialog openfiledialog = new OpenFileDialog();
                    openfiledialog.Filter = nodeInfo.GetFilterString();
                    openfiledialog.RestoreDirectory = true;
                    if (openfiledialog.ShowDialog() == DialogResult.OK)
                    {
                        string filepath = openfiledialog.FileName;
                        //File.Copy(filepath, Path.Combine( parentnodefolderPath ,openfiledialog.SafeFileName), true);

                        CopingfileModel Copingfile = new CopingfileModel();
                        Copingfile.SourcefilePath = filepath;
                        Copingfile.SourcefileFilename = nodeSelected.Text;
                        Copingfile.SourcefileSafeFileName = openfiledialog.SafeFileName;
                        Copingfile.DestFolderpath = parentnodefolderPath;
                        Copingfile.Property = "file";
                        lstCopingfileModel.Add(Copingfile);

                        //nodeSelected.BackColor = Color.Green;//节点变为绿色，表示该节点的文件已经复制到相应地方。
                        nodeInfo.newPath = filepath;
                    }
                }
                //MessageBox.Show("文件更新成功！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }
        }

        /// <summary>
        /// 开始拖放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                //只有房产文件夹下的图片才可以拖放
                if (GetNodeType(((TreeNode)e.Item).Parent) != NodeType.ntBuildingFolder) return;

                DoDragDrop(e.Item, DragDropEffects.Move);
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }
        }

        /// <summary>
        /// DragEnter事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            try
            {

                if (e.Data.GetDataPresent(typeof(TreeNode)))
                {
                    e.Effect = DragDropEffects.Move;
                }
                else
                    e.Effect = DragDropEffects.None;
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        /// <summary>
        /// 拖放结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                TreeNode myNode = null;
                if (e.Data.GetDataPresent(typeof(TreeNode)))
                {
                    myNode = (TreeNode)(e.Data.GetData(typeof(TreeNode)));
                }
                else
                {
                    return;
                }
                Point pt = new Point(e.X, e.Y);
                pt = treeView.PointToClient(pt);
                TreeNode DropNode = this.treeView.GetNodeAt(pt);

                // 1.目标节点不是空。2.目标节点是被拖拽接点的兄弟结点。3.目标节点不是被拖拽节点本身
                if (DropNode != null && DropNode.Parent == myNode.Parent && DropNode != myNode)
                {
                    myNode.Remove();
                    // 在目标节点下增加被拖拽节点
                    DropNode.Parent.Nodes.Insert(DropNode.Index, myNode);
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        /// <summary>
        /// 结点右击处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {

                TreeNode curTreeNode = e.Node;//当前结点
                if (curTreeNode != null)
                {
                    treeView.SelectedNode = curTreeNode;
                    if (e.Button == MouseButtons.Right)
                    {
                        ContextMenuStrip mnuPopup = new ContextMenuStrip();
                        BuildMenu(mnuPopup, curTreeNode);
                        mnuPopup.Show(treeView, e.Location);
                        return;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }


        }

        /// <summary>
        /// 菜单项响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClick(object sender, EventArgs e)
        {
            try
            {

                ToolStripItem item = sender as ToolStripItem;
                ICommand cmd = item.Tag as ICommand;
                if (cmd != null)
                {
                    cmd.Run();
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }
        /// <summary>
        /// 结点开始编辑事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            try
            {
                if (GetNodeType(e.Node) == NodeType.ntFloorJPGFile)
                    e.CancelEdit = false;
                else
                    e.CancelEdit = true;
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }
        }

        /// <summary>
        /// 结点结束编辑事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            try
            {
                foreach (TreeNode node in e.Node.Parent.Nodes)
                {
                    if (node != e.Node && string.Compare(node.Text, e.Label, true) == 0)//不能重名
                    {
                        e.CancelEdit = true;
                        MessageBox.Show("文件重名，请重新命名！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                //todo:关联文件重命名 ，如果是服务器已经存在的,可以根据列表.txt文件的内容，更新为新名字和新文件
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        private void treeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            try
            {

                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null)
                    nodeFont = ((TreeView)sender).Font;
                Brush textBrush = SystemBrushes.WindowText;

                NodeInfo nodeInfo = e.Node.Tag as NodeInfo;
                if (nodeInfo.nodeType != NodeType.ntFloorJPGFile)//只能编辑分层平面图名字
                    textBrush = SystemBrushes.GrayText;             //如果不可编辑，则将字体颜色置灰
                if (nodeInfo.nodeType != NodeType.ntRoot && nodeInfo.nodeType != NodeType.ntFolder && nodeInfo.nodeType != NodeType.ntBuildingFolder)
                {
                    if (nodeInfo.existedInServer == false) textBrush = new SolidBrush(Color.Red);
                    if (nodeInfo.newPath != "") textBrush = new SolidBrush(Color.Green);
                }
                //反色突出显示
                if ((e.State & TreeNodeStates.Focused) != 0)
                    textBrush = SystemBrushes.Window;

                //不限定文本区域，以免大字体时长文本被截取
                e.Graphics.DrawString(e.Node.Text, nodeFont, textBrush, e.Node.Bounds.Left, e.Node.Bounds.Top);
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        //private void treeView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        //{
        //    if (e.Effect == DragDropEffects.Move)
        //    {
        //        // Show pointer cursor while dragging   
        //        e.UseDefaultCursors = false;
        //        this.treeView.Cursor = Cursors.Hand;
        //    }
        //    else e.UseDefaultCursors = true;   
        //}

        #endregion

        #region 功能函数

        /// <summary>
        /// 获取结点类型
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static NodeType GetNodeType(TreeNode node)
        {
            return ((NodeInfo)node.Tag).nodeType;
        }

        /// <summary>
        /// 设置结点图标等属性
        /// </summary>
        /// <param name="node"></param>
        public static void SetNodeProperty(TreeNode node)
        {
            try
            {
                switch (GetNodeType(node))
                {
                    case NodeType.ntRoot:
                        node.ImageIndex = 0;
                        node.SelectedImageIndex = 0;
                        break;
                    case NodeType.ntFolder:
                    case NodeType.ntBuildingFolder:
                        node.ImageIndex = 1;
                        node.SelectedImageIndex = 1;
                        break;
                    case NodeType.ntJPGFile:
                    case NodeType.ntFloorJPGFile:
                    case NodeType.ntParcelFlvFile:
                    case NodeType.ntCompanyTxtFile:
                    case NodeType.ntBuildingCADFile:
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        /// <summary>
        /// 根据xml文件构建公司树
        /// </summary>
        /// <param name="xmlPath"></param>
        public void createTreeBYxml_company(string xmlPath)
        {
            try
            {
                XmlDocument XMLdoc = new XmlDocument();
                XMLdoc.Load(xmlPath);
                XmlNode XMLrootnode = XMLdoc.SelectSingleNode("company");//得到XML根节点
                XmlNodeList XMLrootchildnodes = XMLrootnode.ChildNodes;//得到根节点的所有子节点

                XMLrootnode = (XmlElement)XMLrootnode;
                //添加树的根节点
                TreeNode rootnode = new TreeNode();
                rootnode.Text = XMLrootnode.Attributes["companyname"].Value;
                rootnode.Tag = new NodeInfo(NodeType.ntRoot);
                SetNodeProperty(rootnode);
                treeView.Nodes.Add(rootnode);

                foreach (XmlNode XMLrootchild in XMLrootchildnodes)
                {
                    XmlElement XMLrootchildelement = (XmlElement)XMLrootchild;
                    if (XMLrootchildelement.Name == "file")
                    {
                        NodeInfo nodeInfo = new NodeInfo(
                            NodeInfo.GetNodeType(XMLrootchildelement.GetAttribute("type").ToString()),
                            XMLrootchildelement.GetAttribute("filename").ToString(),
                            bool.Parse(XMLrootchildelement.GetAttribute("exist").ToString())
                            );

                        TreeNode companyfilenode = new TreeNode();//添加公司文件夹内的文件
                        companyfilenode.Text = nodeInfo.nodeName;
                        companyfilenode.Tag = nodeInfo;
                        SetNodeProperty(companyfilenode);
                        if (!nodeInfo.existedInServer) companyfilenode.BackColor = Color.Red;//如果文件不存在则节点颜色为红色
                        rootnode.Nodes.Add(companyfilenode);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        /// <summary>
        /// 根据xml文件构建宗地树
        /// </summary>
        /// <param name="xmlPath"></param>
        public void createTreeBYxml_parcel(string xmlPath)
        {
            try
            {
                XmlDocument XMLdoc = new XmlDocument();
                XMLdoc.Load(xmlPath);
                XmlNode XMLrootnode = XMLdoc.SelectSingleNode("parcel");//得到XML根节点
                XmlNodeList XMLrootchildnodes = XMLrootnode.ChildNodes;//得到根节点的所有子节点

                //treeView1.LabelEdit = true;

                XMLrootnode = (XmlElement)XMLrootnode;
                //添加树的根节点
                TreeNode rootnode = new TreeNode();
                rootnode.Text = XMLrootnode.Attributes["parcelname"].Value;
                rootnode.Tag = new NodeInfo(NodeType.ntRoot);
                SetNodeProperty(rootnode);
                treeView.Nodes.Add(rootnode);

                foreach (XmlNode XMLrootchild in XMLrootchildnodes)
                {
                    XmlElement XMLrootchildelement = (XmlElement)XMLrootchild;
                    if (XMLrootchildelement.Name == "file")
                    {
                        NodeInfo nodeInfo = new NodeInfo(
                            NodeInfo.GetNodeType(XMLrootchildelement.GetAttribute("type").ToString()),
                            XMLrootchildelement.GetAttribute("filename").ToString(),
                            bool.Parse(XMLrootchildelement.GetAttribute("exist").ToString())
                            );

                        TreeNode parcelfilenode = new TreeNode();//添加宗地文件夹内的文件
                        parcelfilenode.Text = nodeInfo.nodeName;
                        parcelfilenode.Tag = nodeInfo;
                        SetNodeProperty(parcelfilenode);
                        //if (!nodeInfo.existedInServer) parcelfilenode.BackColor = Color.Red;//如果文件不存在则节点颜色为红色
                        rootnode.Nodes.Add(parcelfilenode);
                    }
                    else if (XMLrootchildelement.Name == "folder")
                    {
                        TreeNode parcelfoldernode = new TreeNode();//添加宗地文件夹内的文件夹

                        {
                            NodeInfo nodeInfo = new NodeInfo(
                            NodeType.ntFolder,
                            XMLrootchildelement.GetAttribute("foldername").ToString()
                            );
                            if (nodeInfo.nodeName == "全景" || nodeInfo.nodeName == "鸟瞰图")
                                nodeInfo.nodeType = NodeType.ntFolder;
                            else
                                nodeInfo.nodeType = NodeType.ntBuildingFolder;

                            parcelfoldernode.Text = nodeInfo.nodeName;
                            parcelfoldernode.Tag = nodeInfo;

                            SetNodeProperty(parcelfoldernode);
                            //if (!nodeInfo.existedInServer)
                            //{
                            //    parcelfoldernode.BackColor = Color.Red;//如果文件夹不存在则节点颜色为红色
                            //    rootnode.Nodes.Add(parcelfoldernode);
                            //    continue;
                            //}
                            rootnode.Nodes.Add(parcelfoldernode);
                        }

                        XmlNodeList XMLfolderchildnodes = XMLrootchildelement.ChildNodes;//得到文件夹内的所有文件节点
                        foreach (XmlNode XMLfolderchild in XMLfolderchildnodes)//添加文件夹内的所有文件节点到树控件
                        {
                            XmlElement XMLfolderchildelement = (XmlElement)XMLfolderchild;
                            if (XMLfolderchildelement.Name == "file")
                            {
                                NodeInfo nodeInfo = new NodeInfo(
                                    NodeInfo.GetNodeType(XMLfolderchildelement.GetAttribute("type").ToString()),
                                    XMLfolderchildelement.GetAttribute("filename").ToString(),
                                    bool.Parse(XMLfolderchildelement.GetAttribute("exist").ToString())
                                    );

                                TreeNode folderfilenode = new TreeNode();
                                folderfilenode.Text = nodeInfo.nodeName;
                                folderfilenode.Tag = nodeInfo;
                                SetNodeProperty(folderfilenode);
                                //if (!nodeInfo.existedInServer) folderfilenode.BackColor = Color.Red;//如果文件不存在则节点颜色为红色
                                parcelfoldernode.Nodes.Add(folderfilenode);
                            }
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }
        }

        /// <summary>
        /// 生成列表.txt文件
        /// </summary>
        public void createTXT()
        {
            try
            {

                TreeNode rootnode = treeView.Nodes[0];//得到根节点
                string BuildingfolderPath = "";
                foreach (TreeNode tn in rootnode.Nodes)
                {
                    if (tn.Nodes.Count != 0 && tn.Text != "鸟瞰图")//判断是否为建筑节点
                    {
                        BuildingfolderPath = Path.Combine(folderpath, tn.Text);//建筑文件夹路径
                        if (Directory.Exists(BuildingfolderPath))
                        {
                            createBuildingFloorListFile(tn, BuildingfolderPath + "\\列表.txt");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        /// <summary>
        /// 创建分层平面图 列表.txt 文件
        /// </summary>
        /// <param name="buildingNode">建筑节点</param>
        /// <param name="path">建筑文件夹路径</param>
        private void createBuildingFloorListFile(TreeNode buildingNode, string path)
        {

            try
            {
                NodeInfo nodeInfo = buildingNode.Tag as NodeInfo;
                Debug.Assert(nodeInfo.nodeType == NodeType.ntBuildingFolder);

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    foreach (TreeNode child in buildingNode.Nodes)
                    {
                        if (child.Text != "分层CAD图" && child.Text != "房产证" && child.Text != "外墙实景图")
                        {
                            NodeInfo childNodeInfo = child.Tag as NodeInfo;
                            if (childNodeInfo.existedInServer || !string.IsNullOrEmpty(childNodeInfo.newPath))
                            {
                                sw.WriteLine(childNodeInfo.nodeName);
                            }
                        }

                    }
                    sw.Close();
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }


        }

        /// <summary>
        /// 为当前结点构建右键菜单并弹出
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="node"></param>
        private void BuildMenu(ContextMenuStrip menu, TreeNode node)
        {
            try
            {
                menu.Items.Clear();
                switch (GetNodeType(node))
                {
                    case NodeType.ntBuildingFolder:
                        AddItem(menu, "添加分层平面图", node, NodeOperation.noAdd);
                        break;
                    case NodeType.ntFloorJPGFile:
                        AddItem(menu, "编辑", node, NodeOperation.noEdit);
                        AddItem(menu, "删除", node, NodeOperation.noDelete);
                        if (node.Index > 0)
                        {
                            AddItem(menu, "向上", node, NodeOperation.noUp);
                        }
                        if (node.Index < node.Parent.Nodes.Count - 1)
                        {
                            AddItem(menu, "向下", node, NodeOperation.noDown);
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }

        }

        /// <summary>
        /// 添加菜单项
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="text"></param>
        /// <param name="node"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        private ToolStripItem AddItem(ContextMenuStrip menu, string text, TreeNode node, NodeOperation no)
        {
            ToolStripItem item = new ToolStripMenuItem();
            try
            {
                ICommand cmd = CoreHelper.CreateCommand(typeof(NodeCommand), "node", node, "NodeOperation", no);
                item.Text = text;
                item.Tag = cmd;
                item.Click += new EventHandler(OnClick);
                menu.Items.Add(item);
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }
            return item;
        }

        /// <summary>
        /// 把文件集中复制进目标文件夹
        /// </summary>
        private void copyFiles()
        {
            try
            {
                foreach (CopingfileModel Copingfile in lstCopingfileModel)
                {
                    if (Copingfile.Property == "file")
                    {
                        File.Copy(Copingfile.SourcefilePath, Path.Combine(Copingfile.DestFolderpath, Copingfile.SourcefileSafeFileName), true);
                    }
                    else if (Copingfile.Property == "folder")
                    {
                        IOHelper.CopyDir(Copingfile.OldFolderpath, Copingfile.NewFolderpath);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(zipmaker), ex);
            }


        }

        #endregion

    }
}
