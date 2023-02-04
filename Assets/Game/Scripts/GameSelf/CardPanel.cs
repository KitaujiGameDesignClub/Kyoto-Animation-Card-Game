using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Core;
using System;
using System.Linq;
using Core.Interface;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework.Tools;
using Random = System.Random;
using System.Threading.Tasks;

/// <summary>
/// 图形化显示卡牌的信息与动画效果，并执行有关逻辑
/// </summary>
public class CardPanel : MonoBehaviour, ICharacterCardInGame//接口可以以后实现玩家自定义行为（写代码）
{
    
    [Header("信息模式")]
    public TMP_Text cardName;
    public TMP_Text cv;
    public TMP_Text description;
    [Header("通用")]
    public SpriteRenderer image;
    /// <summary>
    /// 所用卡牌在游戏中的状态
    /// </summary>
    [Header("游戏模式")]
    public GameObject[] othersToDestroy;
    public CharacterInGame cardStateInGame;
    public Transform tr;
    public TMP_Text powerValue;
    public TMP_Text hpValue;



    /// <summary>
    /// 信息展示用（即不是游戏模式）
    /// </summary>
    /// <param name="cardName"></param>
    /// <param name="cv"></param>
    /// <param name="description"></param>
    /// <param name="image"></param>
    public void UpdateBasicInformation(string cardName,string cv, string description,Sprite image)
    {
        this.cardName.text = cardName ?? throw new ArgumentNullException(nameof(cardName));
        this.cv.text = cv == "不设置声优" ? string.Empty : $"cv:{cv}";
        this.description.text = description ?? throw new ArgumentNullException(nameof(description));
        this.image.sprite = image;
        Destroy(powerValue.gameObject);
        Destroy(hpValue.gameObject);
    }

