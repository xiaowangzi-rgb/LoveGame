using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SharedLibary;
using System.Collections.Generic;

namespace MM {
    public class MMWindowRootAdapter : IWindowRootAdapter {

        /// <summary>
        /// RectTransform
        /// </summary>
        private RectTransform mUIWindowRoot;
        /// <summary>
        /// 顶层UI的父
        /// </summary>
        private RectTransform mWindowTop;
        /// <summary>
        /// 底层UI的父
        /// </summary>
        public RectTransform mWindowLow { get; private set; }
        /// <summary>
        /// CanvasScaler
        /// </summary>
        private CanvasScaler mCanvasScaler;
        /// <summary>
        /// EventMask的Object
        /// </summary>
        private RectTransform mEventMask;
        /// <summary>
        /// GraphicRaycaster
        /// </summary>
        private GraphicRaycaster mGraphicRaycaster;
        /// <summary>
        /// EventSystem
        /// </summary>
        private EventSystem mEventSystem;
        /// <summary>
        /// Canvas
        /// </summary>
        private Canvas mCanvas;
        /// <summary>
        /// GetCanvas
        /// </summary>
        /// <returns></returns>
        public Canvas GetCanvas() {
            return mCanvas;
        }
        /// <summary>
        /// GetCanvasScaler
        /// </summary>
        /// <returns></returns>
        public CanvasScaler GetCanvasScaler() {
            return mCanvasScaler;
        }
        /// <summary>
        /// GetEventSystem
        /// </summary>
        /// <returns></returns>
        public EventSystem GetEventSystem() {
            return mEventSystem;
        }
        /// <summary>
        /// GetGameObject
        /// </summary>
        /// <returns></returns>
        public GameObject GetGameObject() {
            return GameObject.Find("mCanvas/mUIWindowRoot");
        }
        /// <summary>
        /// GetGraphicRaycasterl
        /// </summary>
        /// <returns></returns>
        public GraphicRaycaster GetGraphicRaycasterl() {
            return mGraphicRaycaster;
        }
        /// <summary>
        /// InitWindowRoot
        /// </summary>
        public virtual void InitWindowRoot() {

            mCanvasScaler = GameObject.Find("mCanvas").GetComponent<CanvasScaler>();
            mUIWindowRoot = GameObject.Find("mCanvas/mUIWindowRoot").GetComponent<RectTransform>();
            mWindowTop = GameObject.Find("mCanvas/mWindowTop").GetComponent<RectTransform>();
            if (mEventSystem == null) {
                GameObject eventObj = GameObject.Find("mEventSystem");
                if (eventObj == null) {
                    Debug.LogError("can not find event system!");
                } else {
                    mEventSystem = eventObj.transform.GetComponent<EventSystem>();
                    mEventSystem.pixelDragThreshold = Screen.height / 50;
                }
            }
            mGraphicRaycaster = mUIWindowRoot.parent.GetComponent<GraphicRaycaster>();
            mCanvas = mGraphicRaycaster.GetComponent<Canvas>();

            if (mCanvas != null) {
                mEventMask = mCanvas.transform.Find("MaskEvent").GetComponent<RectTransform>();
                if (mEventMask != null) {
                    Tools.ListenerEvent(mEventMask.gameObject, EventTriggerType.PointerDown, (e) => { }, true);
                    Tools.ListenerEvent(mEventMask.gameObject, EventTriggerType.PointerClick, (e) => { }, true);
                    mEventMask.gameObject.SetActive(false);
                }
            }

            mWindowLow = GameObject.Find("mCanvas/mWindowLow").GetComponent<RectTransform>();
            if (UIManager.Singleton.IsWideScreen()) {

                mUIWindowRoot.offsetMin = Vector2.zero;
                mUIWindowRoot.offsetMax = Vector2.zero;

                mWindowLow.offsetMin = Vector2.zero;
                mWindowLow.offsetMax = Vector2.zero;

                mWindowTop.offsetMin = Vector2.zero;
                mWindowTop.offsetMax = Vector2.zero;

                mEventMask.offsetMin = Vector2.zero;
                mEventMask.offsetMax = Vector2.zero;

                mUIWindowRoot.sizeDelta = new Vector2(mUIWindowRoot.sizeDelta.x - (2 * WindowRoot.offsize), mUIWindowRoot.sizeDelta.y);
                mUIWindowRoot.anchoredPosition = Vector2.zero;

                mWindowLow.sizeDelta = new Vector2(mWindowLow.sizeDelta.x - (2 * WindowRoot.offsize), mWindowLow.sizeDelta.y);
                mWindowLow.anchoredPosition = Vector2.zero;

                mWindowTop.sizeDelta = new Vector2(mWindowTop.sizeDelta.x - (2 * WindowRoot.offsize), mWindowTop.sizeDelta.y);
                mWindowTop.anchoredPosition = Vector2.zero;

                mEventMask.sizeDelta = new Vector2(mEventMask.sizeDelta.x - (2 * WindowRoot.offsize), mEventMask.sizeDelta.y);
                mEventMask.anchoredPosition = Vector2.zero;
            }
        }

        private void ResetWindowRoot() {
        }
        /// <summary>
        /// GetWindowTop
        /// </summary>
        /// <returns></returns>
        public Transform GetWindowTop() {
            return mWindowTop;
        }
        /// <summary>
        /// GetEventMask
        /// </summary>
        /// <returns></returns>
        public Transform GetEventMask() {
            return mEventMask;
        }
        /// <summary>
        /// IsEventSystemEnabled
        /// </summary>
        /// <returns></returns>
        public bool IsEventSystemEnabled() {
            if (mEventSystem == null || mEventSystem.gameObject == null) {
                return false;
            }
            return mEventSystem.gameObject.activeInHierarchy;
        }

        public virtual Transform GetWindowLow() {
            return mWindowLow;
        }
    }
}
