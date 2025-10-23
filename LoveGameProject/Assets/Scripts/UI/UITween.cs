using System;
using UnityEngine;

/// <summary>
/// 自己维护的Tween
/// </summary>
public static class UITween{
    public static LTDescr alpha(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.alpha(gameObject, to, time));
    }
    public static LTDescr alpha(this IUITween uiTween,RectTransform rectTrans, float to, float time){
        return uiTween.AddTween(LeanTween.alpha(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr alpha(this IUITween uiTween,LTRect ltRect, float to, float time){
        return uiTween.AddTween(LeanTween.alpha(ltRect, to, time));
    }
    public static LTDescr alphaCanvas(this IUITween uiTween,CanvasGroup canvasGroup, float to, float time){
        return uiTween.AddTween(LeanTween.alphaCanvas(canvasGroup, to, time),canvasGroup.gameObject);
    }
    public static LTDescr alphaText(this IUITween uiTween,RectTransform rectTransform, float to, float time){
        return uiTween.AddTween(LeanTween.alphaText(rectTransform, to, time),rectTransform.gameObject);
    }
    public static LTDescr alphaVertex(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.alphaVertex(gameObject, to, time),gameObject);
    }
    public static void cancel(this IUITween uiTween,GameObject gameObject){
        uiTween?.CancelTween(gameObject);
    }
    public static void cancel(this IUITween uiTween,int uniqueId){
        uiTween?.CancelTween(uniqueId);
    }
    public static LTDescr color(this IUITween uiTween,GameObject gameObject, Color to, float time){
        return uiTween.AddTween(LeanTween.color(gameObject, to, time),gameObject);
    }
    public static LTDescr color(this IUITween uiTween,RectTransform rectTrans, Color to, float time){
        return uiTween.AddTween(LeanTween.color(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr colorText(this IUITween uiTween,RectTransform rectTransform, Color to, float time){
        return uiTween.AddTween(LeanTween.colorText(rectTransform, to, time),rectTransform.gameObject);
    }
    public static LTDescr delayedCall(this IUITween uiTween,float delayTime, Action callback){
        return uiTween.AddTween(LeanTween.delayedCall(delayTime, callback));
    }
    public static LTDescr delayedCall(this IUITween uiTween,GameObject gameObject, float delayTime, Action callback){
        return uiTween.AddTween(LeanTween.delayedCall(gameObject, delayTime, callback),gameObject);
    }
    public static LTDescr delayedCall(this IUITween uiTween,GameObject gameObject, float delayTime, Action<object> callback){
        return uiTween.AddTween(LeanTween.delayedCall(gameObject, delayTime, callback),gameObject);
    }
    public static LTDescr delayedCall(this IUITween uiTween,float delayTime, Action<object> callback){
        return uiTween.AddTween(LeanTween.delayedCall(delayTime, callback));
    }
    public static LTDescr delayedSound(this IUITween uiTween,GameObject gameObject, AudioClip audio, Vector3 pos, float volume){
        return uiTween.AddTween(LeanTween.delayedSound(gameObject, audio, pos, volume),gameObject);
    }
    public static LTDescr delayedSound(this IUITween uiTween,AudioClip audio, Vector3 pos, float volume){
        return uiTween.AddTween(LeanTween.delayedSound(audio, pos, volume));
    }
    public static LTDescr destroyAfter(this IUITween uiTween,LTRect rect, float delayTime){
        return uiTween.AddTween(LeanTween.destroyAfter(rect, delayTime));
    }
    public static LTDescr followBounceOut(this IUITween uiTween,Transform trans, Transform target, LeanProp prop, float smoothTime, float maxSpeed = -1, float friction = 2, float accelRate = 0.5F, float hitDamping = 0.9F){
        return uiTween.AddTween(LeanTween.followBounceOut(trans,target,prop,smoothTime,maxSpeed,friction,accelRate,hitDamping),trans.gameObject);
    }
    public static LTDescr followDamp(this IUITween uiTween,Transform trans, Transform target, LeanProp prop, float smoothTime, float maxSpeed = -1){
        return uiTween.AddTween(LeanTween.followDamp(trans,target,prop,smoothTime,maxSpeed));
    }
    public static LTDescr followLinear(this IUITween uiTween,Transform trans, Transform target, LeanProp prop, float moveSpeed){
        return uiTween.AddTween(LeanTween.followDamp(trans,target,prop,moveSpeed));
    }
    public static LTDescr followSpring(this IUITween uiTween,Transform trans, Transform target, LeanProp prop, float smoothTime, float maxSpeed = -1, float friction = 2, float accelRate = 0.5F){
        return uiTween.AddTween(LeanTween.followSpring(trans,target,prop,smoothTime,maxSpeed,friction,accelRate));
    }
    public static LTDescr move(this IUITween uiTween,RectTransform rectTrans, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.move(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr move(this IUITween uiTween,LTRect ltRect, Vector2 to, float time){
        return uiTween.AddTween(LeanTween.move(ltRect, to, time));
    }
    public static LTDescr move(this IUITween uiTween,GameObject gameObject, LTSpline to, float time){
        return uiTween.AddTween(LeanTween.move(gameObject, to, time),gameObject);
    }
    public static LTDescr move(this IUITween uiTween,GameObject gameObject, LTBezierPath to, float time){
        return uiTween.AddTween(LeanTween.move(gameObject, to, time),gameObject);
    }
    public static LTDescr move(this IUITween uiTween,GameObject gameObject, Vector3[] to, float time){
        return uiTween.AddTween(LeanTween.move(gameObject, to, time),gameObject);
    }
    public static LTDescr move(this IUITween uiTween,GameObject gameObject, Vector2 to, float time){
        return uiTween.AddTween(LeanTween.move(gameObject, to, time),gameObject);
    }
    public static LTDescr move(this IUITween uiTween,GameObject gameObject, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.move(gameObject, to, time),gameObject);
    }
    public static LTDescr move(this IUITween uiTween,GameObject gameObject, Transform to, float time){
        return uiTween.AddTween(LeanTween.move(gameObject, to, time),gameObject);
    }
    public static LTDescr moveLocal(this IUITween uiTween,GameObject gameObject, LTBezierPath to, float time){
        return uiTween.AddTween(LeanTween.moveLocal(gameObject, to, time),gameObject);
    }
    public static LTDescr moveLocal(this IUITween uiTween,GameObject gameObject, Vector3[] to, float time){
        return uiTween.AddTween(LeanTween.moveLocal(gameObject, to, time),gameObject);
    }
    public static LTDescr moveLocal(this IUITween uiTween,GameObject gameObject, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.moveLocal(gameObject, to, time),gameObject);
    }
    public static LTDescr moveLocal(this IUITween uiTween,GameObject gameObject, LTSpline to, float time){
        return uiTween.AddTween(LeanTween.moveLocal(gameObject, to, time),gameObject);
    }
    public static LTDescr moveLocalX(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.moveLocalX(gameObject, to, time),gameObject);
    }
    public static LTDescr moveLocalY(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.moveLocalY(gameObject, to, time),gameObject);
    }
    public static LTDescr moveLocalZ(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.moveLocalZ(gameObject, to, time),gameObject);
    }
    public static LTDescr moveMargin(this IUITween uiTween,LTRect ltRect, Vector2 to, float time){
        return uiTween.AddTween(LeanTween.moveMargin(ltRect, to, time));
    }
    public static LTDescr moveSpline(this IUITween uiTween,GameObject gameObject, LTSpline to, float time){
        return uiTween.AddTween(LeanTween.moveSpline(gameObject, to, time),gameObject);
    }
    public static LTDescr moveSpline(this IUITween uiTween,GameObject gameObject, Vector3[] to, float time){
        return uiTween.AddTween(LeanTween.moveSpline(gameObject, to, time),gameObject);
    }
    public static LTDescr moveSplineLocal(this IUITween uiTween,GameObject gameObject, Vector3[] to, float time){
        return uiTween.AddTween(LeanTween.moveSplineLocal(gameObject, to, time),gameObject);
    }
    public static LTDescr moveX(this IUITween uiTween,RectTransform rectTrans, float to, float time){
        return uiTween.AddTween(LeanTween.moveX(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr moveX(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.moveX(gameObject, to, time),gameObject);
    }
    public static LTDescr moveY(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.moveY(gameObject, to, time),gameObject);
    }
    public static LTDescr moveY(this IUITween uiTween,RectTransform rectTrans, float to, float time){
        return uiTween.AddTween(LeanTween.moveY(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr moveZ(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.moveZ(gameObject, to, time),gameObject);
    }
    public static LTDescr moveZ(this IUITween uiTween,RectTransform rectTrans, float to, float time){
        return uiTween.AddTween(LeanTween.moveZ(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr rotate(this IUITween uiTween,RectTransform rectTrans, float to, float time){
        return uiTween.AddTween(LeanTween.rotate(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr rotate(this IUITween uiTween,RectTransform rectTrans, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.rotate(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr rotate(this IUITween uiTween,LTRect ltRect, float to, float time){
        return uiTween.AddTween(LeanTween.rotate(ltRect, to, time));
    }
    public static LTDescr rotate(this IUITween uiTween,GameObject gameObject, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.rotate(gameObject, to, time),gameObject);
    }
    public static LTDescr rotateAround(this IUITween uiTween,GameObject gameObject, Vector3 axis, float add, float time){
        return uiTween.AddTween(LeanTween.rotateAround(gameObject, axis, add, time),gameObject);
    }
    public static LTDescr rotateAround(this IUITween uiTween,RectTransform rectTrans, Vector3 axis, float to, float time){
        return uiTween.AddTween(LeanTween.rotateAround(rectTrans, axis, to, time),rectTrans.gameObject);
    }
    public static LTDescr rotateAroundLocal(this IUITween uiTween,RectTransform rectTrans, Vector3 axis, float to, float time){
        return uiTween.AddTween(LeanTween.rotateAroundLocal(rectTrans, axis, to,time),rectTrans.gameObject);
    }
    public static LTDescr rotateAroundLocal(this IUITween uiTween,GameObject gameObject, Vector3 axis, float add, float time){
        return uiTween.AddTween(LeanTween.rotateAroundLocal(gameObject, axis, add, time),gameObject);
    }
    public static LTDescr rotateLocal(this IUITween uiTween,GameObject gameObject, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.rotateLocal(gameObject, to, time),gameObject);
    }
    public static LTDescr rotateX(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.rotateX(gameObject, to, time),gameObject);
    }
    public static LTDescr rotateY(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.rotateY(gameObject, to, time),gameObject);
    }
    public static LTDescr rotateZ(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.rotateZ(gameObject, to, time),gameObject);
    }
    public static LTDescr scale(this IUITween uiTween,LTRect ltRect, Vector2 to, float time){
        return uiTween.AddTween(LeanTween.scale(ltRect, to, time));
    }
    public static LTDescr scale(this IUITween uiTween,RectTransform rectTrans, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.scale(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr scale(this IUITween uiTween,GameObject gameObject, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.scale(gameObject, to, time),gameObject);
    }
    public static LTDescr scaleX(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.scaleX(gameObject, to, time),gameObject);
    }
    public static LTDescr scaleY(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.scaleY(gameObject, to, time),gameObject);
    }
    public static LTDescr scaleZ(this IUITween uiTween,GameObject gameObject, float to, float time){
        return uiTween.AddTween(LeanTween.scaleZ(gameObject, to, time),gameObject);
    }
    public static LTDescr size(this IUITween uiTween,RectTransform rectTrans, Vector2 to, float time){
        return uiTween.AddTween(LeanTween.size(rectTrans, to, time),rectTrans.gameObject);
    }
    public static LTDescr textAlpha(this IUITween uiTween,RectTransform rectTransform, float to, float time){
        return uiTween.AddTween(LeanTween.textAlpha(rectTransform, to, time),rectTransform.gameObject);
    }
    public static LTDescr textColor(this IUITween uiTween,RectTransform rectTransform, Color to, float time){
        return uiTween.AddTween(LeanTween.textColor(rectTransform, to, time),rectTransform.gameObject);
    }
    public static LTDescr value(this IUITween uiTween,float from, float to, float time){
        return uiTween.AddTween(LeanTween.value(from, to, time));
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, float from, float to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Color from, Color to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, callOnUpdate,from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Action<float, float> callOnUpdateRatio, float from, float to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, callOnUpdateRatio,from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Action<Color> callOnUpdate, Color from, Color to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, callOnUpdate,from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Action<Color, object> callOnUpdate, Color from, Color to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, callOnUpdate,from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Action<Vector2> callOnUpdate, Vector2 from, Vector2 to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, callOnUpdate,from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, callOnUpdate,from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Action<float, object> callOnUpdate, float from, float to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, callOnUpdate,from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Vector3 from, Vector3 to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, from, to, time),gameObject);
    }
    public static LTDescr value(this IUITween uiTween,GameObject gameObject, Vector2 from, Vector2 to, float time){
        return uiTween.AddTween(LeanTween.value(gameObject, from, to, time),gameObject);
    }

}