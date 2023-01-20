using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using Core;
using Cysharp.Threading.Tasks;
using TMPro;
using KitaujiGameDesignClub.GameFramework.UI;
using UnityEngine.Events;
using System.IO;

public class TestMode : MonoBehaviour
{
    //��ע���������ģʽ֮ǰ��Ӧ���޸��ڴ��е����ã�ʹ����ģʽ��Զ��������̨��֡����ʾ

    [Header("ͨ��")]
    public TMP_Text title;
    /// <summary>
    /// �л�����Ϸ����֮ǰ����Ҫִ�е��¼�
    /// </summary>
    private List<UnityAction> eventBeforeSwitchToGame = new();
    public CanvasGroup panel;
    public TMP_Text loadState;
    public LeanButton panelCloseButton;
    public LeanButton CardSelectorOpenByFileExplorerButton;
    [Header("����ѡ����")]
    public Toggle CardSelectorToggle;
    public GameObject cardSelector;
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
    /// <summary>
    /// ѡ��Ŀ����id
    /// </summary>
    int selectedBundleId = -1;
    /// <summary>
    /// ѡ��Ŀ��Ƶ�id��ѡ�������ڵģ�
    /// </summary>
    int selectedCardId = -1;

    [Header("����������")]
    public GameObject voiceTestor;
    public Toggle voiceTestorToggle;
    public AudioSource voiceTestPlayer;
    public Slider voiceTestVolume;
    /// <summary>
    /// 6����Ƶ������
    /// </summary>
    public TMAudioTestor[] tMAudioTestors = new TMAudioTestor[6];

    private Animator animator;

    private void Awake()
    {
        //���û�п�������ģʽ���������������


    }

    /// <summary>
    /// ���ز���ģʽ
    /// </summary>
    async void Start()
    {        
        //����ʼ��
        title.text = $"Ŀǰ���ڲ���ģʽ\nDevice:{SystemInfo.deviceType}  CPU:{SystemInfo.processorType}  OS:{SystemInfo.operatingSystem}  RAM:{SystemInfo.systemMemorySize}MiB  Screen:{Screen.currentResolution}";


        #region ��������ѡ�������ļ���״̬
        BundleList.gameObject.SetActive(true);
        BundleInformationDisplay.SetActive(false);
        CardList.gameObject.SetActive(false);
        CardInformationDisplay.SetActive(false);
        #endregion

        #region ������Ƶ���������ļ���״̬
        foreach (var item in tMAudioTestors)
        {
            item.gameObject.SetActive(false);
        }
        #endregion


        //���ز���ģʽ
        await LoadTestMode();

    }

