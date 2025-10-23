using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MM.UI;

namespace SharedLibary {
    /// <summary>
    /// UI的根窗体
    /// </summary>
    public class WindowRoot : WObject {

        public static int offsize {
            get {
                _offsize = 80;
                return _offsize;
            }
        }
        private static int _offsize = 0;

        /// <summary>
        /// 当前的windowroot
        /// </summary>
        private static WindowRoot mInstance = null;
        public static WindowRoot Singleton {
            get {
                if (mInstance == null) {
                    mInstance = new WindowRoot();
                }
                return mInstance;
            }
        }

        public GraphicRaycaster GraphicRaycaster => mAdapter.GetGraphicRaycasterl();
        public EventSystem EventSystem => mAdapter.GetEventSystem();
        public Canvas Canvas => mAdapter.GetCanvas();
        public CanvasScaler CanvasScaler => mAdapter.GetCanvasScaler();

        public Transform GetWindowTop => mAdapter.GetWindowTop();

        public GameObject GetGameObject => mAdapter.GetGameObject();

        public override bool DontDestory => true;

        public void Create() {
            Create(mInstance, mAdapter.GetGameObject());
        }
        public bool IsEventSystemEnabled() {
            return mAdapter.IsEventSystemEnabled();
        }
        /// <summary>
        /// WindowRoot适配器
        /// </summary>
        private IWindowRootAdapter mAdapter = null;
        public void SetAdapter(IWindowRootAdapter adapter) {
            mAdapter = adapter;
        }

        protected override void InitUI() {
            mAdapter.InitWindowRoot();
            UIManager.Singleton.Init(
                transform,
                mAdapter.GetWindowTop(),
                mAdapter.GetWindowLow(),
                mAdapter.GetEventMask() != null ? mAdapter.GetEventMask() : mAdapter.GetEventSystem().transform);
            //初始化bg节点
            BgRoot.Singleton.InitBG();
        }

        protected override void OnDestroy() {
        }
    }
}
