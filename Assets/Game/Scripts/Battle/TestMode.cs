using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using Core;
using Cysharp.Threading.Tasks;
using TMPro;
using KitaujiGameDesignClub.GameFramework.UI;

public class TestMode : MonoBehaviour
{
    //��ע���������ģʽ֮ǰ��Ӧ���޸��ڴ��е����ã�ʹ����ģʽ��Զ��������̨��֡����ʾ

    [Header("ͨ��")]
    public TMP_Text title;

    [Header("����ѡ����")]
    public CanvasGroup CardSelector;
    public Button CardSelectorToggle;
    private bool isExpanded;
    public LeanButton CardSelectorCloseButton;
    public LeanButton CardSelectorOpenByFileExplorerButton;
    public LeanButton CardSelectorConfirmButton;
    /// <summary>
    /// ������ѡ����"�Ŀ����б�
    /// </summary>
    public InputFieldWithDropdown BundleList;
    /// <summary>
    /// ����ѡ�������Ŀ�����Ϣչʾ
    /// </summary>
    public GameObject BundleInformationDisplay;
    public TMP_Text manifestFriendlyName;
    public TMP_Text manifestName;
    public TMP_Text manifestAnime;
    public TMP_Text manifestAuthorName;
    public TMP_Text manifestDescription;
    /// <summary>
    /// ������ѡ����"�Ŀ����б�
    /// </summary>
    public InputFieldWithDropdown CardList;
    /// <summary>
    /// ����ѡ�����Ҳ�Ŀ�����Ϣչʾ
    /// </summary>
    public GameObject CardInformationDisplay;
    public TMP_Text cardFriendlyName;
    public TMP_Text cardCharacterName;
    public TMP_Text cardCharacterVoiceName;
    public TMP_Text cardAnime;
    public TMP_Text cardBasicInf;//������������ֵ
    public TMP_Text cardTag;
    public TMP_Text cardDescription;
    /// <summary>
    /// ��������п���
    /// </summary>
    private Bundle[] allBundles;



    private Animator animator;

    private void Awake()
    {
        //���û�п�������ģʽ���������������

        //����ʼ��
        title.text = $"Ŀǰ���ڲ���ģʽ\nDevice:{SystemInfo.deviceType}  CPU:{SystemInfo.processorType}  OS:{SystemInfo.operatingSystem}  RAM:{SystemInfo.systemMemorySize}MiB  Screen:{Screen.currentResolution}";

        #region ����ѡ����
        //͸���ȵ���
        CardSelector.alpha = 0f;
        CardSelector.blocksRaycasts = false;
        //��������ѡ�������ļ���״̬
        CardSelector.gameObject.SetActive(true);
        BundleList.gameObject.SetActive(true);
        BundleInformationDisplay.SetActive(false);
        CardList.gameObject.SetActive(false);
        CardInformationDisplay.SetActive(false);

        #endregion
    }

    /// <summary>
    /// ���ز���ģʽ
    /// </summary>
    async void Start()
    {
       //���ز���ģʽ
       await  LoadTestMode();

    }


