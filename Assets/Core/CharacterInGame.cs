using System;
using System.Collections.Generic;
using System.Linq;
using KitaujiGameDesignClub.GameFramework.Tools;
using PlasticGui.Configuration.CloudEdition.Welcome;

namespace Core
{
    /// <summary>
    /// 角色卡在游戏中的表现和状态
    /// </summary>
    public class CharacterInGame : CharacterCard
    {
        /// <summary>
        /// 解析能力的结果
        /// </summary>
        public struct AnalyseAbilityConclusion
        {
        }

        /// <summary>
        /// 是哪一个玩家的可用牌 0=A 1=B
        /// </summary>
        public int playerId;

        /// <summary>
        /// 沉默回合数 
        /// </summary>
        public int silence = 0;

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
        public Information.CardState State = Information.CardState.None;

        /// <summary>
        /// 羁绊被激活
        /// </summary>
        public bool connectEnabled = false;


        /// <summary>
        /// 按照角色的配置文件，创建游戏中可用的角色卡
        /// </summary>
        /// <param name="characterCard">角色卡配置</param>
        /// <param name="playerId">属于哪个玩家？ 0=A 1=B</param>
        public CharacterInGame(CharacterCard characterCard, int playerId)
        {
            CardName = characterCard.CardName;
            imageName = characterCard.imageName;
            gender = characterCard.gender;
            tags = characterCard.tags;
            Connects = characterCard.Connects;
            // Reason = characterCard.Reason;不需要。因为后面解析完角色能力后就可以直接用了
            // Result = characterCard.Result;
            AbilityDescription = characterCard.AbilityDescription;
            AbilityType = characterCard.AbilityType;
            allowAsChief = characterCard.allowAsChief;
            BasicPower = characterCard.BasicPower;
            actualPower = BasicPower;
            CardCount = characterCard.CardCount;
            CharacterName = characterCard.CharacterName;
            CV = characterCard.CV;
            BasicHealthPoint = characterCard.BasicHealthPoint;
            actualHealthPoint = BasicHealthPoint;
            //  characterCard.BelongBundleName 不需要，因为只有游戏开始时（选择阵营阶段）用到此变量
            FriendlyCardName = characterCard.FriendlyCardName;
            silence = 0;
            State = Information.CardState.Available;
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
                BasicHealthPoint += value1;

                //受到伤害
                if (value1 < 0)
                {
                    OnHurt(Activator);
                }

                if (BasicHealthPoint <= 0)
                {
                    //do something....
                }
            }


            if (changePower)
            {
                BasicPower += value2;

                if (BasicPower <= 0)
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
            if (AbilityType == Information.CardAbilityTypes.Debut)
            {
                AbilityReasonAnalyze(this);
            }
        }

        /// <summary>
        /// 每次轮到该卡都执行
        /// </summary>
        public void Normal()
        {
            if (AbilityType == Information.CardAbilityTypes.Normal)
            {
                AbilityReasonAnalyze(this);
            }
        }

        /// <summary>
        /// 退场时执行
        /// </summary>
        public void Exit()
        {
            if (AbilityType == Information.CardAbilityTypes.Exit)
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
            if (AbilityType == Information.CardAbilityTypes.GetHurt)
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

            //如果是any情况下都能运行，那么直接运行结果逻辑，之后终止
            if (Reason.NeededObjects.LargeScope == Information.Objects.Any)
            {
                Result.RegardActivatorAsResultObject = false;
                AbilityResultAnalyze();
                return;
            }
            else
            {
                //确定条件对象们
                ReasonObjects = GetNeededCards(Reason.NeededObjects, activator); //确定范围内的条件对象
                chief = GetNeededChief(Reason.NeededObjects); //储存主持/部长的条件对象
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

            switch (Reason.ReasonParameter)
            {
                //部长/主席/主持的金币数量
                case Information.Parameter.Coin:
                    if (chief != null) parameterValues[0] = chief.coin.ToString();
                    else
                        throw new Exception(
                            $"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断部长金币数，但是能力原因的条件对象不是chief");
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
                            $"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断角色卡攻击力，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡是否被静默
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
                            $"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断角色卡是否被沉默，但是能力原因的条件对象不是角色卡");

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
                            $"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断角色卡状态，但是能力原因的条件对象不是角色卡");

                    break;

                //tag对比
                case Information.Parameter.Tag:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            foreach (var tag in ReasonObjects[i].tags)
                            {
                                parameterValues[i] =
                                    $"{parameterValues[i]}={tag}"; //最终的效果就是，每一个角色卡记录的tags:=SOS=coward，即每个tag间都有个=连接，第一个标签前有一个=
                            }
                        }
                    }
                    else
                        throw new Exception(
                            $"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断角色卡标签，但是能力原因的条件对象不是角色卡");

                    break;

