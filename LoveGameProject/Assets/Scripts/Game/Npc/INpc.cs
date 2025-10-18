using System;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Npc样式
/// </summary>
public class NpcStyle {
    [SerializeField]
    public string Name;
    [SerializeField]
    public Transform Anim;
}

/// <summary>
/// 默认Npc样式
/// </summary>
public class DefaultNpcStyle : NpcStyle {
    public DefaultNpcStyle(){
        Name = "LiMei";
        Anim = Resources.Load<Transform>("Prefabs/Player/Npc/LiMei");
    }
}


/// <summary>
/// Npc接口
/// </summary>
public interface INpc {
    void DoSpeak(string text);
    void DoMove(Vector3 pos, Action OnComplete);
    void Destroy();
}
