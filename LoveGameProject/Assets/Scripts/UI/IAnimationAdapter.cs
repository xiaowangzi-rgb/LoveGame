using System;
using UnityEngine;
using UnityEngine.UI;

namespace SharedLibary {
    public interface IAnimationAdapter {
        void Init(WWindow window);
        bool Hiding();
        bool Showing();
        /// <summary>
        /// 显示动画
        /// </summary>
        /// <param name="after"></param>
        /// <param name="finished"></param>
        void Show(Action<bool> after, Action finished);
        /// <summary>
        /// 隐藏动画
        /// </summary>
        /// <param name="after"></param>
        /// <param name="finished"></param>
        void Hide(Action<bool> after, Action finished);
        /// <summary>
        /// 内部显示(不真正关闭UI,播放除Background外元素的显示动画)
        /// </summary>
        /// <param name="finish"></param>
        bool InnoShow(Action finish);
        /// <summary>
        /// 内部隐藏(不真正关闭UI，播放除Background外元素的隐藏动画)
        /// </summary>
        /// <param name="finish"></param>
        bool InnoHide(Action finish);
        bool HaveBackground(bool checkType = true);
        void FadeInBackground();
        void FadeOutBackground();

        bool IsPlaying { get; }

        float FadeAlpha { set; }

        RectTransform Background { get; }

        /// <summary>
        /// 显示窗口动画
        /// </summary>
        /// <param name="isTween"></param>
        void ShowBackground(GameObject bg, bool isTween);
        /// <summary>
        /// 隐藏窗口动画
        /// </summary>
        /// <param name="bg"></param>
        /// <param name="isTween"></param>
        void HideBackground(GameObject bg, bool isTween, Action onSuccess);


        void ShowAnimObj();
    }
}
