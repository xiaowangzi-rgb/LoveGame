using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using iBoxDB.LocalServer;
using LSave;
using UnityEngine;
using Game.Message;

namespace LSave {
    public class LSaveManager : IManager<LSaveManager>{
        private LSaveManager(){}
        /// <summary>
        /// 数据库对象
        /// </summary>
        /// <value></value>
        private DB _DB { get; set; }
        /// <summary>
        /// 数据库操作类
        /// </summary>
        /// <value></value>
        private LDataDao _DataDao { get; set; }
        /// <summary>
        /// 数据库ID
        /// </summary>
        private const int _DBID = 65534;
        /// <summary>
        /// 数据库主键
        /// </summary>
        private const string _DBMainKey = "id";
        /// <summary>
        /// 数据库表名
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        private Dictionary<int, string> _DbTableNameDic { get; set; } = new Dictionary<int, string>();
        /// <summary>
        /// 用户数据类型<----->用户PB数据
        /// </summary>
        private Dictionary<int, IMessage> UserPBDatas = new Dictionary<int, IMessage>();
        /// <summary>
        /// 当前需要存档的用户数据类型
        /// </summary>
        private List<LUserDataType> NeedSavePBDataType = new List<LUserDataType>();
        /// <summary>
        /// 上次保存数据时间
        /// </summary>
        /// <value></value>
        private int _lastSaveTime {get;set;} = 0;
        /// <summary>
        /// 是否初始化
        /// </summary>
        /// <value></value>
        public bool _IsInit{get; private set;} = false;

        public void Init() {
            //初始化数据库存储路径
            DB.Root(Application.persistentDataPath);
            //赋值加密存档的适配器
            LEncryptDatabaseConfig.ResetStorage();
            //初始化DB对象 参数为数据库ID
            _DB = new DB(_DBID);
            //设置数据库表
            SetDBTable();
            //初始化数据库操作类
            _DataDao = new LDataDao(_DB.Open());
            //设置上次保存时间
            SetLastSaveTime();
        }

        /// <summary>
        /// 设置上次Save时间
        /// </summary>        
        public void SetLastSaveTime(){
            //上次存档时间
            var mem = _DataDao.LoadData("Last_Save_Time_Data");
            if (mem != null) {
                if (BitConverter.IsLittleEndian) {
                    Array.Reverse(mem.data);
                }
                _lastSaveTime = BitConverter.ToInt32(mem.data, 0);
            }
        }

        /// <summary>
        /// 设置DB数据库表
        /// </summary>
        private void SetDBTable() {
            for (LUserDataType i = LUserDataType.BaseDataInfo; i <= LUserDataType.SystemSettingInfo; ++i) {
                var type = i.ToString();
                if (_DbTableNameDic.ContainsKey((int)i)) {
                    _DbTableNameDic[(int)i] = type;
                } else {
                    _DbTableNameDic.Add((int)i, type);
                }
                _DB.GetConfig().EnsureTable<LUserDataEntity>(type, _DBMainKey);
            }
            _DB.GetConfig().EnsureTable<LUserDataEntity>("Last_Save_Time_Data", _DBMainKey);
        }

