using System;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// ִ������
        /// </summary>
        /// <returns>�Ƿ�ɹ���0��ʾʧ��</returns>
        int Run();

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="Name">������</param>
        /// <returns>����ֵ</returns>
        object getCustomProperty( object Name);

        /// <summary>
        /// ���ö�������
        /// </summary>
        /// <param name="Name">������</param>
        /// <param name="Value">����ֵ</param>
        void setCustomProperty(object Name, object Value);
    }
}
