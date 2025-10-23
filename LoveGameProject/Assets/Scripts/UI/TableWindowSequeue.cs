using System;

namespace MM.Config {

    /// <summary>
    ///窗口配置
    /// </summary>
    public class TableWindowSequeue : ITable {
        public int GroupID;
        public string windowType;
        private string WindowSequeueType;
        public int _Sort;
        public string Name;

        public WindowSequeueType _WindowSequeueType;

        public static TableWindowSequeue _DefaultConfig;

        public override void Clear() {
        }

        protected override void MapData(ISerializer s) {
            s.Parse(ref GroupID);
            s.Parse(ref windowType);
            s.Parse(ref WindowSequeueType);
            s.Parse(ref _Sort);
            s.Parse(ref Name);
        }
        public override void OnLoad() {
            base.OnLoad();
            _WindowSequeueType = (WindowSequeueType)Enum.Parse(typeof(WindowSequeueType), WindowSequeueType);
        }

        /// <summary>
        /// 获取排序数字
        /// </summary>
        /// <returns></returns>
        public int GetSortNumber() {
            return GroupID + 100000 + _Sort;
        }

        /// <summary>
        /// 获取窗口配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TableWindowSequeue GetConfig(WindowType type) {
            if (type == WindowType.None) {
                return null;
            }
            if (type == WindowType.Default || type == WindowType.PreActivity) {
                return _DefaultConfig;
            }
            var configs = TableConfigManager.Singleton.WindowSequeueTable.GetValues();
            if (configs == null || configs.Count <= 0) {
                return null;
            }
            for (var i = 0; i < configs.Count; i++) {
                if (!configs[i].windowType.Equals(type.ToString())) continue;
                return configs[i];
            }
            return null;
        }
    }
}
