using System.Collections.Generic;

namespace Events {

    public static class SingletonClass {
        /// <summary>
        /// 所有的单例
        /// </summary>
        private static List<ISinglton> singletons = new List<ISinglton>();
        /// <summary>
        /// 单例初始化时 增加到列表中
        /// </summary>
        /// <param name="s"></param>
        /// 
        public static void Add(ISinglton s) {
            singletons.Add(s);
        }
        /// <summary>
        /// 销毁所有的单例
        /// </summary>
        public static void ClearSingleton() {
            for (int i = 0; i < singletons.Count; ++i) {
                var s = singletons[i];
                if (s != null) {
                    s.Clear();
                }
            }
            singletons.Clear();
        }
    }

    public interface ISinglton {
        void Clear();
    }

    public abstract class TSingleton<T> : ISinglton where T : TSingleton<T>, new() {
        private static T s_Instance = null;
        //private static readonly Type[] EmptyTypes = new Type[0];
        public static T Singleton {
            get {
                if (null == s_Instance) {
                    s_Instance = new T();

                    SingletonClass.Add(s_Instance);
                }
                return s_Instance;
            }
        }
        /// <summary>
        /// 初始化单例
        /// </summary>
        protected virtual void InitSingleton() { }

        public virtual void Clear() { }
    }
}
