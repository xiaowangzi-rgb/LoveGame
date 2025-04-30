using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;



public class GameManager : TSingleton<GameManager>
{
    private Queue<GameLoader> m_InitQueue = new Queue<GameLoader>();
    private bool m_IsInit = false;

    public void Init(){
        m_InitQueue.Enqueue(new GameLoader(InitData));
        m_InitQueue.Enqueue(new GameLoader(InitTable));
        m_InitQueue.Enqueue(new GameLoader(InitScene));
        m_InitQueue.Enqueue(new GameLoader(InitUI));
        m_IsInit = true;
    }

    private void InitTable(Action loadedAction){
        Debug.Log("InitTable");
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
}
