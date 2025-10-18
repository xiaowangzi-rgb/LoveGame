using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType {
    StartPoint,
}

[System.Serializable]
public class PointData {
    [SerializeField]
    public PointType pointName;
    [SerializeField]
    public Transform pointTransform;
}

/// <summary>
/// 点位组工具
/// </summary>
public class PointGroupTools : MonoBehaviour
{
    public static PointGroupTools Instance;
    
    [Header("默认点位数据")]
    public PointData[] pointDataList;
    /// <summary>
    /// 点位数据字典
    /// </summary>
    public Dictionary<PointType, PointData> pointDataDict = new Dictionary<PointType, PointData>();
    

    private void Awake(){
        Instance = this;
    }

    private void Start(){
        if (pointDataList != null && pointDataList.Length > 0){
            foreach (var item in pointDataList){
                pointDataDict.Add(item.pointName, item);
            }
        }
    }

    /// <summary>
    /// 获取点位数据
    /// </summary>
    /// <param name="pointName"></param>
    /// <returns></returns>
    public static Transform GetPointData(PointType pointName){
        if(Instance == null){
            return null;
        }
        if (!Instance.pointDataDict.TryGetValue(pointName, out PointData pointData)){
            return null;
        }
        return pointData.pointTransform;
    }

    /// <summary>
    /// 添加点位数据
    /// </summary>
    /// <param name="pointName"></param>
    /// <param name="pointTransform"></param>
    public static void AddPointData(PointType pointName, Transform pointTransform){
        if(Instance == null){
            return;
        }
        if (Instance.pointDataDict.ContainsKey(pointName)){
            return;
        }
        PointData pointData = new PointData();
        pointData.pointName = pointName;
        pointData.pointTransform = pointTransform;
        Instance.pointDataDict.Add(pointName, pointData);
    }

    /// <summary>
    /// 删除点位数据
    /// </summary>
    /// <param name="pointName"></param>
    public static void RemovePointData(PointType pointName){
        if(Instance == null){
            return;
        }
        if (!Instance.pointDataDict.TryGetValue(pointName, out PointData pointData)){
            return;
        }
        Instance.pointDataDict.Remove(pointName);
    }
}
