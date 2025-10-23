using System.Collections.Generic;
using MM;
using UnityEngine;
/// <summary>
/// 基于解决UI中tween无法自主清理的问题
/// </summary>
public interface IUITween {
    LTDescr AddTween(LTDescr tw);
    LTDescr AddTween(LTDescr tw, GameObject obj);
    void CancelTween(GameObject tw);
    void CancelTween(int id);
}

/// <summary>
/// uiTweenData
/// </summary>
public class UTweenData {
    public List<int> mTweenID { get; private set; }
    public GameObject mTweenObj { get; private set; }

    public UTweenData(int tweenID) {
        mTweenID = new List<int>();
        mTweenID.Add(tweenID);
        mTweenObj = null;
    }

    public UTweenData(int tweenID, GameObject obj) {
        mTweenID = new List<int>();
        mTweenID.Add(tweenID);
        mTweenObj = obj;
    }

    public void AddTweenID(int tweenID) {
        if (mTweenID == null) {
            mTweenID = new List<int>();
        }
        if (!mTweenID.Contains(tweenID)) {
            mTweenID.Add(tweenID);
        }
    }

    public void LeanTweenCancel(int id) {
        if (mTweenID != null && mTweenID.Count > 0) {
            mTweenID.Remove(id);
            LeanTween.cancel(id);
        }
    }

    public void OnLeanTweenClear() {
        if (mTweenID != null) {
            for (var i = 0; i < mTweenID.Count; i++) {
                LeanTween.cancel(mTweenID[i]);
            }
            mTweenID?.Clear();
        }
        if (mTweenObj != null) {
            LeanTween.cancel(mTweenObj);
            mTweenObj = null;
        }
    }
}

/// <summary>
/// Tween
/// </summary>
public class BaseUITween {
    public List<UTweenData> mTweenDatas { get; set; } = new List<UTweenData>();
    /// <summary>
    /// 初始化tween
    /// </summary>
    public virtual void OnTweenInit() {
        mTweenDatas = new List<UTweenData>();
    }
    /// <summary>
    /// 添加tween
    /// </summary>
    /// <param name="tw"></param>
    /// <returns></returns>
    public virtual LTDescr AddTween(LTDescr tw) {
        if (tw == null) {
            return null;
        }
        if (mTweenDatas == null) {
            mTweenDatas = new List<UTweenData>();
        }
        if (FindTweenData(tw.uniqueId) == null) {
            var newTween = new UTweenData(tw.uniqueId);
            mTweenDatas.Add(newTween);
        }
        return tw;
    }

    /// <summary>
    /// 添加tween
    /// </summary>
    /// <param name="tw"></param>
    /// <returns></returns>
    public virtual int AddTween(int uniqueId) {
        if (mTweenDatas == null) {
            mTweenDatas = new List<UTweenData>();
        }
        if (FindTweenData(uniqueId) == null) {
            var newTween = new UTweenData(uniqueId);
            mTweenDatas.Add(newTween);
        }
        return uniqueId;
    }

    /// <summary>
    /// 添加tween
    /// </summary>
    /// <param name="tw"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public virtual LTDescr AddTween(LTDescr tw, GameObject obj) {
        if (tw == null) {
            return null;
        }
        if (mTweenDatas == null) {
            mTweenDatas = new List<UTweenData>();
        }
        if (obj != null) {
            var tweenData = FindTweenData(obj);
            if (tweenData == null) {
                var newTween = new UTweenData(tw.uniqueId, obj);
                mTweenDatas.Add(newTween);
            } else {
                tweenData.AddTweenID(tw.uniqueId);
            }
        } else {
            if (FindTweenData(tw.uniqueId) == null) {
                var newTween = new UTweenData(tw.uniqueId);
                mTweenDatas.Add(newTween);
            }
        }
        return tw;
    }

    protected UTweenData FindTweenData(int id) {
        if (mTweenDatas == null) {
            return null;
        }
        for (var i = 0; i < mTweenDatas.Count; i++) {
            if (mTweenDatas[i].mTweenID == null || mTweenDatas[i].mTweenID.Count <= 0) {
                continue;
            }
            if (mTweenDatas[i].mTweenID.Contains(id)) {
                return mTweenDatas[i];
            }
        }
        return null;
    }

    protected UTweenData FindTweenData(GameObject obj) {
        if (mTweenDatas == null) {
            return null;
        }
        return mTweenDatas.Find((UTweenData data) => data.mTweenObj == obj);
    }

    public virtual void CancelTween(GameObject tw) {
        if (mTweenDatas == null) {
            LeanTween.cancel(tw.gameObject);
            return;
        }
        var tweenData = FindTweenData(tw);
        if (tweenData == null) {
            LeanTween.cancel(tw.gameObject);
            return;
        }
        tweenData.OnLeanTweenClear();
    }

    public virtual void CancelTween(int id) {
        if (mTweenDatas == null) {
            LeanTween.cancel(id);
            return;
        }
        var tweenData = FindTweenData(id);
        if (tweenData == null) {
            LeanTween.cancel(id);
            return;
        }
        tweenData.LeanTweenCancel(id);
    }

    public virtual void OnTweenClear() {
        if (mTweenDatas != null && mTweenDatas.Count > 0) {
            for (var i = 0; i < mTweenDatas.Count; i++) {
                mTweenDatas[i].OnLeanTweenClear();
            }
            mTweenDatas.Clear();
        }
    }
}

