using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using MM;
using MM.Common;
using System.Text;
using Events;
using MM.UI;
using SuperScrollView;

namespace SharedLibary {

    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : TSingleton<UIManager> {
        /// <summary>
        /// 界面关闭时返回上层界面选项
        /// </summary>
        public enum BackOption {
            /// <summary>
            /// 普通界面
            /// </summary>
            Normal,
            /// <summary>
            /// 需要返回到上层界面, 返回的跟节点需要设置NeedBackRoot
            /// </summary>
            NeedBack,
            /// <summary>
            /// 不需要返回
            /// </summary>
            DontBack,
            /// <summary>
            /// 一组需要返回上级界面的跟节点，只是此选项
            /// </summary>
            NeedBackRoot
        }

        /// <summary>
        /// UI按钮反馈动画类型
        /// </summary>
        public enum ButtonAnimationType {
            None,               //无动画
            CommonAnim,         //通用的动画(按下和抬起)
            ClickAnim,           //点击动画
            CommonAnimNotEnterExit, //通用动画，不包含进入和退出
        }

        /// <summary>
        /// 预加载的UI缓存
        /// </summary>
        private Dictionary<string, GameObject> uiCache = new Dictionary<string, GameObject>();

        /// <summary>
        /// UI的画布Rect
        /// </summary>
        public RectTransform canvasRect { get; private set; }

        /// <summary>
        /// 普通界面的根节点
        /// </summary>
        public Transform mWindowRoot { get; private set; }

        /// <summary>
        /// 在普通界面上层的界面的根节点
        /// </summary>
        public Transform mWindowTop { get; private set; }

        /// <summary>
        /// 最下层界面的根节点
        /// </summary>
        public Transform mWindowLow { get; private set; }

        /// <summary>
        /// 事件屏蔽
        /// </summary>
        private Transform mEventMask {
            get {
                if (_maskEvent == null) {
                    _maskEvent = GameObject.Find("mCanvas")?.transform?.Find("MaskEvent");
                }
                return _maskEvent;
            }
            set {
                _maskEvent = value;
            }
        }
        private Transform _maskEvent;

        /// <summary>
        /// 所有加载出来的界面
        /// </summary>
        private Dictionary<string, WWindow> mAllWindows;

        /// <summary>
        /// 关闭的时候需要返回上一个的界面队列
        /// </summary>
        private Stack<string> mCurrentQueue;

        /// <summary>
        /// 当前打开的界面
        /// </summary>
        private List<WWindow> mCurActiveWindow;

        /// <summary>
        /// 不能手动关闭的界面，比如loading，播放动画，加载圈界面
        /// </summary>
        private List<WWindow> mCantBackWindow;

        /// <summary>
        /// 保存未关闭的上级界面的参数
        /// </summary>
        private Dictionary<string, IWindowParam> mWindowParam;

        private class EventSystemDisable {
            public float time = 0f;
            public bool ignoreTime = false;
        }

        private Dictionary<object, EventSystemDisable> mEventSystemDisabaleCache = new Dictionary<object, EventSystemDisable>();

        /// <summary>
        /// 窗口创建器的缓存
        /// </summary>
        public static Dictionary<string, Func<WWindow>> windowCreatrDic { get; private set; } = new Dictionary<string, Func<WWindow>>();

        /// <summary>:
        /// 当某个界面隐藏时的事件列表
        /// </summary>
        public List<Action> windowOnHide = new List<Action>();
        /// <summary>
        /// 当某个界面被用户出发隐藏时的事件列表
        /// </summary>
        public List<Action> windowToback = new List<Action>();
        /// <summary>
        /// 只存在主界面时的事件
        /// </summary>
        public List<Action> onlyHaveMainWindowEvent = new List<Action>();

        public List<Action<string>> OnWindowShowAction = new List<Action<string>>();
        public List<Action<string>> OnWindowHideAction = new List<Action<string>>();

        /// <summary>
        /// 是否锁定弹窗
        /// </summary>
        public bool IsLockPop = false;

        /// <summary>
        /// UI弹窗队列
        /// </summary>
        /// <returns></returns>
        public UIWindowSequeue UIWindowSequeue = new UIWindowSequeue();

        /// <summary>
        /// 获取等待显示的UI个数
        /// </summary>
        /// <returns></returns>
        public int GetWaitShowUISequeueCount {
            get {
                if (UIWindowSequeue == null) {
                    return 0;
                }
                return UIWindowSequeue.GetQueueAllCount();
            }
        }

        private Action showQuitAction;

        public void RegisterShowQuitAction(Action action) {
            showQuitAction = action;
        }

        public int NotifyWindowCount {
            get {
                int count = 0;
                if (mCurActiveWindow == null || mCurActiveWindow.Count <= 0) {
                    return count;
                }
                for (int j = 0; j < mCurActiveWindow.Count; ++j) {
                    var window = mCurActiveWindow[j];
                    if (window == null || window.gameObject == null || !window.NeedNotifyManager || !window.gameObject.activeInHierarchy) {
                        continue;
                    }
                    if (window.WindowLayer != WWindow.LAYER_TYPE.NORMAL) {
                        continue;
                    }
                    count++;
                }
                return count;

            }
        }
        /// <summary>
        /// 当前背景的数量
        /// </summary>
        public int FadeOutBackground { get; set; } = 0;

        public UIManager() {
            mAllWindows = new Dictionary<string, WWindow>();
            mCurrentQueue = new Stack<string>();
            mCurActiveWindow = new List<WWindow>();
            mCantBackWindow = new List<WWindow>();
            mWindowParam = new Dictionary<string, IWindowParam>();

            //UIRegister.RegisterUI();
        }

        /// <summary>
        /// 注册UI事件
        /// </summary>
        public static void RegisterUI() {
            //注册循环列表全局拖拽事件
            LoopListView2.onGlobalLoopViewListBeginDrag = OnGlobalLoopViewListBeginDrag;
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init(Transform windowRoot, Transform windowTop, Transform windowLow, Transform eventMask) {

            mWindowRoot = windowRoot;
            mWindowTop = windowTop;
            mWindowLow = windowLow;
            mEventMask = eventMask;
            canvasRect = windowRoot.parent.GetComponent<RectTransform>();
        }

        /// <summary>
        /// AddWindow
        /// </summary>
        /// 
        /// <param name="path"></param>
        /// <param name="func"></param>
        /// <param name="addToList"></param>
        /// <returns></returns>
        public WWindow AddWindow(string path, Func<WWindow> func, bool addToList = true) {
            WWindow w;
            // Debug.Log("path : " + path);
            if (uiCache.TryGetValue(path, out var value)) {
                w = (WWindow)WObject.Create(value, func);
            } else {
                w = (WWindow)WObject.Create(path, mWindowRoot, func);
                if (w != null && w.gameObject != null) {
                    uiCache[path] = w.gameObject;
                }
            }
            if (w == null || w.gameObject == null) {
                Debug.LogError("prefab is not found! path=" + path);
                return null;
            }
            //没有自定义父节点
            var isCustomRoot = w.WindowCustomRoot != null;
            if (!isCustomRoot) {
                switch (w.WindowLayer) {
                    case WWindow.LAYER_TYPE.LOW:
                        w.transform.SetParent(mWindowLow);
                        break;
                    case WWindow.LAYER_TYPE.NORMAL:
                        w.transform.SetParent(mWindowRoot);
                        break;
                    case WWindow.LAYER_TYPE.TOP:
                        w.transform.SetParent(mWindowTop);
                        break;
                }
            }
            w.transform.localPosition = Vector3.zero;

            if (addToList && path != PathUtils.CombinePath(PathUtils.UIPath, "UIMergeCastalBubbleWindow")) {
                mAllWindows.Add(path, w);
                w.onHideAction += OnWindowClosed;
                w.mOnToBackAction += OnWindowToBack;
            }
            w.path = path;
            return w;
        }


        /// <summary>
        /// 加载并显示UI
        /// </summary>
        /// <param name="path">UI预设路径</param>
        /// <param name="windowName">UI预设名称</param>
        /// <param name="addToList">是否存放在列表内</param>
        /// <returns></returns>
        public T AddWindow<T>(string path, bool addToList = true) where T : WWindow, new() {
            return (T)AddWindow(path, () => { return new T(); }, addToList);
        }

        /// <summary>
        /// 显示一个UI窗口
        /// </summary>
        /// <param name="path">UI预设路径</param>
        /// <returns>UI脚本的引用</returns>
        public T ShowWindow<T>(string path) where T : WWindow, new() {
            return ShowWindow<T>(path, null);
        }

        /// <summary>
        /// /// 显示一个UI窗口
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public WWindow ShowWindow(string path) {
            return ShowWindow(path, null, null);
        }

        /// <summary>
        /// 预加载UI的prefab
        /// </summary>
        /// <param name="windows"></param>
        public void PreLoadUI(List<string> windows, Action onComplete) {
            int count = windows.Count;
            for (int i = 0; i < windows.Count; ++i) {
                var path = windows[i];
                if (uiCache.ContainsKey(path)) {
                    count--;
                    if (count <= 0) {
                        onComplete?.Invoke();
                        return;
                    }
                    continue;
                }
                RPrefab.Singleton.InstantiateAsync(path, mWindowRoot, (obj) => {
                    if (obj == null) {
                        Debug.LogError("PreLoadUI Error " + path);
                    }
                    obj.SetActive(false);
                    uiCache[path] = obj;

                    count--;
                    if (count <= 0) {
                        onComplete?.Invoke();
                    }
                });
            }
        }

        /// <summary>
        /// 预加载UI
        /// </summary>
        /// <param name="path"></param>
        public void PreLoadUI(string path, Action onComplete) {
            RPrefab.Singleton.InstantiateAsync(path, mWindowRoot, (obj) => {
                obj.SetActive(false);
                uiCache[path] = obj;
                onComplete?.Invoke();
            });
        }

        public void LoadUI(string path) {
            var obj = RPrefab.Singleton.Instantiate(path, mWindowRoot);
            if (obj != null) {
                obj.SetActive(false);
                uiCache[path] = obj;
            }
        }

        public T GetPreloadedWindow<T>(string path) where T : WWindow, new() {
            if (uiCache.TryGetValue(path, out var value)) {
                return (T)WObject.Create(value, () => { return new T(); });
            }
            return null;
        }

        /// <summary>
        /// 预加载UI
        /// </summary>
        /// <param name="path"></param>
        public void PreLoadUI(string path, Action<WWindow> onComplete) {
            RPrefab.Singleton.InstantiateAsync(path, mWindowRoot, (obj) => {
                obj.SetActive(false);
                uiCache[path] = obj;
                WWindow w = (WWindow)WObject.Create(obj, () => { return null; });
                if (w != null) {
                    w.windowName = path;
                }
                onComplete?.Invoke(w);
            });
        }

        /// <summary>
        /// 显示一个UI窗口
        /// </summary>
        /// <param name="path">UI预设路径</param>
        /// <returns>UI脚本的引用</returns>
        public T ShowWindow<T>(string path, IWindowParam param) where T : WWindow, new() {
            return (T)ShowWindow(path, param, () => new T());
        }

        /// <summary>
        /// 显示一个UI窗口
        /// </summary>
        /// <param name="path">UI预设路径</param>
        /// <returns>UI脚本的引用</returns>
        public T PopUpWindow<T>(string path, IWindowParam param) where T : WWindow, new() {
            return (T)ShowWindow(path, param, () => new T(), isPopUpWindow: true);
        }

        /// <summary>
        /// 显示一个UI
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public WWindow ShowWindow(string path, IWindowParam param, Func<WWindow> func, bool isNaitve = false, bool isPopUpWindow = false, string popupWindowName = "") {
            if (func == null) {
                if (!windowCreatrDic.TryGetValue(path, out func)) {
                    Debug.Log("没有缓存这个UI : " + path);
                    return null;
                }
            }
            //弹窗逻辑
            // if (SingleScene.Singleton != null && isPopUpWindow) {
            //     SingleScene.Singleton.AddWaitShowUI(path, param, func.Invoke()._PopUpWindowType);
            //     return null;
            // }
            // //如果不是线上调用过来的得去清除下UI队列中的重复路径
            // SingleScene.Singleton?.RemoveWaitShowUI(path);
            if (!isNaitve) {
            }
            WWindow w;
            if (!Singleton.mAllWindows.ContainsKey(path)) {
                w = Singleton.AddWindow(path, func);
            } else {
                w = Singleton.mAllWindows[path];
            }

            if (w != null) {
                w.windowName = path;
                w.transform.SetAsLastSibling();
                w.SetShowPopupWindowName(popupWindowName);
                w.Show(param);
                Singleton.PushWindow(path, w, param);
                OpenWindowEvent();
            }
            return w;
        }

        /// <summary>
        /// 显示一个窗口，上层调用
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        public void ShowWindowNaitve(string path, IWindowParam param) {
            ShowWindow(path, param, null, true);
        }

        /// <summary>
        /// 显示一个窗口，上层调用
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        public void ShowPopupWindowNaitve(string path, IWindowParam param, string popupWindowName) {
            ShowWindow(path, param, null, true, popupWindowName: popupWindowName);
        }

        public void HideWindowNaitve(string path, bool isPlayTween) {
            HideWindow(path, isPlayTween);
        }

        private WWindow ShowWindowInno(string path, IWindowParam param) {
            if (!Singleton.mAllWindows.ContainsKey(path)) {
                return null;
            }
            WWindow w = Singleton.mAllWindows[path];
            if (w != null) {
                w.windowName = path;
                w.transform.SetAsLastSibling();
                if (w.ActivityedCallShow || (!w.ActivityedCallShow && !w.activeInHierarchy)) {
                    w.Show(param);
                }
                Singleton.PushWindow(path, w, param);
            }
            return w;
        }

        public bool IsOnlyMainWindowShow() {
            int count = 0;
            if (mCurActiveWindow == null || mCurActiveWindow.Count <= 0) {
                return false;
            }
            for (int i = 0; i < mCurActiveWindow.Count; i++) {
                var window = mCurActiveWindow[i];
                if (window == null) {
                    continue;
                }
                if (window.gameObject == null) {
                    continue;
                }
                if (window.transform == null) {
                    continue;
                }
                if (window.transform.parent == Singleton.mWindowRoot &&
                    window.gameObject.activeInHierarchy)
                    count++;
            }
            //Logger.Error("count : " + count);
            return count == 1;
        }

        /// <summary>
        /// 是否只有主界面或者主界面和另外一个界面显示
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public bool IsOnlyMainAndAnotherWindowShow(string windowName) {
            int count = 0;
            if (mCurActiveWindow == null || mCurActiveWindow.Count <= 0) {
                return false;
            }
            for (int i = 0; i < mCurActiveWindow.Count; i++) {
                var window = mCurActiveWindow[i];
                if (window == null || window.transform == null || window.gameObject == null) {
                    continue;
                }
                if (window.transform.parent == Singleton.mWindowRoot &&
                    window.gameObject.activeInHierarchy) {
                    if (window.gameObject.name == windowName) {
                        continue;
                    }
                    count++;
                }
            }
            return count <= 1;
        }

        /// <summary>
        /// 获取当前显示的窗口 也就是最顶部的窗口
        /// </summary>
        /// <returns></returns>
        public WWindow GetCurrentShowWindow() {
            if (mCurActiveWindow == null || mCurActiveWindow.Count <= 0) {
                return null;
            }
            for (int i = mCurActiveWindow.Count - 1; i >= 0; i--) {
                var window = mCurActiveWindow[i];
                if (window == null || window.transform == null || window.gameObject == null) {
                    continue;
                }
                if (window.transform.parent == Singleton.mWindowRoot &&
                    window.gameObject.activeInHierarchy) {
                    return window;
                }
            }
            return null;
        }

        public WindowType GetTopWindowType() {
            var window = GetCurrentShowWindow();
            if (window == null) {
                return WindowType.None;
            }
            return window._PopUpWindowType;
        }

        public void HideTopWindow() {
            var window = GetCurrentShowWindow();
            if (window == null) {
                return;
            }
            HideWindow(window.path, true);
        }

        /// <summary>
        /// 隐藏某个窗口
        /// </summary>
        /// <param name="path"></param>
        public void HideWindow(string path, bool playTween = true) {
            WWindow window = Singleton.GetWindow<WWindow>(path);
            if (window != null && window.transform != null && window.gameObject != null && window.activeInHierarchy) {
                bool cacheShowTween = window.ShowTween;
                window.ShowTween = playTween;
                window.Hide();
                window.ShowTween = cacheShowTween;
            }
        }

        /// <summary>
        /// 模拟用户点击隐藏界面
        /// </summary>
        /// <param name="path"></param>
        public void ToBackWindow(string path, bool playTween = true) {
            WWindow window = Singleton.GetWindow<WWindow>(path);
            if (window != null && window.activeInHierarchy) {
                bool cacheShowTween = window.ShowTween;
                window.ShowTween = playTween;
                window.ToBack();
                window.ShowTween = cacheShowTween;
            }
        }

        /// <summary>
        /// 隐藏打开的所有窗口,除开不能清理的窗口
        /// </summary>
        public void HideSceneWindow() {
            for (int i = 0; i < mCurActiveWindow.Count; ++i) {
                var v = mCurActiveWindow[i];
                if (v != null && !v.mDontClean && v.gameObject.activeInHierarchy && v.gameObject.activeSelf) {
                    v.Hide();
                    mCurActiveWindow.Remove(v);
                }
            }
        }

        /// <summary>
        /// 销毁所有的窗口
        /// </summary>
        public void DestoryWindows() {
            List<string> lists = new List<string>();
            List<string> windows = new List<string>(mAllWindows.Keys);
            for (int i = 0; i < windows.Count; ++i) {
                lists.Add(windows[i]);
            }
            for (int i = lists.Count - 1; i >= 0; i--) {
                DestroyWindow(lists[i]);
            }
            mEventSystemDisabaleCache?.Clear();
            mCurrentQueue?.Clear();
            uiCache?.Clear();
            mWindowParam?.Clear();
            mCantBackWindow?.Clear();
            FadeOutBackground = 0;
            mCurActiveWindow?.Clear();
            mAllWindows?.Clear();
            OnWindowShowAction?.Clear();
            OnWindowHideAction?.Clear();
            BgRoot.Singleton.ResetBg();
        }

        /// <summary>
        /// 删除所有窗口
        /// 不会销毁对应的gameobject
        /// </summary>
        public void RemoveAllWindows() {
            List<string> lists = new List<string>();
            List<string> windows = new List<string>(mAllWindows.Keys);
            for (int i = 0; i < windows.Count; ++i) {
                lists.Add(windows[i]);
            }
            for (int i = lists.Count - 1; i >= 0; i--) {
                RemoveWindow(lists[i]);
            }
            mEventSystemDisabaleCache.Clear();
            FadeOutBackground = 0;
        }

        /// <summary>
        /// 移除 window，保留缓存
        /// </summary>
        /// <param name="path"></param>
        private void RemoveWindow(string path) {
            WWindow window = GetWindow<WWindow>(path);
            if (window == null) {
                return;
            }
            if (mCurActiveWindow.Contains(window)) {
                mCurActiveWindow.Remove(window);
            }
            if (mCantBackWindow.Contains(window)) {
                mCantBackWindow.Remove(window);
            }
            if (mAllWindows.ContainsKey(window.path)) {
                mAllWindows.Remove(window.path);
            }
            window.gameObject.gameObject.SetActive(false);

            ObjectManager.Singleton.TryRemove(window.gameObject);
        }


        /// <summary>
        /// 移除 window，清除缓存
        /// </summary>
        /// <param name="path"></param>
        public void DestroyWindow(string path) {
            WWindow window = GetWindow<WWindow>(path);
            if (window == null) {
                return;
            }
            if (mCurActiveWindow.Contains(window)) {
                mCurActiveWindow.Remove(window);
            }
            if (mCantBackWindow.Contains(window)) {
                mCantBackWindow.Remove(window);
            }
            if (mAllWindows.ContainsKey(window.path)) {
                mAllWindows.Remove(window.path);
            }
            if (uiCache.ContainsKey(window.path)) {
                uiCache.Remove(window.path);
            }

            ObjectManager.Singleton.TryDestory(window.gameObject);
            //window.Destory();
        }


        static float mLastTime = 0;
        public static bool checkEsc() {
            // if (SingleScene.Singleton.sceneTime < 0.25f) {
            //     return false;
            // }

            // var sceneLoader = SingleScene.Singleton.CurrentScene?.SceneLoader;
            // if (sceneLoader != null && (sceneLoader.IsLoading || sceneLoader.loaderGameobj != null)) {
            //     return false;
            // }

            var guidePanel = GameObject.Find("mCanvas/mWindowTop/UIShareGuideWindowPanel");
            if (guidePanel != null && guidePanel.activeSelf) {
                return false;
            }
            var dialogPanel = GameObject.Find("mCanvas/mWindowTop/DialogBox");
            if (dialogPanel != null && dialogPanel.activeSelf) {
                return false;
            }
            if (Time.realtimeSinceStartup - mLastTime < 0.1f) {
                return false;
            }
            mLastTime = Time.realtimeSinceStartup;
            return true;
        }

        /// <summary>
        /// 当按返回键的时候，自动关闭最上层界面
        /// </summary>
        public void OnEscPress() {
            if (!checkEsc()) {
                return;
            }
            if (mCurActiveWindow.Count == 0) {
                return;
            }
            WWindow window = mCurActiveWindow[mCurActiveWindow.Count - 1];
            //不能自动返回的UI
            if (window.OnEscPress() && !window.mDontClean &&
                (window.mBackOption == BackOption.NeedBack || window.mBackOption == BackOption.Normal)) {
                window.ToBack();
                return;
            }
            if (IsOnlyMainWindowShow()) {
                showQuitAction?.Invoke();
            }
        }

        private void SaveParam(string path, IWindowParam param) {
            if (mWindowParam.ContainsKey(path)) {
                mWindowParam[path] = param;
            } else {
                mWindowParam.Add(path, param);
            }
        }

        private void RemoveParam(string path) {
            if (mWindowParam.ContainsKey(path)) {
                mWindowParam.Remove(path);
            }
        }

        private IWindowParam GetParam(string path) {
            if (mWindowParam.ContainsKey(path)) {
                return mWindowParam[path];
            }
            return null;
        }

        private void OnWindowClosed(string path) {
            WWindow window = GetWindow<WWindow>(path);
            if (window == null) {
                return;
            }

            if (window.ToBackClicked && window.mBackOption == BackOption.NeedBack) {
                PopWindow(window);
            }

            if (window.ToBackClicked && window.mBackOption == BackOption.Normal) {
                PopWindow(window, false);
            }

            if (!window.ToBackClicked) {
                PopWindow(window, false);
            }

            if (window.ToBackClicked && window.mBackOption == BackOption.NeedBackRoot) {
                if (mCurrentQueue.Contains(path)) {
                    string peekPath = string.Empty;
                    while (mCurrentQueue.Count > 0) {
                        peekPath = mCurrentQueue.Pop();
                        Singleton.RemoveParam(peekPath);
                        if (peekPath == path) {
                            break;
                        }
                    }
                }
            }

            if (mCurActiveWindow.Contains(window)) {
                mCurActiveWindow.Remove(window);
            }
            if (mCantBackWindow.Contains(window)) {
                mCantBackWindow.Remove(window);
            }
            for (int i = 0; i < windowOnHide.Count; ++i) {
                windowOnHide[i]?.Invoke();
            }

            if (window.immediateDestroy) {
                if (mAllWindows.ContainsKey(window.path)) {
                    mAllWindows.Remove(window.path);
                }
                ObjectManager.Singleton.TryDestory(window.gameObject);
            }
        }

        public void CheckIsRemoveActWindow() {

        }

        /// <summary>
        /// 删除某一个活动的窗口
        /// </summary>
        /// <param name="window"></param>
        public void RemoveCurActivityWindow(WWindow window) {
            if (mCurActiveWindow == null) return;
            if (mCurActiveWindow.Contains(window)) {
                mCurActiveWindow.Remove(window);
            }
        }

        private void OnWindowToBack(string path) {
            WWindow w = GetWindow<WWindow>(path);
            if (w == null) {
                return;
            }

            //if (w.HaveBackground()) {
            //    FadeInTopWindow(w);
            //}

            for (int i = 0; i < windowToback.Count; ++i) {
                windowToback[i]?.Invoke();
            }
        }

        public void FadeOutTopWindow(WWindow w) {
            if (!w.IsHaveBackground) return;
            int count = 0;
            int curActiveCount = mCurActiveWindow.Count;
            for (int i = curActiveCount - 1; i >= 0; i--) {
                var window = mCurActiveWindow[i];
                if (window.IsHaveBackground) {
                    count += 1;
                }
            }
            BgRoot.Singleton.ShowBg(w, count > 0);
        }

        public void FadeInTopWindow(WWindow w) {
            int count = mCurActiveWindow.Count;
            if (count > 0) {
                for (int i = count - 1; i >= 0; i--) {
                    var window = mCurActiveWindow[i];
                    if (w != window && window.IsHaveBackground) {
                        BgRoot.Singleton.HideBg(window);
                        return;
                    }
                }
            }
            BgRoot.Singleton.HideBg(null);
            return;
        }

        /// <summary>
        /// 推出一个队列中的窗口
        /// </summary>
        private void PopWindow(WWindow window, bool isShow = true) {
            if (mCurrentQueue.Count == 0) {
                return;
            }
            string path = string.Empty;
            if (mCurrentQueue.Contains(window.path)) {
                WWindow w = null;
                //最顶端不是当前界面， 全部移除
                while (mCurrentQueue.Count > 0) {
                    path = mCurrentQueue.Pop();
                    w = GetWindow<WWindow>(path);
                    if (window == w) {
                        break;
                    }
                }
            }

            if (mCurrentQueue.Count > 0) {
                path = mCurrentQueue.Peek();
                IWindowParam param = GetParam(path);
                RemoveParam(path);
                if (isShow) {
                    ShowWindowInno(path, param);
                }
            }
        }

        private void PushWindow(string path, WWindow window, IWindowParam param) {
            if (mCurActiveWindow.Contains(window)) {
                mCurActiveWindow.Remove(window);
            }

            mCurActiveWindow.Add(window);

            if (window.mBackOption == BackOption.NeedBack ||
                window.mBackOption == BackOption.NeedBackRoot) {
                if (mCurrentQueue.Contains(path)) {
                    //移除多余的
                    string peekPath;
                    while (mCurrentQueue.Count > 0) {
                        peekPath = mCurrentQueue.Pop();
                        RemoveParam(peekPath);
                        if (peekPath == path) {
                            break;
                        }
                    }
                }
                mCurrentQueue.Push(path);
                SaveParam(path, param);
            }
            if (window.mDontClean) {
                mCantBackWindow.Add(window);
            }
        }

        /// <summary>
        /// 获得一个窗口脚本基类引用
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public T GetWindow<T>(string path) where T : WWindow {
            if (!mAllWindows.TryGetValue(path, out WWindow w)) {
                return null;
            }
            return w as T;
        }

        /// <summary>
        /// 关掉某个类型的界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HideWindows<T>(bool playTween = true) where T : WWindow {
            bool cacheShowTween = false;
            for (int i = mCurActiveWindow.Count - 1; i >= 0; i--) {
                WWindow w = mCurActiveWindow[i];
                if (w is T && !w.Hiding) {
                    cacheShowTween = w.ShowTween;
                    w.ShowTween = playTween;
                    w.Hide();
                    w.ShowTween = cacheShowTween;
                }
            }
        }

        public void SendMsg(string windowPath, IWindowParam msg = null) {
            WWindow w = GetWindow<WWindow>(windowPath);
            if (w == null) {
                return;
            }
            w.OnRevMsg(msg);
        }

        /// <summary>
        /// 获得一个窗口是否显示
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool GetWindowActive(string path) {
            if (!mAllWindows.TryGetValue(path, out WWindow w)) {
                return false;
            }
            return w.activeInHierarchy;
        }

        public bool SetEventSystemEnabled(object who, bool bEnabled, bool ignoreTime = false) {
            if (mEventMask == null || who == null) {
                return false;
            }

            if (bEnabled) {
                if (!mEventSystemDisabaleCache.ContainsKey(who)) {
                    return false;
                }

                mEventSystemDisabaleCache.Remove(who);
                if (mEventSystemDisabaleCache.Count > 0) {
                    return false;
                }
            } else {
                if (!mEventSystemDisabaleCache.ContainsKey(who)) {
                    mEventSystemDisabaleCache.Add(who, new EventSystemDisable() {
                        time = Time.realtimeSinceStartup,
                        ignoreTime = ignoreTime
                    });
                }
            }
            mEventMask.gameObject.SetActive(!bEnabled);
            return true;
        }

        public bool IsWideScreen() {
            if ((float)Screen.width / (float)Screen.height > 16.0f / 9f) {
                return true;
            }
            return false;
        }

        public static bool IsPad() {
            var per = (float)Screen.width / (float)Screen.height;
            if (per >= (4f / 3f) && per <= (16f / 9f)) {
                return true;
            }
            return false;
        }

        public void ForceEnableEventSystem() {
            if (mEventMask == null) {
                return;
            }
            mEventSystemDisabaleCache.Clear();
            mEventMask.gameObject.SetActive(false);
        }

        public bool HasWindowActive() {
            int count = mCurActiveWindow == null ? 0 : mCurActiveWindow.Count;
            for (int i = 0; i < count; i++) {
                if (mCurActiveWindow[i].activeInHierarchy) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 为一个Button增加点击事件/点击动画/点击事件/点击音效
        /// </summary>
        /// <param name="uIButton"></param>
        /// <param name="call">点击事件</param>
        /// <param name="animType">动画类型</param>
        /// <param name="audioType">音效</param>
        public static void AddListenerAndVoice(UIButtonExtension uIButton, UnityAction call,
            ButtonAnimationType animType = ButtonAnimationType.CommonAnim,
            string audioType = "ui_click_common", bool breakGuide = true) {
            if (uIButton == null) {
                return;
            }
            if (uIButton.transition == UnityEngine.UI.Selectable.Transition.ColorTint) {
                uIButton.transition = UnityEngine.UI.Selectable.Transition.None;
            }
            uIButton.onClick.RemoveAllListeners();
            uIButton.onClick.AddListener(() => {
                call?.Invoke();
            });

            switch (animType) {
                case ButtonAnimationType.None:
                    break;
                case ButtonAnimationType.CommonAnim:
                    AddCommonBtnAnim(uIButton);
                    break;
                case ButtonAnimationType.ClickAnim:
                    AddBtnClickAnim(uIButton);
                    break;
                case ButtonAnimationType.CommonAnimNotEnterExit:
                    AddCommonBtnNotEnterExitAnim(uIButton);
                    break;
            }
        }



        public static void AddDownListenerAndVoice(UIButtonExtension uIButton, UnityAction call,
            ButtonAnimationType animType = ButtonAnimationType.CommonAnim,
            string audioType = "ui_click_common", bool breakGuide = true) {
            if (uIButton == null) {
                return;
            }
            if (uIButton.transition == UnityEngine.UI.Selectable.Transition.ColorTint) {
                uIButton.transition = UnityEngine.UI.Selectable.Transition.None;
            }
            switch (animType) {
                case ButtonAnimationType.None:
                    uIButton.onDown.RemoveAllListeners();
                    uIButton.onUp.RemoveAllListeners();
                    uIButton.onEnter.RemoveAllListeners();
                    uIButton.onExit.RemoveAllListeners();
                    break;
                case ButtonAnimationType.CommonAnim:
                    AddCommonBtnAnim(uIButton);
                    break;
                case ButtonAnimationType.ClickAnim:
                    AddBtnClickAnim(uIButton);
                    break;
            }
            uIButton.onDown.AddListener(() => {
                if (Input.touchCount >= 2) {
                    return;
                }
                call?.Invoke();
            });
        }

        /// <summary>
        /// 循环列表全局开始拖拽事件
        /// </summary>
        private static void OnGlobalLoopViewListBeginDrag() {
        }

        /// <summary>
        /// 判断当前是否只有主界面存在 给上层调用
        /// </summary>
        /// <returns></returns>
        private bool IsOnlyHaveMainWindowNaitve() {
            if (GetWaitShowUISequeueCount > 0) {
                //Debug.Log("隐藏界面成功的通知 : UI对列中有等待展示数量不得触发引导相关内容");
                return false;
            }
            if (!IsOnlyMainWindowShow()) {
                //Debug.Log("隐藏界面成功的通知 : IsOnlyMainWindowShow返回为False证明当前不只有主页面存在不得触发引导相关内容");
                return false;
            }
            // if (!GetWindowActive(UIMainWindow.WindowPath)) {
            //     //Debug.Log("隐藏界面成功的通知 : 主界面没有显示出来 不得触发引导相关内容");
            //     return true;
            // }
            return true;
        }

        /// <summary>
        /// 隐藏界面成功的通知
        /// </summary>
        public void CheckIsOnlyHaveMainWindow() {
            if (GetWaitShowUISequeueCount > 0) {
                //Debug.Log("隐藏界面成功的通知 : UI对列中有等待展示数量不得触发引导相关内容");
                return;
            }
            if (!IsOnlyMainWindowShow()) {
                //Debug.Log("隐藏界面成功的通知 : IsOnlyMainWindowShow返回为False证明当前不只有主页面存在不得触发引导相关内容");
                return;
            }
            // if (!GetWindowActive(UIMainWindow.WindowPath)) {
            //     //Debug.Log("隐藏界面成功的通知 : 主界面没有显示出来 不得触发引导相关内容");
            //     return;
            // }
            Debug.Log("隐藏界面触发成功的通知 ！！！");
            if (onlyHaveMainWindowEvent == null || onlyHaveMainWindowEvent.Count <= 0) {
                //Debug.Log("隐藏界面成功的通知 : 引导事件列表为空 不得触发引导相关内容");
                return;
            }
            for (int i = 0; i < onlyHaveMainWindowEvent.Count; i++) {
                onlyHaveMainWindowEvent[i]?.Invoke();
            }

            //
        }

        public void OpenWindowEvent() {
        }

        /// <summary>
        /// 点击动画
        /// </summary>
        /// <param name="uIButton"></param>
        private static void AddBtnClickAnim(UIButtonExtension uIButton) {
            Vector3 startScale = uIButton.transform.localScale;
            uIButton.onClick.AddListener(() => {
                // Tweens Group
                LeanTween.scale(uIButton.gameObject,
                   startScale * 1.1f, 0.4f).
                    setEase(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(0.65f, -0.125f), new Keyframe(1f, 0f))).
                    setFrom(startScale).updateNow();
            });
        }

        /// <summary>
        /// 注册一个UI
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fun"></param>
        public static void UIRegisterCreaterPath(string path, Func<WWindow> fun) {
            if (windowCreatrDic == null) {
                return;
            }
            if (windowCreatrDic.ContainsKey(path)) {
                windowCreatrDic[path] = fun;
            } else {
                windowCreatrDic.Add(path, fun);
            }
        }

        /// <summary>
        /// 通用动画
        /// </summary>
        /// <param name="uIButton"></param>
        private static void AddCommonBtnAnim(UIButtonExtension uIButton) {
            if (uIButton == null || uIButton.gameObject == null) {
                return;
            }
            uIButton.onDown.RemoveAllListeners();
            Vector3 startScale = uIButton.transform.localScale;
            uIButton.onDown.AddListener(() => {
               if (uIButton != null && uIButton.gameObject != null) {
                    LeanTween.scale(uIButton.gameObject, startScale * 0.9f, 0.1f)?.setEaseOutQuad();
                }
            }); 
            uIButton.onEnter.RemoveAllListeners();
            uIButton.onEnter.AddListener(() => {
                if (uIButton != null && uIButton.gameObject != null && uIButton.isDown) {
                    LeanTween.scale(uIButton.gameObject, startScale * 0.9f, 0.1f).setEaseOutQuad();
                }
            });
            uIButton.onUp.RemoveAllListeners();
            uIButton.onUp.AddListener(() => {
                if (uIButton != null && uIButton.gameObject != null) {
                    LeanTween.scale(uIButton.gameObject, startScale, 0.1f).setEaseOutQuad();
                }
            });
            uIButton.onExit.RemoveAllListeners();
            uIButton.onExit.AddListener(() => {
                if (uIButton != null && uIButton.gameObject != null) {
                    LeanTween.scale(uIButton.gameObject, startScale, 0.1f).setEaseOutQuad();
                }
            });
        }

        /// <summary>
        /// 通用动画
        /// </summary>
        /// <param name="uIButton"></param>
        private static void AddCommonBtnNotEnterExitAnim(UIButtonExtension uIButton) {
            if (uIButton == null || uIButton.gameObject == null) {
                return;
            }
            uIButton.onDown.RemoveAllListeners();
            Vector3 startScale = uIButton.transform.localScale;
            uIButton.onDown.AddListener(() => {
                if (uIButton != null && uIButton.gameObject != null) {
                    LeanTween.scale(uIButton.gameObject, startScale * 0.95f, 0.1f)?.setEaseOutQuad();
                }
            });
            uIButton.onUp.RemoveAllListeners();
            uIButton.onUp.AddListener(() => {
                if (uIButton != null && uIButton.gameObject != null) {
                    LeanTween.scale(uIButton.gameObject, startScale, 0.1f).setEaseOutQuad();
                }
            });
        }

        public int GetCurrentQueueCount() {
            return mCurrentQueue.Count;
        }

        public override void Clear() {
            if (onlyHaveMainWindowEvent != null) {
                onlyHaveMainWindowEvent.Clear();
            }
            OnWindowShowAction.Clear();
            OnWindowHideAction.Clear();
            LoopListView2.onGlobalLoopViewListBeginDrag = null;
        }

        /// <summary>
        /// 当前有window显示时调用
        /// </summary>
        public void OnWindowShow(string windowName) {
            for (int i = 0; i < OnWindowShowAction.Count; ++i) {
                OnWindowShowAction[i]?.Invoke(windowName);
            }
        }

        /// <summary>
        /// 当有window隐藏时调用
        /// </summary>
        public void OnWindowHide(string windowName) {
            for (int i = 0; i < OnWindowHideAction.Count; ++i) {
                OnWindowHideAction[i]?.Invoke(windowName);
            }
        }

        /// <summary>
        /// 最顶层的窗口
        /// </summary>
        /// <returns></returns>
        public WWindow TopWindow {
            get {
                if (mCurActiveWindow.Count <= 0) {
                    return null;
                }
                return mCurActiveWindow[mCurActiveWindow.Count - 1];
            }
        }

        /// <summary>
        /// 焦点恢复触发事件
        /// </summary>
        public void OnApplicationFocusTrue() {
            //SendMsg(UIMainWindow.WindowPath, new IWindowParam((int)UIMainWindow.MainWindowEvent.RefreshAllIcon));
        }

        public void OnApplicationFocusFalse() {
            //SendMsg(UIMainWindow.WindowPath, new IWindowParam((int)UIMainWindow.MainWindowEvent.RefreshAllIcon));
        }

        public bool MaskEventEnable = false;
        /// <summary>
        /// 设置屏蔽操作(UI+场景)
        /// </summary>
        /// <param name="enable"></param>
        public void SetMaskEventEnable(bool enable) {
            var _mCanvas = GameObject.Find("mCanvas")?.transform;
            if (_mCanvas != null) {
                var _mMaskEvent = _mCanvas.transform.Find("MaskEvent");
                if (_mMaskEvent != null) {
                    _mMaskEvent.gameObject.SetActive(enable);
                    MaskEventEnable = enable;
                }
            }
        }
    }
}