    private async UniTask LoadTestMode()
    {
        animator = GetComponent<Animator>();
        GameUI.gameUI.SetBanInputLayer(true, "����ģʽ������...");

        #region ����ѡ����


        //��ȡ���еĿ���
        GameUI.gameUI.SetBanInputLayer(true, "�����ȡ��...");
        allBundles = await CardReadWrite.GetAllBundles();

#if UNITY_EDITOR || UNITY_STANDALONE
        //����Դ�������򿪹涨��Ŀ¼
        CardSelectorOpenByFileExplorerButton.OnClick.AddListener(delegate
        {
            Application.OpenURL($"file://{Information.bundlesPath}");
        });

#else
  Destroy(CardSelectorOpenByFileExplorerButton.gameObject);
#endif


        //չ���͹ر�
        CardSelectorToggle.onClick.AddListener(delegate
        {
            //�л�����״̬
            isExpanded = !isExpanded;
            animator.SetBool("expanded", isExpanded);
        });
        CardSelectorCloseButton.OnClick.AddListener(delegate
        {
            //����ر�״̬
            isExpanded = false;
            animator.SetBool("expanded", isExpanded);
        });


        //�ѿ����嵥������ӳ�䵽�������б��У�����ѡ�������Ķ�����
        List<string> bundlesName = new();
        // bundlesName.Add("<align=\"center\"><alpha=#CC>����Ϊ���ÿ���");
        foreach (var item in allBundles)
        {
            Debug.Log(item);
            if (string.IsNullOrEmpty(item.manifest.Anime)) bundlesName.Add($"{item.manifest.FriendlyBundleName}");
            else bundlesName.Add($"��{item.manifest.Anime}��{item.manifest.FriendlyBundleName}");
        }
        BundleList.ChangeOptionDatas(bundlesName);
        bundlesName = null;
        //  BundleList.ban.Add("<align=\"center\"><alpha=#CC>����Ϊ���ÿ���");

        //ѡ���Ŀ�����Ϣͬ��
        BundleList.onDropdownValueChangedWithoutInt.AddListener(delegate
        {
            //������ѡ����Ϣͬ���뿨���б���
            if (BundleList.DropdownValue != 0)
            {
                UpdateSelectorBundleInformation(allBundles[BundleList.DropdownValue - 1].manifest);
                //��ʾ�˿�����Ϣ��������ѡ����
                BundleInformationDisplay.SetActive(true);
                CardList.gameObject.SetActive(true);
                //���ÿ���Ҳ�ŵ��б���
                List<string> cardsName = new();
                foreach (var item in allBundles[BundleList.DropdownValue - 1].cards)
                {
                    if (string.IsNullOrEmpty(item.CharacterName)) cardsName.Add($"{item.FriendlyCardName}");
                    else cardsName.Add($"��{item.CharacterName}��{item.FriendlyCardName}");
                }
                CardList.ChangeOptionDatas(cardsName);
            }
            //value = 0��û��ѡ���飬������Ҫ�İ�����
            else
            {
                BundleInformationDisplay.SetActive(false);
                CardList.gameObject.SetActive(false);
                CardInformationDisplay.SetActive(false);
            }

        });
        //ѡ���Ŀ�����Ϣͬ��
        CardList.onDropdownValueChangedWithoutInt.AddListener(delegate 
        {
            UpdateSelectorCardInformation(allBundles[BundleList.DropdownValue - 1].cards[CardList.DropdownValue - 1]);
            CardInformationDisplay.SetActive(true);
        });

        CardSelectorConfirmButton.OnClick.AddListener(delegate { });
        #endregion



        //�ر���������
        GameUI.gameUI.SetBanInputLayer(false, "����ģʽ������...");
    }




#region ����ѡ�������׷���

    /// <summary>
    /// �ڿ���ѡ������ͬ�������嵥��Ϣ
    /// </summary>
    /// <param name="manifestContent"></param>
    void UpdateSelectorBundleInformation(CardBundlesManifest manifestContent)
    {
        manifestFriendlyName.text = $"<b>�Ѻ����ƣ�</b>\n<margin-left=1em><size=80%>{manifestContent.FriendlyBundleName}";
        manifestName.text =  $"<b>ʶ�����ƣ�</b>\n<margin-left=1em><size=80%>{manifestContent.BundleName}";
        manifestAnime.text = $"<b>����������</b>\n<margin-left=1em><size=80%>{manifestContent.Anime}";
        manifestAuthorName.text = $"<b>�������ƣ�</b>\n<margin-left=1em><size=80%>{manifestContent.AuthorName}";
        manifestDescription.text = $"<b>������ܣ�</b>\n<margin-left=1em><size=80%>{manifestContent.Description}";
    }

    /// <summary>
    /// �ڿ���ѡ������ͬ��������Ϣ
    /// </summary>
    /// <param name="manifestContent"></param>
    void UpdateSelectorCardInformation(CharacterCard cardContent)
    {
        cardFriendlyName.text = $"<b>�Ѻ����ƣ�</b>\n<margin-left=1em><size=80%>{cardContent.FriendlyCardName}";
        cardCharacterName.text = $"<b>��ɫ���ƣ�</b>\n<margin-left=1em><size=80%>{cardContent.CharacterName}";
        cardCharacterVoiceName.text = $"<b>�������ƣ�</b>\n<margin-left=1em><size=80%>{cardContent.CV}";
        cardAnime.text = $"<b>����������</b>\n<margin-left=1em><size=80%>{cardContent.Anime}";
        cardBasicInf.text = $"<b>ִ����/����ֵ��</b>{cardContent.BasicPower}/{cardContent.BasicHealthPoint}";
        //��ǩչʾ
        cardTag.text = string.Empty;
        foreach (var item in cardContent.tags)
        {
            cardTag.text = $"{cardTag.text}��{item}";
        }
        cardTag.text = $"<b>��ǩ��</b>{cardTag.text.Substring(1)}";
        cardDescription.text = $"<b>�������ܣ�</b>\n<margin-left=1em><size=80%>{cardContent.AbilityDescription}";
    }

    #endregion



}
