
using Events;
using MM.Config;

/// <summary>
/// 配置表管理器
/// </summary>
public class TableConfigManager : TSingleton<TableConfigManager>{
    /// <summary>
    /// 剧情表
    /// </summary>
    public Table<TablePlotConfig> PlotTable {get;private set;} = new Table<TablePlotConfig>();
    /// <summary>
    /// 房间表
    /// </summary>
    /// <typeparam name="TableRoomConfig"></typeparam>
    /// <returns></returns>
    public Table<TableRoomConfig> RoomTable { get; private set; } = new Table<TableRoomConfig>();
    /// <summary>
    /// 弹窗表
    /// </summary>
    public Table<TableWindowSequeue> WindowSequeueTable { get; private set; } = new Table<TableWindowSequeue>();

    public void Init(){

    }

    public void LoadTable(){
        //PlotTable.Load("Submarine_group_config.csv", () => new TablePlotConfig());
        RoomTable.Load("room_config.csv", () => new TableRoomConfig());
        WindowSequeueTable.Load("room_config.csv", () => new TableWindowSequeue());
    }
}
