using System;
using System.Collections.Generic;
using System.Linq;
using KitaujiGameDesignClub.GameFramework.Tools;


namespace Core
{
    /// <summary>
    /// 角色卡在游戏中的表现和状态
    /// </summary>
    public class CharacterInGame
    {
        /// <summary>
        /// 此角色卡的配置文件
        /// </summary>
        public CharacterCard profile;



        /// <summary>
        /// 是哪一个玩家的可用牌 0=A 1=B
        /// </summary>
        public int playerId;

        /// <summary>
        /// 沉默回合数 
        /// </summary>
        public int silence = 0;
        
        /// <summary>
        /// 嘲讽回合数
        /// </summary>
        public int ridicule = 0;

        /// <summary>
        /// 实际攻击力（各种影响攻击力的都对这个参数修改）
        /// </summary>
        public int actualPower;

        /// <summary>
        /// 实际生命值（各种影响攻击力的都对这个参数修改）
        /// </summary>
        public int actualHealthPoint;

        /// <summary>
        /// 此卡状态
        /// </summary>
        public Information.CardState State;

        /// <summary>
        /// 羁绊被激活
        /// </summary>
        public bool connectEnabled = false;

    


        /// <summary>
        /// 按照角色的配置文件，创建游戏中可用的角色卡（登场时调用）
        /// </summary>
        /// <param name="characterCard">角色卡配置</param>
        /// <param name="playerId">属于哪个玩家？ 0=A 1=B</param>
        public CharacterInGame(CharacterCard characterCard, int playerId)
        {
            profile = characterCard;
            silence = 0;
            ridicule = 0;
            State = Information.CardState.Present;
            connectEnabled = false;
            this.playerId = playerId;
        }


        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">正数</param>
        public void GetDamaged(int damage, CharacterInGame activator) =>
            ChangeHealthAndPower(true, damage, false, 0, activator);

        /// <summary>
        /// 攻击力提升
        /// </summary>
        /// <param name="value">正数</param>
        public void PowerUp(int value, CharacterInGame activator) =>
            ChangeHealthAndPower(false, 0, true, value, activator);


        /// <summary>
        /// 修改血量和攻击力
        /// </summary>
        /// <param name="changeHealth">要修改生命值吗</param>
        /// <param name="value1">对血量修改，加法运算</param>
        /// <param name="changePower">要修改攻击力吗</param>
        /// <param name="value2">对攻击力修改，加法运算</param>
        /// <param name="Activator">是谁触发了这个函数</param>
        public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2,
            CharacterInGame Activator)
        {
            if (changeHealth)
            {
               actualHealthPoint += value1;

                //受到伤害
                if (value1 < 0)
                {
                    OnHurt(Activator);
                }

                if (actualHealthPoint <= 0)
                {
                    //do something....
                }
            }


            if (changePower)
            {
               actualPower += value2;

                if (actualPower <= 0)
                {
                    //do something....
                }
            }
        }


        /// <summary>
        /// 改变此卡状态
        /// </summary>
        public void ChangeState(Information.CardState cardState)
        {
            State = cardState;

            switch (cardState)
            {
                //do something...
            }
        }

        /// <summary>
        /// 登场执行
        /// </summary>
        public void OnDebut()
        {
            if (profile.AbilityActivityType == Information.CardAbilityTypes.Debut)
            {
                AbilityReasonAnalyze(this);
            }
        }

        /// <summary>
        /// 每次轮到该卡都执行
        /// </summary>
        public void Normal()
        {
            if (profile.AbilityActivityType == Information.CardAbilityTypes.Normal)
            {
                AbilityReasonAnalyze(this);
            }
        }

        /// <summary>
        /// 退场时执行
        /// </summary>
        public void Exit()
        {
            if (profile.AbilityActivityType == Information.CardAbilityTypes.Exit)
            {
                AbilityReasonAnalyze(this);
            }
        }

        /// <summary>
        /// 被击时执行
        /// </summary>
        /// <param name="activator">是谁触发了这个函数（谁打我了）</param>
        public void OnHurt(CharacterInGame activator)
        {
            if (profile.AbilityActivityType == Information.CardAbilityTypes.GetHurt)
            {
                AbilityReasonAnalyze(activator);
            }
        }


