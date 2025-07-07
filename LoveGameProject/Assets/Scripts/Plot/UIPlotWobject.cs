using System;
using System.Collections.Generic;
using SharedLibary;
using TMPro;
using UnityEngine;

public class UIPlotWobject : WObject{
    protected TablePlotConfig m_PlotConfig;
    protected int PlotId {
        get{
            if(m_PlotConfig == null){
                return 0;
            }
            return m_PlotConfig.plotId;
        }
    }

    private List<int> m_TweenIds {get;set;} = new List<int>();

    protected Action m_OnComplete;

    protected override void InitUI(){
    }

    protected void SetData(TablePlotConfig _config,Action _onComplete){
        m_TweenIds = new List<int>();
        m_PlotConfig = _config;
        m_OnComplete = _onComplete;
    }

    protected void OnPlayPlotContent(TextMeshProUGUI _text,string _content,Action _complete){
        if(string.IsNullOrEmpty(_content)){
            _complete?.Invoke();
            return;
        }
        _text.text = string.Empty;
        int len = _content.Length;
        float interval = 0.05f; // 每个字的间隔时间，可根据需要调整
        int currentIndex = 0;
        // 先取消之前的Tween，防止叠加
        LeanTween.cancel(_text.gameObject);
        // 用 LeanTween 实现逐字显示
        m_TweenIds.Add(LeanTween.value(_text.gameObject, 0, len, len * interval)
            .setOnUpdate((float val) => {
                int charCount = Mathf.Clamp(Mathf.FloorToInt(val), 0, len);
                if (charCount != currentIndex) {
                    currentIndex = charCount;
                    _text.text = _content.Substring(0, charCount);
                }
            })
            .setOnComplete(()=>{
                _text.text = _content;
                _complete?.Invoke();
            }).uniqueId);
    }

    /// <summary>
    /// 清空所有Tween
    /// </summary>
    private void OnClearTween(){
        foreach(var id in m_TweenIds){
            LeanTween.cancel(id);
        }
        m_TweenIds.Clear();
    }

    public void OnClear(){
        OnClearTween();
        m_OnComplete = null;
        m_PlotConfig = null;
    }
}