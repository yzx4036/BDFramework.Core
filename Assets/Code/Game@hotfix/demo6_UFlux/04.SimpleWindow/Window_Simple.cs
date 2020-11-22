﻿using BDFramework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BDFramework.UFlux.UFluxTest004
{
    public enum WinMsg
    {
        testMsg = 0,
    }

    public enum SubWindow
    {
        testSubWindows001
    }

    /// <summary>
    /// 这个是最简单的窗口模型，
    /// 可以基于这个做任何逻辑，
    /// 不强制使用flux
    /// </summary>
    [UI((int)  WinEnum.Win_Demo6_Test004, "Windows/UFlux/demo004/Window_SimpleWindow")]
    public class Window_Simple : AWindow
    {
        public Window_Simple(string path) : base(path)
        {
        }

        public Window_Simple(Transform transform) : base(transform)
        {
        }

        [TransformPath("btn_OpenSubWin")]
        private Button btn_OpenSubWin;

        [TransformPath("btn_CloseSubWin")]
        private Button btn_CloseSubWin;

        [TransformPath("btn_SendMessage")]
        private Button btn_SndMessage;
        [TransformPath("btn_Close")]
        private Button btn_close;
        [TransformPath("Content")]
        private Text Content;

        public override void Init()
        {
            base.Init();

            //注册子窗口
            var trans = this.Transform.Find("SubWindow");
            RegisterSubWindow(new SubWindow_Demo004(trans));
            btn_close.onClick.AddListener((() => this.Close()));
            //点击测试 
            btn_OpenSubWin.onClick.AddListener(() =>
            {
                GetSubWindow<SubWindow_Demo004>().Open();
            });

            btn_CloseSubWin.onClick.AddListener(() =>
            {
                GetSubWindow<SubWindow_Demo004>().Close();
            });

            btn_SndMessage.onClick.AddListener(() =>
            {
                var msg = new UIMessageData(WinMsg.testMsg, "我是一个测试消息");

                UIManager.Inst.SendMessage( WinEnum.Win_Demo6_Test004, msg);
                
            });
        }


        [UIMessage((int) WinMsg.testMsg)]
        private void TestMessage(UIMessageData msg)
        {
            Content.text = "父窗口收到消息:" + msg.GetData<string>();
        }
    }
}