using Core;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// 图形化显示卡牌的信息与动画效果，并执行有关逻辑
/// </summary>
public class CardPanel : MonoBehaviour//接口可以以后实现玩家自定义行为（写代码）
{
    /// <summary>
    /// 缓存
    /// </summary>
   public CardCache cardCache
    {
        get => GameState.cardCaches[cacheId];
        private set { GameState.cardCaches[cacheId] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public CharacterCard Profile
    {
        get { return cardCache.Profile; }
        set { cardCache.Profile = value; }
    }

    /// <summary>
    /// 卡牌缓存中的id
    /// </summary>
    public int cacheId { get; set; }

    /// <summary>
    /// 是哪一个玩家的可用牌 0=A 1=B
    /// </summary>
    public int TeamId { get; set; }
    /// <summary>
    /// 这一组内第几个卡牌（从0开始）
    /// </summary>
   public int CardId { get; set; }
    /// <summary>
    /// 沉默回合数  新卡为0
    /// </summary>
     public int Silence { 
        get; 
        set;
    }
    /// <summary>
    /// 嘲讽回合数 新卡为0
    /// </summary>
     public int Ridicule {
        get; 
        set; 
    }
    
    /// <summary>
    /// 实际攻击力（各种影响攻击力的都对这个参数修改）
    /// </summary>
    public int ActualPower
    {
        get => Profile.BasicPower; 
        set {
            //火力提升了，通知一下
            if (value > ActualPower)
            {
                cardInformation.Show($"Power\n+ {value - ActualPower}", false);
            }
            //火力被降低了
            else if (value < ActualPower)
            {
                cardInformation.Show($"Power\n- {ActualPower - value}", true);
            }

            //参数修改
            Profile.BasicPower = value;
            powerValue.text = value.ToString();
        }
    }
    
    /// <summary>
    /// 实际生命值（各种影响攻击力的都对这个参数修改）
    /// </summary>
    public int ActualHealthPoint
    {
        get => Profile.BasicHealthPoint; 
       private set {
            Profile.BasicHealthPoint = value;
            hpValue.text = value.ToString();
                }
    }
    /// <summary>
    /// 这一轮游戏这个卡牌已经干过活了
    /// </summary>
    public bool ThisRoundHasActiviated { get; set; }

    //之后的话，需要给资源建立一个缓存池，省的卡牌上场的时候卡顿



    [Header("通用")]
    public SpriteRenderer image;

    [Header("信息模式")]
    public TMP_Text cardName;
    public TMP_Text cv;
    public TMP_Text description;    
    public GameObject[] infModeToDestroy;

    /// <summary>
    /// 所用卡牌在游戏中的状态
    /// </summary>
    [Header("游戏模式")]
    public Transform tr;
    public TMP_Text powerValue;
    public TMP_Text hpValue;
    public CardPanelInformation cardInformation;
    public GameObject[] gameModeToDestroy;


    #region 信息填充与恢复
    /// <summary>
    /// 回复卡牌至最初始的状态
    /// </summary>
    public void RecoverCard()
    {
        //各种数据的恢复

        Silence = 0;
        Ridicule = 0;
        ThisRoundHasActiviated = false;
    }


    /// <summary>
    /// 信息展示用（即不是游戏模式）
    /// </summary>
    /// <param name="cardName"></param>
    /// <param name="cv"></param>
    /// <param name="description"></param>
    /// <param name="image"></param>
    public void UpdateBasicInformation(string cardName, string cv, string description, Sprite image)
    {
        this.cardName.text = cardName ?? throw new ArgumentNullException(nameof(cardName));
        this.cv.text = cv == "不设置声优" ? string.Empty : $"cv:{cv}";
        this.description.text = description ?? throw new ArgumentNullException(nameof(description));
        this.image.sprite = image;
        if (powerValue != null)
        {
            foreach (var item in infModeToDestroy)
            {
                DestroyImmediate(item);
            }
        }      
    }

    /// <summary>
    /// 进入到游戏模式，从里文件CharacterInGame中读取，并展示出来
    /// </summary>
    /// <param name="cardState"></param>
    public void EnterGameMode()
    {
        //设置上图片
        image.sprite = cardCache.CoverImage == null ? image.sprite : cardCache.CoverImage;
        image.sortingOrder = 0;//层级调整
        //初始化体力值与行动力
        ActualPower = Profile.BasicPower;
        ActualHealthPoint = Profile.BasicHealthPoint;
        powerValue.text = ActualPower.ToString();
        powerValue.gameObject.SetActive(true);
        hpValue.text = ActualHealthPoint.ToString();
        hpValue.gameObject.SetActive(true);

        //销毁信息显示用的东西（这些东西游戏模式用不到）
        if (cardName.gameObject != null)
        {
            foreach (var item in gameModeToDestroy)
            {
                DestroyImmediate(item);
            }
        }


        // cardName.gameObject.SetActive(false);
        //cv.gameObject.SetActive(false);
        // description.gameObject.SetActive(false);

        //修复莫名其妙的图片变大的（显示不完全）的bug
        image.size = Vector2.one * 0.6228073f;

        //将图片放到panel中间
        image.transform.localPosition = Vector3.zero;
    }

    #endregion

    #region 游戏状态


    public void GetDamaged(int damage, CardPanel activator) => ChangeHealth(ActualHealthPoint - damage,  activator);

    public void ChangeTeam(int targetTeamId)
    {

    }

    /// <summary>
    /// 修改血量和攻击力
    /// </summary>
    /// <param name="changeHealth">要修改生命值吗</param>
    /// <param name="value1">火力最终值</param>
    /// <param name="changePower">要修改攻击力吗</param>
    /// <param name="value2">生命最终值</param>
    /// <param name="Activator">是谁触发了这个函数</param>
    public void ChangeHealth(int value1, CardPanel Activator)
    {
           //是挨打          
          if(ActualHealthPoint > value1)
            {
                cardInformation.Show($"HP\n- {ActualHealthPoint - value1}", true);
                //修改参数
                ActualHealthPoint = value1;
                //告知自己挨打了
                OnHurt(Activator);
              
            }
          //是回血
            else  if(ActualHealthPoint < value1)
            {
                cardInformation.Show($"HP\n+ {value1 - ActualHealthPoint}", false);
                //修改参数
                ActualHealthPoint = value1;
            }      
    }


    public async UniTask OnDebut()
    {
        if (Profile.AbilityActivityType == Information.CardAbilityTypes.Debut)
        {
            await UniTask.Delay(200);
            await  AbilityReasonAnalyze(null,"from OnDebut");
            await UniTask.Delay(300);
        }

    }

    public async UniTask Attack(CardPanel target)
    {
        //实战能力
        if (Profile.AbilityActivityType == Information.CardAbilityTypes.Round)
        {
            //分析一下该做什么，顺便触发能力
            await AbilityReasonAnalyze(null,"from Attack");

            await UniTask.Delay(300);

        }

        if(ActualPower <= 0)
        {
            GameStageCtrl.stageCtrl.ShowAbilityNews($"{Profile.FriendlyCardName}(tid:{TeamId},id{CardId})", null, "因为执行力（攻击力）=0，无法攻击");
            await UniTask.Delay(400);
            return;
        }


        //记录原位置
        var originalPos = tr.position;
        //记录挨打一方的位置
        var attackPoint = target.tr.position;

        //靠近要攻击目标卡牌
        while (true)
        {
            tr.position = Vector2.Lerp(tr.position, attackPoint, 0.04f);
            //足够近，停止循环
            if (Math.Abs(tr.position.x - attackPoint.x) <= 0.1f && Math.Abs(tr.position.y - attackPoint.y) <= 0.1f)
            {
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        //施暴
        target.GetDamaged(ActualPower, this);

        await UniTask.Delay(250);

        //回到原地点
        while (true)
        {
            tr.position = Vector2.Lerp(tr.position, originalPos, 0.06f);
            //足够近，停止循环（尽量能靠近）
            if (Math.Abs(tr.position.x - originalPos.x) <= 0.1f && Math.Abs(tr.position.y - originalPos.y) <= 0.1f)
            {
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        await UniTask.Delay(400);
    }

    public async UniTask Exit(CardPanel activator)
    {
        if(Profile.AbilityActivityType == Information.CardAbilityTypes.Exit)
        {
            await AbilityReasonAnalyze(activator);
        }

        GameStageCtrl.stageCtrl.RecycleCardOnSpot(TeamId, CardId);
    }

    public async UniTask OnHurt(CardPanel activator)
    {
        //能力设定为挨打发动，并且得有血
        if (Profile.AbilityActivityType == Information.CardAbilityTypes.GetHurt && ActualHealthPoint > 0)
        {           
            //分析一下该做什么，顺便触发能力
          await AbilityReasonAnalyze(activator,"from OnHurt");
            await UniTask.Delay(100);
        }

        if (ActualHealthPoint <= 0) await Exit(activator);
    }

    #endregion


    #region 能力解析相关

    async UniTask Summon()
    {

        var ar = await GameStageCtrl.stageCtrl.DisplayCardFromCache(Profile.Result.SummonCardName, TeamId, CardId + 1);
        ShowNews(null, $"召唤了一张卡牌“{ar.Profile.FriendlyCardName}”");

        await UniTask.Delay(250);

    }

    /// <summary>
    /// 展示能力的一些新信息（格式弄好了）
    /// </summary>
    void ShowNews(string Recepetors, string DoWhat) => GameStageCtrl.stageCtrl.ShowAbilityNews(Profile.FriendlyCardName, Recepetors, DoWhat);

    /// <summary>
    /// 能力触发原因判定
    /// </summary>
    async UniTask AbilityReasonAnalyze(CardPanel activator,string debugInf = null)
    {
#if UNITY_EDITOR
        Debug.Log(debugInf);
#endif

        //等20ms
        await UniTask.Delay(20);

        //确定条件对象们
        CardPanel[] ReasonObjects = null; //确定范围内的条件对象
        Chief chief = null; //储存主持/部长的条件对象

        #region 确定条件对象们

        //如果是any情况下都能运行，直接运行结果逻辑
        if (Profile.Reason.NeededObjects.LargeScope == Information.Objects.Any)
        {
            Profile.Result.RegardActivatorAsResultObject = false;
            AbilityResultAnalyze();
            return;
        }
        else
        {
            //确定条件对象们（条件对象可以是角色卡牌，也可以是部长卡牌）
            ReasonObjects = GetNeededCards(Profile.Reason.NeededObjects, activator); //确定范围内的条件对象
            chief = GetNeededChief(Profile.Reason.NeededObjects); //储存主持/部长的条件对象

        }

        #endregion

        //以下为二次判定，可以做多条件判断的那种（从判断参数开始）

        //一开始就没选到卡牌的话，就算了
        if (chief != null || ReasonObjects != null)
        {       

            //判断的参数
            string[] parameterValues; //获取要判断的参数的值

            //如果是对部长进行判定，那么只需要长度为1的数组就行
            if (chief != null)
            {
                parameterValues = new string[1];
            }
            //反之将数组长度拓展为条件对象的数量
            else
            {
                parameterValues = new string[ReasonObjects.Length];
            }

            #region 获取判断的参数的值

            switch (Profile.Reason.JudgeParameter)
            {
                //如果“判定参数”选择“不涉及参数”，那此能力不会发动
                case Information.Parameter.None:
                  return;

                //部长/主席/主持的金币数量
                case Information.Parameter.Coin:
                    if (chief != null) parameterValues[0] = chief.coin.ToString();
                    else
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})想要判断部长金币数，但是能力原因的条件对象不是chief");
                    break;

                //角色卡的攻击力
                case Information.Parameter.Power:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].ActualPower.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})想要判断角色卡攻击力，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡是否被静默（沉默回合数）
                case Information.Parameter.Silence:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].Silence.ToString(); //0 1 2...
                        }
                    }
                    else
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})想要判断角色卡沉默回合数，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡的嘲讽回合数
                case Information.Parameter.Ridicule:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].Ridicule.ToString(); //0 1 2...
                        }
                    }
                    else
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})想要判断角色卡炒粉回合数，但是能力原因的条件对象不是角色卡");

                    break;


                //角色卡性别
                case Information.Parameter.Gender:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].Profile.gender.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})想要判断角色卡性别，但是能力原因的条件对象不是角色卡");
                    break;

                //所属社团（队伍）
                case Information.Parameter.Team:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].TeamId.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})想要判断角色卡性别，但是能力原因的条件对象不是角色卡");
                    break;

                //tag
                case Information.Parameter.Tag:
                    if (chief == null)
                    {

                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            foreach (var tag in ReasonObjects[i].Profile.tags)
                            {
                                //合并每个卡牌的tag
                                parameterValues[i] = $"{parameterValues[i]}，{tag}";
                            }
                        }
                    }
                    else
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})想要判断角色卡标签，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡&部长的角色名字（固定的）
                case Information.Parameter.CharacterName:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].Profile.CharacterName;
                        }
                    }
                    else
                    {
                        parameterValues[0] = chief.CharacterName;
                    }

                    break;

                case Information.Parameter.CV:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].Profile.CV;
                        }
                    }
                    else
                    {
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})想要判断部长{chief.ChiefName}的声优，但是这是禁止事项");
                    }

                    break;
            }

