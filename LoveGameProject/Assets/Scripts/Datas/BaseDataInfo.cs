
using Game;
using Game.Message;
using LSave;
using MM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 游戏基础数据
/// </summary>
public class BaseDataInfo : LUserData
{
    public override LUserDataType DataType => LUserDataType.BaseDataInfo;

    //参数
    public RepeatedFieldRecordList<Int32> getScore_scene { get; set; }


    public BaseGameData _SelfData { get { return (BaseGameData)base.PBData; } set { base.PBData = value; } }



    //public int CurrentChapter {
    //    get { return _SelfData.CurrentChapter; }
    //    set { 
    //        _SelfData.CurrentChapter = value;
    //        Save();
    //    }
    //}

    public int CurrentLevel {
        get {
            return _SelfData.CurrentLevel;
        }
        set {
            _SelfData.CurrentLevel = value;
            Save();
        }
    }

    public bool isViewPreviousPlot {
        get {
            return _SelfData.IsViewPreviousPlot;
        }
        set {
            _SelfData.IsViewPreviousPlot = value;
            Save();
        }
    }

    public bool isPlaying {
        get {
            return _SelfData.IsPlaying;
        }
        set {
            _SelfData.IsPlaying = value;
            Save();
        }
    }


    public int PlayNum
    {
        get {
            return _SelfData.PlayNum;
        }
        set { 
            _SelfData.PlayNum = value;
            Save();
        }
    
    }


    //好像是一生只存一次
    public override void InitData()
    {
        //_SelfData.CurrentChapter = 1;
        CurrentLevel= 0;
        isPlaying = false;
        isViewPreviousPlot= false;
        PlayNum= 0;
    }

    public override void OnInit()
    {
        //Debug.Log("记录的测试数据: "+ _SelfData.CurrentChapter);
        getScore_scene = new RepeatedFieldRecordList<Int32>(_SelfData.GetScoreScene, OnDataChange);
    }

    /// <summary>
    /// 添加章节分数
    /// </summary>
    /// <param name="chapter_id"></param>
    public void Add_SceneScore(int scene_id) {
        //先确认有没有，没有再加


        if (getScore_scene.Contains(scene_id))
        {
            return;
        }

        getScore_scene.Add(scene_id);
        Save();
    }

    /// <summary>
    /// 获取章节分数
    /// </summary>
    /// <param name="chapter_id"></param>
    public int Get_ChapterScores(int chapter_id) {
        int score = 0;
        for (var i = 0; i < getScore_scene.Count; i++) {
            if (getScore_scene[i] / 100 == chapter_id) {
                score++;
            }
        }
        return score;
    }


    /// <summary>
    /// 结束一次游玩
    /// </summary>
    public void FinishPlay() {
        CurrentLevel = 0;
        isViewPreviousPlot = false;
        isPlaying = false;
        PlayNum++;
    }

}