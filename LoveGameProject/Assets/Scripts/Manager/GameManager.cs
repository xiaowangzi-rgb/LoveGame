using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;


/// <summary>
/// 游戏管理器
/// </summary>
public class GameManager : TSingleton<GameManager>{
    private Queue<GameLoader> m_InitQueue = new Queue<GameLoader>();
    private bool m_IsInit = false;
    public static PlotManager PlotManager => Singleton.m_PlotManager;
    public static NpcManager NpcManager => Singleton.m_NpcManager;
    public static RoomManager RoomManager => Singleton.m_RoomManager;
    private PlotManager m_PlotManager;
    private NpcManager m_NpcManager;
    private RoomManager m_RoomManager;

    public void Init(){
        m_InitQueue.Enqueue(new GameLoader(InitData));
        m_InitQueue.Enqueue(new GameLoader(InitTable));
        m_InitQueue.Enqueue(new GameLoader(InitManager));
        m_InitQueue.Enqueue(new GameLoader(InitScene));
        m_InitQueue.Enqueue(new GameLoader(InitUI));
        m_InitQueue.Enqueue(new GameLoader(BeginPlay));
        m_IsInit = true;
    }

    private void InitTable(Action loadedAction){
        Debug.Log("InitTable");
        TableConfigManager.Singleton.Init();
        TableConfigManager.Singleton.LoadTable();
        loadedAction?.Invoke();
    }

    private void InitData(Action loadedAction){
        Debug.Log("InitData");
        loadedAction?.Invoke();
    }

    private void InitScene(Action loadedAction){
        Debug.Log("InitScene");
        loadedAction?.Invoke();
    }

    private void InitUI(Action loadedAction){
        Debug.Log("InitUI");
        loadedAction?.Invoke();
    }

    private void BeginPlay(Action loadedAction){
        Debug.Log("BeginPlay");
        //加载第一个主房间
        var player = PlayerController.CreatePlayer();
        RoomManager.EnterRoom(player.gameObject, RoomManager.MAIN_ROOM_NAME, 1);
        loadedAction?.Invoke();
    }

    private void InitManager(Action loadedAction){
        Debug.Log("InitManager");
        if(m_PlotManager == null){
            m_PlotManager = new PlotManager();
        }
        if(m_NpcManager == null){
            m_NpcManager = new NpcManager();
        }
        if(m_RoomManager == null){
            m_RoomManager = new RoomManager();
        }
        m_PlotManager.Init();
        m_NpcManager.Init();
        m_RoomManager.Init();
        loadedAction?.Invoke();
    }

    public void Update(){
        if(!m_IsInit){
            return;
        }
        if(m_InitQueue != null && m_InitQueue.Count > 0){
            //顶部的状态
            var loader = m_InitQueue.Peek();
            if(!loader.m_State){
                loader.OnLoad(()=>m_InitQueue.Dequeue());
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        if(m_PlotManager != null){
            m_PlotManager.OnClear();
        }
        if(m_NpcManager != null){
            m_NpcManager.Clear();
        }
    }
}