    /// <summary>
    /// ���� ��ʼ������ģʽ
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadTestMode()
    {
        animator = GetComponent<Animator>();
        //��ʼ���������״̬
        loadState.text = string.Empty;
        GameUI.gameUI.SetBanInputLayer(true, "����ģʽ������...");
        backgroundActivity();


        #region ����ѡ����
        //����ѡ���� չ���͹ر�
        Toggle(CardSelectorToggle,cardSelector);

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

        //�ѿ����嵥������ӳ�䵽�������б��У�����ѡ�������Ķ�����
        List<string> bundlesName = new();
        // bundlesName.Add("<align=\"center\"><alpha=#CC>����Ϊ���ÿ���");
        for (int i = 0; i < allBundles.Length; i++)
        {
            Bundle bundle = allBundles[i];
            if (string.IsNullOrEmpty(bundle.manifest.Anime)) bundlesName.Add($"{bundle.manifest.FriendlyBundleName}<alpha=#00>{i}");
            else bundlesName.Add($"��{bundle.manifest.Anime}��{bundle.manifest.FriendlyBundleName}<alpha=#00>{i}");
            //͸����Ϊ0�������ַ���<alpha=#00>{i}��������¼��Щ�������ţ�����������ѡ��
        }
        BundleList.ChangeOptionDatas(bundlesName);
        bundlesName = null;
        //  BundleList.ban.Add("<align=\"center\"><alpha=#CC>����Ϊ���ÿ���");

        //ѡ���Ŀ�����Ϣͬ��
        BundleList.onDropdownValueChangedWithoutInt.AddListener(delegate
        {
            //allBundles[selectedBundleId]����ѡ����

            //��ȡ��ѡbundle�����
            Debug.Log(BundleList.text.Split("<alpha=#00>")[1]);
            selectedBundleId = BundleList.text.Contains("<alpha=#00>") ? int.Parse(BundleList.text.Split("<alpha=#00>")[1]) : -1;

            //������ѡ����Ϣͬ���뿨���б���
            if (BundleList.DropdownValue != 0)
            {
                UpdateSelectorBundleInformation(allBundles[selectedBundleId].manifest);
                //��ʾ�˿�����Ϣ��������ѡ����
                BundleInformationDisplay.SetActive(true);
                CardList.gameObject.SetActive(true);
                //���ÿ���Ҳ�ŵ��б���
                List<string> cardsName = new();
                for (int i = 0; i < allBundles[selectedBundleId].cards.Length; i++)
                {
                    CharacterCard card = allBundles[selectedBundleId].cards[i];
                    if (string.IsNullOrEmpty(card.CharacterName)) cardsName.Add($"{card.FriendlyCardName}<alpha=#00>{i}");
                    else cardsName.Add($"��{card.CharacterName}��{card.FriendlyCardName}<alpha=#00>{i}");
                    //͸����Ϊ0�������ַ���<alpha=#00>{i}��������¼��Щ�������ţ�����������ѡ��
                }
                CardList.ChangeOptionDatas(cardsName);
            }
            //value = 0��û��ѡ���飬������Ҫ�İ�����
            else
            {
                BundleInformationDisplay.SetActive(false);
                CardList.gameObject.SetActive(false);
                CardInformationDisplay.SetActive(false);
                CardList.text = string.Empty;
                CardList.ClearOptions();
            }

        });
        //ѡ���Ŀ�����Ϣͬ��
        CardList.onDropdownValueChangedWithoutInt.AddListener(delegate 
        {
            //��ȡ��ѡcard�����
            Debug.Log(CardList.text.Split("<alpha=#00>")[1]);
            selectedCardId = CardList.text.Contains("<alpha=#00>") ? int.Parse(CardList.text.Split("<alpha=#00>")[1]) : -1;

            UpdateSelectorCardInformation(allBundles[selectedBundleId].cards[selectedCardId]);
            CardInformationDisplay.SetActive(true);
        });

        //ȷ�ϴ˿����ϳ�������Ӽ�����Դ���¼�
        CardSelectorConfirmButton.OnClick.AddListener(delegate
        {
            //allBundles[selectedBundleId]����ѡ����

            //��ѡ���ƣ�������ִ��ȷ�ϲ���
            if (CardList.DropdownValue > 0)
            {
                //��Ӽ�����Դ���¼�
                eventBeforeSwitchToGame.Add(UniTask.UnityAction(async () =>
                {
                    //��ѡ����
                    var card = allBundles[selectedBundleId].cards[selectedCardId];
                    loadState.text = $"���ڼ��ء�{card.FriendlyCardName}������ȴ�...";
                    //��ѡ���Ƶ��ļ���·��
                    var cardDiectoryPath = $"{Path.GetDirectoryName(allBundles[selectedBundleId].manifestFullPath)}/cards/{card.CardName}";

                    //ͼƬ����              
                    var image = await LoadCoverImage($"{cardDiectoryPath}/{card.ImageName}");
                    //��Ƶ����
                    var audios = await LoadAllAudioOfOneCard(card, cardDiectoryPath);
                    GameStageCtrl.stageCtrl.AddCardAndDisplayInStage(card, 0, image, audios[0], audios[1], audios[2], audios[3]);
                    loadState.text = string.Empty;
                    image = null;
                    audios = null;
                }));
            }
        });
        #endregion

        #region ����������
        Toggle(voiceTestorToggle,voiceTestor);
        //����Ƶ������֮�󣬶�ȡ���ϴ��ڵĿ��ƣ����ڲ�����Ƶ
        voiceTestorToggle.onValueChanged.AddListener(delegate
        {
            //��Ȼ�ǵü���Ŷ�ȡ
            if (voiceTestorToggle.isOn)
            {
                var allCardPanels = GameStageCtrl.stageCtrl.GetAllCardOnStage(0);
                //�����������Ȼ��������Ӧ����Դ
                for (int i = 0; i < allCardPanels.Length; i++)
                {
                    tMAudioTestors[i].EnableAudioTestor(allCardPanels[i]);
                }
               
            }
            //�رղ��������棬�Ͱ����еĲ�����������
            else
            {
                foreach (var item in tMAudioTestors)
                {
                    item.gameObject.SetActive(false);
                }
            }
          
        });

        //��������
        voiceTestVolume.onValueChanged.AddListener(delegate (float arg0)
        {
            voiceTestPlayer.volume = arg0;
        });

        #endregion

        //�ر���������
        GameUI.gameUI.SetBanInputLayer(false, "����ģʽ������...");
    }


