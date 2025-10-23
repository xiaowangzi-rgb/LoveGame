using System;
using UnityEngine;
using UnityEngine.UI;

namespace SharedLibary {
    public class DefaultAnimationAdapter : IAnimationAdapter {

        public Transform AniObject { get; protected set; } = null;

        protected WWindow mWindow;
        /// <summary>
        /// 是否在播放隐藏动画
        /// </summary>
        protected bool mHiding_ = false;

        protected bool mShowing_ = false;
        protected float mFadeAlpha = 0.85f;

        public void FoceSetHiding(bool hiding) {
            mHiding_ = hiding;
        }
        /// <summary>
        /// 背景消失的时间
        /// </summary>
        protected virtual float alphaBgTime { get; set; } = 0;

        public float FadeAlpha { set { mFadeAlpha = value; } }

        public RectTransform Background { get; protected set; }

        public bool IsPlaying => mShowing_ || mHiding_;

        public virtual void Init(WWindow window) {
            mWindow = window;
            if (window.transform != null) {
                AniObject = window.transform.Find("Animation");
            }

            if (window.gameObject != null) {
                var bg = window.FindObject("BG");
                if (bg?.GetComponent<Empty4Raycast>() != null) {
                    Background = bg.GetComponent<RectTransform>();
                } else {
                    if (window.gameObject.GetComponent<Empty4Raycast>() != null) {
                        Background = window.rectTransform;
                    }
                }
            }
        }

        /// <summary>
        /// 是否在播放隐藏动画
        /// </summary>
        public bool Hiding() {
            return mHiding_;
        }

        /// <summary>
        /// 是否正在播放开始动画
        /// </summary>
        /// <returns></returns>
        public bool Showing() {
            return mShowing_;
        }

        public virtual void Show(Action<bool> OnTweenStart, Action finished) {
            mHiding_ = false;
            mShowing_ = true;
            bool doAnimation = false;
            Action onFinished = () => {
                mShowing_ = false;
                finished?.Invoke();
            };
            if (mWindow.mAnimType == WWindow.ANIM_TYPE.FULL_SCREEN) {
                mWindow.rectTransform.anchoredPosition3D = new Vector3(0, -UIManager.Singleton.canvasRect.sizeDelta.y, 0);
                LeanTween.move(mWindow.rectTransform, Vector3.zero, 0.3f)?.
                    setEaseOutQuad()?.
                    setOnComplete(onFinished);
                doAnimation = true;
            } else {
                doAnimation = InnoShow(onFinished);
            }
            OnTweenStart?.Invoke(doAnimation);
        }

        public virtual bool InnoShow(Action finished) {
            if (AniObject != null) {
                LeanTween.cancel(AniObject.gameObject);
                AniObject.gameObject.SetActive(true);
                AniObject.localScale = new Vector3(0.9f, 0.9f);
                // Tweens Group
                LeanTween.scale(AniObject.gameObject, Vector3.one, 0.4f)?.
                    setEase(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1.2f), new Keyframe(0.75f, 0.9f), new Keyframe(1f, 1f)))?.
                    setOnComplete(finished)?.updateNow();
                return true;
            }
            return false;
        }

        public virtual void Hide(Action<bool> after, Action finished) {
            bool doAnimation = false;
            mHiding_ = true;
            Action onFinished = () => {
                mHiding_ = false;
                finished?.Invoke();
            };
            if (mWindow.mAnimType == WWindow.ANIM_TYPE.FULL_SCREEN) {
                LeanTween.move(mWindow.rectTransform,
                    new Vector3(0, -UIManager.Singleton.canvasRect.sizeDelta.y, 0), 0.3f)?.
                    setEaseOutQuad()?.
                    setFrom(Vector3.zero)?.
                    setOnComplete(onFinished);
                doAnimation = true;
            } else {
                doAnimation = InnoHide(onFinished);
            }
            after(doAnimation);
        }

        public virtual bool InnoHide(Action finished) {
            if (AniObject != null) {
                if (AniObject.gameObject.activeSelf && AniObject.gameObject.activeInHierarchy) {
                    var ltID = LeanTween.scale(AniObject.gameObject, new Vector3(0.85f, 0.85f, 0.85f), 0.14f).
                           setEase(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.428f, -0.05f), new Keyframe(1f, 1f)))?.setOnComplete(() => {
                               AniObject?.gameObject?.SetActive(false);
                               finished?.Invoke();
                           }).updateNow();
                    return true;
                } else {
                    finished?.Invoke();
                    return false;
                }
            }
            return false;
        }

        public bool HaveBackground(bool checkType = true) {
            if (checkType) {
                return mWindow.mAnimType == WWindow.ANIM_TYPE.POP_UP
                    && Background != null;
            } else {
                return Background != null;
            }
        }

        public virtual void FadeInBackground() {
            if (mWindow.mAnimType != WWindow.ANIM_TYPE.POP_UP || Background == null || UIManager.Singleton.FadeOutBackground <= 0) {
                return;
            }

            //var image = Background.gameObject.GetComponent<Image>();
            //if (image != null) {
            //    var color = image.color;
            //    color.a = mFadeAlpha;
            //    image.color = color;
            //}
        }

        public virtual void FadeOutBackground() {
            if (mWindow.mAnimType != WWindow.ANIM_TYPE.POP_UP || Background == null) {
                return;
            }

            //var image = Background.gameObject.GetComponent<Image>();
            //if (image != null) {
            //    var color = image.color;
            //    color.a = 0f;
            //    image.color = color;
            //}
        }

        private static void alphaRecursive(Transform transform, float val, bool useRecursion = true) {
            Renderer renderer = transform.gameObject.GetComponent<Renderer>();
            if (renderer != null) {
                for (int i = 0; i < renderer.materials.Length; ++i) {
                    var mat = renderer.materials[i];
                    if (mat.HasProperty("_Color")) {
                        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, val);
                    } else if (mat.HasProperty("_TintColor")) {
                        Color col = mat.GetColor("_TintColor");
                        mat.SetColor("_TintColor", new Color(col.r, col.g, col.b, val));
                    }
                }
            }
            if (useRecursion && transform.childCount > 0) {
                for (int i = 0; i < transform.childCount; ++i) {
                    alphaRecursive(transform.GetChild(i), val);
                }
            }
        }

        public virtual void ShowBackground(GameObject bg, bool isTween) {
            if (bg == null) {
                return;
            }
            bg.gameObject.SetActive(true);
            LeanTween.cancel(bg.gameObject);
            Tools.AlphaImage(bg.gameObject, 0, mFadeAlpha, isTween ? 0.2f : alphaBgTime).updateNow().setOnComplete(() => { mWindow.OnTweenEndShow?.Invoke(); });
            UIManager.Singleton.FadeOutBackground++;
        }

        public virtual void HideBackground(GameObject bg, bool isTween, Action onSuccess) {
            if (bg == null) {
                return;
            }
            bg.gameObject.SetActive(true);
            LeanTween.cancel(bg.gameObject);
            Tools.AlphaImage(bg.gameObject, mFadeAlpha, 0, isTween ? 0.2f : alphaBgTime).updateNow().setOnComplete(() => {
                // mWindow.OnTweenEndShow?.Invoke();
                onSuccess?.Invoke();
            });
            UIManager.Singleton.FadeOutBackground--;
        }


        public void ShowAnimObj() {
            if (AniObject != null && AniObject.gameObject != null && !AniObject.gameObject.activeSelf) {
                AniObject.gameObject.SetActive(true);
                AniObject.localScale = Vector3.one;
            }
        }
    }
}
