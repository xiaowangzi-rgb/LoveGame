using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SharedLibary {
    public interface IWindowRootAdapter {
        GameObject GetGameObject();
        void InitWindowRoot();
        Canvas GetCanvas();
        CanvasScaler GetCanvasScaler();
        GraphicRaycaster GetGraphicRaycasterl();
        EventSystem GetEventSystem();
        Transform GetWindowTop();
        Transform GetWindowLow();
        Transform GetEventMask();
        bool IsEventSystemEnabled();
    }
}
