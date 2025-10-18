using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 交互触发器组件
/// </summary>
public class InteractionTriggerComponent : MonoBehaviour
{
    [Header("交互对象类型")]
    public InteractionObjectType InteractionObjectType;
    [Header("是否可以触发")]
    public bool IsCanInteraction = true;
}
