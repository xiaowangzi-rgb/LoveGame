using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 说话组件
/// </summary>
public class SpeakComponent : MonoBehaviour
{
    [Header("说话用的气泡")]
    public Transform _speakBubblePrefab;
    [Header("说话气泡偏移")]
    public Vector2 SpeakBubbleOffset = new Vector2(0,1f);

    /// <summary>
    /// 说话气泡对象
    /// </summary>
    private GameObject _speakBubble;

    /// <summary>
    /// 是否正在说话
    /// </summary>
    private bool _isSpeaking = false;

    void Start()
    {
        _isSpeaking = false;
        _speakBubble = null;
    }

    /// <summary>
    /// 说话    
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    public SpeakComponent Speak(
        Transform parent,
        string text,
        float duration,
        float hideDuration,
        Action onComplete)
    {
        if (_isSpeaking)
        {
            return this;
        }
        _speakBubble = CreateSpeakBubble(parent, SpeakBubbleOffset);
        //播放出现动画
        LeanTween.scale(_speakBubble, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            //播放说话动画
            StartCoroutine(TypewriterEffect(text, duration, () => {
                if (hideDuration > 0)
                {
                    LeanTween.delayedCall(hideDuration, HideSpeakBubble);
                }
                //播放完成
                onComplete?.Invoke();
            }));
        });
        _isSpeaking = true;
        return this;
    }

    /// <summary>
    /// 打字机效果协程
    /// </summary>
    /// <param name="textComponent">文本组件</param>
    /// <param name="text">要显示的完整文本</param>
    /// <param name="duration">显示完整文本所需的时间</param>
    /// <param name="onComplete">完成回调</param>
    private IEnumerator TypewriterEffect(string text, float duration, Action onComplete)
    {
        if (_speakBubble == null) {
            onComplete?.Invoke();
            yield break;
        }
        var textComponent = _speakBubble.transform.Find("Triangle/Text").GetComponent<TextMeshPro>();
        if (textComponent == null) {
            onComplete?.Invoke();
            yield break;
        }

        textComponent.text = "";

        if (string.IsNullOrEmpty(text))
        {
            onComplete?.Invoke();
            yield break;
        }

        // 计算每个字符的显示间隔时间
        float charDelay = duration > 0 ? duration / text.Length : 0.05f;

        // 逐字显示
        for (int i = 0; i < text.Length; i++)
        {
            textComponent.text += text[i];
            yield return new WaitForSeconds(charDelay);
        }

        // 打字机效果完成，调用回调
        onComplete?.Invoke();
    }

    /// <summary>
    /// 创建说话气泡
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    private GameObject CreateSpeakBubble(Transform parent, Vector2 offset)
    {
        if (_speakBubblePrefab == null)
        {
            return null;
        }
        _speakBubble = Instantiate(_speakBubblePrefab, parent).gameObject;
        _speakBubble.transform.localPosition = offset;
        _speakBubble.transform.localRotation = Quaternion.identity;
        _speakBubble.transform.localScale = Vector3.zero;
        return _speakBubble;
    }

    /// <summary>
    /// 隐藏说话气泡
    /// </summary>
    private void HideSpeakBubble()
    {
        if (_speakBubble == null)
        {
            return;
        }
        LeanTween.cancel(_speakBubble.gameObject);
        _speakBubble.transform.localScale = Vector3.one;
        LeanTween.scale(_speakBubble, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            DestroySpeakBubble();
        });
    }

    /// <summary>
    /// 销毁说话气泡
    /// </summary>
    private void DestroySpeakBubble()
    {
        if (_speakBubble != null)
        {
            LeanTween.cancel(_speakBubble.gameObject);
            GameObject.Destroy(_speakBubble);
            _speakBubble = null;
        }
        _isSpeaking = false;
    }
}