        /// <summary>
        /// 自动存档 每帧存档
        /// </summary>
        public void AutoSave(bool isForceSave = false){
            if(_DataDao == null){
                Debug.Log("没有存档操作类");
                return;
            }
            if(UserPBDatas.Count <= 0){
                Debug.Log("没有填出用户pb数据");
                return;
            }
            if(NeedSavePBDataType == null || NeedSavePBDataType.Count <= 0){
                //Debug.Log("没有需要存档的数据");
                return;
            }
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // Unix纪元时间
            try {
                foreach (var type in NeedSavePBDataType) {
                    bool bsaved = false;
                    if (!bsaved) {
                        if (_DbTableNameDic.TryGetValue((int)type, out var value) && UserPBDatas.TryGetValue((int)type, out var dataValue)) {
                            _DataDao.Update(value, new LUserDataEntity() { id = 0L, data = dataValue.ToByteArray() });
                        }
                    }
                }
                var seconds = (int)(DateTime.UtcNow - unixEpoch).TotalSeconds;
                //更新上次存档时间
                _lastSaveTime = seconds;
                var bytes = BitConverter.GetBytes(_lastSaveTime);
                if (BitConverter.IsLittleEndian) {
                    Array.Reverse(bytes);
                }
                //将上次保存数据时间存入本地存档中
                _DataDao.Replace("Last_Save_Time_Data",new LUserDataEntity(){id = 0L,data = bytes});
                Debug.Log("自动存档成功");
            } catch (IOException _) {
                Debug.Log("Disk Full! Please Clear Your Disk!");
            } catch (Exception _) {
                Debug.Log("Save Record Failed!");
            } finally {
                NeedSavePBDataType.Clear();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baddSave"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public T LoadData<T,M>(LUserDataType type, bool baddSave = true) where T : LUserData , new () where M : Google.Protobuf.IMessage ,new (){
            var infoData = new T();
            string tableName = _DbTableNameDic[(int)type];
            LUserDataEntity data = null;
            try{
                data = _DataDao.LoadData(tableName);
            }
            catch (System.Exception _){
                Debug.LogError(_.ToString());
            }
            //本地没有这个数据 第一次创建初始化时
            if(data == null){
                infoData.PBData = new M();
                infoData.InitData();
            }else{
                //用PB自带的MessageParser转换数据类型
                var parser = new MessageParser(()=>{return new M();});
                try {
                    infoData.PBData = parser.ParseFrom(data.data);
                } catch (Exception e) {
                    Debug.Log(type.ToString() + e);
                    infoData.PBData = new M();
                    infoData.InitData();
                }
            }
            //调用数据初始化接口
            infoData?.OnInit();
            //然后判断数据库中是否有这个数据没有就添加
            if (data == null) {
                _DataDao.Insert(tableName, new LUserDataEntity() { id = 0L, data = infoData.PBData.ToByteArray() });
            }
            if (baddSave) {
                AddData(type, infoData.PBData);
            }
            return infoData;
        }

        /// <summary>
        /// 增加数据 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pbData"></param>
        public void AddData(LUserDataType type ,IMessage pbData){
            UserPBDatas[(int)type] = pbData;
        }

        /// <summary>
        /// 增加存档
        /// </summary>
        /// <param name="type"></param>
        private void AddSave(LUserDataType type){
            if(!NeedSavePBDataType.Contains(type)){
                NeedSavePBDataType.Add(type);
            }
        }

        /// <summary>
        /// 增加需要保存的数据
        /// </summary>
        /// <param name="type"></param>
        public void AddNeedSaveData(LUserDataType type){
            AddSave(type);
        }

        /// <summary>
        /// 立刻强制保存数据
        /// </summary>
        public void ForceSaveDataImmediately(){
            AutoSave(true);
        }

        /// <summary>
        /// 立刻保存数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="message"></param>
        public void SaveImmediately(string tableName, IMessage message) {
            _DataDao.Replace(tableName, new LUserDataEntity() { id = 0L, data = message.ToByteArray() });
        }

        /// <summary>
        /// 立刻保存数据
        /// </summary>
        /// <param name="datas"></param>
        public void SaveImmediately(Dictionary<string, IMessage> datas) {
            if (datas == null || datas.Count <= 0) {
                return;
            }
            var msgDatas = new Dictionary<string,LUserDataEntity>();
            foreach (var item in datas){
                if(!msgDatas.ContainsKey(item.Key)){
                    msgDatas.Add(item.Key,new LUserDataEntity(){id = 0L,data = item.Value.ToByteArray()});
                }
            }
            _DataDao.Replace(msgDatas);
        }

        /// <summary>
        /// 删除本地的存档
        /// </summary>
        public void DeleteGameData() {
            ClearData();
            var list = new List<string>();
            list.AddRange(new List<string>(_DbTableNameDic.Values));
            _DataDao.Delete(list);
        }

        /// <summary>
        /// 强制删掉玩家的所有数据
        /// </summary>
        public void ForceDeleteGameData() {
            ClearData();
            var list = new List<string>();
            for (var type = LUserDataType.BaseDataInfo; type <= LUserDataType.SystemSettingInfo; ++type) {
                list.Add(type.ToString());
            }
            list.Add("Last_Save_Time_Data");
            _DataDao.Delete(list);
        }

        /// <summary>
        /// 删除指定类型数据
        /// </summary>
        /// <param name="userDataTypes"></param>
        public void DeleteGameData(List<string> userDataTypes) {
            if (userDataTypes == null || userDataTypes.Count <= 0) {
                return;
            }
            _DataDao.Delete(userDataTypes);
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData(){
            UserPBDatas?.Clear();
            NeedSavePBDataType?.Clear();
        }

        /// <summary>
        /// 关闭存档系统
        /// </summary>
        public void OnDestory() {
            _DB.Dispose();
            _DB.Close();
            _DbTableNameDic.Clear();
        }
    }
}
