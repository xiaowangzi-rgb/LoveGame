using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MM.Common;

namespace SharedLibary {
    public enum DestroyCondition {
        Immediate = 0,
        Dont = 1,
    }
    /// <summary>
    /// 所有的界面最外层都需要继承这个类
    /// </summary>
    public abstract class WWindow : WObject {
        public enum ANIM_TYPE {
            POP_UP,
            FULL_SCREEN,
        }

        /// <summary>
        /// 适配设置
        /// </summary>
        public enum Adaptation_Type {
            Normal,
            Full_screen
        }

        public enum LAYER_TYPE {
            LOW,
            NORMAL,
            TOP
        }

        private RectTransform mRectTransform = null;
        public RectTransform rectTransform {
            get {
                if (mRectTransform == null) {
                    mRectTransform = transform as RectTransform;
                }
                return mRectTransform;
            }
        }

        /// <summary>
        /// 弹出窗口名称 如果没有赋值的话就是空的 就说明不是弹窗
        /// </summary>
        /// <value></value>
        protected string _ShowPopupWindowName { get; set; } = "";

        /// <summary>
        /// 是否是顶层窗口
        /// </summary>
        public virtual LAYER_TYPE WindowLayer { get; set; } = LAYER_TYPE.NORMAL;

        /// <summary>
        /// 弹出窗口类型
        /// </summary>
        /// <value></value>
        public virtual WindowType _PopUpWindowType { get; } = WindowType.Default;

        /// <summary>
        /// 自定义父节点
        /// </summary>
        /// <value></value>
        public virtual Transform WindowCustomRoot {get;} = null;

        public virtual bool bDialogBox { get; } = false;

        protected virtual Adaptation_Type AdaptationType { get; } = Adaptation_Type.Normal;

        /// <summary>
        ///播放动画时是否屏蔽事件通知
        /// </summary>
        protected bool mPlayTweenStopEvent = true;

        /// <summary>
        /// 当期窗口的名字
        /// </summary>
        public string windowName { get; set; }

        /// <summary>
        /// 打开关闭时是否播放动画
        /// </summary>
        public bool ShowTween { get; set; } = true;

        /// <summary>
        /// 当前状态已经是显示，是否还需要调显示动画
        /// </summary>
        /// <value></value>
        public virtual bool ActivityedCallShow { get; } = true;

        private IAnimationAdapter mAnimation = null;
        public IAnimationAdapter animation {
            get {
                if (mAnimation == null) {
                    mAnimation = GetAnimation();
                }
                return mAnimation;
            }
        }

        /// <summary>
        /// 是否在播放隐藏动画
        /// </summary>
        public bool Hiding {
            get {
                if (animation == null) {
                    return false;
                }
                return animation.Hiding();
            }
        }

        public bool ToBackClicked { get; set; } = false;

        public virtual ANIM_TYPE mAnimType { get; } = ANIM_TYPE.POP_UP;

        protected IWindowParam mParam;
        protected bool mbStopEventing = false;

        /// <summary>
        /// The window need back.
        /// </summary>
        public virtual UIManager.BackOption mBackOption { get; protected set; } = UIManager.BackOption.Normal;
        /// <summary>
        /// The window need clean.
        /// </summary>
        public virtual bool mDontClean => false;
        /// <summary>
        /// 界面关闭后是否自动删除
        /// 0：不自动删除；
        /// 1：立即删除；
        /// </summary>
        protected DestroyCondition mDestroyCondition = DestroyCondition.Dont;

        /// <summary>
        /// 打开时是否通知UIManager
        /// </summary>
        public virtual bool NeedNotifyManager => true;

        /// <summary>
        /// 打开窗口音效
        /// </summary>
        protected string mStrAudio = "ui_boardopen";

        /// <summary>
        /// 是否在显示之前就已经是activity状态
        /// </summary>
        public bool beforeShowActivity { get; private set; } = false;

        /// <summary>
        /// 点击空白处返回
        /// </summary>
        protected bool mClickToBack = false;

        /// <summary>
        /// 删除标志
        /// </summary>
        // protected bool mImmediateDestroy = false;

        protected bool isPlayAudio = true;
        public bool immediateDestroy {
            get {
                return mDestroyCondition == DestroyCondition.Immediate;
            }
        }

