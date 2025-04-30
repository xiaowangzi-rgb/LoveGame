using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Awake(){
        GameManager.Singleton.Init();
        DontDestroyOnLoad(this);
    }


    void Update(){
        GameManager.Singleton.Update();
    }
}