#if UNITY_EDITOR
            Debug.Log($"判定参数：{parameterValues.Length}");
#endif
            #endregion


            //根据判断方法，准备数值计算（储存参数数据亦或是储存参数数量）
            string[] values = new string[0];

        #region 根据判断方法，准备数值计算

        switch (Profile.Reason.ReasonJudgeMethod)
        {
            //count：有几个参数？
            case Information.JudgeMethod.Count:
                values = new string[1];
                values[0] = parameterValues.Length.ToString();
                break;

            //如果是value，直接把参数的值作为判断的值
            case Information.JudgeMethod.Value:
                values = parameterValues;
                break;
        }

        #endregion


        //运用判断逻辑，对阈值进行判定
        //只要存在一个符合要求的，就说明能力能发动
        bool AllowAbilityExection = false;

        #region 运用判断逻辑，对阈值进行判定

        //count（计数）判定：
        if (Profile.Reason.ReasonJudgeMethod == Information.JudgeMethod.Count)
        {
            //满足要求的参数的长度
            var parameterValuesLength = int.Parse(values[0]);
            int thresholdInt = int.Parse(Profile.Reason.Threshold);

            switch (Profile.Reason.Logic)
            {
                //不等于（不包含）
                case -3:
                    AllowAbilityExection = parameterValuesLength != thresholdInt;
                    break;

                //小于
                case -2:
                    AllowAbilityExection = parameterValuesLength < thresholdInt;
                    break;
                //小于等于
                case -1:
                    AllowAbilityExection = parameterValuesLength <= thresholdInt;
                    break;
                //等于
                case 0:
                    AllowAbilityExection = parameterValuesLength == thresholdInt;
                    break;

                //大于等于
                case 1:
                    AllowAbilityExection = parameterValuesLength >= thresholdInt;
                    break;

                //大于
                case 2:
                    AllowAbilityExection = parameterValuesLength > thresholdInt;
                    break;
            }
        }
        //values判定：
        else
        {
            //这些都是对values（数值进行判定）
            switch (Profile.Reason.JudgeParameter)
            {
                //数据为Int
                case Information.Parameter.Coin or Information.Parameter.Power or Information.Parameter.Silence
                    or Information.Parameter.HealthPoint or Information.Parameter.Gender or Information.Parameter.Team:
                    //将string转换为正规的类型（int）
                    int[] fixedValues = new int[values.Length];
                    int thresholdInt = int.Parse(Profile.Reason.Threshold);

                    //将记录的values转换成Int，并进行有关的判断逻辑处理
                    for (int i = 0; i < values.Length; i++)
                    {
                        //将记录的values转换成Int
                        if (!int.TryParse(values[i], out fixedValues[i]))
                        {
                            throw new Exception(
                                $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})的能力出发原因中，{Profile.Reason.JudgeParameter}是int的，但是给定的阈值形式上不符合int类型");
                        }
                        else
                        {
                            //按照记录的逻辑方式判断能否
                            switch (Profile.Reason.Logic)
                            {
                                //-3 不包含（不等于）
                                case -3:
                                    AllowAbilityExection = fixedValues[i] != thresholdInt;
                                    break;

                                //-2 小于
                                case -2:
                                    AllowAbilityExection = fixedValues[i] < thresholdInt;
                                    break;

                                //小于等于
                                case -1:
                                    AllowAbilityExection = fixedValues[i] <= thresholdInt;
                                    break;

                                //等于（包含）
                                case 0:
                                    AllowAbilityExection = fixedValues[i] == thresholdInt;
                                    break;

                                //大于等于
                                case 1:
                                    AllowAbilityExection = fixedValues[i] >= thresholdInt;
                                    break;

                                //大于
                                case 2:
                                    AllowAbilityExection = fixedValues[i] > thresholdInt;
                                    break;
                            }
                        }


                        //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                        if (!Profile.Result.RegardActivatorAsResultObject || Profile.Result.SummonCardName != String.Empty)
                        {
                            if (AllowAbilityExection) break;
                        }
                    }

                    break;

                //其他定性的（不含tag）
                case Information.Parameter.CharacterName or Information.Parameter.CV:
                    //看看记录的values（包含参数的数据值本身）有没有满足要求
                    for (int i = 0; i < values.Length; i++)
                    {
                        //按照记录的逻辑方式判断能否
                        switch (Profile.Reason.Logic)
                        {
                            //-3 不等于（不包含）
                            case -3:
                                AllowAbilityExection = values[i] != Profile.Reason.Threshold;
                                break;

                            //等于（包含）
                            case 0:
                                AllowAbilityExection = values[i] == Profile.Reason.Threshold;
                                break;
                        }


                        //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                        if (!Profile.Result.RegardActivatorAsResultObject || Profile.Result.SummonCardName != String.Empty)
                        {
                            if (AllowAbilityExection) break;
                        }
                    }

                    break;

                //每一个角色卡记录的tags:，SOS，coward
                case Information.Parameter.Tag:

                    //看看记录的values（包含参数的数据值本身）有没有满足要求
                    for (int i = 0; i < values.Length; i++)
                    {
                        //得到每一个对象的所有tag（0不能要）
                        string[] allTags = values[i].Split('，');


                        //按照记录的逻辑方式判断能否
                        switch (Profile.Reason.Logic)
                        {
                            //-3 不包含
                            case -3:
                                AllowAbilityExection = !allTags.Contains(Profile.Reason.Threshold);
                                break;

                            //等于（包含）
                            case 0:
                                AllowAbilityExection = allTags.Contains(Profile.Reason.Threshold);
                                break;
                        }


                        //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                        if (!Profile.Result.RegardActivatorAsResultObject || Profile.Result.SummonCardName != String.Empty)
                        {
                            if (AllowAbilityExection) break;
                        }
                    }

                    break;
            }
        }

        #endregion


        //有符合要求的条件对象，就执行能力的结果逻辑
        if (AllowAbilityExection)
        {
#if UNITY_EDITOR
                Debug.Log("满足要求，能力发动");
#endif
                AbilityResultAnalyze(ReasonObjects);
        }

        }
    }


    /// <summary>
    /// 能力结果解析
    /// </summary>
    /// <param name="checkedKixedValuesState">储存每个条件对象对于此参数要求是否满足 （true就符合条件）</param>
    /// <param name="reasonObjects">得到那些条件对象</param>
    async void AbilityResultAnalyze(CardPanel[] reasonObjects = null)
    {
     if(reasonObjects != null) ShowNews(null, $"得到了{reasonObjects.Length}个符合要求的卡牌");

        //能力发动到谁身上？
        Chief chiefToOperate = null;
        CardPanel[] characterToOperate = null;

        //召唤
        if (!string.IsNullOrEmpty(Profile.Result.SummonCardName))
        { 
           await Summon();

            //如果要召唤，那就直接不把激活能力的条件对象作为结果对象
            Profile.Result.RegardActivatorAsResultObject = false;
        }




        #region 获取能力发动的对象 能力发动到谁身上？

        //如果把激活能力的条件对象作为结果对象，才查找对象
        if (Profile.Result.RegardActivatorAsResultObject)
        {
            characterToOperate = reasonObjects;
        }
        //如果不把激活能力的条件对象作为结果对象，则重新寻找一次，确定修改参数的范围
        else
        {
            chiefToOperate = GetNeededChief(Profile.Result.ResultObject);
            characterToOperate = GetNeededCards(Profile.Result.ResultObject);
        }

        #endregion

        #region 修改参数

        if (chiefToOperate != null)
        {
            //对目标角色卡的参数进行修改
            foreach (var card in characterToOperate)
            {
                //额外的沉默
                card.Silence += Convert.ToInt32(Profile.Result.Silence);
                ShowNews(card.Profile.FriendlyCardName, $"的沉默回合数变为了{card.Silence}");

                //额外的嘲讽
                card.Ridicule += Convert.ToInt32(Profile.Result.Ridicule);
                ShowNews(card.Profile.FriendlyCardName, $"的嘲讽回合数变为了{card.Ridicule}");


                switch (Profile.Result.ParameterToChange)
                {

                    case Information.Parameter.Coin:
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})无法修改Coin参数，因为他的能力指向的结果对象不是CharacterCard，而是chief");

                    case Information.Parameter.HealthPoint:
                        card.ChangeHealth(ChangeIntValue(card.ActualHealthPoint), this);
                        ShowNews(card.Profile.FriendlyCardName, $"的体力值（HP）变为了{card.ActualHealthPoint}");

                        break;
                    case Information.Parameter.Power:
                        card.ActualPower = ChangeIntValue(card.ActualPower);
                        ShowNews(card.Profile.FriendlyCardName, $"的执行力（攻击力）变为了{card.ActualPower}");
                        break;

                    case Information.Parameter.Gender:
                        card.Profile.gender = ChangeIntValue(card.Profile.gender);


                        var gender = card.Profile.gender switch
                        {
                            1 => "男",
                            2 => "女",
                            _ => "未知或不重要",
                        };
                        ShowNews(card.Profile.FriendlyCardName, $"的性别变为了{gender}");
                        break;

                    //修改所属的team。0=玩家方 1=电脑方 2=双方交换
                    case Information.Parameter.Team:
                        //ChangeIntValue里要求必须为changeTo
                        if (Profile.Result.ChangeMethod != Information.CalculationMethod.ChangeTo)
                        {
                            Debug.LogError($"{Profile.FriendlyCardName}的能力想要修改卡牌的所属社团（队伍），但是他没有将“修改方法”设置为“ChangeTo”");
                        }
                        else
                        {
                            //实现2：双方互换
                            if (Profile.Result.Value == 2.ToString())
                            {
                                card.ChangeTeam(card.TeamId == 0 ? 1 : 0);
                            }
                            else
                            {
                                card.ChangeTeam(Convert.ToInt32(Profile.Result.Value));
                            }


                            //改完之后，才设置上消息通知
                            var team = card.CardId switch
                            {
                                0 => "玩家社团",
                                1 => "电脑社团",

                            };

                            ShowNews(card.Profile.FriendlyCardName, $"的所属社团变为了{team}");
                        }


                        break;

                    case Information.Parameter.Silence:
                        card.Silence = ChangeIntValue(card.Silence);
                        ShowNews(card.Profile.FriendlyCardName, $"的沉默回合数变为了{card.Silence}");
                        break;

                    case Information.Parameter.Ridicule:
                        card.Ridicule = ChangeIntValue(card.Ridicule);
                        ShowNews(card.Profile.FriendlyCardName, $"的嘲讽回合数变为了{card.Ridicule}");
                        break;

                    case Information.Parameter.Tag:
                        switch (Profile.Result.ChangeMethod)
                        {
                            //添加/删除一个tag  values种，如果有个“-”。说明是减去这个tag
                            case Information.CalculationMethod.addition:
                                //开头没有-号，加上一个tag
                                if (Profile.Result.Value.Substring(0, 1) != "-" && !card.Profile.tags.Contains(Profile.Result.Value))
                                {
                                    card.Profile.tags.Add(Profile.Result.Value);
                                    ShowNews(card.Profile.FriendlyCardName, $"的标签添加了{Profile.Result.Value}");
                                }
                                //开头有-号，减去一个tag
                                else if (Profile.Result.Value.Substring(0, 1) == "-" && card.Profile.tags.Contains(Profile.Result.Value))
                                {
                                    card.Profile.tags.Remove(Profile.Result.Value);
                                    ShowNews(card.Profile.FriendlyCardName, $"的标签删除了{Profile.Result.Value}");
                                }
                                break;

                            default:
                                throw new Exception($"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})对标签的修改方法不正确。只能使用addition");
                        }
                        break;
                }
            }
        }

        if (chiefToOperate != null)
        {
            //主持
            switch (Profile.Result.ParameterToChange)
            {
                case Information.Parameter.Coin:
                    chiefToOperate.coin = ChangeIntValue(chiefToOperate.coin);
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    /// 修改int类型的值，并返回最终值（涉及到 修改方法）
    /// </summary>
    /// <param name="parameter">被修改的参数</param>
    /// <param name="values">（</param>
    private int ChangeIntValue(int parameter)
    {
        switch (Profile.Result.ChangeMethod)
        {
            case Information.CalculationMethod.addition:
                parameter += int.Parse(Profile.Result.Value);
                break;

            case Information.CalculationMethod.ChangeTo:
                parameter = int.Parse(Profile.Result.Value);
                break;

            case Information.CalculationMethod.multiplication:
                parameter = (int)(parameter * float.Parse(Profile.Result.Value));
                break;
        }

        return parameter;
    }

    /// <summary>
    /// 按照对象要求，获取需要的卡牌（不适用于chief和Any）
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="objectsScope">对象查找范围</param>
    /// <param name="activator">触发器，用于处理objectsScope=Activator时的对象</param>
    /// <returns></returns>
    private CardPanel[] GetNeededCards(NeededObjects objectsScope, CardPanel activator = null)
    {
        //需要的卡牌对象
        CardPanel[] neededCards = null;

        if (objectsScope.LargeScope == Information.Objects.Any || objectsScope.LargeScope == Information.Objects.None)
        {
            return neededCards;
        }

            #region 根据大范围筛选

            Random rd = new();
        switch (objectsScope.LargeScope)
        {
            //条件对象是：触发此能力的卡牌
            case Information.Objects.Activator:
                //如果是受击是触发能力，则把activator（攻击者）作为条件对象
                //其他情况下，把自己作为条件对象

                neededCards = new CardPanel[1];
                neededCards[0] = activator;
                break;

            //己方上一位卡牌
            case Information.Objects.Last:
                //只有一张卡，不执行
                if (GameState.CardOnSpot[TeamId].Count == 1)
                {
                    neededCards = null;
                    return null;
                }

                neededCards = new CardPanel[1];
                neededCards[0] =
                    GameState.CardOnSpot[TeamId][GameState.whichCardPerforming[TeamId] == 1 ? 6 : -1];
                break;
            //己方下一位卡牌
            case Information.Objects.Next:
                //只有一张卡，不执行
                if (GameState.CardOnSpot[TeamId].Count == 1)
                {
                    neededCards = null;
                    return null;
                }

                neededCards = new CardPanel[1];
                neededCards[0] =
                    GameState.CardOnSpot[TeamId][GameState.whichCardPerforming[TeamId] == 6 ? 1 : +1];
                break;

            case Information.Objects.Self:
                neededCards = new CardPanel[1];
                neededCards[0] = this;
                break;

            //己方场上所有的角色卡牌
            case Information.Objects.AllInTeam:
                neededCards = CommonTools.ListArrayConversion(GameState.CardOnSpot[TeamId]);
                break;

            //敌方场上所有的角色卡牌
            case Information.Objects.AllOfEnemy:
                neededCards = CommonTools.ListArrayConversion(GameState.CardOnSpot[TeamId == 1 ? 0 : 1]);
                break;

            //场上所有角色卡牌
            case Information.Objects.AllOnSpot:
               
                neededCards = new CardPanel[GameState.CardOnSpot[0].Count + GameState.CardOnSpot[1].Count];
                for (int i = 0; i < neededCards.Length; i++)
                {
                    if (i < GameState.CardOnSpot[0].Count)
                    {
                        neededCards[i] = GameState.CardOnSpot[0][i];
                    }
                    else
                    {
                        neededCards[i] = GameState.CardOnSpot[1][i - GameState.CardOnSpot[0].Count];
                    }
                }

                break;

            // 己方场上随机一位角色
            case Information.Objects.RandomInTeam:

                neededCards = new CardPanel[1];
                neededCards[0] =
                    GameState.CardOnSpot[TeamId][rd.Next(1, GameState.CardOnSpot[TeamId].Count + 1)];
                break;

            // 地方方场上随机一位角色
            case Information.Objects.RandomOfEnemy:
                neededCards = new CardPanel[1];
                neededCards[0] =
                    GameState.CardOnSpot[TeamId == 1 ? 0 : 1][
                        rd.Next(1, GameState.CardOnSpot[TeamId].Count + 1)];
                break;
        }

        #endregion

#if UNITY_EDITOR
   Debug.Log($"检索范围：{neededCards.Length}");
#endif

        #region 根据参数进行范围缩小

        //None时，直接把需要的对象定义为大范围内的所有对象，不进行后续处理
        if (objectsScope.ParameterToShrinkScope == Information.Parameter.None)
        {

#if UNITY_EDITOR
            Debug.Log($"确定范围：{neededCards.Length}");
#endif
            return neededCards;
        }

        //缓存一下，储存符合条件要求的卡牌对象
        List<CardPanel> cache = new List<CardPanel>();

        for (int i = 0; i < neededCards.Length; i++)
        {
            //neededCards[i]的参数，要进行判断的参数
            string parameter = null;

            //获取参数值
            switch (objectsScope.ParameterToShrinkScope)
            {
                case Information.Parameter.CharacterName:
                    parameter = neededCards[i].Profile.CharacterName.ToString();
                    break;

                case Information.Parameter.CV:
                    parameter = neededCards[i].Profile.CV.ToString();
                    break;

                case Information.Parameter.HealthPoint:
                    parameter = neededCards[i].ActualHealthPoint.ToString();
                    break;

                case Information.Parameter.Power:
                    parameter = neededCards[i].ActualPower.ToString();
                    break;

                case Information.Parameter.Silence:
                    parameter = neededCards[i].Silence.ToString();
                    break;

                case Information.Parameter.Ridicule:
                    parameter = neededCards[i].Ridicule.ToString();
                    break;
          

                case Information.Parameter.Anime:
                    parameter = neededCards[i].Profile.Anime;
                    break;

                case Information.Parameter.Gender:
                    parameter = neededCards[i].Profile.gender.ToString();
                    break;

                case Information.Parameter.Team:
                    parameter = neededCards[i].TeamId.ToString();
                    break;

                case Information.Parameter.Tag:
                    foreach (var tag in neededCards[i].Profile.tags)
                    {
                        //用这种格式合并所有的tag
                        parameter =
                            $"{parameter}，{tag}";
                    }

                    break;
            }


            //是否是符合要求的对象
            bool allowed = false;

            //与阈值对比，判断此对象是否符合要求
            switch (objectsScope.ParameterToShrinkScope)
            {
                //数据为Int
                case Information.Parameter.Coin or Information.Parameter.Power or Information.Parameter.Silence
                    or Information.Parameter.HealthPoint or Information.Parameter.Ridicule or Information.Parameter.Gender or Information.Parameter.Team:
                    //将string转换为正规的类型（int）
                    //得到的，最终进行对比的值
                    int fixedValue;
                    int thresholdInt = int.Parse(Profile.Reason.NeededObjects.Threshold);

                    //将记录的values转换成Int，并进行有关的判断逻辑处理
                    //将记录的values转换成Int
                    if (!int.TryParse(parameter, out fixedValue))
                    {
                        throw new Exception(
                            $"{Profile.FriendlyCardName}(内部名称：{Profile.CardName})无法找到正确的对象，因为所找对象的参数是int型，但是给定的阈值形式上不符合int类型");
                    }
                    else
                    {
                        //按照记录的逻辑方式判断能否
                        allowed = Profile.Reason.NeededObjects.Logic switch
                        {
                            //-3 不包含（不等于）
                            -3 => fixedValue != thresholdInt,
                            //-2 小于
                            -2 => fixedValue < thresholdInt,
                            //小于等于
                            -1 => fixedValue <= thresholdInt,
                            //等于（包含）
                            0 => fixedValue == thresholdInt,
                            //大于等于
                            1 => fixedValue >= thresholdInt,
                            //大于
                            2 => fixedValue > thresholdInt,
                            _ => throw new NotImplementedException(),
                        };
                    }

                    break;

                //其他定性的（不含tag）
                //这里面的parameter就记载了要判断的参数
                case Information.Parameter.CharacterName or Information.Parameter.CV:

                    //按照记录的逻辑方式判断能否
                    switch (Profile.Reason.NeededObjects.Logic)
                    {
                        //-3 不等于（不包含）
                        case -3:
                            allowed = parameter != Profile.Reason.NeededObjects.Threshold;
                            break;

                        //等于（包含）
                        case 0:
                            allowed = parameter == Profile.Reason.NeededObjects.Threshold;
                            break;
                    }
                    break;

                //每一个角色卡记录的Profile.tags:=SOS=coward
                case Information.Parameter.Tag:

                    //得到每一个对象的所有tag
                    string[] allTags = parameter.Split('，');


                    //按照记录的逻辑方式判断能否
                    switch (Profile.Reason.NeededObjects.Logic)
                    {
                        //-3 不包含
                        case -3:
                            allowed = !allTags.Contains(Profile.Reason.NeededObjects.Threshold);
                            break;

                        //等于（包含）
                        case 0:
                            allowed = allTags.Contains(Profile.Reason.NeededObjects.Threshold);
                            break;
                    }

                    break;
            }

            //如果符合要求，加入到之前的缓存中
            if (allowed) cache.Add(neededCards[i]);

        }

        //将缓存应用到返回值中，作为最终结果
        neededCards = CommonTools.ListArrayConversion(cache);

        #endregion

#if UNITY_EDITOR
        Debug.Log($"确定范围：{neededCards.Length}");
#endif

        return neededCards;
    }

    /// <summary>
    /// 按照对象要求，获取需要的卡牌（仅适用于chief）
    /// </summary>
    /// <param name="objectsScope">哪一方的chief</param>
    /// <returns></returns>
    private Chief GetNeededChief(NeededObjects objectsScope)
    {
        switch (objectsScope.LargeScope)
        {
            //对家主持/主席/部长
            case Information.Objects.ChiefOfEnemy:
                return GameState.chiefs[TeamId == 0 ? 1 : 0];

            //自家主持/主席/部长
            case Information.Objects.OurChief:

                return GameState.chiefs[TeamId];
        }

        return null;
    }


    #endregion


}
