using MM;
using MM.Config;

/// <summary>
/// 窗口队列数据
/// </summary>
public class WindowSequeueData {
    /// <summary>
    /// 窗口路径
    /// </summary>
    public string _Path;
    /// <summary>
    /// 窗口类型
    /// </summary>
    public WindowType _WindowType;
    /// <summary>
    /// 窗口队列类型
    /// </summary>
    public WindowSequeueType _WindowSequeueType {
        get {
            if (_Config == null) {
                if (_WindowType == WindowType.PreActivity) {
                    return WindowSequeueType.CommonGetReward;
                } else {
                    return WindowSequeueType.Chance;
                }
            }
            return _Config._WindowSequeueType;
        }
    }
    /// <summary>
    /// 配置中的窗口类型
    /// </summary>
    /// <value></value>
    public string WindowTypeName_Config {
        get {
            if (_Config == null) {
                return _WindowType.ToString();
            }
            return _Config.Name;
        }
    }

    /// <summary>
    /// 延时时间
    /// </summary>
    public float _DelayTime;
    /// <summary>
    /// 排序ID
    /// </summary>
    public int _Sort {
        get {
            if (_Config == null) {
                if (_WindowType == WindowType.PreActivity) {
                    return 0;
                } else {
                    return 1;
                }
            }
            return _Config.GetSortNumber();
        }
    }
    /// <summary>
    /// 配置
    /// </summary>
    private TableWindowSequeue _Config;
    public TableWindowSequeue Config => _Config;
    /// <summary>
    /// 参数
    /// </summary>
    public IWindowParam _WindowParam;

    public WindowSequeueData(string path, WindowType type, IWindowParam param, float delayTime) {
        _Path = path;
        _WindowType = type;
        _DelayTime = delayTime;
        _WindowParam = param;
        SetConfig();
    }

    public WindowSequeueData(string path, WindowType type) {
        _Path = path;
        _WindowType = type;
        _DelayTime = 0;
        SetConfig();
    }

    public WindowSequeueData(string path, WindowType type, IWindowParam param) {
        _Path = path;
        _WindowType = type;
        _DelayTime = 0;
        _WindowParam = param;
        SetConfig();
    }

    /// <summary>
    /// 设置配置
    /// </summary>
    private void SetConfig() {
        _Config = TableWindowSequeue.GetConfig(_WindowType);
    }
}
