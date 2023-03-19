using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using KitaujiGameDesignClub.GameFramework.UI;
using Lean.Gui;
using SimpleFileBrowser;
using TMPro;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Maker
{
    public class CardEditor : MonoBehaviour
    {
        public GameObject editor;
        public GameObject abilityEditor;
        public GameObject abilityDescriptionEditor;
        public GameObject voiceEditor;


        [Header("信息侧")] public TMP_InputField cardNameField;
        public TMP_InputField friendlyNameField;
        public TMP_InputField AuthorNameField;
        public InputFieldWithDropdown AnimeField;
        public InputFieldWithDropdown CharacterNameField;
        public InputFieldWithDropdown CVField;
        public TMP_InputField cardNumberField;
        public TMP_Dropdown genderField;
        public TMP_InputField basicPower;
        public TMP_InputField basicHp;
        public Image imageOfCardField;
        public LeanToggle AsChiefToggle;
        public Sprite defaultImage;

        [Header("标签侧")] public InputFieldWithDropdown tagField;

        /// <summary>
        /// tag list里所有的按钮的预设
        /// </summary>
        public tagListItem tagListButtonPerfebs;

        public RectTransform tagParent;

        /// <summary>
        /// 暂存tag
        /// </summary>
        private List<string> tagStorage = new();


        [Header("原因侧")] public TMP_Dropdown abilityReasonType;
        public TMP_Dropdown abilityReasonLargeScope;
        public TMP_Dropdown abilityReasonParameter;
        public TMP_Dropdown abilityReasonLogic;
        public InputFieldWithDropdown abilityReasonThreshold;
        public TMP_Dropdown abilityReasonJudgeParameter;
        public TMP_Dropdown abilityReasonJudgeMethod;
        public TMP_Dropdown abilityReasonJudgeLogic;
        public InputFieldWithDropdown abilityReasonJudgeThreshold;
        [Header("效果侧")] public Lean.Gui.LeanToggle abilityReasonObjectAsTarget;
        public TMP_Dropdown abilityResultLargeScope;
        public TMP_Dropdown abilityResultParameter;
        public TMP_Dropdown abilityResultLogic;
        public InputFieldWithDropdown abilityResultThreshold;
        public TMP_Dropdown abilityResultParameterToChange;
        public InputFieldWithDropdown abilityResultSummon;
        public TMP_InputField abilityResultRidicule;
        public TMP_InputField abilityResultSilence;
        public TMP_Dropdown abilityResultChangeMethod;
        public InputFieldWithDropdown abilityResultChangeValue;
        [Header("描述侧")]
        public TMP_InputField abilityDescription;
        [Header("音频侧")] public audioSetting voiceDebut;
        [FormerlySerializedAs("voiceKill")] public audioSetting voiceDefeat;
        public audioSetting voiceAbility;
        public audioSetting voiceExit;

        [FormerlySerializedAs("returnToTitle")] [Header("交互控件")] 
        public LeanButton returnToTPanel;

        public LeanButton SaveButtonGameObject;
        public LeanButton voiceEditButton;
        public LeanButton abilityDescriptionButton;

        public LeanButton switchToBundleEditor;

        public Button DeleteButton;

        [Space()] public CardPanel preview;


        /// <summary>
        /// 现在显示的 正在编辑的卡牌
        /// </summary>
        private CharacterCard nowEditingCard { get; set; }

        /// <summary>
        /// 新的图片所在路径
        /// </summary>
        private string newImageFullPath = string.Empty;

        internal void Initialization()
        {
            #region 能力编辑初始化（不可变下拉栏初始化)
            var allOption = new List<string>();

            abilityReasonType.ClearOptions();
            var length = Enum.GetNames(typeof(Information.CardAbilityTypes)).Length;
            for (int i = 0; i < length; i++)
            {
                allOption.Add(Information.AbilityChineseIntroduction((Information.CardAbilityTypes)i));

            }
            abilityReasonType.AddOptions(allOption);
            allOption.Clear();


            abilityReasonLargeScope.ClearOptions();
            abilityResultLargeScope.ClearOptions();
            length = Enum.GetNames(typeof(Information.Objects)).Length;
            for (int i = 0; i < length; i++)
            {
                allOption.Add(Information.AbilityChineseIntroduction((Information.Objects)i));
            }
            abilityReasonLargeScope.AddOptions(allOption);
            abilityResultLargeScope.AddOptions(abilityReasonLargeScope.options);
            allOption.Clear();

            abilityReasonParameter.ClearOptions();
            abilityResultParameter.ClearOptions();
            abilityResultParameterToChange.ClearOptions();
            abilityReasonJudgeParameter.ClearOptions();
            length = Enum.GetNames(typeof(Information.Parameter)).Length;
            for (int i = 0; i < length; i++)
            {
                allOption.Add(Information.AbilityChineseIntroduction((Information.Parameter)i));
            }

            abilityReasonParameter.AddOptions(allOption);
            abilityReasonJudgeParameter.AddOptions(abilityReasonParameter.options);
            abilityResultParameter.AddOptions(abilityReasonParameter.options);
            abilityResultParameterToChange.AddOptions(abilityReasonParameter.options);
            allOption.Clear();

            abilityReasonLogic.ClearOptions();
            abilityResultLogic.ClearOptions();
            abilityReasonJudgeLogic.ClearOptions();
            abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("不等于/不包含"));
            abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("小于"));
            abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("小于等于"));
            abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("等于/包含"));
            abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("大于等于"));
            abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("大于"));
            abilityResultLogic.AddOptions(abilityReasonLogic.options);
            abilityReasonJudgeLogic.AddOptions(abilityReasonLogic.options);

            abilityResultChangeMethod.ClearOptions();
            length = Enum.GetNames(typeof(Information.CalculationMethod)).Length;
            for (int i = 0; i < length; i++)
            {
                allOption.Add(Information.AbilityChineseIntroduction((Information.CalculationMethod)i));
            }
            abilityResultChangeMethod.AddOptions(allOption);
            allOption.Clear();

            abilityReasonJudgeMethod.ClearOptions();
            length = Enum.GetNames(typeof(Information.JudgeMethod)).Length;
            for (int i = 0; i < length; i++)
            {
                allOption.Add(Information.AbilityChineseIntroduction((Information.JudgeMethod)i));
            }
            abilityReasonJudgeMethod.AddOptions(allOption);
            allOption = null;
            #endregion

            basicHp.contentType = TMP_InputField.ContentType.IntegerNumber;
            basicPower.contentType = TMP_InputField.ContentType.IntegerNumber;

            #region 事件注册

            #region 自动禁用一些参数

            //触发类型为None（不触发能力时）禁用其他所有的内容
            abilityReasonType.onValueChanged.AddListener(delegate (int value)
            {
                //启用的话，不能随便启用
                //禁用的话，随便禁用
                    abilityReasonLargeScope.interactable = value != 0;
                // 0就是选择了None。禁用有关的参数输入
                    abilityReasonLargeScope.onValueChanged.Invoke(value == 0 ? 0:abilityReasonLargeScope.value);

               
            });
            //触发条件中，检索范围=None（不设定范围） 0 or any任何情况下都可以 1
            abilityReasonLargeScope.onValueChanged.AddListener(delegate (int value)
            {
                //启用的话，不能随便启用
                //禁用的话，随便禁用
                bool disable = value == 0 || value == 1;
                abilityReasonParameter.interactable = !disable;
                abilityReasonParameter.onValueChanged.Invoke(disable ? 0 : abilityReasonParameter.value);
                abilityReasonJudgeParameter.interactable = !disable;
                abilityReasonJudgeParameter.onValueChanged.Invoke(value == 0 ? 0 : abilityReasonJudgeParameter.value);
               
            });
            //当“范围参数”变化时，同步更新“范围阈值”的辅助下拉框内容
            abilityReasonParameter.onValueChanged.AddListener(delegate (int value)
            {
                abilityReasonLogic.interactable = value != 0;
                inputFieldHelperContent(abilityReasonThreshold, value);
            });
            //当“判定参数”变化时，同步更新“判定阈值”的辅助下拉框内容
            abilityReasonJudgeParameter.onValueChanged.AddListener(delegate (int value)
            {
               
                //这个为None时，能力不能发动，后续的触发效果也不能修改
                //但是，如果触发条件中，检索范围=any任何情况下都可以(1)时，允许激活
                //总而言之就是，如果检索范围时any的话，触发条件都禁用，触发效果正常填写

                    abilityReasonJudgeMethod.interactable = abilityReasonLargeScope.value == 1 ? false: value != 0;
                    abilityReasonJudgeLogic.interactable = abilityReasonLargeScope.value == 1 ? false : value != 0;
                    inputFieldHelperContent(abilityReasonJudgeThreshold, abilityReasonLargeScope.value == 1 ? 0 : value);
                    abilityResultLargeScope.interactable = abilityReasonLargeScope.value == 1 ? true : value != 0;

                //能进行能力效果的编辑吗
                abilityResultLargeScope.onValueChanged.Invoke(!abilityResultLargeScope.interactable ? 0 : abilityResultLargeScope.value);
            });           
            //当“修改参数”变化时，同步更新“值”的辅助下拉框内容
            abilityResultParameterToChange.onValueChanged.AddListener(delegate (int value)
            {
                abilityResultChangeMethod.interactable = value != 0;
                inputFieldHelperContent(abilityResultChangeValue, value);
            });
            //当“判定参数”变化时，同步更新“判定阈值”的辅助下拉框内容
            abilityResultParameter.onValueChanged.AddListener(delegate (int value)
            {
                abilityResultLogic.interactable = value != 0;
                inputFieldHelperContent(abilityResultThreshold, value);
            });

            //触发效果中，检索范围=None（不设定范围）
            abilityResultLargeScope.onValueChanged.AddListener(delegate (int value)
            {              

                switch (abilityResultLargeScope.value)
                {
                    //任何情况下都可以：等效于“将触发效果的卡牌作为对象卡牌”
                    //触发器同一效果
                    case 1 or (int)Information.Objects.Activator:
                        abilityResultParameter.interactable = false;
                        abilityResultLogic.interactable = false;
                        abilityResultThreshold.interactable = false;
                        abilityReasonObjectAsTarget.TurnOn();
                        break;

                    //除了any（任何情况下都可以）和触发器以外，其余的都正常处理禁用状态
                    default:
                        abilityResultParameter.interactable = value != 0;
                        abilityResultParameter.onValueChanged.Invoke(value == 0 ? 0 : abilityResultParameter.value);
                        abilityReasonObjectAsTarget.TurnOff();
                        break;
                }

                //二者公用的
                abilityResultSummon.interactable = value != 0;
                abilityResultRidicule.interactable = value != 0;
                abilityResultSilence.interactable = value != 0;
                abilityResultParameterToChange.interactable = value != 0;
                abilityResultParameterToChange.onValueChanged.Invoke(value == 0 ? 0 : abilityResultParameterToChange.value);


            });

            #region 暂时不要这个入口了
            /*
            //启用“将触发效果的卡牌作为对象卡牌”，则自动禁用一些东西（默认是off，所以没啥问题）
            abilityReasonObjectAsTarget.OnOn.AddListener(delegate ()
            {
                abilityResultLargeScope.interactable = false;
               abilityResultParameter.interactable = false;
                abilityResultLogic.interactable = false;
                abilityResultThreshold.interactable = false;
            });
            abilityReasonObjectAsTarget.OnOff.AddListener(delegate ()
            {
                //如果嘲讽那边可以输入，说明能力触发效果这边是可以用的                
                if (abilityResultRidicule.interactable)
                {
                    abilityResultLargeScope.interactable = true;

                    //还有个要求，就是触发效果的检索范围不是触发器和任何情况都可以
                    if (abilityResultLargeScope.value != 1 && abilityResultLargeScope.value != (int)Information.Objects.Activator)
                    {
                        abilityResultParameter.interactable = true;
                        abilityResultParameter.onValueChanged.Invoke(abilityResultParameter.value);
                        abilityResultLogic.interactable = abilityResultParameter.value != 0;
                    }
                
                }               
            });
            */
            #endregion

            #endregion
            //保存热键
            CardMaker.cardMaker.WantToSave.AddListener(UniTask.UnityAction(async () => { await SaveOrSaveTo(); }));

            //删除卡牌
            DeleteButton.onClick.AddListener(delegate
            {

                Notify.notify.CreateStrongNotification(null, null, "确认删除？",
                    $"此过程不可撤销。\n要删除“{friendlyNameField.text}”吗？", UniTask.UnityAction(
                        async () =>
                        {
                            CardMaker.cardMaker.BanInputLayer(true, "删除中...");
                            //先删除
                            Debug.Log(CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath);
                            File.Delete(Path.Combine("file://", CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath));

                            Notify.notify.CreateBannerNotification(null, "删除成功", 0.7f);

                            //关闭修改标记
                            CardMaker.cardMaker.changeSignal.SetActive(false);

                            //移出缓存的卡牌记录
                            CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName.Remove(nowEditingCard
                                .FriendlyCardName);
                            CardMaker.cardMaker.nowEditingBundle.allCardName.Remove(nowEditingCard.CardName);
                            CardMaker.cardMaker.nowEditingBundle.allCardUuid.Remove(nowEditingCard.UUID);

                          //然后切换
                          switchToBundleEditor.OnClick.Invoke();


                        }), "确认删除", delegate { }, "再想想");

            });

            //返回标题
            returnToTPanel.OnClick.AddListener(delegate
            {
                CardMaker.cardMaker.SwitchPanel(UniTask.Action(async () =>
                {
                    //检查保存（仅限在这个事件中）
                    await SaveOrSaveTo();
                }), delegate { CardMaker.cardMaker.ReturnToCreateEditPanel(); });
            });


            //切换到清单编辑器
            switchToBundleEditor.OnClick.AddListener(delegate
            {
                CardMaker.cardMaker.SwitchPanel(UniTask.Action(async () =>
                {
                    //检查保存（仅限在这个事件中）
                    await SaveOrSaveTo();
                }),
                    UniTask.Action(async () =>
                    {
                        //切换界面

                        //然后切换
                        await CardMaker.cardMaker.bundleEditor.OpenManifestEditor();
                        gameObject.SetActive(false);
                    }));
            });

            //开关音频编辑界面
            voiceEditButton.OnClick.AddListener(delegate { voiceEditor.SetActive(!voiceEditor.activeSelf); });

            //开关能力编辑界面
            abilityDescriptionButton.OnClick.AddListener(delegate { abilityDescriptionEditor.SetActive(!abilityDescriptionEditor.activeSelf); });

           

            //ability描述点击 清楚内容 按钮，成品预览那边同步更新
            abilityDescription.onValueChanged.AddListener(delegate (string arg0)
            {
                var text = arg0.Replace("<rb>", "<color=red><b>");
                text = text.Replace("<g>", "<#00FF25>");
                text = text.Replace("<bl>", "<#0158B7>");
                text = text.Replace("</rb>", "</color>");
                text = text.Replace("</g>", "</color>");
                text = text.Replace("</bl>", "</color>");
                preview.description.text = text;
            });

            //音频修改事件
            voiceAbility.OnPrepareToSelectAudio.AddListener(SelectAudio);
            voiceDebut.OnPrepareToSelectAudio.AddListener(SelectAudio);
            voiceExit.OnPrepareToSelectAudio.AddListener(SelectAudio);
            voiceDefeat.OnPrepareToSelectAudio.AddListener(SelectAudio);

            #endregion

            //音频英文标准名称设定
            voiceExit.VoiceName = nameof(voiceExit);
            voiceDefeat.VoiceName = nameof(voiceDefeat);
            voiceAbility.VoiceName = nameof(voiceAbility);
            voiceDebut.VoiceName = nameof(voiceDebut);
        }



        public async UniTask OpenCardEditor()
        {
            //如果没有加载了的manifest，则禁用“保存”功能
            SaveButtonGameObject.gameObject.SetActive(
                !string.IsNullOrEmpty(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath));

            CardMaker.cardMaker.BanInputLayer(true, "卡牌配置加载中...");

            //上层方法已经将CardMaker.cardMaker.nowEditingBundle.card附好值了（就是已经获得卡牌了）
            nowEditingCard = CardMaker.cardMaker.nowEditingBundle.card;

            if (nowEditingCard == null)
            {
                Notify.notify.CreateBannerNotification(null, "意外错误：没有创建卡牌实例，请重新创建或联系作者");
                throw new Exception("意外错误：没有创建卡牌实例，请重新创建或联系作者");
            }

            //如果已经设定（保存）过识别名称了，则不允许修改
            cardNameField.interactable = string.IsNullOrEmpty(nowEditingCard.CardName);



            //关闭能力和声音编辑器
            editor.SetActive(true);
            voiceEditor.SetActive(false);
            abilityEditor.SetActive(false);
            abilityDescriptionEditor.SetActive(false);


            //信息显示到editor里

            #region 常规的信息加载

            cardNameField.SetTextWithoutNotify(nowEditingCard.CardName);
            AuthorNameField.SetTextWithoutNotify(nowEditingCard.AuthorName);
            friendlyNameField.SetTextWithoutNotify(nowEditingCard.FriendlyCardName);
            AnimeField.inputField.SetTextWithoutNotify(nowEditingCard.Anime);
            CharacterNameField.inputField.SetTextWithoutNotify(nowEditingCard.CharacterName);
            CVField.inputField.SetTextWithoutNotify(nowEditingCard.CV);
            AsChiefToggle.Set(nowEditingCard.allowAsChief);
            cardNumberField.SetTextWithoutNotify(nowEditingCard.CardCount.ToString());
            genderField.SetValueWithoutNotify(nowEditingCard.gender);
            basicHp.SetTextWithoutNotify(nowEditingCard.BasicHealthPoint.ToString());
            basicPower.SetTextWithoutNotify(nowEditingCard.BasicPower.ToString());

            //设置为-1，便于激活onvalueChanged（value-1时）
            abilityReasonType.value = -1;
            abilityReasonType.value =((int)nowEditingCard.AbilityActivityType);
            abilityReasonLargeScope.value = -1;
            abilityReasonLargeScope.value = ((int)nowEditingCard.Reason.NeededObjects.LargeScope);
            abilityReasonParameter.value = -1;
            abilityReasonParameter.value =((int)nowEditingCard.Reason.NeededObjects.ParameterToShrinkScope);
            abilityReasonLogic.value = (nowEditingCard.Reason.NeededObjects.Logic+ 3);
            abilityReasonThreshold.inputField.text = (nowEditingCard.Reason.NeededObjects.Threshold);
            abilityReasonJudgeParameter.value = -1;
            abilityReasonJudgeParameter.value = ((int)nowEditingCard.Reason.JudgeParameter);      
            abilityReasonJudgeMethod.value = ((int)nowEditingCard.Reason.ReasonJudgeMethod);
            abilityReasonJudgeLogic.value = (nowEditingCard.Reason.Logic + 3);
            abilityReasonJudgeThreshold.inputField.text = (nowEditingCard.Reason.Threshold);
            abilityReasonObjectAsTarget.Set(nowEditingCard.Result.RegardActivatorAsResultObject);
            abilityResultLargeScope.value = -1;
            abilityResultLargeScope.value = ((int)nowEditingCard.Result.ResultObject.LargeScope);
            abilityResultParameter.value=((int)nowEditingCard.Result.ResultObject.ParameterToShrinkScope);
            abilityResultLogic.value=((int)nowEditingCard.Result.ResultObject.Logic + 3);
            abilityResultThreshold.inputField.text= (nowEditingCard.Result.ResultObject.Threshold);
            abilityResultParameterToChange.value = -1;
            abilityResultParameterToChange.value=((int)nowEditingCard.Result.ParameterToChange);
            abilityResultChangeMethod.value=((int)nowEditingCard.Result.ChangeMethod);
            abilityResultChangeValue.inputField.text=(nowEditingCard.Result.Value);
            //适用于旧版本兼容（其实是引入Uuid之后，这方面的逻辑忘了改了。。。）
            if(!Guid.TryParse(nowEditingCard.Result.SummonCardName, out Guid guid))
            {
                //如果不是一个合法的guid（那就是friendlyName了）。则自动修复

                //确确实实有这个卡牌的话，那就直接显示这个友好名称了
                if (CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName.Contains(nowEditingCard.FriendlyCardName))
                {
                    abilityResultSummon.inputField.text = nowEditingCard.FriendlyCardName;
                }
                //这个卡牌有都没有的话 ，就无视这个参数
                else
                {
                    abilityResultSummon.inputField.text = string.Empty;
                    Debug.LogError($"“{nowEditingCard.Result.SummonCardName}”既不是一个合法的uuid，也不是一个已有卡牌的友好名称");
                }
            }
            else
            {
                //如果是一个合法的guid。那就读取对应卡牌的友好名称
                int index = CardMaker.cardMaker.nowEditingBundle.allCardUuid.FindIndex(n => n.Contains(nowEditingCard.Result.SummonCardName));
                abilityResultSummon.inputField.text = CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName[index];
            }
            
            abilityResultSummon.ChangeOptionDatas(CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName);
            abilityResultRidicule.text = (nowEditingCard.Result.Ridicule.ToString());
            abilityResultSilence.text = (nowEditingCard.Result.Silence.ToString());
            abilityDescription.text = nowEditingCard.AbilityDescription;//这个必须通知onValueChanged，以便让预览更新
         
            //tag也同步一下
            //移出所有无用(残留）的tag对象
            var UnusedTags = tagParent.GetComponentsInChildren<tagListItem>(false);
            for (int i = 0; i < UnusedTags.Length; i++)
            {
                UnusedTags[i].button.onClick.Invoke();
            }

            tagStorage = nowEditingCard.tags;
            if (nowEditingCard.tags.Count > 0)
            {
                foreach (var tag in nowEditingCard.tags)
                {
                    addTagListItem(tag);
                }
            }


            //获取可变下拉列表内容
            RefreshVariableDropdownList();

            //图片，音频资源加载
            //已经有加载的卡牌了
            if (CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath != string.Empty)
            {
                var cardRootPath = Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath);

                //图片加载
                CardMaker.cardMaker.BanInputLayer(true, "图片资源加载中...");
                await AsyncLoadImage($"{cardRootPath}/{nowEditingCard.ImageName}");

                //音频加载
                CardMaker.cardMaker.BanInputLayer(true, "音频资源加载中...");
                //先清除音频
                voiceAbility.clear();
                voiceDebut.clear();
                voiceDefeat.clear();
                voiceExit.clear();
                //加载音频，没加载上的就保持clear状态了
                string audioPath = $"{cardRootPath}/{nowEditingCard.voiceAbilityFileName}";
                if (File.Exists(audioPath)) await AsyncLoadSelectedAudio(voiceAbility, audioPath);
                audioPath = $"{cardRootPath}/{nowEditingCard.voiceExitFileName}";
                if (File.Exists(audioPath)) await AsyncLoadSelectedAudio(voiceExit, audioPath);
                audioPath = $"{cardRootPath}/{nowEditingCard.voiceDebutFileName}";
                if (File.Exists(audioPath)) await AsyncLoadSelectedAudio(voiceDebut, audioPath);
                audioPath = $"{cardRootPath}/{nowEditingCard.voiceDefeatFileName}";
                if (File.Exists(audioPath)) await AsyncLoadSelectedAudio(voiceDefeat, audioPath);
            }
            //新建的卡牌，清理之前遗留的音频图片资源
            else
            {
                //先清除音频
                voiceAbility.clear();
                voiceDebut.clear();
                voiceDefeat.clear();
                voiceExit.clear();
                //然后是图片
                imageOfCardField.sprite = defaultImage;
                preview.image.sprite = defaultImage;
            }
            #endregion
