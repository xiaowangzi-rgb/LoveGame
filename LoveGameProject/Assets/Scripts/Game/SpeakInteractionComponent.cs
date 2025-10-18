using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话交互组件
/// </summary>
public class SpeakInteractionComponent : InteractionComponent
{
    [Header("说话文本")]
    public string[] SpeakTexts;
    [Header("是否显示自己说话气泡")]
    public bool IsShowSpeakBubble = false;

    public override void Execute(InteractionTriggerComponent target)
    {
        if (SpeakTexts == null || SpeakTexts.Length <= 0)
        {
            return;
        }
        var speakText = GetRandomSpeakText();
        if (string.IsNullOrEmpty(speakText))
        {
            return;
        }
        //如果不需要本体显示说话就调用对方的说话
        if (!IsShowSpeakBubble)
        {
            Tools.DoSpeak(target.transform, speakText, 1f, 1.0f, null);
        }
        else if(IsShowSpeakBubble &&transform.GetComponent<SpeakComponent>() != null){
            Tools.DoSpeak(transform, speakText, 1f, 1.0f, null);
        }
    }

    private string GetRandomSpeakText()
    {
        if (SpeakTexts == null || SpeakTexts.Length <= 0)
        {
            return "";
        }
        return SpeakTexts[Random.Range(0, SpeakTexts.Length)];
    }
}
