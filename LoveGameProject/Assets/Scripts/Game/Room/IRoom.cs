using UnityEngine;


/// <summary>
/// 房间接口
/// </summary>
public interface IRoom
{
    void Init();
    void OnEnter(GameObject player, int transferId);
    void OnExit(GameObject player);
    void OnDestroy();
}

/// <summary>
/// 每个房间需要的基础数据
/// </summary>
[System.Serializable]
public class RoomData {
    [SerializeField]
    public string roomName;
    [SerializeField]
    public float[] roomClampX;
    [SerializeField]
    public float[] roomClampY;
    [SerializeField]
    public RoomTransferData[] roomTransferDataList;
}

/// <summary>
/// 传送id 对应 传送数据
/// </summary>
[System.Serializable]
public class RoomTransferData {
    [SerializeField]
    public int transferId;
    [SerializeField]
    public Transform triggerPoint;
}