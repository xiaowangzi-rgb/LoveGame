using System;
using UnityEngine;

/// <summary>
/// Npc基类
/// </summary>
public class Npc : MonoBehaviour, INpc {
    [Header("行为类型")]
    public NpcBehaviorType BehaviorType = NpcBehaviorType.Idle;
    [Header("待机行为数据")]
    public IdleBehaviorData IdleBehaviorData;
    [Header("随机巡逻行为数据")]
    public RandomPatrolBehaviorData RandomPatrolBehaviorData;
    [Header("固定巡逻行为数据")]
    public FixPatrolBehaviorData FixPatrolBehaviorData;
    [Header("当前房间")]
    public Room CurrentRoom;
    /// <summary>
    /// 当前行为
    /// </summary>
    protected INpcBehavior _currentBehavior;
    /// <summary>
    /// 寻路组件
    /// </summary>
    private PlayerObjNavMesh _playerObjNavMesh;
    /// <summary>
    /// Npc对象
    /// </summary>
    private GameObject _npcObj;
    /// <summary>
    /// 是否初始化
    /// </summary>
    public bool IsInit {get; private set;} = false;

    [Header("Npc样式")]
    public NpcStyle Style;
    
    void Start(){
        if (_playerObjNavMesh == null) {
            _playerObjNavMesh = GetComponent<PlayerObjNavMesh>();
        }
        IsInit = false;
    }

    /// <summary>
    /// 更新
    /// </summary>
    void Update()
    {
        //npc管理器没有初始化不进行npc操作
        if(GameManager.NpcManager == null || !GameManager.NpcManager.IsInit){
            return;
        }
        //没有初始化
        if(!IsInit && GameManager.NpcManager.AddNpc(gameObject)){
            Init();
        }
        //初始化后进行更新
        if(IsInit){
            OnUpdate();
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    protected virtual void OnUpdate(){
        if (_currentBehavior != null) {
            _currentBehavior.OnUpdate();
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Init(){
        IsInit = true;
        InitStyle();
        InitBehavior();
    }

    /// <summary>
    /// 初始化Npc样式
    /// </summary>
    protected virtual void InitStyle(){
        if (Style == null){
            Style = new DefaultNpcStyle();
        }
        //创建Obj
        if (Style.Anim != null && _npcObj == null){
            _npcObj = Instantiate(Style.Anim.gameObject, transform);
            _npcObj.transform.localPosition = Vector3.zero;
            _npcObj.transform.localRotation = Quaternion.identity;
            _npcObj.transform.localScale = Vector3.one;
            _playerObjNavMesh._prefabs = _npcObj.GetComponent<SPUM_Prefabs>();
            _playerObjNavMesh.Init();
        }
    }

    /// <summary>
    /// 初始化行为
    /// </summary>
    protected virtual void InitBehavior(){
        switch(BehaviorType){
            case NpcBehaviorType.Idle:
                _currentBehavior = new NpcIdleBehavior(this, IdleBehaviorData);
                break;
            case NpcBehaviorType.RandomPatrol:
                _currentBehavior = new NpcRandomPatrolBehavior(this, RandomPatrolBehaviorData);
                break;
            case NpcBehaviorType.FixPatrol:
                _currentBehavior = new NpcFixPatrolBehavior(this, FixPatrolBehaviorData);
                break;
        }
        if(_currentBehavior != null){
            _currentBehavior.OnInit();
        }
    }
    
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="OnComplete"></param>
    public virtual void DoMove(Vector3 pos, Action OnComplete)
    {
        _playerObjNavMesh.SetMovePos(pos, OnComplete);
    }

    /// <summary>
    /// 说话
    /// </summary>
    /// <param name="text"></param>
    public virtual void DoSpeak(string text)
    {
        Tools.DoSpeak(transform, text, 1f, 1.0f, null);
    }

    /// <summary>
    /// 触发交互
    /// </summary>
    /// <param name="interactionTriggerComponent"></param>
    public virtual void OnTriggerInteraction(InteractionTriggerComponent interactionTriggerComponent){
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void Destroy()
    {
        _npcObj = null;
        _playerObjNavMesh = null;
        CurrentRoom?.OnExit(gameObject);
        GameObject.Destroy(gameObject);
    }

    public Room GetRoom()
    {
        return CurrentRoom;
    }
}