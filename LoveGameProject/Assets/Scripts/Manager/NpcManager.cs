using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;


/// <summary>
/// NPC管理器
/// </summary>
public class NpcManager {

    /// <summary>
    /// NPC字典 和房间对应 一个房间可以有多个NPC
    /// </summary>
    private Dictionary<Room,List<Npc>> _npcList {get; set;} = new Dictionary<Room,List<Npc>>();

    /// <summary>
    /// 是否初始化
    /// </summary>
    public bool IsInit {get; private set;} = false;

    /// <summary>
    /// 初始化NPC管理器
    /// </summary>
    public void Init(){
        _npcList = new Dictionary<Room,List<Npc>>();
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
            _npcList = new Dictionary<Room,List<Npc>>();
        }
        if (npc.CurrentRoom == null) {
            Debug.LogError("NPC当前房间为空,无法添加NPC");
            return false;
        }
        List<Npc> npcList = null;
        if (npc != null && !_npcList.TryGetValue(npc.CurrentRoom, out npcList)){
            npcList = new List<Npc>();
            _npcList.Add(npc.CurrentRoom, npcList);
        }
        if (npcList != null && !npcList.Contains(npc)){
            npcList.Add(npc);
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
        if (npc == null) {
            return;
        }
        //销毁NPC
        npc.Destroy();
        //移除NPC
        if (_npcList != null && npc.CurrentRoom != null && _npcList.TryGetValue(npc.CurrentRoom, out var npcList)){
            if (npcList != null && npcList.Contains(npc)){
                npcList.Remove(npc);
            }
        }
    }

    /// <summary>
    /// 获取房间所有NPC
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public List<Npc> GetAllNpc(Room room) {
        if (_npcList == null || _npcList.Count <= 0) {
            return new List<Npc>();
        }
        if (!_npcList.TryGetValue(room, out var npcList)) {
            return new List<Npc>();
        }
        return npcList;
    }

    /// <summary>
    /// 获取所有NPC
    /// </summary>
    /// <returns></returns>
    public List<Npc> GetAllNpc() {
        if (_npcList == null || _npcList.Count <= 0) {
            return new List<Npc>();
        }
        return _npcList.Values.SelectMany(npc => npc).ToList();
    }

    /// <summary>
    /// 清理NPC管理器
    /// </summary>
    public void Clear(){
        if (_npcList != null && _npcList.Count > 0){
            foreach (var npc in _npcList){
                if (npc.Value != null && npc.Value.Count > 0){
                    for (int i = npc.Value.Count - 1; i >= 0; i--){
                        if (npc.Value[i] != null){
                            npc.Value[i].Destroy();
                        }
                    }
                    npc.Value.Clear();
                }
            }
        }
        _npcList.Clear();
    }
}