        /// <summary>
        /// 能力触发原因判定
        /// </summary>
        public void AbilityReasonAnalyze(CharacterInGame activator)
        {
            //确定条件对象们
            CharacterInGame[] ReasonObjects = null; //确定范围内的条件对象
            Chief chief = null; //储存主持/部长的条件对象

            #region 确定条件对象们

            //如果是any情况下都能运行，直接运行结果逻辑
            if (profile.Reason.NeededObjects.LargeScope == Information.Objects.Any)
            {
                profile.Result.RegardActivatorAsResultObject = false;
                AbilityResultAnalyze();
                return;
            }
            else
            {
                //确定条件对象们（条件对象可以是角色卡牌，也可以是部长卡牌）
                ReasonObjects = GetNeededCards(profile.Reason.NeededObjects, activator); //确定范围内的条件对象
                chief = GetNeededChief(profile.Reason.NeededObjects); //储存主持/部长的条件对象
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

            switch (profile.Reason.ReasonParameter)
            {
                //部长/主席/主持的金币数量
                case Information.Parameter.Coin:
                    if (chief != null) parameterValues[0] = chief.coin.ToString();
                    else
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要判断部长金币数，但是能力原因的条件对象不是chief");
                    break;

                //角色卡的攻击力
                case Information.Parameter.Power:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].actualPower.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要判断角色卡攻击力，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡是否被静默（沉默回合数）
                case Information.Parameter.Silence:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].silence.ToString(); //0 1 2...
                        }
                    }
                    else
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要判断角色卡沉默回合数，但是能力原因的条件对象不是角色卡");

                    break;
                
                //角色卡的嘲讽回合数
                case Information.Parameter.Ridicule:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].ridicule.ToString(); //0 1 2...
                        }
                    }
                    else
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要判断角色卡炒粉回合数，但是能力原因的条件对象不是角色卡");

                    break;


                //角色卡状态
                case Information.Parameter.State:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].State.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要判断角色卡状态，但是能力原因的条件对象不是角色卡");

                    break;
                
                //角色卡性别
                case Information.Parameter.Gender:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].profile.gender.ToString();
                        }
                    }
                    else
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要判断角色卡性别，但是能力原因的条件对象不是角色卡");
                    break;

                //tag对比
                case Information.Parameter.Tag:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            foreach (var tag in ReasonObjects[i].profile.tags)
                            {
                                parameterValues[i] =
                                    $"{parameterValues[i]}={tag}"; //最终的效果就是，每一个角色卡记录的tags:=SOS=coward，即每个tag间都有个=连接，第一个标签前有一个=
                            }
                        }
                    }
                    else
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要判断角色卡标签，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡&部长的角色名字（固定的）
                case Information.Parameter.CharacterName:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].profile.CharacterName;
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
                            parameterValues[i] = ReasonObjects[i].profile.CV;
                        }
                    }
                    else
                    {
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要判断部长{chief.ChiefName}的声优，但是这是禁止事项");
                    }

                    break;
            }

            #endregion


            //根据判断方法，准备数值计算（储存参数数据亦或是储存参数数量）
            string[] values = new string[0];

            #region 根据判断方法，准备数值计算

            switch (profile.Reason.ReasonJudgeMethod)
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
            if (profile.Reason.ReasonJudgeMethod == Information.JudgeMethod.Count)
            {
                //满足要求的参数的长度
                var parameterValuesLength = int.Parse(values[0]);
                int thresholdInt = int.Parse(profile.Reason.Threshold);

                switch (profile.Reason.Logic)
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
                switch (profile.Reason.ReasonParameter)
                {
                    //数据为Int
                    case Information.Parameter.Coin or Information.Parameter.Power or Information.Parameter.Silence
                        or Information.Parameter.HealthPoint or Information.Parameter.Gender:
                        //将string转换为正规的类型（int）
                        int[] fixedValues = new int[values.Length];
                        int thresholdInt = int.Parse(profile.Reason.Threshold);

                        //将记录的values转换成Int，并进行有关的判断逻辑处理
                        for (int i = 0; i < values.Length; i++)
                        {
                            //将记录的values转换成Int
                            if (!int.TryParse(values[i], out fixedValues[i]))
                            {
                                throw new Exception(
                                    $"{profile.FriendlyCardName}(内部名称：{profile.CardName})的能力出发原因中，{profile.Reason.ReasonParameter}是int的，但是给定的阈值形式上不符合int类型");
                            }
                            else
                            {
                                //按照记录的逻辑方式判断能否
                                switch (profile.Reason.Logic)
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
                            if (!profile.Result.RegardActivatorAsResultObject || profile.Result.SummonCardName != String.Empty)
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
                            switch (profile.Reason.Logic)
                            {
                                //-3 不等于（不包含）
                                case -3:
                                    CheckedFixedValuesState[i] = values[i] != profile.Reason.Threshold;
                                    break;

                                //等于（包含）
                                case 0:
                                    CheckedFixedValuesState[i] = values[i] == profile.Reason.Threshold;
                                    break;
                            }


                            //如果有触发对象的参数符合要求，则记录以下，说明能力能正常被触发了
                            if (CheckedFixedValuesState[i]) AllowAbilityExection = true;


                            //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                            if (!profile.Result.RegardActivatorAsResultObject || profile.Result.SummonCardName != String.Empty)
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
                            switch (profile.Reason.Logic)
                            {
                                //-3 不包含
                                case -3:
                                    CheckedFixedValuesState[i] = !allTags.Contains(profile.Reason.Threshold);
                                    break;

                                //等于（包含）
                                case 0:
                                    CheckedFixedValuesState[i] = allTags.Contains(profile.Reason.Threshold);
                                    break;
                            }


                            //如果有触发对象的参数符合要求，则记录以下，说明能力能正常被触发了
                            if (CheckedFixedValuesState[i]) AllowAbilityExection = true;


                            //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                            if (!profile.Result.RegardActivatorAsResultObject || profile.Result.SummonCardName != String.Empty)
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
        private void AbilityResultAnalyze(CharacterInGame[] reasonObjects = null)
        {
            //能力发动到谁身上？
            Chief chiefToOperate = null;
            CharacterInGame[] characterToOperate = null;

            //召唤
            if (profile.Result.SummonCardName != String.Empty)
            {
                Manager.CardDebut(playerId,
                    GameState.AllAvailableCards[playerId]
                        .Find(new Predicate<CharacterCard>(game =>
                            game.FriendlyCardName.Equals(profile.Result.SummonCardName))));
               
            }
            
          
            #region 获取能力发动的对象 能力发动到谁身上？

            //如果要召唤，那就直接不把激活能力的条件对象作为结果对象
            if (profile.Result.SummonCardName != string.Empty)
            {
                profile.Result.RegardActivatorAsResultObject = false;
            }

            //如果把激活能力的条件对象作为结果对象，才查找对象
            if (profile.Result.RegardActivatorAsResultObject)
            {
                characterToOperate = reasonObjects;
            }
            //如果不把激活能力的条件对象作为结果对象，则重新寻找一次
            else
            {
                chiefToOperate = GetNeededChief(profile.Result.ResultObject);
                characterToOperate = GetNeededCards(profile.Result.ResultObject);
            }

            #endregion

            #region 修改参数

            //角色卡
            foreach (var card in characterToOperate)
            {
                switch (profile.Result.ParameterToChange)
                {
                    case Information.Parameter.Coin:
                        throw new Exception(
                            $"{profile.FriendlyCardName}(内部名称：{profile.CardName})无法修改Coin参数，因为他的能力指向的结果对象不是CharacterCard，而是chief");
                    
                    case Information.Parameter.HealthPoint:
                        card.actualHealthPoint = ChangeIntValue(card.actualHealthPoint);
                        break;
                    case Information.Parameter.Power:
                        card.actualPower = ChangeIntValue(card.actualPower);
                        break;
                    
                    case Information.Parameter.Gender:
                        card.profile.gender = ChangeIntValue(card.profile.gender);
                        break;
                    
                    case Information.Parameter.Silence:
                        card.silence = ChangeIntValue(card.silence);
                        break;
                    
                    case Information.Parameter.Ridicule:
                        card.ridicule = ChangeIntValue(card.ridicule);
                        break;
                    
                    case Information.Parameter.Tag:
                        switch (profile.Result.CalculationMethod)
                        {
                            //添加/删除一个tag  values种，如果有个“-”。说明是减去这个tag
                            case Information.CalculationMethod.addition:
                                //开头没有-号，加上一个tag
                                if (profile.Result.Value.Substring(0, 1) != "-" && !card.profile.tags.Contains(profile.Result.Value))
                                {
                                    card.profile.tags.Add(profile.Result.Value);
                                }
                                //开头有-号，减去一个tag
                                else if (profile.Result.Value.Substring(0, 1) == "-" && card.profile.tags.Contains(profile.Result.Value))
                                {
                                    card.profile.tags.Remove(profile.Result.Value);
                                }
                                break;
                            
                            default:
                                throw new Exception($"{profile.FriendlyCardName}(内部名称：{profile.CardName})想要用乘法修改tag");
                                                       }
                        break;
                    
                    case Information.Parameter.State:
                        card.State = (Information.CardState)Enum.Parse(typeof(Information.CardState), profile.Result.Value);
                        break;

                }
            }

            //主持
            switch (profile.Result.ParameterToChange)
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
            switch (profile.Result.CalculationMethod)
            {
                case Information.CalculationMethod.addition:
                   parameter += int.Parse(profile.Result.Value); 
                    break;
                    
                case Information.CalculationMethod.ChangeTo:
                    parameter = int.Parse(profile.Result.Value); 
                    break;
                    
                case Information.CalculationMethod.multiplication:
                    parameter = (int)(parameter * float.Parse(profile.Result.Value)); 
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
        private CharacterInGame[] GetNeededCards(NeededObjects objectsScope, CharacterInGame activator = null)
        {
            //需要的卡牌对象
            CharacterInGame[] neededCards = null;

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

                    neededCards = new CharacterInGame[1];
                    neededCards[0] = activator;

                    break;

                //己方上一位卡牌
                case Information.Objects.Last:
                    //只有一张卡，不执行
                    if (GameState.CardOnSpot[playerId].Count == 1)
                    {
                        neededCards = null;
                        return null;
                    }

                    neededCards = new CharacterInGame[1];
                    neededCards[0] =
                        GameState.CardOnSpot[playerId][GameState.whichCardPerforming[playerId] == 1 ? 6 : -1];
                    break;
                //己方下一位卡牌
                case Information.Objects.Next:
                    //只有一张卡，不执行
                    if (GameState.CardOnSpot[playerId].Count == 1)
                    {
                        neededCards = null;
                        return null;
                    }

                    neededCards = new CharacterInGame[1];
                    neededCards[0] =
                        GameState.CardOnSpot[playerId][GameState.whichCardPerforming[playerId] == 6 ? 1 : +1];
                    break;

                case Information.Objects.Self:
                    neededCards = new CharacterInGame[1];
                    neededCards[0] = this;
                    break;

                //己方场上所有的角色卡牌
                case Information.Objects.AllInTeam:
                    neededCards = CommonTools.ListArrayConversion(GameState.CardOnSpot[playerId]);
                    break;

                //敌方场上所有的角色卡牌
                case Information.Objects.AllOfEnemy:
                    neededCards = CommonTools.ListArrayConversion(GameState.CardOnSpot[playerId == 1 ? 0 : 1]);
                    break;

                //场上所有角色卡牌
                case Information.Objects.AllOnSpot:
                    neededCards = new CharacterInGame[GameState.CardOnSpot[0].Count + GameState.CardOnSpot[1].Count];
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

                    neededCards = new CharacterInGame[1];
                    neededCards[0] =
                        GameState.CardOnSpot[playerId][rd.Next(1, GameState.CardOnSpot[playerId].Count + 1)];
                    break;

                // 地方方场上随机一位角色
                case Information.Objects.RandomOfEnemy:
                    neededCards = new CharacterInGame[1];
                    neededCards[0] =
                        GameState.CardOnSpot[playerId == 1 ? 0 : 1][
                            rd.Next(1, GameState.CardOnSpot[playerId].Count + 1)];
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
            List<CharacterInGame> cache = new List<CharacterInGame>();

            for (int i = 0; i < neededCards.Length; i++)
            {
                //neededCards[i]的参数
                string parameter = null;

                //获取参数值
                switch (objectsScope.ParameterToShrinkScope)
                {
                    case Information.Parameter.CharacterName:
                        parameter = neededCards[i].profile.CharacterName.ToString();
                        break;

                    case Information.Parameter.CV:
                        parameter = neededCards[i].profile.CV.ToString();
                        break;

                    case Information.Parameter.HealthPoint:
                        parameter = neededCards[i].actualHealthPoint.ToString();
                        break;

                    case Information.Parameter.Power:
                        parameter = neededCards[i].actualPower.ToString();
                        break;

                    case Information.Parameter.Silence:
                        parameter = neededCards[i].silence.ToString();
                        break;
                    
                    case Information.Parameter.Ridicule:
                        parameter = neededCards[i].ridicule.ToString();
                        break;

                    case Information.Parameter.State:
                        parameter = neededCards[i].State.ToString();
                        break;

                    case Information.Parameter.Anime:
                        parameter = neededCards[i].profile.Anime;
                        break;
                    
                    case Information.Parameter.Gender:
                        parameter = neededCards[i].profile.gender.ToString();
                        break;

                    case Information.Parameter.Tag:
                        foreach (var tag in profile.tags)
                        {
                            parameter =
                                $"{parameter}={tag}"; //最终的效果就是，每一个角色卡记录的profile.tags:=SOS=coward，即每个tag间都有个=连接，第一个标签前有一个=
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
                        int thresholdInt = int.Parse(profile.Reason.Threshold);

                        //将记录的values转换成Int，并进行有关的判断逻辑处理
                        //将记录的values转换成Int
                        if (!int.TryParse(parameter, out fixedValue))
                        {
                            throw new Exception(
                                $"{profile.FriendlyCardName}(内部名称：{profile.CardName})无法找到正确的对象，因为所找对象的参数是int型，但是给定的阈值形式上不符合int类型");
                        }
                        else
                        {
                            //按照记录的逻辑方式判断能否
                            allowed = profile.Reason.Logic switch
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
                                
                            };
                        }

                        break;

                    //其他定性的（不含tag）
                    case Information.Parameter.CharacterName or Information.Parameter.CV:
                    
                            //按照记录的逻辑方式判断能否
                            switch (profile.Reason.Logic)
                            {
                                //-3 不等于（不包含）
                                case -3:
                                    allowed =parameter != profile.Reason.Threshold;
                                    break;

                                //等于（包含）
                                case 0:
                                    allowed =parameter == profile.Reason.Threshold;
                                    break;
                            }
                        break;

                    //每一个角色卡记录的profile.tags:=SOS=coward
                    case Information.Parameter.Tag:
                        
                            //得到每一个对象的所有tag（0不能要）
                            string[] allTags = parameter.Split('=');

                            //按照记录的逻辑方式判断能否
                            switch (profile.Reason.Logic)
                            {
                                //-3 不包含
                                case -3:
                                    allowed = !allTags.Contains(profile.Reason.Threshold);
                                    break;

                                //等于（包含）
                                case 0:
                                    allowed = allTags.Contains(profile.Reason.Threshold);
                                    break;
                            }
                            
                        break;
                }
                
                //如果符合要求，加入到之前的缓存中
                cache.Add(neededCards[i]);
                
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
                    return GameState.chiefs[playerId == 0 ? 1 : 0];

                //自家主持/主席/部长
                case Information.Objects.OurChief:

                    return GameState.chiefs[playerId];
            }

            return null;
        }
    }
}