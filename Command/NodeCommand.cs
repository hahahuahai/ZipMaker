using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ZipMaker
{

    /// <summary>
    /// 结点操作类型
    /// </summary>
    public enum NodeOperation
    {
        noAdd,
        noDelete,
        noEdit,
        noUp,
        noDown
    };

    /// <summary>
    /// 结点命令
    /// </summary>
    public class NodeCommand : AbstractCommand
    {
        /// <summary>
        /// 自动创建文件名，保证不重复
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        string CreateChildName(TreeNode parent)
        {
            string prefix = "未命名";
            bool nameExisted = false;

            int step = 1;
            string name;
            do
            {
                nameExisted = false;
                name = prefix + (step++).ToString();
                foreach (TreeNode node in parent.Nodes)
                {
                    if (node.Text == name)
                    {
                        nameExisted = true;
                        break;
                    }
                }
            } while (nameExisted);

            return name;
        }

        /// <summary>
        /// 点击菜单右键菜单项的响应函数
        /// </summary>
        /// <returns></returns>
        public override int Run()
        {
            TreeNode curNode = getCustomProperty("node") as TreeNode;
            TreeNode parentNode = null;
            NodeOperation no = (NodeOperation)getCustomProperty("NodeOperation");
            TreeNode node = null;
            int index;

            switch (no)
            {
                case NodeOperation.noAdd:
                    node = new TreeNode(CreateChildName(curNode));
                    node.Tag = new NodeInfo( NodeType.ntFloorJPGFile , node.Text ,false);
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = node.ImageIndex;

                    curNode.Nodes.Add(node);
                    node.BeginEdit();
                    break;
                case NodeOperation.noEdit:
                    curNode.BeginEdit();
                    break;
                case NodeOperation.noDelete:
                    curNode.Remove();
                    break;
                case NodeOperation.noUp:
                    index = curNode.Index;
                    parentNode = curNode.Parent;
                    curNode.Remove();
                    parentNode.Nodes.Insert(index - 1, curNode);
                    break;
                case NodeOperation.noDown:
                    index = curNode.Index;
                    parentNode = curNode.Parent;
                    curNode.Remove();
                    parentNode.Nodes.Insert(index + 1, curNode);
                    break;
            }
            return 1;
        }

    }
}
