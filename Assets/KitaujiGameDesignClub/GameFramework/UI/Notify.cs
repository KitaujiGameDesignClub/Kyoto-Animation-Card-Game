using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace KitaujiGameDesignClub.GameFramework.UI
{
    [DisallowMultipleComponent]
    public class Notify : MonoBehaviour
    {
        public static Notify notify = null;

       
        /// <summary>
        /// 通知面板
        /// </summary>
        [FormerlySerializedAs("Modal")]
        [Header("强通知")]
        [SerializeField] private LeanWindow strongNotification;

        /// <summary>
        /// 通知内容
        /// </summary>
        [FormerlySerializedAs("content")] [SerializeField] private TMP_Text strongContent;
    
        /// <summary>
        /// 关闭通知
        /// </summary>
        [SerializeField] private LeanButton Shutdown;

        
        [Header("普通通知")] [SerializeField] private LeanPulse BannerNotification;
        [SerializeField] private TMP_Text BannerContent;

        /// <summary>
        /// 默认字体大小0=Banner 1=strong
        /// </summary>
        private float[] DefaultfontSize = new float[2];
    
        private void Awake()
        {
            //关闭面板
            strongNotification.TurnOff();
            strongContent.text = string.Empty;
            //清除事件
            Shutdown.OnDown.RemoveAllListeners();
            strongNotification.OnOn.RemoveAllListeners();
            //缓存字体大小
            DefaultfontSize[1] = strongContent.fontSize;
            DefaultfontSize[0] = BannerContent.fontSize;
            
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
        /// 创建强通知（弹窗通知）
        /// </summary>
        /// <param name="onNotify">通知开始时的事件</param>
        /// <param name="OnOff">通知关闭后的事件</param>
        /// <param name="title">通知标题</param>
        /// <param name="content">通知内容</param>
        /// <param name="fontSizeRate">全局字体大小（相对值）</param>
        public void CreateStrongNotification(UnityAction onNotify,UnityAction OnOff,string title,string content,float fontSizeRate = 1f)
        {
            if(onNotify != null)  strongNotification.OnOn.AddListener(onNotify);
       
            Shutdown.OnDown.AddListener(delegate
            {
                strongNotification.TurnOff();
                if(OnOff != null) OnOff.Invoke();
            });

            this.strongContent.text = $"<size=132%><align=\"center\">{title}</align></size>\n\n{content}";
            this.strongContent.fontSize = DefaultfontSize[1] * fontSizeRate;
        
            strongNotification.TurnOn();
            Debug.Log($"发生强通知：{title} - {content}");
     
      
        }

/// <summary>
/// 创建通知（横幅通知）
/// </summary>
/// <param name="onNotify">通知开始时的事件</param>
/// <param name="content">通知内容</param>
/// <param name="fontSizeRate">全局字体大小（相对值）</param>
        public void CreateBannerNotification(UnityAction onNotify, string content,float fontSizeRate = 1f)
        {
            if(onNotify != null)  BannerNotification.OnPulse.AddListener(delegate(int arg0) { onNotify.Invoke(); });

         

            this.strongContent.text = content;
            this.strongContent.fontSize = DefaultfontSize[0] * fontSizeRate;
        
            BannerNotification.Pulse();
            Debug.Log($"发生通知：{content}");
        }


#if UNITY_EDITOR
        /// <summary>
        /// 测试横幅通知
        /// </summary>
        [ContextMenu("test banner notification")]
        public void TestBanner()
        {
            CreateBannerNotification(null,"测试通知");
        }
        
        
        /// <summary>
        /// 测试强通知
        /// </summary>
        [ContextMenu("test strong notification")]
        public void TestStrong()
        {
           CreateStrongNotification(null,null,"测试通知","1145141919810");
        }
#endif
    }
}
