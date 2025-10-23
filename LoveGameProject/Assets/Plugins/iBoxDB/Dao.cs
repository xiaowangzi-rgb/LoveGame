//using System;
//using System.Collections.Generic;
//using iBoxDB.LocalServer;
//using UnityEngine;

//namespace Plugins.iBoxDB {
//    public class Dao<TEntity> where TEntity : class, new() {
//        protected readonly AutoBox Auto;
//        public readonly string Table;
//        //private readonly CountlyLogHelper Log;
//        //�����̷߳���ʱ����ҪΪ�����������ʽͬʱ��ȡ������,����Ϊ��̬�ģ�
//        //��Ϊֻ��һ��boxdb���ݿ⣬���Զ�д��Ӧ���ǵ��̵߳�
//        public readonly object LockObject = null;
//        public Dao(AutoBox auto, string table, object lockObj) {
//            Auto = auto;
//            Table = table;
//            //Log = log;
//            LockObject = lockObj;
//        }

//        public bool Save(TEntity entity) {
//            lock (LockObject) {
//                try {
//                    return Auto.Insert(Table, entity);
//                } catch (Exception ex) {
//                    //Log.Error("[Dao] Save: Couldn't complete db operation, [" + ex.Message + "]");
//                }
//            }
//            return false;
//        }

//        public bool Update(TEntity entity) {
//            lock (LockObject) {
//                try {
//                    return Auto.Update(Table, entity);
//                } catch (Exception ex) {
//                    //Log.Error("[Dao] Update: Couldn't complete db operation, [" + ex.Message + "]");
//                }
//            }
//            return false;
//        }

//        public List<TEntity> LoadAll() {
//            lock (LockObject) {
//                List<TEntity> result = new List<TEntity>();
//                try {
//                    result = Auto.Select<TEntity>("from " + Table);
//                } catch (Exception ex) {
//                    //Log.Error("[Dao] LoadAll: Couldn't complete db operation, [" + ex.Message + "]");
//                }

//                return result;
//            }
//        }

//        public void Remove(params object[] key) {
//            lock (LockObject) {
//                try {
//                    Auto.Delete(Table, key);
//                } catch (Exception ex) {
//                    //Log.Error("[Dao] Remove: Couldn't complete db operation, [" + ex.Message + "]");
//                }
//            }
//        }

//        public void RemoveAll() {
//            lock (LockObject) {
//                try {
//                    List<TEntity> list = Auto.Select<TEntity>("from " + Table);
//                    foreach (TEntity entity in list) {
//                        Auto.Delete(Table, entity.GetId());
//                    }
//                } catch (Exception ex) {
//                    //Log.Error("[Dao] RemoveAll: Couldn't complete db operation, [" + ex.Message + "]");
//                }
//            }
//        }

//        public long GenerateNewId() {
//            lock (LockObject) {
//                long result;
//                try {
//                    result = Auto.NewId();
//                } catch (Exception ex) {
//                    result = 0;
//                    //Log.Error("[Dao] GenerateNewId: Couldn't complete db operation, [" + ex.Message + "]");
//                }

//                return result;
//            }
//        }

//        public long GenerateNewId(byte name) {
//            lock (LockObject) {
//                long result;
//                try {
//                    result = Auto.NewId(name, 1);
//                } catch (Exception ex) {
//                    result = 0;
//                    //Log.Error("[Dao] GenerateNewId: Couldn't complete db operation, [" + ex.Message + "]");
//                }

//                return result;
//            }
//        }
//    }
//}