    private async UniTask backgroundActivity()
    {
        while (true)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);

            if(eventBeforeSwitchToGame.Count > 0)
            {
              eventBeforeSwitchToGame[0].Invoke();
                //��ʹ��remove��������¼��Ի��������
              eventBeforeSwitchToGame.RemoveAt(0);
            }
        }
    }

    #region ͨ��
    void Toggle(Toggle toggle,GameObject panelObject)
    {
        //�Ϸ����л���
        toggle.onValueChanged.AddListener(delegate
        {
            //ȫ���������ˣ������л�����
            if (string.IsNullOrEmpty(loadState.text))
            {
                //�л�����״̬                
                animator.SetBool("Expanded", toggle.isOn);
                panelObject.SetActive(toggle.isOn);
            }

        });

        //ÿ��panel�ڲ��Ĺرհ�ť
        panelCloseButton.OnClick.AddListener(delegate
        {
            //����ر�״̬
            toggle.isOn = false;

        });

        //�ر�������ߵĽ���
        panelObject.SetActive(false);
    }
    #endregion


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
        cardTag.text = $"<b>��ǩ��</b>";
        for (int i = 0; i < cardContent.tags.Count; i++)
        {
            string item = cardContent.tags[i];
            if (i == 0) cardTag.text = $"{cardTag.text}{item}";           
            cardTag.text = $"{cardTag.text}��{item}";
        }
        cardDescription.text = $"<b>�������ܣ�</b>\n<margin-left=1em><size=80%>{cardContent.AbilityDescription}";
    }

    async UniTask<Sprite> LoadCoverImage(string inmageFullPath)
    {
        //����ͼƬ���������ʧ�ܵĻ�������Ԥ���Դ���Ĭ��ͼƬ��
        var texture = await CardReadWrite.CoverImageLoader(inmageFullPath);
        if (texture != null)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one / 2);
        }
        else return null;

    }

    /// <summary>
    /// ����debut ability defeat exit��˳���ȡĳ���������е���Ƶ
    /// </summary>
    /// <param name="cardContent"></param>
    /// <param name="cardDirectoryPath"></param>
    /// <returns></returns>
    async UniTask<AudioClip[]> LoadAllAudioOfOneCard(CharacterCard cardContent,string cardDirectoryPath)
    {
        var (debut, ability, defeat, exit) = await UniTask.WhenAll(CardReadWrite.CardVoiceLoader($"{cardDirectoryPath}/{cardContent.voiceDebutFileName}"),
                                             CardReadWrite.CardVoiceLoader($"{cardDirectoryPath}/{cardContent.voiceAbilityFileName}"),
                                             CardReadWrite.CardVoiceLoader($"{cardDirectoryPath}/{cardContent.voiceDefeatFileName}"),
                                             CardReadWrite.CardVoiceLoader($"{cardDirectoryPath}/{cardContent.voiceExitFileName}"));

        AudioClip[] clips =  {debut,ability,defeat,exit};
        return clips;


    }
    #endregion

}


