using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class CommonUtils
{
    //��������תUI����
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

    //������¼����ݵ��²�UI��gameobject
    public static void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        var current = data.pointerCurrentRaycast.gameObject;
        foreach (var t in results.Where(t => current != t.gameObject))
        {
            ExecuteEvents.Execute(t.gameObject, data, function);
            //RaycastAll��ugui���Լ����������ֻ����Ӧ͸��ȥ�������һ����Ӧ��ExecuteEvents.Execute��ֱ��break
        }
    }
}
