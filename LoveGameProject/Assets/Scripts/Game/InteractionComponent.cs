using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 交互类型
/// </summary>
public enum InteractionType
{
    Speak,          // 对话
    PickUp,         // 拾取
    Examine,        // 检查
    Event, //
}

/// <summary>
/// 交互对象类型
/// </summary>
public enum InteractionObjectType
{
    Player,
    Npc,
    Item,           // 物品
    Environment,    // 环境对象
}

/// <summary>
/// 交互组件 
/// </summary>
public abstract class InteractionComponent : MonoBehaviour
{
    [Header("交互类型")]
    public InteractionType InteractionType;

    [Header("交互对象类型")]
    public InteractionObjectType InteractionObjectType;

    [Header("是否可以交互")]
    public bool IsCanInteraction = true;

    [Header("交互距离")]
    public float InteractionDistance = 2f;

    [Header("交互按键")]
    public KeyCode InteractionKey = KeyCode.E;

    [Header("是否自动交互")]
    public bool IsAutoInteraction = false;

    [Header("是否可以连续交互")]
    public bool IsCanContinuousInteraction = false;

    [Header("检测层级")]
    public LayerMask InteractionLayer;

    /// <summary>
    /// 交互碰撞器
    /// </summary>
    private Collider2D _interactionCollider;

    /// <summary>
    /// 当前交互对象列表
    /// </summary>
    private List<InteractionTriggerComponent> _currentInteractionObjects = new List<InteractionTriggerComponent>();

    /// <summary>
    /// 交互过的对象列表
    /// </summary>
    private List<InteractionTriggerComponent> _interactionedObjects = new List<InteractionTriggerComponent>();

    /// <summary>
    /// 最近的可交互对象
    /// </summary>
    private InteractionTriggerComponent _nearestInteractionObject;

    void Start()
    {
        _interactionCollider = GetComponent<Collider2D>();
        _interactionedObjects = new List<InteractionTriggerComponent>();
        _currentInteractionObjects = new List<InteractionTriggerComponent>();
        _nearestInteractionObject = null;
        if (_interactionCollider == null)
        {
            // 如果没有碰撞器，自动添加一个圆形碰撞器
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = InteractionDistance;
            _interactionCollider = collider;
        }
    }

    void Update()
    {
        if (!IsCanInteraction)
        {
            return;
        }

        // 检测周边交互对象
        CheckInteraction();
        // 自动交互
        if (IsAutoInteraction && _nearestInteractionObject != null)
        {
            ExecuteInteraction(_nearestInteractionObject);
        }
        // 检测交互输入
        else if (Input.GetKeyDown(InteractionKey) && _nearestInteractionObject != null)
        {
            ExecuteInteraction(_nearestInteractionObject);
        }
    }

    /// <summary>
    /// 检测周边交互对象
    /// </summary>
    void CheckInteraction()
    {
        // 清除之前的列表
        _currentInteractionObjects.Clear();

        // 使用物理检测查找范围内的对象
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            InteractionDistance,
            InteractionLayer
        );

        foreach (var col in colliders)
        {
            // 排除自己
            if (col.gameObject == gameObject)
                continue;

            // 检查对象是否有交互组件
            InteractionTriggerComponent interactionComp = col.GetComponent<InteractionTriggerComponent>();
            if (interactionComp != null && interactionComp.IsCanInteraction)
            {
                _currentInteractionObjects.Add(interactionComp);
            }
        }

        //清除不存在的交互对象
        if (_interactionedObjects != null && _interactionedObjects.Count > 0)
        {
            if (_currentInteractionObjects == null || _currentInteractionObjects.Count <= 0)
            {
                _interactionedObjects.Clear();
            }
            else
            {
                for (int i = _interactionedObjects.Count - 1; i >= 0; i--)
                {
                    if (!_currentInteractionObjects.Contains(_interactionedObjects[i]))
                    {
                        _interactionedObjects.RemoveAt(i);
                    }
                }
            }
        }

        // 找到最近的可交互对象
        UpdateNearestInteractionObject();
    }

    /// <summary>
    /// 更新最近的交互对象
    /// </summary>
    private void UpdateNearestInteractionObject()
    {
        _nearestInteractionObject = null;
        float nearestDistance = float.MaxValue;

        foreach (var obj in _currentInteractionObjects)
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                _nearestInteractionObject = obj;
            }
        }
    }


    /// <summary>
    /// 执行交互
    /// </summary>
    /// <param name="target">交互目标</param>
    public void ExecuteInteraction(InteractionTriggerComponent target)
    {

        if (target == null)
            return;

        if (!target.IsCanInteraction)
            return;

        // 检查距离
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance > InteractionDistance)
        {
            //Debug.Log("距离太远，无法交互");
            return;
        }
        // 如果已经交互过，则不执行
        if (_interactionedObjects != null && !IsCanContinuousInteraction && _interactionedObjects.Contains(target))
        {
            return;
        }

        // 执行交互
        Execute(target);
        // 触发交互通用事件
        OnInteractionExecuted(target);

        if (_interactionedObjects == null)
        {
            _interactionedObjects = new List<InteractionTriggerComponent>();
        }
        // 添加到交互过的对象列表
        _interactionedObjects.Add(target);
    }

    /// <summary>
    /// 执行交互
    /// </summary>
    /// <param name="target"></param>
    public abstract void Execute(InteractionTriggerComponent target);

    /// <summary>
    /// 交互执行后的回调
    /// </summary>
    protected virtual void OnInteractionExecuted(InteractionTriggerComponent target)
    {
        // 可以在这里添加交互后的通用处理
        // 例如：播放音效、触发事件等
    }

    /// <summary>
    /// 设置是否可以交互
    /// </summary>
    public void SetCanInteraction(bool canInteraction)
    {
        IsCanInteraction = canInteraction;
    }

    /// <summary>
    /// 绘制交互范围（编辑器可视化）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InteractionDistance);

        if (_nearestInteractionObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _nearestInteractionObject.transform.position);
        }
    }
}