using System;
using System.Diagnostics;
using System.Reflection;

namespace ZipMaker
{
    /// <summary>
    /// һ�������̬��
    /// </summary>
    public static class CoreHelper
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CoreHelper));

        /// <summary>
        /// ͨ��������������Ҫ�����Ĭ�Ϲ��캯��
        /// </summary>
        /// <param name="assembly">����</param>
        /// <param name="className">����</param>
        /// <returns>����</returns>
        public static object CreateClassInstance(Assembly assembly, string className)
        {
            Debug.Assert(assembly != null);

            Type type = assembly.GetType(className);
            return CreateClassInstance(type);
        }

        /// <summary>
        /// ͨ��������������Ҫ�����Ĭ�Ϲ��캯��
        /// </summary>
        /// <param name="assemblyPath">����·��</param>
        /// <param name="className">����</param>
        /// <returns>����</returns>
        public static object CreateClassInstance(string assemblyPath, string className)
        {
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            return CreateClassInstance(assembly, className);
        }

        /// <summary>
        /// ͨ��������������,���캯�����Դ�����
        /// </summary>
        /// <param name="type">������</param>
        /// <param name="args">�������</param>
        /// <returns>����</returns>
        public static object CreateClassInstance(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// �����������ͺͶ���������Ϣ��������
        /// </summary>
        /// <param name="cmdType">��������</param>
        /// <param name="customPropertySet">����������Ϣ</param>
        /// <returns>����</returns>
        public static ICommand CreateCommand(Type cmdType, params object[] customPropertySet)
        {
            log.DebugFormat("{0} ��ʼ", MethodBase.GetCurrentMethod().Name);

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