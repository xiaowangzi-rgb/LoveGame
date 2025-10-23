using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
/// <summary>
/// prefab加载类
/// </summary>
/// 
namespace SharedLibary {
    public class RPrefab : IManager<RPrefab> {
        public RPrefab() {
        }

        public GameObject Find(string tagName) {
            GameObject[] result = GameObject.FindGameObjectsWithTag(tagName);
            if (result == null || result.Length != 1) {
                return null;
            }
            return result[0];
        }

        public GameObject Find(string tagName, string name) {
            GameObject[] result = GameObject.FindGameObjectsWithTag(tagName);
            if (result == null || result.Length != 1) {
                return null;
            }
            for (int i = 0; i < result.Length; ++i) {
                if (result[i].gameObject.name == name) {
                    return result[i];
                }
            }

            return null;
        }

        // /// <summary>
        // /// 异步加载
        // /// </summary>
        // /// <param name="path"></param>
        // /// <param name="onComplete"></param>
        // public void LoadAsync<T>(string path, Action<T, bool> onComplete) where T : UnityEngine.Object {
        //     ResourceManager.Singleton.LoadResourceAsync(path, ".prefab", (T o, bool dontDestory) => {
        //         onComplete?.Invoke(o, dontDestory);
        //     });
        // }

        public GameObject Load(string path) {
            return Resources.Load<GameObject>(path);
        }

        public GameObject LoadObject(string path, out bool bStatic) {
            bStatic = false;
            return Resources.Load<GameObject>(path);
        }

        public void Destory(GameObject gameObject) {
            if (gameObject == null) {
                return;
            }
            ObjectManager.Singleton.TryRemove(gameObject);
            UnityEngine.Object.Destroy(gameObject);
        }

        public void DestoryLoadObject(GameObject gameObject) {
            if (gameObject == null) {
                return;
            }
#if !UNITY_EDITOR
            ObjectManager.Singleton.TryRemove(gameObject);
            UnityEngine.Object.Destroy(gameObject);
#endif
        }

        public void DestroyImmediate(GameObject gameObject) {
            if (gameObject == null) {
                return;
            }
            UnityEngine.Object.DestroyImmediate(gameObject);
        }


        public void RemoveComponentIfExists<T>(GameObject obj) where T : Component {
            var comp = obj.GetComponent<T>();
            if (comp != null) {
                UnityEngine.Object.Destroy(comp);
            }
        }

        /// <summary>
        /// 实例一个load好的gameobjcet
        /// </summary>
        /// <param name="prefab">load好的对象</param>
        /// <param name="parent">父对象</param>
        /// <returns>实例</returns>
        public GameObject Instantiate(GameObject prefab, Transform parent, string name = "") {
            if (prefab == null) {
                return null;
            }
            GameObject go = GameObject.Instantiate(prefab, prefab.transform.localPosition, prefab.transform.localRotation, parent);
            go.transform.localScale = prefab.transform.localScale;
            if (!string.IsNullOrEmpty(name)) {
                go.name = name;
            }
            return go;
        }

        public GameObject Instantiate(string path, Transform parent, Vector3 pos) {
            var obj = Load(path);
            if (obj == null) {
                return null;
            }
            return GameObject.Instantiate(obj, pos, Quaternion.identity, parent);
        }

        /// <summary>
        /// 实例化一个prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject Instantiate(GameObject prefab) {
            if (prefab == null) {
                return null;
            }
            GameObject go = GameObject.Instantiate(prefab, prefab.transform.localPosition, prefab.transform.localRotation, prefab.transform.parent);
            go.transform.localScale = prefab.transform.localScale;
            return go;
        }


        public GameObject Instantiate(string path, Transform parent) {
            return Instantiate(Load(path), parent, PathUtils.GetFileNameFormURL(path));
        }


        /// <summary>
        /// 实例化，异步
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="onComplete"></param>
        public void InstantiateAsync(string path, Transform parent, Action<GameObject> onComplete) {
            // LoadAsync<GameObject>(path, (obj, dontDestory) => {
            //     onComplete?.Invoke(Instantiate(obj, parent, PathUtils.GetFileNameFormURL(path)));
            // });
        }
    }
}
