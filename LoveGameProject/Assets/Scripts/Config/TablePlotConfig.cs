using System.Collections.Generic;
using MM.Config;

public class TablePlotConfig : ITable{
    public int plotId;
    public int nextPlotId;
    public List<string> Names;
    public List<string> Images;
    public string content;
    public int isSkip;
    public string eventName;
    public override void Clear()
    {
    }

    protected override void MapData(ISerializer s)
    {
        s.Parse(ref plotId);
        s.Parse(ref nextPlotId);
        s.Parse(ref Names);
        s.Parse(ref Images);
        s.Parse(ref content);
        s.Parse(ref isSkip);
        s.Parse(ref eventName);
    }

    public string GetPlotContent(){
        return content;
    }

    public TablePlotConfig GetNextPlotConfig(){
        if(nextPlotId == 0){
            return null;
        }
        if(!TableConfigManager.Singleton.PlotTable.TryGetValue(nextPlotId,out var config)){
            return null;
        }
        return config;
    }
}
