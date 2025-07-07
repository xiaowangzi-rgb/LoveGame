
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

    public void Init(){

    }

    public void LoadTable(){
        PlotTable.Load("Submarine_group_config.csv", () => new TablePlotConfig());
    }
}
