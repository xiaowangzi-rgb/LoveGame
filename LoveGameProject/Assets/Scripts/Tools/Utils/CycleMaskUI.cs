
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 圆圈遮照
/// </summary>
public class CycleMaskUI : MonoBehaviour {
    private Material _MaskMat;
    private int _TwId = int.MinValue;

    private void OnEnable() {
        _MaskMat = GetComponent<Image>().material;
    }

    public void Show(float initVal, float endVal, float time, float delayTime, Action endCallBack) {
        if (_MaskMat == null) {
            _MaskMat = GetComponent<Image>().material;
        }
        _MaskMat.SetFloat("_Ridus", initVal);
        _TwId = LeanTween.value(initVal, endVal, time).setOnUpdate((float val) => {
            _MaskMat.SetFloat("_Ridus", val);
        }).setOnComplete(() => {
            endCallBack?.Invoke();
            _TwId = int.MinValue;
        }).setDelay(delayTime).updateNow().uniqueId;
    }

    private void OnDestroy() {
        LeanTween.cancel(_TwId);
        _TwId = int.MinValue;
    }
}