        public string path { get; set; }

        public event Action<string> onHideAction;

        public event Action<string> mOnToBackAction;

        public Action OnTweenEndShow;
        public Action OnTweenEndHide;

        protected uint showWindowAudioId;


        protected virtual bool bDelayedTweenStartFunc { get; set; } = true;

        /// <summary>
        /// 是否有背景需求
        /// </summary>
        public virtual bool IsHaveBackground { get; set; } = true;

        /// <summary>
        /// 是否屏蔽场景渲染
        /// </summary>
        protected virtual bool IsBlockSceneRendering { get; set; } = false;

        protected override void InitUI() {
            animation?.Init(this);
            if (animation != null && animation.Background != null && AdaptationType == Adaptation_Type.Full_screen) {
                SetBGAdaptation(animation.Background);
            }

            Graphic[] allGraphics = gameObject.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < allGraphics.Length; i++) {
                if (allGraphics[i].raycastTarget == false) {
                    GraphicRegistry.UnregisterGraphicForCanvas(WindowRoot.Singleton.Canvas, allGraphics[i]);
                }
            }
        }

        /// <summary>
        /// 设置弹出窗口名称
        /// </summary>
        /// <param name="name"></param>
        public void SetShowPopupWindowName(string name) {
            _ShowPopupWindowName = name;
        }
        
        /// <summary>
        /// 获取弹出窗口名称
        /// </summary>
        /// <returns></returns>
        public string GetShowPopupWindowName(){
            return _ShowPopupWindowName;
        }

