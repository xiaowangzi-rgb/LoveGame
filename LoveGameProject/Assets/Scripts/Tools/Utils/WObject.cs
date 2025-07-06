using UnityEngine;

namespace SharedLibary {

    /// <summary>
    /// 所有UI的父
    /// </summary>
    public abstract class WObject : IWObject {
        /// <summary>
        /// 当前是否显示状态
        /// </summary>
        public bool activeInHierarchy {
            get { return gameObject != null && gameObject.activeSelf && gameObject.activeInHierarchy; }
        }

        public Transform transform { get { return gameObject?.transform; } }

        public Vector3 initPos { get; private set; }

        public GameObject gameObject { get; set; }

        public virtual bool DontDestory => false;

        protected abstract void InitUI();

        protected virtual void InitValue() {
        }

        protected virtual void AddEvent() {
        }

        ///// <summary>
        ///// 给目标对象创建gameobject
        ///// </summary>
        ///// <param name="target">目标对象</param>
        ///// <param name="path">路径</param>
        ///// <param name="parent">父节点</param>
        ///// <returns></returns>
        //public static WObject Create(WObject target, string path, Transform parent) {
        //    if (target == null) {
        //        return null;
        //    }
        //    GameObject obj = RPrefab.Singleton.Instantiate(path, parent);
        //    if (obj == null) {
        //        return null;
        //    }
        //    obj.name = PathUtils.GetFileNameFormURL(path);
        //    return Create(target, obj);
        //}

        /// <summary>
        /// 创建一个WObject,和obj关联起来
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">gameobjcet</param>
        /// <returns></returns>
        public static T Create<T>(GameObject obj) where T : WObject, new() {
            if (obj == null) {
                Debug.Log("Create WObject error! type is" + typeof(T));
                return null;
            }
            if (ObjectManager.Singleton.TryGetObject(obj.GetInstanceID(), out IWObject value)) {
                return value as T;
            }
            return (T)Create(new T(), obj);
        }

        /// <summary>
        /// 创建一个WObject,和obj关联起来
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static WObject Create(GameObject obj, System.Func<WObject> func) {
            if (obj == null) {
                Debug.Log("Create WObject error!");
                return null;
            }
            if (ObjectManager.Singleton.TryGetObject(obj.GetInstanceID(), out IWObject value)) {
                return (WObject)value;
            }
            return Create(func?.Invoke(), obj);
        }

        ///// <summary>
        ///// 创建一个Wobject
        ///// </summary>
        ///// <param name="path"></param>
        ///// <param name="parent"></param>
        ///// <param name="func"></param>
        ///// <returns></returns>
        //public static WObject Create(string path, Transform parent, System.Func<WObject> func) {
        //    GameObject obj = RPrefab.Singleton.Instantiate(path, parent);
        //    if (obj == null) {
        //        return null;
        //    }
        //    obj.name = PathUtils.GetFileNameFormURL(path);

        //    return Create(func?.Invoke(), obj);
        //}

        ///// <summary>
        ///// 实例化一个gameobjcet,并且创建与之关联的WObject
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static T InInstantiate<T>(GameObject obj) where T : WObject, new() {
        //    if (obj == null) {
        //        LogUtils.LogError("Create WObject error! type is" + typeof(T));
        //        return null;
        //    }
        //    var v = RPrefab.Singleton.Instantiate(obj, obj.transform.parent);
        //    return (T)Create(new T(), v);
        //}

        /// <summary>
        /// 把一个WObject和GameObject关联起来
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        public static WObject Create(WObject target, GameObject obj) {
            if (target == null || obj == null) {
                Debug.Log("Create Error!");
                return null;
            }
            target.gameObject = obj;
            ObjectManager.Singleton.TryAdd(target, target.gameObject);
            target.initPos = target.transform.position;

            target.InitUI();
            target.InitValue();
            target.AddEvent();
            return target;
        }

        public virtual void Show() {
            SetActive(true);
        }

        /// <summary>
        /// 直接隐藏
        /// </summary>
        public virtual void Hide() {
            SetActive(false);
        }

        /// <summary>
        /// 显示接口，所有的内部显示隐藏都调用这个
        /// </summary>
        /// <param name="visible"></param>
        public void SetActive(bool visible) {
            if (gameObject != null && gameObject.activeSelf != visible) {
                gameObject.SetActive(visible);
            }
        }

        protected virtual void OnDestroy() {
            GameObject.Destroy(gameObject);
            gameObject = null;
        }

        /// <summary>
        /// 获取WObject对象
        /// </summary>
        /// <typeparam name="T">获取类型</typeparam>
        /// <param name="target">目标GameObject</param>
        /// <returns>如果此对象创建了WObject则返回，否则返回null。</returns>
        public static T GetWObject<T>(GameObject target) where T : WObject {
            if (target == null) {
                Debug.Log("GetWObject error! type is" + typeof(T));
                return null;
            }
            if (ObjectManager.Singleton.TryGetObject(target.GetInstanceID(), out IWObject value)) {
                return value as T;
            }

            return null;
        }

        /// <summary>
        /// 查找GameObject
        /// </summary>
        /// <param name="path">查找路径</param>
        /// <returns></returns>
        public GameObject FindObject(string path) {
            Transform trans = transform.Find(path);
            if (trans != null) {
                return trans.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="path">在transform下的路径</param>
        /// <returns>返回找到的组件</returns>
        public T GetComponent<T>(string path) where T : Component {
            return GetComponent<T>(transform, path);
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <typeparam name="root">根节点</typeparam>
        /// <param name="path">在transform下的路径</param>
        /// <returns>返回找到的组件</returns>
        public T GetComponent<T>(Transform root, string path) where T : Component {
            if (root == null) {
                return null;
            }
            Transform trans = root.Find(path);
            if (trans == null) {
                return null;
            }
            return trans.GetComponent<T>();
        }

        /// <summary>
        /// 从子获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="path">在transform下的路径</param>
        /// <returns>返回找到的组件</returns>
        public T GetComponentInChildren<T>(string path) where T : Component {
            return GetComponentInChildren<T>(transform, path);
        }

        /// <summary>
        /// 从子获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <typeparam name="root">根节点</typeparam>
        /// <param name="path">在transform下的路径</param>
        /// <returns>返回找到的组件</returns>
        public T GetComponentInChildren<T>(Transform root, string path) where T : Component {
            if (root == null) {
                return null;
            }
            Transform trans = root.Find(path);
            if (trans == null) {
                return null;
            }
            return trans.GetComponentInChildren<T>();
        }

        public void Destroy() {
            OnDestroy();
        }
    }
}
