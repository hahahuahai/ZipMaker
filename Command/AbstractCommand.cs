using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker
{
    /// <summary>
    /// command基类
    /// </summary>
    public abstract class AbstractCommand : ICommand, ICustomPropertyCollection
    {
        public abstract int Run();

        /// <summary>
        /// 获取自定义数据
        /// </summary>
        public virtual object getCustomProperty(object Name)
        {
            return CustomPropertyCollection.getCustomProperty(Name);
        }

        /// <summary>
        /// 设置自定义数据
        /// </summary>
        public virtual void setCustomProperty(object Name, object Value)
        {
            CustomPropertyCollection.setCustomProperty(Name, Value);
        }

        /// <summary>
        /// 一次设置多个自定义数据
        /// </summary>
        public virtual void setCustomProperty( params object[] msgKey_Values )
        {
            CustomPropertyCollection.setCustomProperty( msgKey_Values );
        }

        /// <summary>
        /// 是否存在指定名字的属性
        /// </summary>
        /// <param name="Name">指定名字的属性</param>
        /// <returns>是否存在指定名字的属性</returns>
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
        /// 定制属性集合
        /// </summary>
        ICustomPropertyCollection CustomPropertyCollection = new CustomPropertyCollectionBase();
    }
}
