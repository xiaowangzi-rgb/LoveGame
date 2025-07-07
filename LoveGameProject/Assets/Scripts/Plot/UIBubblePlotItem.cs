using System;
using TMPro;
using UnityEngine;

public class UIBubblePlotItem : UIPlotWobject{
    private TextMeshProUGUI m_ContentText;

    protected override void InitUI(){
        base.InitUI();
        m_ContentText = GetComponent<TextMeshProUGUI>("Content");
    }

    public void SetData(Vector3 offset,TablePlotConfig _config,Action onComplete){
        transform.localPosition += offset;
        SetData(_config,onComplete);
        OnStartPlay();
    }

    private void OnStartPlay(){
        if(m_PlotConfig == null){
            m_OnComplete?.Invoke();
            return;
        }
        OnPlayPlotContent(
            m_ContentText,
            m_PlotConfig.GetPlotContent(),
            ()=>{
                m_PlotConfig = m_PlotConfig.GetNextPlotConfig();
                OnStartPlay();
            }
        );
    }
}
