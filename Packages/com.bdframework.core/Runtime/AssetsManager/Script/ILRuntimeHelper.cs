﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ILRuntime.Mono.Cecil.Pdb;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;


namespace BDFramework
{
    static public class ILRuntimeHelper
    {
        public static AppDomain AppDomain { get; private set; }
        public static bool IsRunning { get; private set; }

        static private FileStream fsDll = null;
        static private FileStream fsPdb = null;


        /// <summary>
        /// 加载Hotfix程序集
        /// </summary>
        /// <param name="dllPath"></param>
        /// <param name="gamelogicBindAction">游戏逻辑测注册</param>
        /// <param name="isDoCLRBinding"></param>
        public static void LoadHotfix(string dllPath,
            Action<bool> gamelogicBindAction = null,
            bool isDoCLRBinding = true)
        {
            //
            IsRunning = true;
            BDebug.Log("DLL加载路径:" + dllPath, "red");
            //
            string pdbPath = dllPath + ".pdb";
            AppDomain = new AppDomain();
            if (File.Exists(pdbPath))
            {
                //这里的流不能释放，头铁的老哥别试了
                fsDll = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
                fsPdb = new FileStream(pdbPath, FileMode.Open, FileAccess.Read);
                AppDomain.LoadAssembly(fsDll, fsPdb, new PdbReaderProvider());
            }
            else
            {
                //这里的流不能释放，头铁的老哥别试了
                fsDll = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
                AppDomain.LoadAssembly(fsDll);
            }


#if UNITY_EDITOR
            AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            //
            gamelogicBindAction?.Invoke(isDoCLRBinding);
            //
            JsonMapper.RegisterILRuntimeCLRRedirection(AppDomain);
            if (BDLauncher.Inst != null && BDLauncher.Inst.GameConfig.IsDebuggerILRuntime)
            {
                AppDomain.DebugService.StartDebugService(56000);
                Debug.Log("热更调试器 准备待命~");
            }

            //
            AppDomain.Invoke("HotfixCheck", "Log", null, null);
        }

        /// <summary>
        /// ILRuntime卸载
        /// </summary>
        public static void Close()
        {
            AppDomain = null;

            if (fsDll != null)
            {
                fsDll.Close();
                fsDll.Dispose();
            }

            if (fsPdb != null)
            {
                fsPdb.Close();
                fsPdb.Dispose();
            }
        }

        #region hotfix类型

        static private List<Type> hotfixTypeList = null;

        /// <summary>
        /// 获取所有的hotfix的类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetHotfixTypes()
        {
            if (hotfixTypeList == null)
            {
                hotfixTypeList = new List<Type>();
                var values = ILRuntimeHelper.AppDomain.LoadedTypes.Values.ToList();
                foreach (var v in values)
                {
                    hotfixTypeList.Add(v.ReflectionType);
                }
            }

            return hotfixTypeList;
        }

        #endregion


        /// <summary>
        /// 所有UIComponent的类型
        /// </summary>
        public static Dictionary<string, Type> UIComponentTypes { get; set; } = new Dictionary<string, Type>();

        #region 辅助工具类

        /// <summary>
        /// 打印堆栈
        /// </summary>
        static public void LogStackTrace()
        {
            // ILRuntimeHelper.AppDomain.DebugService.GetStackTrace()
        }


        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="value_type"></param>
        /// <returns></returns>
        static public object CreateILRuntimeInstance(Type value_type)
        {
            object instance;
            if (value_type is ILRuntime.Reflection.ILRuntimeType)
            {
                instance = ((ILRuntime.Reflection.ILRuntimeType) value_type).ILType.Instantiate();
            }
            else
            {
                if (value_type is ILRuntime.Reflection.ILRuntimeWrapperType)
                    value_type = ((ILRuntime.Reflection.ILRuntimeWrapperType) value_type).RealType;
                instance = Activator.CreateInstance(value_type);
            }

            return instance;
        }

        #endregion
    }
}