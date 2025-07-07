
using System;
using System.Collections.Generic;
using SharedLibary;
using UnityEngine;
/// <summary>
/// 剧情管理器
/// </summary>
public class PlotManager{

    private Dictionary<int, TablePlotConfig> m_PlotDict {get;set;}
    private bool IsBeginPlay {get;set;} = false;
    private Transform m_BubblePlotRoot {get;set;}

    public void Init(){
        m_BubblePlotRoot = GameObject.Find("BubblePlotRoot").transform;
        m_PlotDict = TableConfigManager.Singleton.PlotTable.GetDatas();
        IsBeginPlay = false;
    }

    /// <summary>
    /// 获取剧情的配置
    /// </summary>
    /// <param name="plotId"></param>
    /// <returns></returns>
    private TablePlotConfig GetPlotConfig(int plotId){
        if(m_PlotDict == null || !m_PlotDict.TryGetValue(plotId, out var config)){
            return null;
        }
        return config;
    }

    public void OnPlayPlot(int plotId,Action onComplete){

    }

    /// <summary>
    /// 播放剧情
    /// </summary>
    /// <param name="plotId"></param>
    /// <param name="bParent"></param>
    /// <param name="offset"></param>
    /// <param name="onComplete"></param>
    public void OnPlayPlot(int plotId,Transform bParent,Vector3 offset,Action onComplete){
        var config = GetPlotConfig(plotId);
        if(config == null){
            onComplete?.Invoke();
            return;
        }
        //创建一个bubbleplotitem
        var bItem = WObject.Create<UIBubblePlotItem>(null);
        bItem.transform.SetParent(bParent == null ? m_BubblePlotRoot : bParent);
        bItem.SetData(offset,config,()=>{
            IsBeginPlay = false;
            onComplete?.Invoke();
        });
        IsBeginPlay = true;
    }

    public void OnClear(){
        IsBeginPlay = false;
        m_BubblePlotRoot = null;
        m_PlotDict?.Clear();
    }
}
