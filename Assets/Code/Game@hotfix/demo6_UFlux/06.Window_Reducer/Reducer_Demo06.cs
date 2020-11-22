﻿using System.Net;
using System.Threading.Tasks;
using BDFramework.UFlux.Reducer;
using LitJson;

namespace BDFramework.UFlux.Test
{
    /// <summary>
    /// Reducer 函数处理的集合
    /// </summary>
    public class Reducer_Demo06 : AReducers<S_Hero>
    {
        public enum Reducer06
        {
            //请求英雄属性
            RequestHeroData,
        }
        
        public override void RegisterReducers()
        {
            base.RegisterReducers();

            //这里用显式注册，避免函数签名错误
            this.AddAsyncRecucer(Reducer06.RequestHeroData, RequestServer);
        }


        /// <summary>
        /// url
        /// </summary>
        readonly public string url = "https://1843236967254885.cn-shanghai.fc.aliyuncs.com/2016-08-15/proxy/BDFramework/DemoForUFlux/";
        /// <summary>
        /// 这里是做了个网络请求
        /// </summary>
        /// <param name="old"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        async  private Task<S_Hero>RequestServer(S_Hero old, object @param)
        {
            var api = url + "api/bdframework/getherodata";
            WebClient  wc=new WebClient();
            string ret = await  wc.DownloadStringTaskAsync(api);
            var hero = JsonMapper.ToObject<S_Hero>(ret);
            return hero;
        }
    }
}