        protected override void AddEvent() {
            if (mClickToBack) {
                if (IsHaveBackground) {

                    if (gameObject.GetComponent<Image>()) {
                        UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Image>());
                    }
                    if (!gameObject.GetComponent<Empty4Raycast>()) {
                        gameObject.AddComponent<Empty4Raycast>();
                    }
                }
                Tools.ListenerEvent(gameObject, EventTriggerType.PointerClick, OnPointerClick, true);
            }
        }

        protected override void InitValue() {
        }

        public virtual void OnPointerClick(BaseEventData eventData) {
            if (mClickToBack && gameObject.Equals(((PointerEventData)eventData).pointerEnter)) {
                ToBack();
            }
        }
        public void ClearAction() {
            if (onHideAction != null) {
                Delegate[] dels = onHideAction.GetInvocationList();
                if (dels != null) {
                    for (int i = 0; i < dels.Length; ++i) {
                        onHideAction -= dels[i] as Action<string>;
                    }
                }
            }
            if (mOnToBackAction != null) {
                Delegate[] dels = mOnToBackAction.GetInvocationList();

                if (dels != null) {
                    for (int i = 0; i < dels.Length; ++i) {
                        mOnToBackAction -= dels[i] as Action<string>;
                    }
                }
            }
        }

        public void SetBGAdaptation(RectTransform bg) {
            if (bg == null) {
                return;
            }
            if (UIManager.Singleton.IsWideScreen()) {
                var rectTrans = bg;
                rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x + (2 * WindowRoot.offsize), rectTrans.sizeDelta.y);
                rectTrans.anchoredPosition = Vector2.zero;
            }
        }

        public virtual void Show(IWindowParam param) {
            if (IsHaveBackground) {
                UIManager.Singleton.FadeOutTopWindow(this);
            }
            //注册endshow回调
            OnTweenEndShow = null;
            OnTweenEndShow += TweenEndShow;

            mParam = param;
            Show();
            if (mBackOption != UIManager.BackOption.NeedBackRoot && isPlayAudio) {
                //showWindowAudioId = MM.Audio.AudioManager.Play(mStrAudio);
            }
        }

        public override void Show() {

            ToBackClicked = false;
            // mImmediateDestroy = false;

            if (NeedNotifyManager && WindowLayer == LAYER_TYPE.NORMAL) {
                UIManager.Singleton.OnWindowShow(windowName);
            }

            if (gameObject == null || gameObject.transform == null) {
                return;
            }

            beforeShowActivity = gameObject.activeInHierarchy;

            base.Show();
            if (ShowTween && animation != null) {
                animation.Show((doAnimation) => {
                    if (!doAnimation) {
                        OnTweenStart();

                        LeanTween.delayedCall(0.05f, () => {
                            TweenEndShow();
                        });
                    } else {
                        if (mPlayTweenStopEvent) {
                            UIManager.Singleton.SetEventSystemEnabled(this, false);
                            mbStopEventing = true;
                        }

                        if (bDelayedTweenStartFunc) {
                            LeanTween.delayedCall(0.0f, () => {
                                OnTweenStart();
                            });
                        } else {
                            OnTweenStart();
                        }
                    }
                }, TweenEndShow);
            } else {
                OnTweenStart();

                LeanTween.delayedCall(0.05f, () => {
                    TweenEndShow();
                });
            }
        }
        /// <summary>
        /// 动画播完之后显示
        /// </summary>
        protected virtual void OnTweenEnd() {
            if (IsBlockSceneRendering && gameObject.activeSelf && !animation.Hiding()) {
                //MergeCoreMgr.Singleton.SetMergeSceneVisible(false);
            }
        }

        /// <summary>
        /// 开始播放显示动画
        /// </summary>
        protected virtual void OnTweenStart() {
        }
        /// <summary>
        /// 动画结束后
        /// </summary>
        private void TweenEndShow() {
            if (mPlayTweenStopEvent && mbStopEventing) {
                mbStopEventing = false;
                UIManager.Singleton.SetEventSystemEnabled(this, true);
            }
            OnTweenEnd();
        }

        /// <summary>
        /// 隐藏，设置为隐藏状态，播放动画，待动画播放完成再隐藏
        /// </summary>
        public override void Hide() {
            if (animation != null && animation.Hiding()) {
                return;
            }
            OnTweenEndHide = null;
            OnTweenEndHide += TweenEndHide;

            // if (mDestroyCondition != DestroyCondition.Dont) {
            //     mImmediateDestroy = true;
            // }

            if (ShowTween && animation != null) {
                animation.Hide((doAnimation) => {
                    if (!doAnimation) {
                        TweenEndHide();
                    } else if (mPlayTweenStopEvent) {
                        UIManager.Singleton.SetEventSystemEnabled(this, false);
                        mbStopEventing = true;
                    }
                }, TweenEndHide);
            } else {
                TweenEndHide();
            }
            if (isPlayAudio) {
                //MM.Audio.AudioManager.Play("ui_boardclose");
            }

            if (IsBlockSceneRendering) {
                //MergeCoreMgr.Singleton.SetMergeSceneVisible(true);
            }
        }

        /// <summary>
        /// 开始隐藏，播放隐藏动画
        /// </summary>
        protected virtual void TweenEndHide() {
            OnTweenEndHide = null;
            if (mPlayTweenStopEvent && mbStopEventing) {
                mbStopEventing = false;
                UIManager.Singleton.SetEventSystemEnabled(this, true);
            }
            OnHide();
        }

        protected virtual void OnHide() {
            if (onHideAction != null) {
                onHideAction.Invoke(path);
            }
            if (NeedNotifyManager && WindowLayer == LAYER_TYPE.NORMAL) {
                UIManager.Singleton.OnWindowHide(windowName);
            }
            SetActive(false);
            if (IsHaveBackground) {
                UIManager.Singleton.FadeInTopWindow(this);
            }
            if (mUITween != null) {
                mUITween?.OnTweenClear();
            }
        }

        public virtual void ToBack() {
            ToBackClicked = true;
            if (mOnToBackAction != null) {
                mOnToBackAction.Invoke(path);
            }
            Hide();
        }

        public virtual void FadeOutBackground() {
            animation?.FadeOutBackground();
        }

        public bool HaveBackground() {
            if (animation == null) {
                return false;
            } else {
                return animation.HaveBackground();
            }
        }

        public virtual void FadeInBackground() {
            animation?.FadeInBackground();
        }

        public virtual bool OnEscPress() {
            if (animation != null) {
                if (animation.Showing() || animation.Hiding()) {
                    return false;
                }
            }
            return true;
        }

        public virtual void OnRevMsg(IWindowParam msg) {

        }

        protected virtual IAnimationAdapter GetAnimation() {
            return new DefaultAnimationAdapter();
        }
    }
}
