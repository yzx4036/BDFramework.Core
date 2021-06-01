using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using BDFramework.Editor.Asset;
using BDFramework.Editor.BuildPackage;
using BDFramework.Core.Tools;
using BDFramework.Editor;
using UnityEditor.SceneManagement;

namespace BDFramework.Editor
{
    static public class EditorBuildPackage
    {

        public enum BuildMode
        {
            Debug = 0,
            Release,
        }
        static string SCENEPATH="Assets/Scenes/BDFrame.unity";
        static string[] SceneConfigs =
        {
            "Assets/Scenes/Config/Debug.json",
            "Assets/Scenes/Config/Release.json",
        };


        static EditorBuildPackage()
        {
            //初始化框架编辑器下
            BDFrameEditorLife.InitBDFrameworkEditor();
        }
        

        [MenuItem("BDFrameWork工具箱/打包/BuildAPK(使用当前配置)")]
        public static void EditorBuildAPK_Empty()
        {
            LoadConfig();
            BuildAPK_Empty();
        }

        [MenuItem("BDFrameWork工具箱/打包/BuildAPK(Config-Debug.json)")]
        public static void EditorBuildAPK_Debug()
        {
            if (EditorUtility.DisplayDialog("提示", "此操作会重新编译资源,是否继续？", "OK", "Cancel"))
            {
                BuildAPK_Debug();
            }
          
        }

        [MenuItem("BDFrameWork工具箱/打包/BuildAPK(Config-Release.json)")]
        public static void EditorBuildAPK()
        {
            if (EditorUtility.DisplayDialog("提示", "此操作会重新编译资源,是否继续？", "OK", "Cancel"))
            {
                BuildAPK_Release();
            }
        }
        
        [MenuItem("BDFrameWork工具箱/打包/导出XCode工程(ipa暂未实现)")]
        public static void EditorBuildIpa()
        {
            BuildIpa();
        }





        
        /// <summary>
        /// 加载场景配置
        /// </summary>
        /// <param name="mode"></param>
       
        static public void LoadConfig(BuildMode? mode=null)
        {
            var  scene=  EditorSceneManager.OpenScene(SCENEPATH);
            TextAsset textContent = null;
            if (mode != null)
            {
                string path = SceneConfigs[(int)mode];
                textContent  = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                var config = GameObject.FindObjectOfType<BDLauncher>();
                config.ConfigText = textContent;
            }
            EditorSceneManager.SaveScene(scene);
        }




        #region Android

        /// <summary>
        /// 构建包体，
        /// </summary>
        static public void BuildAPK_Empty()
        {
            LoadConfig();
            BuildAPK(BuildMode.Debug);
        }

        /// <summary>
        /// 构建Debug包体
        /// </summary>
        static public void BuildAPK_Debug()
        {
            LoadConfig(BuildMode.Debug);
            EditorWindow_OnekeyBuildAsset.GenAllAssets(Application.streamingAssetsPath,RuntimePlatform.Android, BuildTarget.Android);
            BuildAPK(BuildMode.Debug);
        }
        
        /// <summary>
        /// 构建Release包体
        /// </summary>
        static public void BuildAPK_Release()
        {
            LoadConfig(BuildMode.Release);
            EditorWindow_OnekeyBuildAsset.GenAllAssets(Application.streamingAssetsPath,RuntimePlatform.Android, BuildTarget.Android);
            BuildAPK(BuildMode.Release);
        }

        
        /// <summary>
        /// 打包APK
        /// </summary>
        static public void BuildAPK(BuildMode mode)
        {

            if (!BDEditorApplication.BdFrameEditorSetting.IsSetConfig())
            {
                BDebug.LogError("请注意设置apk keystore账号密码");
                return;
            }

            var absroot = Application.dataPath.Replace("Assets", "");
            PlayerSettings.Android.keystoreName =absroot  + BDEditorApplication.BdFrameEditorSetting.Android.keystoreName;
            PlayerSettings.keystorePass =  BDEditorApplication.BdFrameEditorSetting.Android.keystorePass;
            PlayerSettings.Android.keyaliasName= BDEditorApplication.BdFrameEditorSetting.Android.keyaliasName;
            PlayerSettings.keyaliasPass =  BDEditorApplication.BdFrameEditorSetting.Android.keyaliasPass;
            //具体安卓的配置
            PlayerSettings.gcIncremental                    = true;
            PlayerSettings.stripEngineCode                  = false;
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            //
            var outdir = BDApplication.ProjectRoot + "/Build";
            var outputPath = IPath.Combine(  outdir,  Application.productName+".apk");
            //文件夹处理
            if (!Directory.Exists(outdir)) Directory.CreateDirectory(outdir);
            if(File.Exists(outputPath)) File.Delete(outputPath);
            //清空StreamingAsset
            var ios = IPath.Combine(Application.streamingAssetsPath, "iOS");
            if (Directory.Exists(ios))
            {
                Directory.Delete(ios, true);
            }
            var win = IPath.Combine(Application.streamingAssetsPath, "Windows");
            if (Directory.Exists(win)) 
            {
                Directory.Delete(win, true);
            }
            //开始项目一键打包
            string[] scenes = { SCENEPATH };
            BuildOptions opa = BuildOptions.None;
            if (mode == BuildMode.Debug)
            {
                opa = BuildOptions.CompressWithLz4HC | BuildOptions.Development | BuildOptions.AllowDebugging|
                      BuildOptions.ConnectWithProfiler| BuildOptions.EnableDeepProfilingSupport;
            }
            else if(mode == BuildMode.Release)
            {
                opa = BuildOptions.CompressWithLz4HC;
            }
            
            UnityEditor.BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.Android,opa);
            if (File.Exists(outputPath))
            {
                Debug.Log( "Build Success :" + outputPath);
                EditorUtility.RevealInFinder(outputPath);
            }
            else
            {
                Debug.LogError(new Exception("Build Fail! Please Check the log! "));
            }
        }
        
        
        #endregion
        
        #region iOS
        /// <summary>
        /// build Ipa
        /// </summary>
        static public void BuildIpa()
        {
           
        }
        
        static public void BuildIpa_Empty()
        {
           
        }
        static public void BuildIpa_Debug()
        {
           
        }
        
        static public void BuildIpa_Release()
        {
           
        }
        #endregion
    }
}