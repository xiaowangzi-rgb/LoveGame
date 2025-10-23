

using MM;
using SharedLibary;
/// <summary>
/// 场景队列接口
/// </summary>
public abstract class ISceneSequeue {
    /// <summary>
    /// 类型
    /// </summary>
    /// <value></value>
    public abstract _SceneSequeueType _Type { get; }
    /// <summary>
    /// 是否正在执行
    /// </summary>
    public bool _Running { get; set; }
    /// <summary>
    /// 刷新间隔
    /// </summary>
    protected virtual float _IntervalTime => 0.02f;
    /// <summary>
    /// 经过的时间
    /// </summary>
    protected virtual float _PassTime { get; set; } = 0f;
    /// <summary>
    /// 是否解锁
    /// </summary>
    /// <value></value>
    protected virtual bool IsUnLock {
        get {
            if (!_Running) {
                return false;
            }
            if (GetQueueAllCount() <= 0) {
                return false;
            }
            //动画锁
            if (UIManager.Singleton.IsLockPop) {
                return false;
            }
            return true;
        }
    }
    /// <summary>
    /// 获取数量
    /// </summary>
    /// <returns></returns>
    public abstract int GetQueueAllCount();
    /// <summary>
    /// 开始
    /// </summary>
    public virtual void _StartRun() {
        _Running = true;
    }
    /// <summary>
    /// 显示
    /// </summary>
    public abstract void _Show();
    /// <summary>
    /// 清除 切场景
    /// </summary>
    public virtual void ClearData() {
        _Running = false;
    }
    /// <summary>
    /// 清除 不用切场景
    /// </summary>
    public abstract void ClearSequeueData();
}


/// <summary>
/// 场景队列类型
/// </summary>
public enum _SceneSequeueType {
    Window,
    Action,
}

