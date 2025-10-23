using System.Collections.Generic;
using MM;
using SharedLibary;
using UnityEngine;

/// <summary>
/// 强制窗口队列
/// </summary>
public class ForceWindowSequeue : BaseWindowSequeue {
    
    public override WindowSequeueType _WindowType => WindowSequeueType.Force;

    public override int _MaxCount => 99999999;
}

/// <summary>
/// 窗口队列接口
/// </summary>
public interface IWSequeue {
    List<WindowSequeueData> _WindowDatas { get; set; }
    WindowSequeueType _WindowType { get; }
    void AddWaitWindow(WindowSequeueData data);
    void RemoveWaitWindow(WindowSequeueData data);
    bool ShowWaitWindow(out WindowSequeueData data);
    List<WindowSequeueData> GetWaitWindow();
}

/// <summary>
/// 基础窗口队列类
/// </summary>
public abstract class BaseWindowSequeue : IWSequeue {
    public List<WindowSequeueData> _WindowDatas { get; set; }
    /// <summary>
    /// 窗口类型 固定还是啥
    /// </summary>
    /// <value></value>
    public abstract WindowSequeueType _WindowType { get; }
    /// <summary>
    /// 最大窗口数量
    /// </summary>
    /// <value></value>
    public abstract int _MaxCount { get; }
    /// <summary>
    /// 当前已经展示的数量
    /// </summary>
    /// <value></value>
    public int _NowShowCount { get; set; }

    public BaseWindowSequeue() {
        _WindowDatas = new List<WindowSequeueData>();
        _NowShowCount = 0;
    }

    /// <summary>
    /// 增加等待窗口
    /// </summary>
    /// <param name="data"></param>
    public virtual void AddWaitWindow(WindowSequeueData data) {
        if (data._WindowSequeueType != _WindowType) {
            return;
        }
        if (_WindowDatas == null) {
            _WindowDatas = new List<WindowSequeueData>();
        }
        //去重
        if (data._WindowType != WindowType.PreActivity && IsContains(data._Path)) {
            Debug.Log("窗口队列已经添加过了");
            return;
        }
        _WindowDatas.Add(data);
        //内部排序
        SortDatas();
    }

    /// <summary>
    /// 删除等待的窗口
    /// </summary>
    /// <param name="data"></param>
    public virtual void RemoveWaitWindow(WindowSequeueData data) {
        if (data == null) {
            return;
        }
        if (!IsContains(data._Path)) {
            return;
        }
        _WindowDatas.Remove(data);
        RemoveWaitWindow(data._Path);
    }

    public virtual void RemoveWaitWindow(string path) {
        //LogUtils.Log("删除窗口:    " +  path);
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        if (!IsContains(path)) {
            return;
        }
        for (var i = 0; i < _WindowDatas.Count; i++) {
            if (_WindowDatas[i]._Path.Equals(path)) {
                _WindowDatas.RemoveAt(i);
                break;
            }
        }
    }

    public virtual void RemoveWaitWindow(string path, WindowType windowType) {
        //LogUtils.Log("删除窗口:    " +  path);
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        if (!IsContains(path)) {
            return;
        }
        for (var i = 0; i < _WindowDatas.Count; i++) {
            if (_WindowDatas[i]._Path.Equals(path) && _WindowDatas[i]._WindowType == windowType) {
                _WindowDatas.RemoveAt(i);
                break;
            }
        }
    }

    public virtual List<WindowSequeueData> GetWaitWindow() {
        return _WindowDatas;
    }

    public virtual int GetWaitWindowCount() {
        //判断是否达到最大上限
        if (_NowShowCount >= _MaxCount) {
            return 0;
        }
        if (_WindowDatas == null) {
            return 0;
        }
        return _WindowDatas.Count;
    }


    /// <summary>
    /// 是否包含
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool IsContains(string path) {
        if (_WindowDatas == null || _WindowDatas.Count <= 0) {
            return false;
        }
        foreach (var _data in _WindowDatas) {
            if (!_data._Path.Equals(path)) continue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 展示等待的窗口
    /// </summary>
    /// <param name="data"></param>
    public virtual bool ShowWaitWindow(out WindowSequeueData data) {
        data = null;
     
        if (_WindowDatas == null || _WindowDatas.Count <= 0) {
            return false;
        }
        //判断是否达到最大上限
        if (_NowShowCount >= _MaxCount) {
            //LogUtils.Log($"窗口当前展示队列已经达到最大上限 当前{_NowShowCount} 最大{_MaxCount}");
            return false;
        }
        
        data = _WindowDatas[0];

        //先删除在展示这样的做法是为了防止多次删除
        RemoveWaitWindow(data);

        UIManager.Singleton.ShowPopupWindowNaitve(data._Path, data._WindowParam, data._WindowType.ToString());
        //增加当前场景展示的窗口数量
        _NowShowCount++;
        return true;
    }

    /// <summary>
    /// 排序数据
    /// </summary>
    protected virtual void SortDatas() {
        if (_WindowDatas == null || _WindowDatas.Count <= 0) {
            return;
        }
        _WindowDatas.Sort((a, b) => { return a._Sort - b._Sort; });
    }

    public virtual void Clear() {
        if (_WindowDatas != null) {
            _WindowDatas.Clear();
        }
        //清空这次场景展示的数量
        _NowShowCount = 0;
    }
}