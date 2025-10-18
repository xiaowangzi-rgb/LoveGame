
using UnityEngine;

/// <summary>
/// Npc行为类型
/// </summary>
public enum NpcBehaviorType {
    Idle,//待机
    RandomPatrol,//随机巡逻
    FixPatrol,//固定巡逻
}

/// <summary>
/// 行为状态
/// </summary>
public enum BehaviorState {
    Init,//初始化
    Start,//开始
    Stop,//停止
}

/// <summary>
/// Npc行为接口
/// </summary>
public interface INpcBehavior {
    /// <summary>
    /// 初始化
    /// </summary>
    void OnInit();
    /// <summary>
    /// 开始
    /// </summary>
    void OnStart();
    /// <summary>
    /// 更新
    /// </summary>
    void OnUpdate();
    /// <summary>
    /// 停止
    /// </summary>
    void OnStop();
    /// <summary>
    /// 触发交互
    /// </summary>
    /// <param name="interactionTriggerComponent"></param>
    void OnTriggerInteraction(InteractionTriggerComponent interactionTriggerComponent);
}

[System.Serializable]
/// <summary>
/// 行为数据
/// </summary>
public class BehaviorData {
    [SerializeField]
    public float Speed = 4f;
}

/// <summary>
/// Npc行为基类
/// </summary>
public abstract class NpcBehavior<T> : INpcBehavior where T : BehaviorData
{
    /// <summary>
    /// 行为数据
    /// </summary>
    protected T _data;
    /// <summary>
    /// Npc
    /// </summary>
    protected Npc _npc {get; private set;}
    /// <summary>
    /// 寻路组件
    /// </summary>
    protected PlayerObjNavMesh _playerObjNavMesh;
    protected Transform _transform {
        get {
            if(_npc == null){
                return null;
            }
            return _npc.transform;
        }
    }
    protected GameObject _gameObject {
        get {
            if(_transform == null){
                return null;
            }
            return _transform.gameObject;
        }
    }
    /// <summary>
    /// 状态
    /// </summary>
    protected BehaviorState _state = BehaviorState.Init;

    public NpcBehavior(Npc npc, T data){
        _npc = npc;
        _data = data;
    } 

    public void OnInit()
    {
        _state = BehaviorState.Init;
        _playerObjNavMesh = _npc.GetComponent<PlayerObjNavMesh>();
        OnStart();
    }

    /// <summary>
    /// 开始
    /// </summary>
    public void OnStart()
    {
        _state = BehaviorState.Start;
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void OnStop()
    {
        _state = BehaviorState.Stop;
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void OnUpdate()
    {
        if (_npc == null || !_npc.IsInit){
            return;
        }
        if (_state == BehaviorState.Start) {
            OnCheckState();
        }
    }

    /// <summary>
    /// 检查状态
    /// </summary>
    protected abstract void OnCheckState();

    /// <summary>
    /// 触发交互
    /// </summary>
    /// <param name="interactionTriggerComponent"></param>
    public virtual void OnTriggerInteraction(InteractionTriggerComponent interactionTriggerComponent){
    }
}