;

            CardMaker.cardMaker.BanInputLayer(false, "卡牌加载中...");
            //启用编辑器，并初始化显示界面
            gameObject.SetActive(true);
;

            //同步一下信息
            OnValueChanged();
            CardMaker.cardMaker.changeSignal.SetActive(false);

        }

        #region 图片选择

        /// <summary>
        /// 玩家选择图片
        /// </summary>
        public void selectImageButton() => AsyncSelectImage();


        async UniTask AsyncSelectImage()
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("图片", Information.SupportedImageExtension));

            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: "选择卡牌图片", loadButtonText: "选择");

            if (FileBrowser.Success)
            {
                //加载图片文件
                await AsyncLoadImage(CardReadWrite.FixedLoadedPathDueToSAF(FileBrowser.Result[0]));
                //显示已修改的印记
                CardMaker.cardMaker.changeSignal.SetActive(true);
                //更新新的图片全路径
                newImageFullPath = CardReadWrite.FixedLoadedPathDueToSAF(FileBrowser.Result[0]);
            }
        }


        async UniTask AsyncLoadImage(string imageFullPath)
        {            
            //加载图片
            Texture2D image = await CardReadWrite.CoverImageLoader(imageFullPath);

            //加载到了
            if (image != null)
            {
                var sprite = Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), Vector2.one / 2);
                imageOfCardField.sprite = sprite;
                preview.image.sprite = sprite;
            }
            //因为各种原因加载不出来，用默认图片
            else
            {
;                imageOfCardField.sprite = defaultImage;
                preview.image.sprite = defaultImage;
            }
        }

        #endregion

        #region 音频选择

        /// <summary>
        /// 选择音频事件调用
        /// </summary>
        /// <param name="setting"></param>
        public void SelectAudio(audioSetting setting) => AsyncSelectAudio(setting);

        /// <summary>
        /// 弹出选择框进行选择
        /// </summary>
        /// <param name="audioSetting"></param>
        private async UniTask AsyncSelectAudio(audioSetting audioSetting)
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("卡牌音频", Information.SupportedAudioExtension));

            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: $"选择{audioSetting.title}",
                loadButtonText: "选择");

            if (FileBrowser.Success)
            {
                await AsyncLoadSelectedAudio(audioSetting, CardReadWrite.FixedLoadedPathDueToSAF(FileBrowser.Result[0]));
            }
        }

        /// <summary>
        /// 加载音频
        /// </summary>
        private async UniTask AsyncLoadSelectedAudio(audioSetting audioSetting, string audioFullPath)
        {
            var audioClip = await CardReadWrite.CardAudioLoader(audioFullPath);


            if (audioClip != null) audioSetting.AudioSelected(audioClip, audioFullPath);
        }

        #endregion
        
        
        //同步变化
        public void OnValueChanged()
        {
            CardMaker.cardMaker.changeSignal.SetActive(true);

            //预览中，除了能力外所有的信息进行同步
            preview.UpdateBasicInformation(friendlyNameField.text, CVField.text,abilityDescription.text,
                imageOfCardField.sprite);
        }

        public  void SaveButton()
        {
            SaveOrSaveTo();
        }

        private async UniTask SaveOrSaveTo()
        {
            if (!gameObject.activeSelf && FileBrowser.IsOpen)
            {
                return;
            }

            if(string.IsNullOrEmpty(cardNameField.text))
            {
                Notify.notify.CreateBannerNotification(null, $"在保存之前，你需要先设置识别名称");
                return;
            }

            //阻止新创建的卡牌与已有卡牌的识别名称相同（通过cardNameField.interactable鉴别这个是不是新创建的卡牌）
            if (CardMaker.cardMaker.nowEditingBundle.allCardName.Contains(cardNameField.text) && cardNameField.interactable)
            {
                Notify.notify.CreateBannerNotification(null,$"”{cardNameField.text}“已存在。可以换个名字");
                return;
            }
            
            //内存保存
            var editing = CardMaker.cardMaker.nowEditingBundle.card;
            editing.UUID = string.IsNullOrEmpty(editing.UUID) ? Guid.NewGuid().ToString() : editing.UUID;
            editing.CardName = cardNameField.text;
            editing.AuthorName = AuthorNameField.text;
            editing.gender = genderField.value;
            editing.FriendlyCardName = friendlyNameField.text;
            editing.Anime = AnimeField.text;
            editing.tags = tagStorage;
            editing.CardCount = int.Parse(cardNumberField.text);           
            editing.CharacterName = CharacterNameField.text;
            editing.CV = CVField.text;
            editing.allowAsChief = AsChiefToggle.On;
            editing.BasicPower = int.Parse(basicPower.text);
            editing.BasicHealthPoint = int.Parse(basicHp.text);
            editing.AbilityDescription = abilityDescription.text;
            editing.AbilityActivityType = (Information.CardAbilityTypes)abilityReasonType.value;
            editing.Reason.NeededObjects.LargeScope = (Information.Objects)abilityReasonLargeScope.value;
            editing.Reason.NeededObjects.ParameterToShrinkScope = (Information.Parameter)abilityReasonParameter.value;
            editing.Reason.NeededObjects.Logic = abilityReasonLogic.value - 3;
            editing.Reason.NeededObjects.Threshold = abilityReasonThreshold.text;
            editing.Reason.Logic = abilityReasonJudgeLogic.value - 3;
            editing.Reason.JudgeParameter = (Information.Parameter)abilityReasonJudgeParameter.value;
            editing.Reason.ReasonJudgeMethod = (Information.JudgeMethod)abilityReasonJudgeMethod.value;
            editing.Reason.Threshold = abilityReasonJudgeThreshold.text;
            editing.Result.RegardActivatorAsResultObject = abilityReasonObjectAsTarget.On;
            int index = CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName.FindIndex(a => a.Contains(abilityResultSummon.text));
            if(index >= 0) editing.Result.SummonCardName = CardMaker.cardMaker.nowEditingBundle.allCardUuid[index];
            //你这输入的卡牌友好名称不存在啊
            else
            {
                Notify.notify.CreateBannerNotification(null,$"此卡组中不存在友好名称为“{abilityResultSummon.text}”的卡牌");
                return;
            }
            editing.Result.Ridicule = int.Parse(abilityResultRidicule.text);
            editing.Result.Silence = int.Parse(abilityResultSilence.text);
            editing.Result.ResultObject.LargeScope = (Information.Objects)abilityResultLargeScope.value;
            editing.Result.ResultObject.ParameterToShrinkScope = (Information.Parameter)abilityResultParameter.value;
            editing.Result.ResultObject.Logic = abilityResultLogic.value - 3;
            editing.Result.ResultObject.Threshold = abilityResultThreshold.text;
            editing.Result.ParameterToChange = (Information.Parameter)abilityResultParameterToChange.value;
            editing.Result.ChangeMethod = (Information.CalculationMethod)abilityResultChangeMethod.value;
            editing.Result.Value = abilityResultChangeValue.text;
        
            //封面 string.IsNullOrEmpty(newImageFullPath) = true 没有选择新图片
            editing.ImageName = string.IsNullOrEmpty(newImageFullPath)
               ? editing.ImageName
               : $"{Information.DefaultCoverNameWithoutExtension}{Path.GetExtension(Path.GetFileName(newImageFullPath))}";
         
            //音频
            var audios = new audioSetting[4];
            audios[0] = voiceAbility;
            audios[1] = voiceDebut;
            audios[2] = voiceDefeat;
            audios[3] = voiceExit;
            //音频文件名保存在内存中
            editing.voiceExitFileName = $"{audios[3].VoiceName}{Path.GetExtension(audios[3].newAudioFullFileName)}";
            editing.voiceDefeatFileName = $"{audios[2].VoiceName}{Path.GetExtension(audios[2].newAudioFullFileName)}";
            editing.voiceDebutFileName = $"{audios[1].VoiceName}{Path.GetExtension(audios[1].newAudioFullFileName)}";
            editing.voiceAbilityFileName = $"{audios[0].VoiceName}{Path.GetExtension(audios[0].newAudioFullFileName)}";

            //禁用识别名称输入
            cardNameField.interactable = false;

            //卡组清单文件存在（保存）
            if (File.Exists(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath))
            {
                //如果卡牌文件不存在，说明是新建的卡牌，以前还没有
                if (!File.Exists($"{Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath)}/cards/{nowEditingCard.CardName}/{Information.CardFileName}"))
                {
                    //注册此卡牌，使其能在manifest编辑器那边的切换器中显示出来
                    var card = CardMaker.cardMaker.nowEditingBundle.card;
                    CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName.Add(card.FriendlyCardName);
                    CardMaker.cardMaker.nowEditingBundle.allCardName.Add(card.CardName);
                    CardMaker.cardMaker.nowEditingBundle.allCardUuid.Add(card.UUID);
                }              

                var saveFullPath =
                   $"{Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath)}/cards/{cardNameField.text}/{Information.CardFileName}";
                await CardMaker.cardMaker.AsyncSave(null, null, FileBrowserHelpers.GetDirectoryName(saveFullPath),
                    newImageFullPath, false, true,
                    audios);

                Debug.Log(
                    $"此卡牌“{CardMaker.cardMaker.nowEditingBundle.card.FriendlyCardName}”属于卡组“{CardMaker.cardMaker.nowEditingBundle.manifest.FriendlyBundleName}”，已自动保存到该卡组中");
            }
            else
            {
                Notify.notify.CreateBannerNotification(null,"卡组不存在");
            }
        }

        /// <summary>
        /// 刷新可变下拉列表内容（AnimeList CV CharacterName tags）
        /// </summary>
        /// <param name="reReadFromDisk">重新从硬盘读一遍吗？</param>
        public void RefreshVariableDropdownList()
        {
            AnimeField.ChangeOptionDatas(Information.AnimeList);
            CVField.ChangeOptionDatas(Information.CV);
            CharacterNameField.ChangeOptionDatas(Information.characterNamesList);
            tagField.ChangeOptionDatas(Information.tags);

            //有分类标记的额外处理一下（处理分类标记和内容，加入禁用列表，合并同类项）
            IncludeClassificationNote.format(CharacterNameField);
            IncludeClassificationNote.format(tagField);
        }

     

        /// <summary>
        /// 当“判定参数”变化时，同步更新“可变下拉栏”的下拉框内容
        /// </summary>
        /// <param name="inputFieldWithDropdown"></param>
        /// <param name="index">所选参数在dropdown内的值</param>
        private void inputFieldHelperContent(InputFieldWithDropdown inputFieldWithDropdown, int index)
        {               
            inputFieldWithDropdown.supportFilter = true;
            inputFieldWithDropdown.interactable = true;

            switch (index)
            {

                //Anime
                case (int)Information.Parameter.Anime:
                    inputFieldWithDropdown.ChangeOptionDatas(AnimeField.options, true);
                    inputFieldWithDropdown.ban = AnimeField.ban;
                    inputFieldWithDropdown.contentType = TMP_InputField.ContentType.Standard;
                    break;

                //tag
                case (int)Information.Parameter.Tag:
                    inputFieldWithDropdown.ChangeOptionDatas(tagField.options, true);
                    inputFieldWithDropdown.ban = tagField.ban;
                    inputFieldWithDropdown.contentType = TMP_InputField.ContentType.Standard;
                    break;

           
                //cv
                case (int)Information.Parameter.CV:
                    inputFieldWithDropdown.ChangeOptionDatas(CVField.options);
                    inputFieldWithDropdown.ban = CVField.ban;
                    inputFieldWithDropdown.contentType = TMP_InputField.ContentType.Standard;
                    break;

                //characterName
                case (int)Information.Parameter.CharacterName:
                    inputFieldWithDropdown.ChangeOptionDatas(CharacterNameField.options);
                    inputFieldWithDropdown.ban = CharacterNameField.ban;
                    inputFieldWithDropdown.contentType = TMP_InputField.ContentType.Standard;
                    break;

                case (int)Information.Parameter.Gender:
                    inputFieldWithDropdown.ChangeOptionDatas(genderField.options);
                    inputFieldWithDropdown.ban = new ();
                    inputFieldWithDropdown.contentType = TMP_InputField.ContentType.Standard;
                    break;
                
                case (int) Information.Parameter.Team:
                    var content = new List<TMP_Dropdown.OptionData>
                    {
                        new TMP_Dropdown.OptionData("从属玩家社团（队伍）"),
                        new TMP_Dropdown.OptionData("从属电脑社团（队伍）"),
                        new TMP_Dropdown.OptionData("改变从属社团（队伍）")
                    };
                    inputFieldWithDropdown.ChangeOptionDatas(content);
                    inputFieldWithDropdown.ban = new ();
                    inputFieldWithDropdown.contentType = TMP_InputField.ContentType.Standard;
                    break;

                case (int)Information.Parameter.Coin or (int)Information.Parameter.HealthPoint or (int)Information.Parameter.Power:
                    inputFieldWithDropdown.Ban();
                    inputFieldWithDropdown.contentType = TMP_InputField.ContentType.IntegerNumber;
                    break;


                    //其他的，不允许输入（因为有特供的输入框给他用了）
                default:
                    inputFieldWithDropdown.interactable =false;
                    break;
            }
        }


        #region 有分类标记的处理方法
