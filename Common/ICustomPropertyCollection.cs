using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker
{
    public interface ICustomPropertyCollection : IEnumerable
    {
        /// <summary>
        /// 获取自定义数据
        /// </summary>
        object getCustomProperty(object Name);

        /// <summary>
        /// 设置自定义数据
        /// </summary>
        void setCustomProperty(object Name, object Value);

        /// <summary>
        /// 一次设置多个自定义数据
        /// </summary>
        void setCustomProperty( params object[] msgKey_Values );

        /// <summary>
        /// 是否存在指定名字的属性
        /// </summary>
        /// <param name="Name">指定名字的属性</param>
        /// <returns>是否存在指定名字的属性</returns>
        bool HasCustomProperty(object Name);
    }
}
