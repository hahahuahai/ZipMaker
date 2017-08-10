using System;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <returns>是否成功，0表示失败</returns>
        int Run();

        /// <summary>
        /// 获取定制属性
        /// </summary>
        /// <param name="Name">属性名</param>
        /// <returns>属性值</returns>
        object getCustomProperty( object Name);

        /// <summary>
        /// 设置定制属性
        /// </summary>
        /// <param name="Name">属性名</param>
        /// <param name="Value">属性值</param>
        void setCustomProperty(object Name, object Value);
    }
}
