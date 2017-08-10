using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker
{
    /// <summary>
    /// command����
    /// </summary>
    public abstract class AbstractCommand : ICommand, ICustomPropertyCollection
    {
        public abstract int Run();

        /// <summary>
        /// ��ȡ�Զ�������
        /// </summary>
        public virtual object getCustomProperty(object Name)
        {
            return CustomPropertyCollection.getCustomProperty(Name);
        }

        /// <summary>
        /// �����Զ�������
        /// </summary>
        public virtual void setCustomProperty(object Name, object Value)
        {
            CustomPropertyCollection.setCustomProperty(Name, Value);
        }

        /// <summary>
        /// һ�����ö���Զ�������
        /// </summary>
        public virtual void setCustomProperty( params object[] msgKey_Values )
        {
            CustomPropertyCollection.setCustomProperty( msgKey_Values );
        }

        /// <summary>
        /// �Ƿ����ָ�����ֵ�����
        /// </summary>
        /// <param name="Name">ָ�����ֵ�����</param>
        /// <returns>�Ƿ����ָ�����ֵ�����</returns>
        public bool HasCustomProperty(object Name)
        {
            return CustomPropertyCollection.HasCustomProperty(Name);
        }

        public virtual IEnumerator GetEnumerator()
        {
            // TODO:  Add StringCollection.GetEnumerator implementation
            return (IEnumerator)CustomPropertyCollection.GetEnumerator();
        }

        /// <summary>
        /// �������Լ���
        /// </summary>
        ICustomPropertyCollection CustomPropertyCollection = new CustomPropertyCollectionBase();
    }
}
