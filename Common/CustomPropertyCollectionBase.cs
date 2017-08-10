using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZipMaker
{
    public class CustomPropertyCollectionBase: ICustomPropertyCollection
    {
        /// <summary>
        /// 获取自定义数据
        /// </summary>
        public virtual object getCustomProperty(object Name)
        {
            if (NameValuePair.ContainsKey(Name))
            {
                return NameValuePair[Name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置自定义数据
        /// </summary>
        public virtual void setCustomProperty(object Name, object Value)
        {
            NameValuePair[Name] = Value;
        }

        /// <summary>
        /// 一次设置多个自定义数据
        /// </summary>
        public virtual void setCustomProperty( params object[] msgKey_Values )
        {
            for ( int i = 0 ; i < msgKey_Values.Length ; i += 2 )
            {
                setCustomProperty( msgKey_Values[i] , msgKey_Values[i + 1] );
            }
        }

        /// <summary>
        /// 是否存在指定名字的属性
        /// </summary>
        /// <param name="Name">指定名字的属性</param>
        /// <returns>是否存在指定名字的属性</returns>
        public bool HasCustomProperty(object Name)
        {
            return NameValuePair.ContainsKey(Name);
        }

        public virtual IEnumerator GetEnumerator()
        {
            // TODO:  Add StringCollection.GetEnumerator implementation
            return (IEnumerator)NameValuePair.Keys.GetEnumerator();
        }

        Dictionary<object, object> NameValuePair = new Dictionary<object, object>();

    }
}
