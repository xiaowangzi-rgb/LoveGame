using System;
using UnityEngine;

/// <summary>
/// 事件交互组件
/// </summary>
public class EventInteractionComponent : InteractionComponent
{
    [Header("事件类型")]
    public EventType EventType;
    [Header("房间事件数据")]
    public RoomEventData roomEventData;
    [Header("用户事件数据")]
    public PlayerEventData playerEventData;

    public override void Execute(InteractionTriggerComponent target)
    {
        if (EventType == EventType.Room) {
            roomEventData.target = target.gameObject;
            GloabEventManager.Instance.TriggerEvent(EventType.ToString(),roomEventData);
        }
        else if (EventType == EventType.Player) {
            GloabEventManager.Instance.TriggerEvent(EventType.ToString(),playerEventData);
        }
    }
}