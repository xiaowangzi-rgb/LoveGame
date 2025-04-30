
using System;

/// <summary>
/// 游戏加载器
/// </summary>
public class GameLoader{
    public Action<Action> m_LoadAction;
    public bool m_State = false;
    public GameLoader(Action<Action> loadAction){
        m_LoadAction = loadAction;
        m_State = false;
    }

    public void OnLoad(Action loadedAction){
        m_LoadAction.Invoke(()=>{
            OnLoaded();
            loadedAction?.Invoke();
        });
        m_State = true;
    }

    private void OnLoaded(){
    }
}
