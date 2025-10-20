
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NavMeshPlus.Components;
/// <summary>
/// 房间基类
/// </summary>
public class Room : MonoBehaviour, IRoom
{
    [Header("房间数据")]
    public RoomData roomData;
    /// <summary>
    /// 房间名称
    /// </summary>
    public string RoomName => roomData?.roomName;
    /// <summary>
    /// 房间玩家列表
    /// </summary>
    /// <typeparam name="GameObject"></typeparam>
    /// <returns></returns>
    private List<GameObject> _roomPlayerList = new List<GameObject>();

    private NavMeshSurface _navMeshSurface; // NavMeshSurface 组件

    public void Init()
    {
        _roomPlayerList = new List<GameObject>();
        if (_navMeshSurface == null){
            _navMeshSurface = GetComponent<NavMeshSurface>();
            _navMeshSurface.BuildNavMesh();
        }
    }

    /// <summary>
    /// 玩家是否在房间内
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsPlayerInRoom(GameObject player) {
        if (player == null) {
            return false;
        }
        if (_roomPlayerList == null || _roomPlayerList.Count <= 0) {
            return false;
        }
        return _roomPlayerList.Contains(player);
    }

    /// <summary>
    /// 房间是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsRoomEmpty() {
        if (_roomPlayerList != null && _roomPlayerList.Count > 0) {
            return false;
        }
        return true;
    }

    public void OnEnter(GameObject player, int transferId)
    {
        if (roomData == null) {
            return;
        }
        var transferData = getTransferData(transferId);
        if (player != null && transferData != null && transferData.triggerPoint != null) {
            if (player.TryGetComponent<PlayerObjNavMesh>(out var navMesh)) {
                navMesh?.ResetMove();
            }
            player.transform.position = transferData.triggerPoint.position;
        }
        if (!IsPlayerInRoom(player)) {
            _roomPlayerList.Add(player);
        }
    }

    public void OnExit(GameObject player)
    {
        if (player != null && _roomPlayerList.Contains(player)) {
            _roomPlayerList.Remove(player);
        }
    }

    public void OnDestroy()
    {
        //卸载资源
        GameObject.Destroy(gameObject);
        //重置数据
        _roomPlayerList.Clear();
    }
    
    private RoomTransferData getTransferData(Transform triggerPoint)
    {
        if (roomData == null || roomData.roomTransferDataList == null || roomData.roomTransferDataList.Length <= 0) {
            return null;
        }
        return roomData.roomTransferDataList.FirstOrDefault(data => data.triggerPoint == triggerPoint);
    }

    private RoomTransferData getTransferData(int transferId) {
        if (roomData == null || roomData.roomTransferDataList == null || roomData.roomTransferDataList.Length <= 0) {
            return null;
        }
        return roomData.roomTransferDataList.FirstOrDefault(data => data.transferId == transferId);
    }
}
