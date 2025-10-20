
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 房间管理器 管理房间的创建、销毁、切换等操作
/// </summary>
public class RoomManager
{
    private const string ROOM_PATH = "Rooms/";
    /// <summary>
    /// 房间根节点
    /// </summary>
    private Transform _roomRoot;
    /// <summary>
    /// 当前存在的房间列表
    /// </summary>
    public Dictionary<string, Room> _currentRoomLists = new Dictionary<string, Room>();

    public void Init() {
        if (_roomRoot == null) {
            _roomRoot = GameObject.Find("RoomRoot").transform;
        }
        _currentRoomLists = new Dictionary<string, Room>();
        //注册事件
        GloabEventManager.Instance.AddListener<RoomEventData>(EventType.Room.ToString(),OnTriggerTransfer);
    }
    
    /// <summary>
    /// 进入房间
    /// </summary>
    /// <param name="player"></param>
    /// <param name="roomName"></param>
    /// <param name="transferId"></param>
    public void EnterRoom(GameObject player, string roomName, int transferId) {
        //同一个人不能重复进入同一个房间
        if (GetRoom(roomName, out Room room) && room.IsPlayerInRoom(player)) {
            return;
        }
        //创建房间
        if (room == null){
            room = CreateRoom(roomName);
            //初始化房间
            room?.Init();
            //添加
            _currentRoomLists.Add(roomName, room);
        }
        //获取之前房子  
        GetPlayerRoom(player,out var lastRoom);
        //进入房间
        room.OnEnter(player, transferId);
        //退出之前房子
        ExitRoom(player, lastRoom);
    }

    /// <summary>
    /// 前往起点
    /// </summary>
    /// <param name="player"></param>
    /// <param name="room"></param>
    public void GoToOriginPoint(GameObject player) {
        if (player == null) {
            return;
        }
        var originPoint = PointGroupTools.GetPointData(PointType.StartPoint);
        if (originPoint == null) {
            return;
        }
        player.transform.position = originPoint.position;
        //获取所在房间
        if (GetPlayerRoom(player, out Room room)) {
            //退出房间
            ExitRoom(player, room);
        }
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    /// <param name="room"></param>
    public void ExitRoom(GameObject player, Room room) {
        if (room == null) {
            return;
        }
        //离开
        room?.OnExit(player);
        //当房间为空时，移除房间
        if (room.IsRoomEmpty()) {
            //移除房间
            _currentRoomLists.Remove(room.RoomName);
            //销毁房间
            room.OnDestroy();
        }
    }

    /// <summary>
    /// 获取房间
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    public bool GetRoom(string roomName,out Room room) {
        if (!_currentRoomLists.TryGetValue(roomName, out room)) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 获取用户所在房间
    /// </summary>
    /// <param name="player"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    public bool GetPlayerRoom(GameObject player, out Room room) {
        room = null;
        if(_currentRoomLists == null || _currentRoomLists.Count <= 0){
            return false;
        }
        foreach (var item in _currentRoomLists.Values)
        {
            if(item.IsPlayerInRoom(player)){
                room = item;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    private Room CreateRoom(string roomName){
        if (string.IsNullOrEmpty(roomName)){
            return null;
        }
        var room = Resources.Load<GameObject>(ROOM_PATH + roomName);
        if (room == null) {
            return null;
        }
        var roomObj = GameObject.Instantiate(room, _roomRoot);
        if (roomObj == null) {
            return null;
        }
        roomObj.transform.localPosition = Vector3.zero;
        roomObj.transform.localRotation = Quaternion.identity;
        roomObj.transform.localScale = Vector3.one;
        return roomObj.GetComponent<Room>();
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="roomInteractionComponent"></param>
    /// <param name="triggerObject"></param>
    public void OnTriggerTransfer(RoomEventData eventData)
    {
        if (eventData == null)
        {
            return;
        }
        if (eventData.target == null)
        {
            return;
        }
        var transferConfig = TableRoomConfig.GetConfig(eventData.RoomName,eventData.TransferId);
        if (transferConfig == null)
        {
            return;
        }
        if (transferConfig.IsToOriginPoint){
            //前往起点
            GoToOriginPoint(eventData.target);
        } else {
            //进入目标房间
            EnterRoom(eventData.target, transferConfig.ToRoomName, transferConfig.ToTransferId);
        }
    }
}
