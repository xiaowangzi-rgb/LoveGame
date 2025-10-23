using System.Collections;
using System.Collections.Generic;
using iBoxDB.LocalServer;
using UnityEngine;

namespace LSave {
    //注释: Bind 是绑定表的方法 会返回Binder

    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class LDataDao {
        /// <summary>
        /// IBox数据库对象
        /// </summary>
        /// <value></value>
        public AutoBox AutoBox { get; private set; }

        public LDataDao(AutoBox autoBox){
            AutoBox = autoBox;
        }

        /// <summary>
        /// 存档
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Insert(string tableName, LUserDataEntity entity) {
            return AutoBox.Insert(tableName, entity);
        }

        /// <summary>
        /// 存档 一次性添加多个数据
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public void Insert(Dictionary<string, LUserDataEntity> entitys){
            if (entitys.Count <= 0) {
                return;
            }
            using(var cube = AutoBox.Cube()){
                foreach (var entity in entitys){
                    cube.Bind(entity.Key, 0L).Insert(entity.Value);
                }
                cube.Commit();
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(string tableName,LUserDataEntity entity){
            return AutoBox.Update(tableName, entity);
        }
        
        /// <summary>
        /// 更新一组数据
        /// </summary>
        /// <param name="entitys"></param>
        public void Update(Dictionary<string,LUserDataEntity> entitys){
            if (entitys.Count <= 0) {
                return;
            }
            using(var cube = AutoBox.Cube()){
                foreach (var entity in entitys){
                    cube.Bind(entity.Key, 0L).Update(entity.Value);
                }
                cube.Commit();
            }
        }

        /// <summary>
        /// 替换数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        public bool Replace(string tableName, LUserDataEntity entity){
            return AutoBox.Replace(tableName, entity);
        }

        /// <summary>
        /// 替换一组数据
        /// </summary>
        /// <param name="entitys"></param>
        public void Replace(Dictionary<string,LUserDataEntity> entitys){
            if (entitys.Count <= 0) {
                return;
            }
            using(var cube = AutoBox.Cube()){
                foreach (var entity in entitys){
                    cube.Bind(entity.Key, 0L).Replace(entity.Value);
                }
                cube.Commit();
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(string tableName, LUserDataEntity entity) {
            return AutoBox.Delete(tableName, entity.GetID());
        }

        /// <summary>
        /// 删除一组数据
        /// </summary>
        /// <param name="tables"></param>
        public void Delete(List<string> tables) {
            if (tables.Count <= 0) {
                return;
            }
            using (var cube = AutoBox.Cube()) {
                foreach (var tableName in tables) {
                    cube.Bind(tableName, 0L).Delete();
                }
                cube.Commit();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public LUserDataEntity LoadData(string tableName){
            var datas = AutoBox.Select<LUserDataEntity>("from " + tableName);
            if (datas == null || datas.Count <= 0) {
                return null;
            }
            return datas[0];
        }
    }
}