/// <summary>
/// 有分类标记的处理方法
/// </summary>
        private class IncludeClassificationNote
        {
            /// <summary>
            /// 格式化
            /// </summary>
            /// <param name="inputFieldWithDropdown"></param>
            public static void format(InputFieldWithDropdown inputFieldWithDropdown)
            {
                for (int i = 0; i < inputFieldWithDropdown.options.Count; i++)
                {
                    //是分类标记的，粗体，禁用选择
                    if (inputFieldWithDropdown.options[i].text.Substring(0, 1) == "%")
                    {
                        var newText = $"<b>{inputFieldWithDropdown.options[i].text[1..]}</b>";
                        inputFieldWithDropdown.EditOptionText(i, newText);
                        //记录分类标记的序号，禁止玩家输入
                        inputFieldWithDropdown.ban.Add(newText);
                    }
                    //不是的，稍许淡化，缩进一点，允许选择
                    else
                    {
                        var newText = $"<margin=5%><alpha=#CC>{inputFieldWithDropdown.options[i].text}";
                        inputFieldWithDropdown.EditOptionText(i, newText);
                    }
                }
            }
        }

        #endregion

        #region 标签编辑那边的方法

        public void TagAddition()
        {
            if (tagStorage.Contains(tagField.text))
            {
                Notify.notify.CreateBannerNotification(null, $"标签“{tagField.text}”已存在");
                return;
            }

            if (tagField.text.Contains(" "))
            {
                Notify.notify.CreateBannerNotification(null, $"标签“{tagField.text}”不能包含空格");
                return;
            }

            if (tagField.text == "")
            {
                Notify.notify.CreateBannerNotification(null, $"标签不能为空");
                return;
            }

            //储存增加一个
            tagStorage.Add(tagField.text);
            //显示出来
            addTagListItem(tagField.text);
            //清空输入框内容
            tagField.text = "";
            //修改标记显示
            CardMaker.cardMaker.changeSignal.SetActive(true);
        }


        /// <summary>
        /// 在列表内添加一个
        /// </summary>
        /// <param name="text"></param>
        private void addTagListItem(string text)
        {
            var item = Instantiate(tagListButtonPerfebs.gameObject, tagParent).GetComponent<tagListItem>();
            //显示出来tag文本
            item.Initialization(text);
            //注册删除tag功能
            item.onRemove.AddListener(delegate(string arg0) { tagStorage.Remove(arg0); });
        }

        #endregion
    }
}