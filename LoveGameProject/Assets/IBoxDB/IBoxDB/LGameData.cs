using Game.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSave {
    public class LGameData : IManager<LGameData> {
        private LGameData(){}

        public BaseDataInfo _BaseDataInfo { get; private set;}


        /// <summary>
        /// 初始化数据,一生一次
        /// </summary>
        public void InitData(){
            //清除数据
            LSaveManager.Singleton.ClearData();
            //设置上次刷新时间
            LSaveManager.Singleton.SetLastSaveTime();
            //加载功能数据
            LoadData();
        }

        private void LoadData(){
            var pb = LSaveManager.Singleton;
            _BaseDataInfo = pb.LoadData<BaseDataInfo, BaseGameData>(LUserDataType.BaseDataInfo);
            
        }
    }
}

