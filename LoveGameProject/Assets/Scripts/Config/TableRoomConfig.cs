
using MM.Config;

/// <summary>
/// 房间配置
/// </summary>
public class TableRoomConfig : ITable
{
    public string RoomName;
    public int TransferPointId;
    public string ToRoomName;
    public int ToTransferId;
    private int isToOriginPoint;
    public bool IsToOriginPoint {
        get{
            return isToOriginPoint == 1;
        }
    }
    public override void Clear()
    {
    }

    protected override void MapData(ISerializer s)
    {
        s.Parse(ref RoomName);
        s.Parse(ref TransferPointId);
        s.Parse(ref ToRoomName);
        s.Parse(ref ToTransferId);
        s.Parse(ref isToOriginPoint);
    }

    public static TableRoomConfig GetConfig(string roomName, int transferPointId){
        var roomConfigs = TableConfigManager.Singleton.RoomTable.GetValues();
        if (roomConfigs == null || roomConfigs.Count <= 0){
            return null;
        }
        for (int i = 0; i < roomConfigs.Count; i++)
        {
            if(roomConfigs[i].RoomName.Equals(roomName) && roomConfigs[i].TransferPointId == transferPointId){
                return roomConfigs[i];
            }    
        }
        return null;
    }

    public static TableRoomConfig GetToConfig(string roomName, int transferPointId) {
        var roomConfig = GetConfig(roomName,transferPointId);
        if(roomConfig == null){
            return null;
        }
        return GetConfig(roomConfig.ToRoomName,roomConfig.ToTransferId);
    }
}
