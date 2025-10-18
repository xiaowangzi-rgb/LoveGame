
using UnityEngine;

/// <summary>
/// 待机行为数据
/// </summary>
[System.Serializable]
public class IdleBehaviorData : BehaviorData {
    [SerializeField]
    public PlayerState[] idleAnimationStates;
    [SerializeField]
    public float intervalTime = 10f;
}
/// <summary>
/// Npc待机行为
/// </summary>
public class NpcIdleBehavior : NpcBehavior<IdleBehaviorData>
{
    private float _passTime = 0f;
    public NpcIdleBehavior(Npc npc, IdleBehaviorData data) : base(npc, data)
    {
    }

    protected override void OnCheckState()
    {
        _passTime += Time.deltaTime;
        if (_passTime >= _data.intervalTime && _playerObjNavMesh != null) {
            _passTime = 0f;
            _playerObjNavMesh.PlayStateAnimation(GetRandomIdleAnimationState());
        }
    }

    /// <summary>
    /// 获取随机待机动画状态
    /// </summary>
    /// <returns></returns>
    private PlayerState GetRandomIdleAnimationState()
    {
        return _data.idleAnimationStates[Tools.GetRandom(0, _data.idleAnimationStates.Length)];
    }
}