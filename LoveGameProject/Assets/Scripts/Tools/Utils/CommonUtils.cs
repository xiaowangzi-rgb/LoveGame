using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class CommonUtils
{
    //世界坐标转UI坐标
    public static Vector3 World2UI(Vector3 wpos, RectTransform uiParent, bool isWorldPos = false)
    {
        var uiCamera = Camera.main;
        Vector3 spos = Camera.main.WorldToScreenPoint(wpos);
        Vector3 retPos;
        if (isWorldPos)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiParent, spos, uiCamera, out retPos);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent, spos, uiCamera, out Vector2 localPos);
            retPos = localPos;
        }
        return retPos;

    }

    //鼠标点击事件传递到下层UI及gameobject
    public static void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        var current = data.pointerCurrentRaycast.gameObject;
        foreach (var t in results.Where(t => current != t.gameObject))
        {
            ExecuteEvents.Execute(t.gameObject, data, function);
            //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，ExecuteEvents.Execute后直接break
        }
    }
}
