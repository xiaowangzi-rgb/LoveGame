using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// 视频播放器
/// </summary>
public class VideoObject : MonoBehaviour
{
    private VideoPlayer _VideoPlayer;
    /// <summary>
    /// 播放结束回调
    /// </summary>
    private Action _OnPlayEndCallBack;
    /// <summary>
    /// 播放失败回调
    /// </summary>
    private Action _OnPlayFailedCallBack;
    /// <summary>
    /// 预加载完成回调
    /// </summary>
    private Action _OnPreCompleted;
    private bool _IsPlaying { get; set; } = false;
    /// <summary>
    /// 是否可以点击跳过
    /// </summary>
    private bool _isCanClickSkip = false;
    private UIButtonExtension _SkipBtn;
    private UIButtonExtension _BtnMask;
    private GameObject _Bg;
    /// <summary>
    /// 椭圆遮照
    /// </summary>
    private Material _CycleMask;
    /// <summary>
    /// 遮照动画TweenID
    /// </summary>
    private int _CycleMaskTweenID = int.MinValue;

    private void OnEnable()
    {
        _Bg = transform.Find("Bg").gameObject;
        _VideoPlayer = transform.Find("VideoPlay").GetComponent<VideoPlayer>();
        _CycleMask = transform.Find("CycleMask").GetComponent<Image>().material;
        _SkipBtn = transform.Find("Btn").GetComponent<UIButtonExtension>();
        _SkipBtn.onClick.RemoveAllListeners();
        _SkipBtn.onClick.AddListener(OnSkip);
        _SkipBtn.gameObject.SetActive(false);
        _BtnMask = transform.Find("BtnMask").GetComponent<UIButtonExtension>();
        _BtnMask.onClick.RemoveAllListeners();
        _BtnMask.onClick.AddListener(OnPointerClick);
        _BtnMask.gameObject.SetActive(false);
        _Bg.gameObject.SetActive(false);
        Tools.AlphaImage(_SkipBtn.gameObject, 1, 0, 0).updateNow();
        Tools.AlphaTextMeshProUGUI(_SkipBtn.transform.Find("Text").gameObject, 1, 0, 0).updateNow();
        _CycleMask.SetFloat("_Ridus", 1);
    }

    /// <summary>
    /// 开始预加载视频
    /// </summary>
    public void OnPreVideo(Action preOnCompleted)
    {
        _OnPreCompleted = preOnCompleted;
        _VideoPlayer.prepareCompleted += OnVideoPrepared;
        _VideoPlayer.Prepare();
        Debug.Log("Start Pre Video！！！！");
    }

    /// <summary>
    /// 预加载视频结束
    /// </summary>
    /// <param name="source"></param>
    private void OnVideoPrepared(VideoPlayer source)
    {
        _OnPreCompleted?.Invoke();
        _OnPreCompleted = null;
        Debug.Log("Pre Video Finished！！！！");
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="showCamera"></param>
    /// <param name="clip"></param>
    public void SetData(string url, bool isCanClickSkip = true)
    {
        if (_VideoPlayer == null)
        {
            return;
        }
        _isCanClickSkip = isCanClickSkip;
        //_VideoPlayer.targetCamera = showCamera;
        var path = PathUtils.GetStreamingAssetsResPath(
                PathUtils.CombinePath("video", url));
        _VideoPlayer.errorReceived += OnVideoErrorReceived;
        _VideoPlayer.url = path;
        Debug.Log("Set Video Path :" + path);
    }

    /// <summary>
    /// 开始播放
    /// </summary>
    /// <param name="isLoop"></param>
    /// <param name="endCall"></param>
    public void Show(bool isLoop, Action endCall, Action failedCall)
    {
        if (_VideoPlayer == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(_VideoPlayer.url))
        {
            return;
        }
        if (!_IsPlaying)
        {
            _OnPlayEndCallBack = endCall;
            _OnPlayFailedCallBack = failedCall;
            _VideoPlayer.isLooping = isLoop;
            _VideoPlayer.loopPointReached += (videoPlayer) => OnPlayFinished();
            _IsPlaying = true;
        }
        _Bg.gameObject.SetActive(true);
        _BtnMask.gameObject.SetActive(true);
        _VideoPlayer.Play();
        Debug.Log("Play Video !!!!!!");
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void Pause()
    {
        if (!_IsPlaying)
        {
            return;
        }
        if (_VideoPlayer == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(_VideoPlayer.url))
        {
            return;
        }
        _VideoPlayer.Pause();
    }

    /// <summary>
    /// 结束播放
    /// </summary>
    public void Stop()
    {
        if (!_VideoPlayer.isPlaying)
        {
            return;
        }
        _VideoPlayer.Stop();
        OnPlayFinished();
    }

    /// <summary>
    /// 点击任意位置
    /// </summary>
    /// <param name="eventData"></param>
    private void OnPointerClick()
    {
        if (_SkipBtn.gameObject.activeInHierarchy)
        {
            return;
        }
        if (!_isCanClickSkip)
        {
            return;
        }
        _SkipBtn.gameObject.SetActive(true);
        Tools.AlphaImage(_SkipBtn.gameObject, 0, 1, 0.3f).updateNow();
        Tools.AlphaTextMeshProUGUI(_SkipBtn.transform.Find("Text").gameObject, 0, 1, 0.3f).updateNow();
    }

    /// <summary>
    /// 点击跳过
    /// </summary>
    private void OnSkip()
    {
        if (!_IsPlaying)
        {
            return;
        }
        if (_VideoPlayer == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(_VideoPlayer.url))
        {
            return;
        }
        Stop();
    }

    /// <summary>
    /// 播放失败回调
    /// </summary>
    /// <param name="source"></param>
    /// <param name="message"></param>
    private void OnVideoErrorReceived(VideoPlayer source, string message)
    {
        Debug.LogError("Video error: " + message);
        _OnPlayFailedCallBack?.Invoke();
        _OnPlayFailedCallBack = null;
        
        // 模拟器上有可能预加载失败
        _OnPreCompleted?.Invoke();
        _OnPreCompleted = null;
    }

    /// <summary>
    /// 播放结束
    /// </summary>
    private void OnPlayFinished()
    {
        HideMask(_OnPlayEndCallBack);
        _IsPlaying = false;
    }

    /// <summary>
    /// 结束动画
    /// </summary>
    private void HideMask(Action OnEndCallBack)
    {
        _CycleMaskTweenID = LeanTween.value(1, 0, 1f).setOnUpdate((float val) =>
        {
            _CycleMask.SetFloat("_Ridus", val);
        }).setOnComplete(() =>
        {
            OnEndCallBack?.Invoke();
            _CycleMaskTweenID = int.MinValue;
        }).uniqueId;
    }

    private void OnDestroy()
    {
        LeanTween.cancel(_CycleMaskTweenID);
        _CycleMaskTweenID = int.MinValue;
    }
}