    /// <summary>
    /// 进入到游戏模式，从里文件CharacterInGame中读取，并展示出来
    /// </summary>
    /// <param name="cardState"></param>
    public void EnterGameMode()
    {
        //设置上图片
        image.sprite = cardStateInGame.CoverImage == null ? image.sprite : cardStateInGame.CoverImage;
        image.sortingOrder = 0;//层级调整
        //初始化体力值与行动力
        powerValue.text = cardStateInGame.actualPower.ToString();
        powerValue.gameObject.SetActive(true);
        hpValue.text = cardStateInGame.actualHealthPoint.ToString();
        hpValue.gameObject.SetActive(true);
      
        //销毁信息显示用的东西（这些东西游戏模式用不到）
        if(cardName.gameObject != null)
        {
            DestroyImmediate(cardName.gameObject);
            DestroyImmediate(cv.gameObject);
            DestroyImmediate(description.gameObject);

            foreach (var item in othersToDestroy)
            {
                if (item != null) DestroyImmediate(item);
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

  
    public void GetDamaged(int damage, CardPanel activator) => ChangeHealthAndPower(true, damage, false, -1, activator);


    public void PowerUp(int value, CardPanel activator)
    {
       
    }

    public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2, CardPanel Activator)
    {

        //更新数据
       if(changePower) cardStateInGame.actualPower += value2;
        if (changeHealth)
        {
            cardStateInGame.actualHealthPoint -= value1;
            //告知自己挨打了
            OnHurt(Activator);
        }
        //更新显示
        powerValue.text = cardStateInGame.actualPower.ToString();
        hpValue.text = cardStateInGame.actualHealthPoint.ToString();
    }

    public void ChangeState(Information.CardState cardState)
    {
        ((ICharacterCardInGame)cardStateInGame).ChangeState(cardState);
    }

    public void OnDebut()
    {
        ((ICharacterCardInGame)cardStateInGame).OnDebut();
    }

    public async UniTask Attack(CardPanel target)
    {
        await attackAnimation(target);
    }

    private async UniTask attackAnimation(CardPanel target)
    {
        //记录原位置
        var originalPos = tr.position;
        Debug.Log(originalPos);
        //记录挨打一方的位置
        var attackPoint = target.tr.position;
        Debug.Log(attackPoint);

        //靠近要攻击目标卡牌
        while (true)
        {
            tr.position = Vector2.Lerp(tr.position, attackPoint, 0.1f);
            //足够近，停止循环
            if(Math.Abs(tr.position.x - attackPoint.x) <= 0.1f)
            {
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        //施暴
        target.GetDamaged(cardStateInGame.actualPower,this);
        Debug.Log("施暴");

        //等100ms
        await UniTask.Delay(100);
        Debug.Log("打完了");

        //回到原地点
        while (true)
        {
            tr.position = Vector2.Lerp(tr.position, originalPos, 0.1f);
            //足够近，停止循环（尽量能靠近）
            if (Math.Abs(tr.position.x - originalPos.x) <= 0.01f)
            {
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        Debug.Log("H回来了");
    }

    public void Exit()
    {
        ((ICharacterCardInGame)cardStateInGame).Exit();
    }

    public void OnHurt(CardPanel activator)
    {
        if (cardStateInGame.actualHealthPoint <= 0) GameStageCtrl.stageCtrl.RecycleCardOnSpot(cardStateInGame.teamId, cardStateInGame.cardId);
    }

    
        #region 能力解析相关
        /// <summary>
        /// 能力触发原因判定
        /// </summary>
        void AbilityReasonAnalyze(CardPanel activator)
        {
            //确定条件对象们
            CardPanel[] ReasonObjects = null; //确定范围内的条件对象
            Chief chief = null; //储存主持/部长的条件对象

            #region 确定条件对象们

            //如果是any情况下都能运行，直接运行结果逻辑
            if (cardStateInGame.profile.Reason.NeededObjects.LargeScope == Information.Objects.Any)
            {
                cardStateInGame.profile.Result.RegardActivatorAsResultObject = false;
                AbilityResultAnalyze();
                return;
            }
            else
            {
                //确定条件对象们（条件对象可以是角色卡牌，也可以是部长卡牌）
                ReasonObjects = GetNeededCards(cardStateInGame.profile.Reason.NeededObjects, activator); //确定范围内的条件对象
                chief = GetNeededChief(cardStateInGame.profile.Reason.NeededObjects); //储存主持/部长的条件对象
            }

            #endregion

            //以下为二次判定，可以做多条件判断的那种

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

            switch (cardStateInGame.profile.Reason.JudgeParameter)
            {
                //部长/主席/主持的金币数量
                case Information.Parameter.Coin:
                    if (chief != null) parameterValues[0] = chief.coin.ToString();
                    else
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要判断部长金币数，但是能力原因的条件对象不是chief");
                    break;

                //角色卡的攻击力
                case Information.Parameter.Power:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].cardStateInGame.actualPower.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要判断角色卡攻击力，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡是否被静默（沉默回合数）
                case Information.Parameter.Silence:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].cardStateInGame.silence.ToString(); //0 1 2...
                        }
                    }
                    else
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要判断角色卡沉默回合数，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡的嘲讽回合数
                case Information.Parameter.Ridicule:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].cardStateInGame.ridicule.ToString(); //0 1 2...
                        }
                    }
                    else
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要判断角色卡炒粉回合数，但是能力原因的条件对象不是角色卡");

                    break;


                //角色卡状态
                case Information.Parameter.State:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].cardStateInGame.State.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要判断角色卡状态，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡性别
                case Information.Parameter.Gender:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].cardStateInGame.profile.gender.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要判断角色卡性别，但是能力原因的条件对象不是角色卡");
                    break;

                //tag对比
                case Information.Parameter.Tag:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            foreach (var tag in ReasonObjects[i].cardStateInGame.profile.tags)
                            {
                                parameterValues[i] =
                                    $"{parameterValues[i]}={tag}"; //最终的效果就是，每一个角色卡记录的tags:=SOS=coward，即每个tag间都有个=连接，第一个标签前有一个=
                            }
                        }
                    }
                    else
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要判断角色卡标签，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡&部长的角色名字（固定的）
                case Information.Parameter.CharacterName:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].cardStateInGame.profile.CharacterName;
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
                            parameterValues[i] = ReasonObjects[i].cardStateInGame.profile.CV;
                        }
                    }
                    else
                    {
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要判断部长{chief.ChiefName}的声优，但是这是禁止事项");
                    }

                    break;
            }

            #endregion


            //根据判断方法，准备数值计算（储存参数数据亦或是储存参数数量）
            string[] values = new string[0];

            #region 根据判断方法，准备数值计算

            switch (cardStateInGame.profile.Reason.ReasonJudgeMethod)
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
            //只要存在一个符合要求的，就说明
            bool AllowAbilityExection = false;
            //储存每个条件对象对于此参数要求是否满足 （true就符合条件）
            var CheckedFixedValuesState = new bool[values.Length];

            #region 运用判断逻辑，对阈值进行判定

            //count（计数）判定：
            if (cardStateInGame.profile.Reason.ReasonJudgeMethod == Information.JudgeMethod.Count)
            {
                //满足要求的参数的长度
                var parameterValuesLength = int.Parse(values[0]);
                int thresholdInt = int.Parse(cardStateInGame.profile.Reason.Threshold);

                switch (cardStateInGame.profile.Reason.Logic)
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
                switch (cardStateInGame.profile.Reason.JudgeParameter)
                {
                    //数据为Int
                    case Information.Parameter.Coin or Information.Parameter.Power or Information.Parameter.Silence
                        or Information.Parameter.HealthPoint or Information.Parameter.Gender:
                        //将string转换为正规的类型（int）
                        int[] fixedValues = new int[values.Length];
                        int thresholdInt = int.Parse(cardStateInGame.profile.Reason.Threshold);

                        //将记录的values转换成Int，并进行有关的判断逻辑处理
                        for (int i = 0; i < values.Length; i++)
                        {
                            //将记录的values转换成Int
                            if (!int.TryParse(values[i], out fixedValues[i]))
                            {
                                throw new Exception(
                                    $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})的能力出发原因中，{cardStateInGame.profile.Reason.JudgeParameter}是int的，但是给定的阈值形式上不符合int类型");
                            }
                            else
                            {
                                //按照记录的逻辑方式判断能否
                                switch (cardStateInGame.profile.Reason.Logic)
                                {
                                    //-3 不包含（不等于）
                                    case -3:
                                        CheckedFixedValuesState[i] = fixedValues[i] != thresholdInt;
                                        break;

                                    //-2 小于
                                    case -2:
                                        CheckedFixedValuesState[i] = fixedValues[i] < thresholdInt;
                                        break;

                                    //小于等于
                                    case -1:
                                        CheckedFixedValuesState[i] = fixedValues[i] <= thresholdInt;
                                        break;

                                    //等于（包含）
                                    case 0:
                                        CheckedFixedValuesState[i] = fixedValues[i] == thresholdInt;
                                        break;

                                    //大于等于
                                    case 1:
                                        CheckedFixedValuesState[i] = fixedValues[i] >= thresholdInt;
                                        break;

                                    //大于
                                    case 2:
                                        CheckedFixedValuesState[i] = fixedValues[i] > thresholdInt;
                                        break;
                                }
                            }

                            //如果有参数符合要求，则记录以下，说明能力能正常被触发了
                            if (CheckedFixedValuesState[i]) AllowAbilityExection = true;


                            //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                            if (!cardStateInGame.profile.Result.RegardActivatorAsResultObject || cardStateInGame.profile.Result.SummonCardName != String.Empty)
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
                            switch (cardStateInGame.profile.Reason.Logic)
                            {
                                //-3 不等于（不包含）
                                case -3:
                                    CheckedFixedValuesState[i] = values[i] != cardStateInGame.profile.Reason.Threshold;
                                    break;

                                //等于（包含）
                                case 0:
                                    CheckedFixedValuesState[i] = values[i] == cardStateInGame.profile.Reason.Threshold;
                                    break;
                            }


                            //如果有触发对象的参数符合要求，则记录以下，说明能力能正常被触发了
                            if (CheckedFixedValuesState[i]) AllowAbilityExection = true;


                            //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                            if (!cardStateInGame.profile.Result.RegardActivatorAsResultObject || cardStateInGame.profile.Result.SummonCardName != String.Empty)
                            {
                                if (AllowAbilityExection) break;
                            }
                        }

                        break;

                    //每一个角色卡记录的tags:=SOS=coward
                    case Information.Parameter.Tag:

                        //看看记录的values（包含参数的数据值本身）有没有满足要求
                        for (int i = 0; i < values.Length; i++)
                        {
                            //得到每一个对象的所有tag（0不能要）
                            string[] allTags = values[i].Split('=');


                            //按照记录的逻辑方式判断能否
                            switch (cardStateInGame.profile.Reason.Logic)
                            {
                                //-3 不包含
                                case -3:
                                    CheckedFixedValuesState[i] = !allTags.Contains(cardStateInGame.profile.Reason.Threshold);
                                    break;

                                //等于（包含）
                                case 0:
                                    CheckedFixedValuesState[i] = allTags.Contains(cardStateInGame.profile.Reason.Threshold);
                                    break;
                            }


                            //如果有触发对象的参数符合要求，则记录以下，说明能力能正常被触发了
                            if (CheckedFixedValuesState[i]) AllowAbilityExection = true;


                            //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                            if (!cardStateInGame.profile.Result.RegardActivatorAsResultObject || cardStateInGame.profile.Result.SummonCardName != String.Empty)
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
                AbilityResultAnalyze(ReasonObjects);
            }
        }


        /// <summary>
        /// 能力结果解析
        /// </summary>
        /// <param name="checkedKixedValuesState">储存每个条件对象对于此参数要求是否满足 （true就符合条件）</param>
        /// <param name="reasonObjects">得到那些条件对象</param>
         void AbilityResultAnalyze(CardPanel[] reasonObjects = null)
        {
            //能力发动到谁身上？
            Chief chiefToOperate = null;
            CardPanel[] characterToOperate = null;

            //召唤
            if (cardStateInGame.profile.Result.SummonCardName != String.Empty)
            {
                
                
              //11451444444

            }


            #region 获取能力发动的对象 能力发动到谁身上？

            //如果要召唤，那就直接不把激活能力的条件对象作为结果对象
            if (cardStateInGame.profile.Result.SummonCardName != string.Empty)
            {
                cardStateInGame.profile.Result.RegardActivatorAsResultObject = false;
            }

            //如果把激活能力的条件对象作为结果对象，才查找对象
            if (cardStateInGame.profile.Result.RegardActivatorAsResultObject)
            {
                characterToOperate = reasonObjects;
            }
            //如果不把激活能力的条件对象作为结果对象，则重新寻找一次
            else
            {
                chiefToOperate = GetNeededChief(cardStateInGame.profile.Result.ResultObject);
                characterToOperate = GetNeededCards(cardStateInGame.profile.Result.ResultObject);
            }

            #endregion

            #region 修改参数

            //角色卡
            foreach (var card in characterToOperate)
            {
                switch (cardStateInGame.profile.Result.ParameterToChange)
                {
                    case Information.Parameter.Coin:
                        throw new Exception(
                            $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})无法修改Coin参数，因为他的能力指向的结果对象不是CharacterCard，而是chief");

                    case Information.Parameter.HealthPoint:
                        card.cardStateInGame.actualHealthPoint = ChangeIntValue(card.cardStateInGame.actualHealthPoint);
                        break;
                    case Information.Parameter.Power:
                        card.cardStateInGame.actualPower = ChangeIntValue(card.cardStateInGame.actualPower);
                        break;

                    case Information.Parameter.Gender:
                        card.cardStateInGame.profile.gender = ChangeIntValue(card.cardStateInGame.profile.gender);
                        break;

                    case Information.Parameter.Silence:
                        card.cardStateInGame.silence = ChangeIntValue(card.cardStateInGame.silence);
                        break;

                    case Information.Parameter.Ridicule:
                        card.cardStateInGame.ridicule = ChangeIntValue(card.cardStateInGame.ridicule);
                        break;

                    case Information.Parameter.Tag:
                        switch (cardStateInGame.profile.Result.ChangeMethod)
                        {
                            //添加/删除一个tag  values种，如果有个“-”。说明是减去这个tag
                            case Information.CalculationMethod.addition:
                                //开头没有-号，加上一个tag
                                if (cardStateInGame.profile.Result.Value.Substring(0, 1) != "-" && !card.cardStateInGame.profile.tags.Contains(cardStateInGame.profile.Result.Value))
                                {
                                    card.cardStateInGame.profile.tags.Add(cardStateInGame.profile.Result.Value);
                                }
                                //开头有-号，减去一个tag
                                else if (cardStateInGame.profile.Result.Value.Substring(0, 1) == "-" && card.cardStateInGame.profile.tags.Contains(cardStateInGame.profile.Result.Value))
                                {
                                    card.cardStateInGame.profile.tags.Remove(cardStateInGame.profile.Result.Value);
                                }
                                break;

                            default:
                                throw new Exception($"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})想要用乘法修改tag");
                        }
                        break;

                    case Information.Parameter.State:
                        card.cardStateInGame.State = (Information.CardState)Enum.Parse(typeof(Information.CardState), cardStateInGame.profile.Result.Value);
                        break;

                }
            }

            //主持
            switch (cardStateInGame.profile.Result.ParameterToChange)
            {
                case Information.Parameter.Coin:
                    chiefToOperate.coin = ChangeIntValue(chiefToOperate.coin);
                    break;
            }
            #endregion
        }

        /// <summary>
        /// 修改int类型的值，并返回最终值
        /// </summary>
        /// <param name="parameter">被修改的参数</param>
        /// <param name="values">（</param>
        private int ChangeIntValue(int parameter)
        {
            switch (cardStateInGame.profile.Result.ChangeMethod)
            {
                case Information.CalculationMethod.addition:
                    parameter += int.Parse(cardStateInGame.profile.Result.Value);
                    break;

                case Information.CalculationMethod.ChangeTo:
                    parameter = int.Parse(cardStateInGame.profile.Result.Value);
                    break;

                case Information.CalculationMethod.multiplication:
                    parameter = (int)(parameter * float.Parse(cardStateInGame.profile.Result.Value));
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

            #region 根据大范围筛选

            Random rd = new Random();
            switch (objectsScope.LargeScope)
            {
                case Information.Objects.None:
                    break;


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
                    if (GameState.CardOnSpot[cardStateInGame.teamId].Count == 1)
                    {
                        neededCards = null;
                        return null;
                    }

                    neededCards = new CardPanel[1];
                    neededCards[0] =
                        GameState.CardOnSpot[cardStateInGame.teamId][GameState.whichCardPerforming[cardStateInGame.teamId] == 1 ? 6 : -1];
                    break;
                //己方下一位卡牌
                case Information.Objects.Next:
                    //只有一张卡，不执行
                    if (GameState.CardOnSpot[cardStateInGame.teamId].Count == 1)
                    {
                        neededCards = null;
                        return null;
                    }

                    neededCards = new CardPanel[1];
                    neededCards[0] =
                        GameState.CardOnSpot[cardStateInGame.teamId][GameState.whichCardPerforming[cardStateInGame.teamId] == 6 ? 1 : +1];
                    break;

                case Information.Objects.Self:
                    neededCards = new CardPanel[1];
                    neededCards[0] = this;
                    break;

                //己方场上所有的角色卡牌
                case Information.Objects.AllInTeam:
                    neededCards = CommonTools.ListArrayConversion(GameState.CardOnSpot[cardStateInGame.teamId]);
                    break;

                //敌方场上所有的角色卡牌
                case Information.Objects.AllOfEnemy:
                    neededCards = CommonTools.ListArrayConversion(GameState.CardOnSpot[cardStateInGame.teamId == 1 ? 0 : 1]);
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
                        GameState.CardOnSpot[cardStateInGame.teamId][rd.Next(1, GameState.CardOnSpot[cardStateInGame.teamId].Count + 1)];
                    break;

                // 地方方场上随机一位角色
                case Information.Objects.RandomOfEnemy:
                    neededCards = new CardPanel[1];
                    neededCards[0] =
                        GameState.CardOnSpot[cardStateInGame.teamId == 1 ? 0 : 1][
                            rd.Next(1, GameState.CardOnSpot[cardStateInGame.teamId].Count + 1)];
                    break;
            }

            #endregion


            #region 根据参数进行范围缩小

            //None时，直接把需要的对象定义为大范围内的所有对象，不进行后续处理
            if (objectsScope.ParameterToShrinkScope == Information.Parameter.None)
            {
                return neededCards;
            }

            //缓存一下，储存符合条件要求的卡牌对象
            List<CardPanel> cache = new List<CardPanel>();

            for (int i = 0; i < neededCards.Length; i++)
            {
                //neededCards[i]的参数
                string parameter = null;

                //获取参数值
                switch (objectsScope.ParameterToShrinkScope)
                {
                    case Information.Parameter.CharacterName:
                        parameter = neededCards[i].cardStateInGame.profile.CharacterName.ToString();
                        break;

                    case Information.Parameter.CV:
                        parameter = neededCards[i].cardStateInGame.profile.CV.ToString();
                        break;

                    case Information.Parameter.HealthPoint:
                        parameter = neededCards[i].cardStateInGame.actualHealthPoint.ToString();
                        break;

                    case Information.Parameter.Power:
                        parameter = neededCards[i].cardStateInGame.actualPower.ToString();
                        break;

                    case Information.Parameter.Silence:
                        parameter = neededCards[i].cardStateInGame.silence.ToString();
                        break;

                    case Information.Parameter.Ridicule:
                        parameter = neededCards[i].cardStateInGame.ridicule.ToString();
                        break;

                    case Information.Parameter.State:
                        parameter = neededCards[i].cardStateInGame.State.ToString();
                        break;

                    case Information.Parameter.Anime:
                        parameter = neededCards[i].cardStateInGame.profile.Anime;
                        break;

                    case Information.Parameter.Gender:
                        parameter = neededCards[i].cardStateInGame.profile.gender.ToString();
                        break;

                    case Information.Parameter.Tag:
                        foreach (var tag in cardStateInGame.profile.tags)
                        {
                            parameter =
                                $"{parameter}={tag}"; //最终的效果就是，每一个角色卡记录的cardStateInGame.profile.tags:=SOS=coward，即每个tag间都有个=连接，第一个标签前有一个=
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
                        or Information.Parameter.HealthPoint or Information.Parameter.Ridicule or Information.Parameter.Gender:
                        //将string转换为正规的类型（int）
                        int fixedValue;
                        int thresholdInt = int.Parse(cardStateInGame.profile.Reason.Threshold);

                        //将记录的values转换成Int，并进行有关的判断逻辑处理
                        //将记录的values转换成Int
                        if (!int.TryParse(parameter, out fixedValue))
                        {
                            throw new Exception(
                                $"{cardStateInGame.profile.FriendlyCardName}(内部名称：{cardStateInGame.profile.CardName})无法找到正确的对象，因为所找对象的参数是int型，但是给定的阈值形式上不符合int类型");
                        }
                        else
                        {
                            //按照记录的逻辑方式判断能否
                            allowed = cardStateInGame.profile.Reason.Logic switch
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
                    case Information.Parameter.CharacterName or Information.Parameter.CV:

                        //按照记录的逻辑方式判断能否
                        switch (cardStateInGame.profile.Reason.Logic)
                        {
                            //-3 不等于（不包含）
                            case -3:
                                allowed = parameter != cardStateInGame.profile.Reason.Threshold;
                                break;

                            //等于（包含）
                            case 0:
                                allowed = parameter == cardStateInGame.profile.Reason.Threshold;
                                break;
                        }
                        break;

                    //每一个角色卡记录的cardStateInGame.profile.tags:=SOS=coward
                    case Information.Parameter.Tag:

                        //得到每一个对象的所有tag（0不能要）
                        string[] allTags = parameter.Split('=');

                        //按照记录的逻辑方式判断能否
                        switch (cardStateInGame.profile.Reason.Logic)
                        {
                            //-3 不包含
                            case -3:
                                allowed = !allTags.Contains(cardStateInGame.profile.Reason.Threshold);
                                break;

                            //等于（包含）
                            case 0:
                                allowed = allTags.Contains(cardStateInGame.profile.Reason.Threshold);
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
                    return GameState.chiefs[cardStateInGame.teamId == 0 ? 1 : 0];

                //自家主持/主席/部长
                case Information.Objects.OurChief:

                    return GameState.chiefs[cardStateInGame.teamId];
            }

            return null;
        }

   
    #endregion


}
