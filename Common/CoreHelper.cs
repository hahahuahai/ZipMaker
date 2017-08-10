using System;
using System.Diagnostics;
using System.Reflection;

namespace ZipMaker
{
    /// <summary>
    /// 一般帮助静态类
    /// </summary>
    public static class CoreHelper
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CoreHelper));

        /// <summary>
        /// 通过类名创建对象，要求具有默认构造函数
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="className">类名</param>
        /// <returns>对象</returns>
        public static object CreateClassInstance(Assembly assembly, string className)
        {
            Debug.Assert(assembly != null);

            Type type = assembly.GetType(className);
            return CreateClassInstance(type);
        }

        /// <summary>
        /// 通过类名创建对象，要求具有默认构造函数
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <param name="className">类名</param>
        /// <returns>对象</returns>
        public static object CreateClassInstance(string assemblyPath, string className)
        {
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            return CreateClassInstance(assembly, className);
        }

        /// <summary>
        /// 通过类名创建对象,构造函数可以带参数
        /// </summary>
        /// <param name="type">类类型</param>
        /// <param name="args">构造参数</param>
        /// <returns>对象</returns>
        public static object CreateClassInstance(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// 根据命令类型和定制属性信息创建命令
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="customPropertySet">定制属性信息</param>
        /// <returns>命令</returns>
        public static ICommand CreateCommand(Type cmdType, params object[] customPropertySet)
        {
            log.DebugFormat("{0} 开始", MethodBase.GetCurrentMethod().Name);

            ICommand cmd = CreateClassInstance(cmdType) as ICommand;
            Debug.Assert(cmd != null);

            if (cmd != null)
            {
                if (customPropertySet != null)
                {
                    for (int i = 0; i < customPropertySet.Length; i += 2)
                    {
                        log.DebugFormat("property = {0} ", customPropertySet[i]);
                        log.DebugFormat("values = {0} ", customPropertySet[i + 1]);
                        cmd.setCustomProperty(customPropertySet[i], customPropertySet[i + 1]);
                    }
                }
                return cmd;
            }
            else
            {
                return null;
            }
        }
    };
}