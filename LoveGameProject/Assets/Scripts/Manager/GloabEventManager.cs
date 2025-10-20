using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型
/// </summary>
public enum EventType {
    Room,
    Player,
}

/// <summary>
/// Room事件数据
/// </summary>
[Serializable]
public class RoomEventData {
    [SerializeField]
    public string RoomName;
    [SerializeField]
    public int TransferId;
    [NonSerialized]
    public GameObject target;
}

/// <summary>
/// 用户事件
/// </summary>
[Serializable]
public class PlayerEventData {
    [SerializeField]
    public string eventParam;
}


/// <summary>
/// 全局事件管理器
/// 用于管理游戏中的各种事件订阅和触发
/// </summary>
public class GloabEventManager
{
    private static GloabEventManager _instance;
    public static GloabEventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GloabEventManager();
            }
            return _instance;
        }
    }

    // 无参数事件字典
    private Dictionary<string, Action> _eventDictionary;
    
    // 带一个参数的事件字典
    private Dictionary<string, Delegate> _eventDictionaryWithParam;

    private GloabEventManager()
    {
        _eventDictionary = new Dictionary<string, Action>();
        _eventDictionaryWithParam = new Dictionary<string, Delegate>();
    }

    #region 无参数事件

    /// <summary>
    /// 注册事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">事件回调</param>
    public void AddListener(string eventName, Action listener)
    {
        if (string.IsNullOrEmpty(eventName) || listener == null)
        {
            UnityEngine.Debug.LogWarning("事件名称或监听器为空！");
            return;
        }

        if (_eventDictionary.ContainsKey(eventName))
        {
            _eventDictionary[eventName] += listener;
        }
        else
        {
            _eventDictionary.Add(eventName, listener);
        }
    }

    /// <summary>
    /// 移除事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">事件回调</param>
    public void RemoveListener(string eventName, Action listener)
    {
        if (string.IsNullOrEmpty(eventName) || listener == null)
        {
            return;
        }

        if (_eventDictionary.ContainsKey(eventName))
        {
            _eventDictionary[eventName] -= listener;
            
            // 如果该事件没有任何监听器了，移除该事件
            if (_eventDictionary[eventName] == null)
            {
                _eventDictionary.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 触发事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void TriggerEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            return;
        }

        if (_eventDictionary.ContainsKey(eventName))
        {
            _eventDictionary[eventName]?.Invoke();
        }
    }

    #endregion

    #region 带参数事件

    /// <summary>
    /// 注册事件（带一个参数）
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">事件回调</param>
    public void AddListener<T>(string eventName, Action<T> listener)
    {
        if (string.IsNullOrEmpty(eventName) || listener == null)
        {
            UnityEngine.Debug.LogWarning("事件名称或监听器为空！");
            return;
        }

        if (_eventDictionaryWithParam.ContainsKey(eventName))
        {
            _eventDictionaryWithParam[eventName] = Delegate.Combine(_eventDictionaryWithParam[eventName], listener);
        }
        else
        {
            _eventDictionaryWithParam.Add(eventName, listener);
        }
    }

    /// <summary>
    /// 移除事件（带一个参数）
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">事件回调</param>
    public void RemoveListener<T>(string eventName, Action<T> listener)
    {
        if (string.IsNullOrEmpty(eventName) || listener == null)
        {
            return;
        }

        if (_eventDictionaryWithParam.ContainsKey(eventName))
        {
            _eventDictionaryWithParam[eventName] = Delegate.Remove(_eventDictionaryWithParam[eventName], listener);
            
            // 如果该事件没有任何监听器了，移除该事件
            if (_eventDictionaryWithParam[eventName] == null)
            {
                _eventDictionaryWithParam.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 触发事件（带一个参数）
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="param">事件参数</param>
    public void TriggerEvent<T>(string eventName, T param)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            return;
        }

        if (_eventDictionaryWithParam.ContainsKey(eventName))
        {
            Delegate d = _eventDictionaryWithParam[eventName];
            if (d != null && d is Action<T>)
            {
                (d as Action<T>)?.Invoke(param);
            }
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 移除指定事件的所有监听器
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void RemoveAllListeners(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            return;
        }

        if (_eventDictionary.ContainsKey(eventName))
        {
            _eventDictionary.Remove(eventName);
        }

        if (_eventDictionaryWithParam.ContainsKey(eventName))
        {
            _eventDictionaryWithParam.Remove(eventName);
        }
    }

    /// <summary>
    /// 清空所有事件监听器
    /// </summary>
    public void ClearAllListeners()
    {
        _eventDictionary.Clear();
        _eventDictionaryWithParam.Clear();
    }

    /// <summary>
    /// 检查是否包含某个事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns></returns>
    public bool HasEvent(string eventName)
    {
        return _eventDictionary.ContainsKey(eventName) || _eventDictionaryWithParam.ContainsKey(eventName);
    }

    #endregion
}