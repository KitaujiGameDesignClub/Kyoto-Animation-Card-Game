using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Notify : MonoBehaviour
{
    public static Notify notify = null;

    /// <summary>
    /// 通知面板
    /// </summary>
    [SerializeField] private LeanWindow Modal;

    /// <summary>
    /// 通知内容
    /// </summary>
    [SerializeField] private TMP_Text content;
    
    /// <summary>
    /// 关闭通知
    /// </summary>
    [SerializeField] private LeanButton Shutdown;

    /// <summary>
    /// 默认字体大小
    /// </summary>
    private float DefaultfontSize;
    
    private void Awake()
    {
        //关闭面板
        Modal.TurnOff();
        content.text = string.Empty;
        //清除事件
        Shutdown.OnDown.RemoveAllListeners();
        Modal.OnOn.RemoveAllListeners();
        //缓存字体大小
        DefaultfontSize = content.fontSize;

        //不能存在多个
        var notificationCount = GameObject.FindObjectsOfType(typeof(Notify)).Length;
        if (notificationCount > 1)
        {
            Debug.LogError($"存在多个通知组件");
        }
        else
        {
            notify = this;
        }
    }

    /// <summary>
    /// 创建一个通知
    /// </summary>
    /// <param name="OnNotify">通知开始时的事件</param>
    /// <param name="OnOff">通知关闭后的事件</param>
    /// <param name="title">通知标题</param>
    /// <param name="content">通知内容</param>
    /// <param name="fontSize">全局字体大小（相对值）</param>
    public void CreateNotification(UnityAction OnNotify,UnityAction OnOff,string title,string content,float fontSizeRate = 1f)
    {
        if(OnNotify != null)  Modal.OnOn.AddListener(OnNotify);
       
        Shutdown.OnDown.AddListener(delegate
        {
            Modal.TurnOff();
            if(OnOff != null) OnOff.Invoke();
        });

        this.content.text = $"<size=132%><align=\"center\">{title}</align></size>\n\n{content}";
        this.content.fontSize = DefaultfontSize * fontSizeRate;
        
        Modal.TurnOn();
        Debug.Log($"发生通知：{title} - {content}");
     
      
    }
}
