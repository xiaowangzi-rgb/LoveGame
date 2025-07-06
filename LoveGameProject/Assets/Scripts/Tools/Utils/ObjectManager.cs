using System.Collections.Generic;
using UnityEngine;

namespace SharedLibary {

    public class ObjectManager : IManager<ObjectManager> {

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool Inited { get; private set; }

        private ObjectManager() {
        }

        /// <summary>
        /// GameObject的instanceID到WObject的映射Map
        /// </summary>
        private Dictionary<int, IWObject> GameObj2Obj = new Dictionary<int, IWObject>();

        public bool TryGetObject(int instanceID, out IWObject value) {
            return GameObj2Obj.TryGetValue(instanceID, out value);
        }

        public void TryAdd(IWObject wObject, GameObject gameObject) {
            if (gameObject != null) {
                GameObj2Obj.Add(gameObject.GetInstanceID(), wObject);
            }
        }

        public void TryDestory(GameObject gameObject) {
            if (gameObject == null) {
                return;
            }

            for (int i = 0; i < gameObject.transform.childCount; i++) {
                Transform trans = gameObject.transform.GetChild(i);
                TryDestory(trans.gameObject);
            }

            var btn = gameObject.GetComponent<UIButtonExtension>();
            if (btn != null) {
                btn.onClick.RemoveAllListeners();
                btn.onLongClick.RemoveAllListeners();
            }

            if (GameObj2Obj.TryGetValue(gameObject.GetInstanceID(), out IWObject wObject)) {
                GameObj2Obj.Remove(gameObject.GetInstanceID());
                if (wObject != null) {
                    wObject.Destroy();
                }
            }
        }

        public void TryRemove(GameObject gameObject) {
            if (gameObject == null) {
                return;
            }
            for (int i = 0; i < gameObject.transform.childCount; i++) {
                Transform trans = gameObject.transform.GetChild(i);
                TryRemove(trans.gameObject);
            }
            if (GameObj2Obj.TryGetValue(gameObject.GetInstanceID(), out IWObject wObject)) {
                GameObj2Obj.Remove(gameObject.GetInstanceID());
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init() {
            Inited = true;
        }

        /// <summary>
        /// 清理
        /// </summary>
        private void Clear() {
            GameObj2Obj.Clear();
            Inited = false;
        }

        public void DestoryAll() {
            List<IWObject> lists = new List<IWObject>();

            foreach (var kvp in GameObj2Obj) {
                lists.Add(kvp.Value);
            }

            for (int i = lists.Count - 1; i >= 0; i--) {
                TryDestory(lists[i].gameObject);
            }

            Clear();
        }

        public static void DestroyBtn(GameObject obj) {
            if (obj == null) {
                return;
            }
            var btns = obj.GetComponentsInChildren<UIButtonExtension>();
            foreach (var v in btns) {
                GameObject.Destroy(v.gameObject);
            }
            GameObject.Destroy(obj);
        }
    }
}
