using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace KitaujiGameDesignClub.GameFramework.UI
{
    [DisallowMultipleComponent]
    public class Notify : MonoBehaviour
    {
        public static Notify notify = null;


        /// <summary>
        /// 通知面板
        /// </summary>
        [FormerlySerializedAs("Modal")] [Header("强通知")] [SerializeField]
        private LeanWindow strongNotification;

        [SerializeField] private Button buttonOne;
        [SerializeField] private Button buttonTwo;
        [SerializeField] private Button buttonThree;
        [SerializeField] private TMP_Text[] buttonTexts = new TMP_Text[3];

        /// <summary>
        /// 通知内容
        /// </summary>
        [FormerlySerializedAs("content")] [SerializeField]
        private TMP_Text strongContent;

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
        public void CreateStrongNotification(UnityAction onNotify, UnityAction OnOff, string title, string content,
            UnityAction Button1 = null, string ButtonOneText = "确认", UnityAction Button2 = null,
            string ButtonTwoText = "返回", UnityAction Button3 = null, string ButtonThreeText = "取消",
            float fontSizeRate = 1f)
        {
            
            //优先处理按钮
            //激活与禁用
            buttonOne.gameObject.SetActive(Button1 != null);
            buttonTwo.gameObject.SetActive(Button2 != null);
            buttonThree.gameObject.SetActive(Button3 != null);
            //以往事件清除
            buttonOne.onClick.RemoveAllListeners();
            buttonThree.onClick.RemoveAllListeners();
            buttonTwo.onClick.RemoveAllListeners();
            //事件添加
            if (Button1 != null)
            {
                buttonOne.onClick.AddListener(Button1);
                buttonTexts[0].text = ButtonOneText;
            }

            if (Button2 != null)
            {
                buttonTwo.onClick.AddListener(Button2);
                buttonTexts[1].text = ButtonTwoText;
            }

            if (Button3 != null)
            {
                buttonTexts[2].text = ButtonThreeText;
                buttonThree.onClick.AddListener(Button3);
            }

//清除残留事件
            strongNotification.OnOn.RemoveAllListeners();
            Shutdown.OnDown.RemoveAllListeners();

            if (onNotify != null) strongNotification.OnOn.AddListener(onNotify);

            Shutdown.OnDown.AddListener(delegate
            {
                TurnOffStrongNotification();
                if (OnOff != null) OnOff.Invoke();
            });

            this.strongContent.text = $"<size=132%><align=\"center\">{title}</align></size>\n\n{content}";
            this.strongContent.fontSize = DefaultfontSize[1] * fontSizeRate;

            strongNotification.TurnOn();
            Debug.Log($"发生强通知：{title} - {content}");
        }


        public void TurnOffStrongNotification()
        {
            strongNotification.TurnOff();
        }

        /// <summary>
        /// 创建通知（横幅通知）
        /// </summary>
        /// <param name="onNotify">通知开始时的事件</param>
        /// <param name="content">通知内容</param>
        /// <param name="fontSizeRate">全局字体大小（相对值）</param>
        public void CreateBannerNotification(UnityAction onNotify, string content, float fontSizeRate = 1f)
        {
            BannerNotification.OnPulse.RemoveAllListeners();

            if (onNotify != null) BannerNotification.OnPulse.AddListener(delegate(int arg0) { onNotify.Invoke(); });


            BannerContent.text = content;
            BannerContent.fontSize = DefaultfontSize[0] * fontSizeRate;

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
            CreateBannerNotification(null, "测试通知");
        }


        /// <summary>
        /// 测试强通知
        /// </summary>
        [ContextMenu("test strong notification")]
        public void TestStrong()
        {
            CreateStrongNotification(null, null, "测试通知", "1145141919810");
        }
#endif
    }
}