                //角色卡&部长的角色名字（固定的）
                case Information.Parameter.CharacterName:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].CharacterName.ToString();
                        }
                    }
                    else
                    {
                        parameterValues[0] = chief.CharacterName.ToString();
                    }

                    break;

                case Information.Parameter.CV:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].CV.ToString();
                        }
                    }
                    else
                    {
                        parameterValues[0] = chief.CV.ToString();
                    }

                    break;
            }

            #endregion


            //根据判断方法，准备数值计算（储存参数数据亦或是储存参数数量）
            string[] values = new string[0];

            #region 根据判断方法，准备数值计算

            switch (Reason.ReasonJudgeMethod)
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
            if (Reason.ReasonJudgeMethod == Information.JudgeMethod.Count)
            {
                //满足要求的参数的长度
                var parameterValuesLength = int.Parse(values[0]);
                int thresholdInt = int.Parse(Reason.Threshold);

                switch (Reason.Logic)
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
                switch (Reason.ReasonParameter)
                {
                    //数据为Int
                    case Information.Parameter.Coin or Information.Parameter.Power or Information.Parameter.Silence
                        or Information.Parameter.HealthPoint:
                        //将string转换为正规的类型（int）
                        int[] fixedValues = new int[values.Length];
                        int thresholdInt = int.Parse(Reason.Threshold);

                        //将记录的values转换成Int，并进行有关的判断逻辑处理
                        for (int i = 0; i < values.Length; i++)
                        {
                            //将记录的values转换成Int
                            if (!int.TryParse(values[i], out fixedValues[i]))
                            {
                                throw new Exception(
                                    $"{FriendlyCardName}(内部名称：{CardName} 隶属：{BundleName})的能力出发原因中，{Reason.ReasonParameter.ToString()}是int的，但是给定的阈值形式上不符合int类型");
                            }
                            else
                            {
                                //按照记录的逻辑方式判断能否
                                switch (Reason.Logic)
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

                            //如果有触发对象的参数符合要求，则记录以下，说明能力能正常被触发了
                            if (CheckedFixedValuesState[i]) AllowAbilityExection = true;


                            //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                            if (!Result.RegardActivatorAsResultObject || Result.SummonCardName != String.Empty)
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
                            switch (Reason.Logic)
                            {
                                //-3 不等于（不包含）
                                case -3:
                                    CheckedFixedValuesState[i] = values[i] != Reason.Threshold;
                                    break;

                                //等于（包含）
                                case 0:
                                    CheckedFixedValuesState[i] = values[i] == Reason.Threshold;
                                    break;
                            }


                            //如果有触发对象的参数符合要求，则记录以下，说明能力能正常被触发了
                            if (CheckedFixedValuesState[i]) AllowAbilityExection = true;


                            //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                            if (!Result.RegardActivatorAsResultObject || Result.SummonCardName != String.Empty)
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
                            switch (Reason.Logic)
                            {
                                //-3 不包含
                                case -3:
                                    CheckedFixedValuesState[i] = !allTags.Contains(Reason.Threshold);
                                    break;

                                //等于（包含）
                                case 0:
                                    CheckedFixedValuesState[i] = allTags.Contains(Reason.Threshold);
                                    break;
                            }


                            //如果有触发对象的参数符合要求，则记录以下，说明能力能正常被触发了
                            if (CheckedFixedValuesState[i]) AllowAbilityExection = true;


                            //如果能触发能力，并且不会对满足要求的条件对象执行有关操作，或者说是要召唤什么，那么有符合要求的条件对象时，直接就去执行能力的逻辑了
                            if (!Result.RegardActivatorAsResultObject || Result.SummonCardName != String.Empty)
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
            Chief chief = null;
            CharacterInGame[] characterInGames = null;

            #region 获取能力发动的对象 能力发动到谁身上？

            //如果要召唤，那就直接不把激活能力的条件对象作为结果对象
            if (Result.SummonCardName != string.Empty)
            {
                Result.RegardActivatorAsResultObject = false;
            }

            //如果把激活能力的条件对象作为结果对象，才查找对象
            if (Result.RegardActivatorAsResultObject)
            {
                characterInGames = reasonObjects;
            }
            //如果不把激活能力的条件对象作为结果对象，则重新寻找一次
            else
            {
                chief = GetNeededChief(Result.ResultObject);
                characterInGames = GetNeededCards(Result.ResultObject);
            }

            #endregion
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
                        parameter = neededCards[i].CharacterName.ToString();
                        break;

                    case Information.Parameter.CV:
                        parameter = neededCards[i].CV.ToString();
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

                    case Information.Parameter.State:
                        parameter = neededCards[i].State.ToString();
                        break;

                    case Information.Parameter.Tag:
                        foreach (var tag in tags)
                        {
                            parameter =
                                $"{parameter}={tag}"; //最终的效果就是，每一个角色卡记录的tags:=SOS=coward，即每个tag间都有个=连接，第一个标签前有一个=
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
                        or Information.Parameter.HealthPoint:
                        //将string转换为正规的类型（int）
                        int fixedValue;
                        int thresholdInt = int.Parse(Reason.Threshold);

                        //将记录的values转换成Int，并进行有关的判断逻辑处理
                        //将记录的values转换成Int
                        if (!int.TryParse(parameter, out fixedValue))
                        {
                            throw new Exception(
                                $"{FriendlyCardName}(内部名称：{CardName} 隶属：{BundleName})无法找到正确的对象，因为所找对象的参数是int型，但是给定的阈值形式上不符合int类型");
                        }
                        else
                        {
                            //按照记录的逻辑方式判断能否
                            allowed = Reason.Logic switch
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
                            switch (Reason.Logic)
                            {
                                //-3 不等于（不包含）
                                case -3:
                                    allowed =parameter != Reason.Threshold;
                                    break;

                                //等于（包含）
                                case 0:
                                    allowed =parameter == Reason.Threshold;
                                    break;
                            }
                        break;

                    //每一个角色卡记录的tags:=SOS=coward
                    case Information.Parameter.Tag:
                        
                            //得到每一个对象的所有tag（0不能要）
                            string[] allTags = parameter.Split('=');

                            //按照记录的逻辑方式判断能否
                            switch (Reason.Logic)
                            {
                                //-3 不包含
                                case -3:
                                    allowed = !allTags.Contains(Reason.Threshold);
                                    break;

                                //等于（包含）
                                case 0:
                                    allowed = allTags.Contains(Reason.Threshold);
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