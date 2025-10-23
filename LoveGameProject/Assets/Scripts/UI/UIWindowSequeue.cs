using System.Collections.Generic;
using MM;
using MM.Common;
using MM.Config;
using SharedLibary;


/// <summary>
/// UI窗口队列Manager
/// </summary>
public class UIWindowSequeue : ISceneSequeue
{
    public override _SceneSequeueType _Type => _SceneSequeueType.Window;
    //目前来看这是固定的结构 四种模式不会变化
    /// <summary>
    /// 强制弹窗
    /// </summary>
    public ForceWindowSequeue _ForceWindowSequeue { get; private set; }
    /// <summary>
    /// 当前显示的窗口类型
    /// </summary>
    /// <value></value>
    public WindowType _CurrentWindowType
    {
        get
        {
            return UIManager.Singleton.GetTopWindowType();
        }
    }
    /// <summary>
    /// 这次场景中出现过的窗口队列数据
    /// </summary>
    private List<WindowSequeueData> _LastWindowSequeueDatas;
    public UIWindowSequeue()
    {
        _ForceWindowSequeue = new ForceWindowSequeue();
        _LastWindowSequeueDatas = new List<WindowSequeueData>();
        _Running = false;
    }

    /// <summary>
    /// 展示
    /// </summary>
    public override void _Show()
    {
        if (!IsUnLock)
        {
            return;
        }
        if (!UIManager.Singleton.IsOnlyMainWindowShow())
        {
            return;
        }
        _PassTime += UnityEngine.Time.deltaTime;
        if (_PassTime >= _IntervalTime)
        {
            _PassTime = 0.0f;
            ShowWaitWindow();
        }
    }

    /// <summary>
    /// 显示等待窗口
    /// </summary>
    private void ShowWaitWindow()
    {
        WindowSequeueData showWindowData = null;
        if (_ForceWindowSequeue.ShowWaitWindow(out showWindowData))
        {

        }
        if (showWindowData != null)
        {
            _LastWindowSequeueDatas.Add(showWindowData);
        }
    }

    /// <summary>
    /// 添加窗口队列
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <param name="delayTime"></param>
    public void AddWaitWindow(string path, WindowType type, IWindowParam param = null, float delayTime = 0f, bool canReplace = false)
    {
        AddWaitWindow(new WindowSequeueData(path, type, param, delayTime), canReplace);
    }

    /// <summary>
    /// 添加窗口队列数据
    /// </summary>
    private void AddWaitWindow(WindowSequeueData data, bool canReplace = false)
    {
        if (data == null)
        {
            return;
        }

        //尝试删除
        if (canReplace)
        {
            RemoveWaitWindow(data._Path);
        }
        _ForceWindowSequeue.AddWaitWindow(data);
    }

    /// <summary>
    /// 删除窗口队列数据
    /// </summary>
    /// <param name="path"></param>
    public void RemoveWaitWindow(string path)
    {
        _ForceWindowSequeue.RemoveWaitWindow(path);
    }

    /// <summary>
    /// 删除窗口队列数据
    /// </summary>
    /// <param name="path"></param>
    /// <param name="windowType"></param>
    public void RemoveWaitWindow(string path, WindowType windowType)
    {
        _ForceWindowSequeue.RemoveWaitWindow(path);
    }

    /// <summary>
    /// 是否已经在等待显示的队列里了
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool HasWaitShowUI(string path)
    {
        return _ForceWindowSequeue.IsContains(path);
    }

    /// <summary>
    /// 获取队列中所有个数
    /// </summary>
    /// <returns></returns>
    public override int GetQueueAllCount()
    {
        return _ForceWindowSequeue.GetWaitWindowCount();
    }

    /// <summary>
    /// 是否已经触发过固定窗口或者强制窗口
    /// </summary>
    /// <returns></returns>
    public bool IsAlearyTriggerFixedOrForceWindow()
    {
        if (_LastWindowSequeueDatas != null)
        {
            for (var i = 0; i < _LastWindowSequeueDatas.Count; i++)
            {
                if (_LastWindowSequeueDatas[i]._WindowSequeueType == WindowSequeueType.Fix ||
                    _LastWindowSequeueDatas[i]._WindowSequeueType == WindowSequeueType.Force)
                {
                    return true;
                }
            }
        }
        return _ForceWindowSequeue != null && _ForceWindowSequeue.GetWaitWindowCount() > 0;
    }

    /// <summary>
    /// 是否可以触发强制打断逻辑
    /// </summary>
    /// <returns></returns>
    private bool IsCanTriggerForceBreak()
    {
        if (_CurrentWindowType == WindowType.None)
        {
            return false;
        }
        // //如果当前只有主界面就不用触发 等着update触发就行
        // if (CameraController.controller.isOnlyMainWindowShow()) {
        //     return false;
        // }
        var config = TableWindowSequeue.GetConfig(_CurrentWindowType);
        if (config == null)
        {
            return false;
        }
        //如果当前是强制窗口就不触发
        if (config._WindowSequeueType == WindowSequeueType.Force)
        {
            return false;
        }
        if (_ForceWindowSequeue == null || _ForceWindowSequeue.GetWaitWindowCount() <= 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 清除队列数据
    /// </summary>
    public override void ClearSequeueData()
    {
        if (_ForceWindowSequeue != null)
        {
            _ForceWindowSequeue.Clear();
        }
    }

    /// <summary>
    /// 清除弹窗数据 每次离开merge场景都会清除
    /// </summary>
    public override void ClearData()
    {
        if (_LastWindowSequeueDatas != null)
        {
            _LastWindowSequeueDatas.Clear();
        }
        ClearSequeueData();
        base.ClearData();
    }
}
