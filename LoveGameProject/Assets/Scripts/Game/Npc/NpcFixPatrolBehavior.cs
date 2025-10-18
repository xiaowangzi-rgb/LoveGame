using UnityEngine;

/// <summary>
/// 固定巡逻行为数据
/// </summary>
[System.Serializable]
public class FixPatrolBehaviorData : BehaviorData {
    [SerializeField]
    [Header("巡逻点")]
    public Transform[] patrolPoints;
    [SerializeField]
    [Header("停留时间")]
    public float[] standingTime = new float[]{2, 5};
    [SerializeField]
    [Header("是否循环巡逻")]
    public bool isLoop = true;
    [SerializeField]
    [Header("是否往返巡逻")]
    public bool isPingPong = false;
}


/// <summary>
/// 固定巡逻行为
/// </summary>
public class NpcFixPatrolBehavior : NpcBehavior<FixPatrolBehaviorData> {
    /// <summary>
    /// 是否正在移动
    /// </summary>
    private bool _isMoving = false;
    /// <summary>
    /// 是否正在停留
    /// </summary>
    private bool _isStanding = false;
    /// <summary>
    /// 当前巡逻点索引
    /// </summary>
    private int _currentPointIndex = 0;
    /// <summary>
    /// 移动方向(1为正向，-1为反向，用于往返巡逻)
    /// </summary>
    private int _direction = 1;
    
    public NpcFixPatrolBehavior(Npc npc, FixPatrolBehaviorData data) : base(npc, data) {
        _isMoving = false;
        _isStanding = false;
        _currentPointIndex = 0;
        _direction = 1;
    }

    protected override void OnCheckState() {
        if (_playerObjNavMesh == null){
            return;
        }
        // 检查巡逻点是否有效
        if(_data.patrolPoints == null || _data.patrolPoints.Length == 0){
            return;
        }
        if (_isMoving){
            return;
        }
        if(_isStanding){
            return;
        }
        
        // 获取当前目标巡逻点
        var targetPoint = GetCurrentPatrolPoint();
        if(targetPoint == null){
            return;
        }
        
        //设置移动目标
        _playerObjNavMesh.SetMovePos(targetPoint.position, () => {
            _isMoving = false;
            _isStanding = true;
            
            // 到达巡逻点后停留一段时间
            LeanTween.delayedCall(GetRandomStandingTime(), ()=>{
                _isStanding = false;
                // 移动到下一个巡逻点
                MoveToNextPoint();
            });
        });
        
        //标记开始移动
        _isMoving = true;
    }

    /// <summary>
    /// 获取当前巡逻点
    /// </summary>
    /// <returns></returns>
    private Transform GetCurrentPatrolPoint()
    {
        if(_data.patrolPoints == null || _data.patrolPoints.Length == 0){
            return null;
        }
        
        // 确保索引在有效范围内
        if(_currentPointIndex < 0 || _currentPointIndex >= _data.patrolPoints.Length){
            _currentPointIndex = 0;
        }
        
        return _data.patrolPoints[_currentPointIndex];
    }

    /// <summary>
    /// 移动到下一个巡逻点
    /// </summary>
    private void MoveToNextPoint()
    {
        if(_data.patrolPoints == null || _data.patrolPoints.Length == 0){
            return;
        }

        // 往返巡逻模式
        if(_data.isPingPong){
            _currentPointIndex += _direction;
            
            // 到达终点，反向
            if(_currentPointIndex >= _data.patrolPoints.Length){
                _currentPointIndex = _data.patrolPoints.Length - 2;
                _direction = -1;
            }
            // 到达起点，正向
            else if(_currentPointIndex < 0){
                _currentPointIndex = 1;
                _direction = 1;
            }
        }
        // 循环巡逻模式
        else if(_data.isLoop){
            _currentPointIndex = (_currentPointIndex + 1) % _data.patrolPoints.Length;
        }
        // 单次巡逻模式
        else{
            _currentPointIndex++;
            // 如果超出范围，停止巡逻
            if(_currentPointIndex >= _data.patrolPoints.Length){
                _currentPointIndex = _data.patrolPoints.Length - 1;
                OnStop();
            }
        }
    }

    /// <summary>
    /// 获取随机停留时间
    /// </summary>
    /// <returns></returns>
    private float GetRandomStandingTime()
    {
        if(_data.standingTime == null || _data.standingTime.Length < 2){
            return 2f; // 默认停留2秒
        }
        return Tools.GetRandom(_data.standingTime[0], _data.standingTime[1]);
    }
}