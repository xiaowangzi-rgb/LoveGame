using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;

namespace LSave {
    /// <summary>
    /// 数据存储基类
    /// </summary>
    public abstract class LSData {
        public LSData() {
        }
        /// <summary>
        /// 注册的消息列表
        /// </summary>
        private List<Action> subcribeList = new List<Action>();

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="subcribe">Subcribe.</param>
        public void Subcribe(Action action) {
            subcribeList.Add(action);
        }

        public void Clear() {
            subcribeList.Clear();
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="subcribe">Subcribe.</param>
        public void UnSubcribe(Action subcribe) {
            if (subcribeList.Contains(subcribe)) {
                subcribeList.Remove(subcribe);
            }
        }

        /// <summary>
        /// 当数据发生变化时
        /// </summary>
        /// <param name="data"></param>
        protected void OnDataChangeAction() {
            for (int i = 0; i < subcribeList.Count; ++i) {
                subcribeList[i]?.Invoke();
            }
        }
    }

    /// <summary>
    /// 用户数据
    /// </summary>
    public abstract class LUserData : LSData {
        /// <summary>
        /// 数据类型
        /// </summary>
        /// <value></value>
        public abstract LUserDataType DataType { get;}
        /// <summary>
        /// 存档数据
        /// </summary>
        /// <value></value>
        public IMessage PBData { get; set; }
        /// <summary>
        /// 数据切换
        /// </summary>
        /// <param name="isSave"></param>
        public void OnDataChange(bool isSave = true){
            OnDataChangeAction();
            if(isSave){
                Save();
            }
        }
        public virtual void Save(){
            LSaveManager.Singleton.AddNeedSaveData(DataType);
        }
        public virtual IMessage GetSaveData() {
            return PBData;
        }
        /// <summary>
        /// 只有第一次初始化数据时调用
        /// </summary>
        public abstract void InitData();
        /// <summary>
        /// 每次进入游戏都会调用
        /// </summary>
        public abstract void OnInit();
        /// <summary>
        /// 配置加载完成后调用
        /// </summary>
        public virtual void OnTableLoaded() {
        }
    }
}
