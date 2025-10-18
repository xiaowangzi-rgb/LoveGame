using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;


/// <summary>
/// NPC管理器
/// </summary>
public class NpcManager {

    /// <summary>
    /// NPC字典
    /// </summary>
    private List<Npc> _npcList {get; set;} = new List<Npc>();

    /// <summary>
    /// 是否初始化
    /// </summary>
    public bool IsInit {get; private set;} = false;

    /// <summary>
    /// 初始化NPC管理器
    /// </summary>
    public void Init(){
        _npcList = new List<Npc>();
        IsInit = true;
    }

    /// <summary>
    /// 添加NPC
    /// </summary>
    /// <param name="npcObj"></param>
    public bool AddNpc(GameObject npcObj){
        if (npcObj == null){
            return false;
        }
        var npc = npcObj.GetComponent<Npc>();
        if (npc != null){
            return AddNpc(npc);
        }
        return false;
    }

    /// <summary>
    /// 添加NPC
    /// </summary>
    /// <param name="npc"></param>
    public bool AddNpc(Npc npc){
        if(_npcList == null){
            _npcList = new List<Npc>();
        }
        if (npc != null && !_npcList.Contains(npc)){
            _npcList.Add(npc);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除NPC
    /// </summary>
    /// <param name="npcObj"></param>
    public void RemoveNpc(GameObject npcObj){
        if (npcObj == null){
            return;
        }
        var npc = npcObj.GetComponent<Npc>();
        if (npc != null){
            RemoveNpc(npc);
        }
    }

    /// <summary>
    /// 移除NPC
    /// </summary>
    /// <param name="npc"></param>
    public void RemoveNpc(Npc npc){
        if (npc != null){
            npc.Destroy();
        }
        if (_npcList != null){
            _npcList.Remove(npc);
        }
    }

    /// <summary>
    /// 清理NPC管理器
    /// </summary>
    public void Clear(){
        if (_npcList != null && _npcList.Count > 0){
            foreach (var npc in _npcList)
            {
                if (npc != null){
                    npc.Destroy();
                }
            }
        }
        _npcList.Clear();
    }
}
