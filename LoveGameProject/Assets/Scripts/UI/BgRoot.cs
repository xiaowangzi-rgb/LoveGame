using SharedLibary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MM.Common;
/// <summary>
/// UI窗口背景的节点管理器
/// </summary>
namespace MM.UI
{
    public class BgRoot
    {
        /// <summary>
        /// 当前的BgRoot
        /// </summary>
        private static BgRoot mInstance = null;
        public static BgRoot Singleton
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new BgRoot();
                }
                return mInstance;
            }
        }

        private const string BGName = "MainBG";

        /// <summary>
        /// 当前处理的窗口
        /// </summary>
        private WWindow CurTopWwindow { get; set; }
        /// <summary>
        /// bg
        /// </summary>
        private GameObject bg;
        public GameObject BG => this.bg;
        /// <summary>
        /// 这个接口只有在切换场景BG更变时调用
        /// </summary>
        public void InitBG()
        {
            CurTopWwindow = null;
            SetBG();
        }

        /// <summary>
        /// 设置BG
        /// </summary>
        /// <returns></returns>
        private GameObject SetBG()
        {
            if (UIManager.Singleton.mWindowRoot == null)
            {
                Debug.LogError("当前场景的节点为空,bg设置失败");
                return null;
            }
            var canvasRoot = UIManager.Singleton.mWindowRoot;
            if (canvasRoot.transform?.Find(BGName))
            {
                bg = canvasRoot.transform?.Find(BGName).gameObject;
            }
            if (bg == null)
            {
                bg = CreateBG(canvasRoot.transform).gameObject;
            }
            InitBgData(true);
            return bg;
        }

        /// <summary>
        /// 创建一个新的BG
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private Transform CreateBG(Transform parent)
        {
            if (parent == null)
            {
                return null;
            }
            var tmpBG = new GameObject();
            tmpBG.transform.SetParent(parent);
            tmpBG.name = BGName;
            return tmpBG.transform;
        }

        /// <summary>
        /// 初始化BG相关的样式和位置
        /// </summary>
        private void InitBgData(bool isSetColor, bool isSetLayer = true)
        {
            if (bg == null)
            {
                return;
            }
            var rect = bg.gameObject.GetComponent<RectTransform>();
            if (rect == null)
            {
                rect = bg.gameObject.AddComponent<RectTransform>();
            }
            var img = rect.gameObject.GetComponent<Image>();
            if (img == null)
            {
                img = rect.gameObject.AddComponent<Image>();
            }
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = Vector2.one * 0.5f;
            rect.localPosition = Vector2.zero;
            rect.localScale = Vector2.one;
            rect.rotation = Quaternion.identity;
            if (isSetLayer)
            {
                rect.gameObject.layer = LayerMask.NameToLayer("UI");
            }
            if (isSetColor)
            {
                img.color = new Color(0, 0, 0, 0f);
            }
            img.raycastTarget = true;
            rect.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public bool ShowBg(WWindow window, bool isHaveOtherWindow)
        {
            // if (SingleScene.Singleton.CurrentScene == null) {
            //     return false;
            // }
            // if (!SingleScene.Singleton.CurrentScene.SceneReady) {
            //     return false;
            // }
            // if (SingleScene.Singleton.CurrentScene.SceneLoader == null) {
            //     return false;
            // }
            // if (SingleScene.Singleton.CurrentScene.SceneLoader.IsLoading) {
            //     return false;
            // }

            if (window == null)
            {
                return false;
            }
            bg.gameObject.SetActive(true);
            //设置当前进入的wwindow
            this.CurTopWwindow = window;
            //首先设置bg的层级和位置顺序
            SetCanvasLayer(window);
            SetBGLayer(window);
            SetBGSibling(window);
            SetBGAdaptation(window);
            //LogUtils.Log("进入窗口:" + window.path);
            //如果有其他的window界面显示的话就直接调用非动画的接口
            window?.animation?.ShowBackground(bg, !isHaveOtherWindow);
            return true;
        }

        /// <summary>
        /// 隐藏BG
        /// </summary>
        /// <returns></returns>
        public bool HideBg(WWindow window)
        {
            //大概的逻辑是 首先获取uimanager中activitywindow 如果此时没有显示的window就
            //直接隐藏bg 然后重置bg位置 如果此时有显示的bg 就设置bg到这个显示的bg下面

            if (window == null)
            {
                if (CurTopWwindow != null)
                {
                    CurTopWwindow?.animation?.HideBackground(bg, true, () =>
                    {
                        if (bg != null && bg.transform != null)
                        {
                            bg?.transform.SetParent(UIManager.Singleton.mWindowRoot.transform);
                            bg?.gameObject.SetActive(false);
                        }
                    });
                    //LogUtils.Log("离开窗口:" + CurTopWwindow.path);
                }
                else
                {
                    if (bg != null && bg.transform != null)
                    {
                        bg?.transform?.SetParent(UIManager.Singleton.mWindowRoot.transform);
                        bg?.gameObject?.SetActive(false);
                    }
                }
            }
            else
            {
                //UIManager.Singleton.RemoveCurActivityWindow();
                if (CurTopWwindow != window)
                {
                    bg?.gameObject.SetActive(true);
                    CurTopWwindow = window;
                    SetCanvasLayer(window);
                    SetBGLayer(window);
                    SetBGSibling(window);
                    SetBGAdaptation(window);
                    //LogUtils.Log("重新进入窗口:" + window.path);
                    CurTopWwindow?.animation?.ShowBackground(bg, false);
                }
            }
            return true;
        }

        /// <summary>
        /// 设置bg到和这个window同一层级下
        /// </summary>
        /// <param name="window"></param>
        private void SetBGLayer(WWindow window)
        {
            if (window == null) return;
            var layer = window.WindowLayer;
            switch (layer)
            {
                case WWindow.LAYER_TYPE.LOW:
                    bg.transform.SetParent(UIManager.Singleton.mWindowLow);
                    break;
                case WWindow.LAYER_TYPE.NORMAL:
                    bg.transform.SetParent(UIManager.Singleton.mWindowRoot);
                    break;
                case WWindow.LAYER_TYPE.TOP:
                    bg.transform.SetParent(UIManager.Singleton.mWindowTop);
                    break;
            }
            InitBgData(false);
        }

        /// <summary>
        /// 设置CanvasLayer
        /// </summary>
        /// <param name="window"></param>
        private void SetCanvasLayer(WWindow window)
        {
            if (window == null || window.transform == null || window.gameObject == null) return;
            var canvas = window.gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                var bgRaycast = bg.GetComponent<GraphicRaycaster>();
                if (bgRaycast != null)
                {
                    UnityEngine.Object.Destroy(bgRaycast);
                }
                var bgCanvas = bg.GetComponent<Canvas>();
                if (bgCanvas != null)
                {
                    UnityEngine.Object.Destroy(bgCanvas);
                }
            }
            else
            {
                var bgCanvas = bg.AddComponentIfNotExist<Canvas>();
                var bgRaycast = bg.AddComponentIfNotExist<GraphicRaycaster>();
                bgCanvas.enabled = true;
                bgRaycast.enabled = true;
                bgCanvas.overrideSorting = true;
                bgCanvas.sortingLayerName = canvas.sortingLayerName;
                bgCanvas.sortingOrder = canvas.sortingOrder - 1;
            }
        }

        /// <summary>
        /// 设置到这个window的下面 特殊情况就是这个window如果是最下层就设置在这一层的最上面
        /// 返回设置层级的index
        /// </summary>
        /// <param name="window"></param>
        private int SetBGSibling(WWindow window)
        {
            if (window == null || window.transform == null || bg == null || bg.transform == null)
            {
                return -1;
            }
            int targetWindowIndex = window.transform.GetSiblingIndex();
            var result = 0;
            if (targetWindowIndex <= 0)
            {
                //LogUtils.Log("目标窗口在最上层，bg直接设置为最上层");
                bg.transform.SetAsFirstSibling();
            }
            else
            {
                result = targetWindowIndex - 1;
                //如果是一个层级 就需要做一个特殊处理就是之前bg的index如果大于你要设置的目标index就将结果设置成你的目标结果 不是就直接减一就行
                if (window.transform.parent == bg.transform.parent)
                {
                    var nowBGindex = bg.transform.GetSiblingIndex();
                    if (nowBGindex > result)
                    {
                        result = targetWindowIndex;
                    }
                }
                //LogUtils.Log($"目标窗口在{targetWindowIndex}层，bg直接设置为{result}当前层");
                bg.transform.SetSiblingIndex(result);
            }
            return result;
        }

        private void SetBGAdaptation(WWindow window)
        {
            if (window == null || bg == null) return;
            window.SetBGAdaptation(bg.GetComponent<RectTransform>());
        }

        public void ResetBg()
        {
            CurTopWwindow = null;
            InitBgData(true);
        }
    }
}
