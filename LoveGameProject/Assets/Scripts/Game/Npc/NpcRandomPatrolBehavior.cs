using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 随机巡逻行为数据
/// </summary>
[System.Serializable]
public class RandomPatrolBehaviorData : BehaviorData {
    [SerializeField]
    [Header("巡逻半径")]
    public float patrolRadius = 10f;
    [SerializeField]
    [Header("停留时间")]
    public float[] standingTime = new float[]{5, 10};
}

/// <summary>
/// 随机巡逻行为
/// </summary>
public class NpcRandomPatrolBehavior : NpcBehavior<RandomPatrolBehaviorData>
{
    /// <summary>
    /// 是否正在移动
    /// </summary>
    private bool _isMoving = false;
    /// <summary>
    /// 是否正在停留
    /// </summary>
    private bool _isStanding = false;
    /// <summary>
    /// 原始位置
    /// </summary>
    private Vector3 _originPosition;
    public NpcRandomPatrolBehavior(Npc npc, RandomPatrolBehaviorData data) : base(npc, data)
    {
        _isMoving = false;
        _originPosition = _transform.position;
    }

    protected override void OnCheckState()
    {
        if (_playerObjNavMesh == null){
            return;
        }
        if (_isMoving){
            return;
        }
        if(_isStanding){
            return;
        }
        var targetPos = GetRandomPatrolPosition();
        if(targetPos.Equals(Vector3.zero)){
            return;
        }
        //设置移动目标
        _playerObjNavMesh.SetMovePos(targetPos,()=> {
            _isMoving = false;
            _isStanding = true;
            LeanTween.delayedCall(GetRandomStandingTime(),()=>{
                _isStanding = false;
            });
        });
        //标记开始移动
        _isMoving = true;
    }

    /// <summary>
    /// 获取随机巡逻位置
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPatrolPosition()
    {
        var _navMeshAgent = _playerObjNavMesh._navMeshAgent;
        if(_navMeshAgent == null){
            return Vector3.zero;
        }
        // 尝试最多10次找到一个有效的随机位置
        int maxAttempts = 30;
        for (int i = 0; i < maxAttempts; i++)
        {
            // 在巡逻半径内生成随机位置
            Vector2 randomDirection = Random.insideUnitCircle * _data.patrolRadius;
            Vector3 randomPosition = _originPosition + new Vector3(randomDirection.x, 0, randomDirection.y);
            
            // 检查该位置是否在NavMesh上
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 1f, NavMesh.AllAreas))
            {
                // 确保找到的位置在巡逻半径内
                if (Vector3.Distance(_originPosition, hit.position) <= _data.patrolRadius)
                {
                    return hit.position;
                }
            }
        }
        // 如果找不到有效位置，返回零向量
        return Vector3.zero;
    }

    /// <summary>
    /// 获取随机停留时间
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private float GetRandomStandingTime()
    {
        return Tools.GetRandom(_data.standingTime[0], _data.standingTime[1]);
    }
}