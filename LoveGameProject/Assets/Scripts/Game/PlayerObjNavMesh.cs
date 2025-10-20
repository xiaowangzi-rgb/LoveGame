using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
/// <summary>
/// 玩家对象寻路版本
/// </summary>
public class PlayerObjNavMesh : MonoBehaviour
{
    public SPUM_Prefabs _prefabs = null;
    private PlayerState _currentState;

    public NavMeshAgent _navMeshAgent;

    private Action _onMoveComplete;
    public Vector3 _goalPos;
    
    public bool isAction = false;
    public Dictionary<PlayerState, int> IndexPair = new();
    void Start()
    {
        Init();
    }

    public void Init() {
        if (_navMeshAgent == null)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        if (_prefabs == null && transform.childCount > 0)
        {
            _prefabs = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
            if (!_prefabs.allListsHaveItemsExist())
            {
                _prefabs.PopulateAnimationLists();
            }
        }
        if(_prefabs != null) {
            _prefabs.OverrideControllerInit();
            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                IndexPair[state] = 0;
            }
        }
    }

    public void SetStateAnimationIndex(PlayerState state, int index = 0)
    {
        IndexPair[state] = index;
    }

    public void PlayStateAnimation(PlayerState state)
    {
        if(_prefabs == null) {
            return;
        }
        _prefabs.PlayAnimation(state, IndexPair[state]);
    }

    void FixedUpdate()
    {
        if (isAction)
        {
            return;
        }
        if(_prefabs == null) {
            return;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.localPosition.y * 0.01f);
        switch (_currentState)
        {
            case PlayerState.IDLE:
                break;
            case PlayerState.MOVE:
                DoMove();
                break;
        }
        PlayStateAnimation(_currentState);
    }

    void DoMove(){
        //设置寻路目标
        _navMeshAgent.SetDestination(_goalPos);
        // 判断是否到达目标点（简化版，更可靠）
        if (!_navMeshAgent.pathPending && // 确保路径已计算完成
            _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) // 剩余距离小于等于停止距离
        {
            Debug.Log("已到达目标点！");
            _currentState = PlayerState.IDLE;
            _onMoveComplete?.Invoke();
            _onMoveComplete = null;
            return;
        }
        //设置方向
        Vector3 _dirVec = _goalPos - transform.position;
        Vector3 _dirMVec = _dirVec.normalized;
        if (_dirMVec.x > 0) _prefabs.transform.localScale = new Vector3(-1, 1, 1);
        else if (_dirMVec.x < 0) _prefabs.transform.localScale = new Vector3(1, 1, 1);
    }


    public void SetMovePos(Vector2 pos)
    {
        isAction = false;
        _goalPos = pos;
        _currentState = PlayerState.MOVE;
    }

    public void SetMovePos(Vector3 pos, Action OnComplete)
    {
        isAction = false;
        _goalPos = pos;
        _currentState = PlayerState.MOVE;
        _onMoveComplete = OnComplete;
    }

    public void ResetMove() {
        _goalPos = transform.position;
        _currentState = PlayerState.IDLE;
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.ResetPath();
        _onMoveComplete?.Invoke();
    }
}
