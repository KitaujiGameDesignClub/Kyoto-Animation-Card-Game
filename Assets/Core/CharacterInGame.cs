using System;
using System.Collections.Generic;
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
            CharacterInGame[] ReasonObjects = new CharacterInGame[0]; //确定范围内的条件对象
            Chief chief = null;//储存主持/部长的条件对象

            #region 确定条件对象们

            Random rd = new Random();
            switch (Reason.ReasonObject)
            {
                //条件对象是：触发此能力的卡牌
                case Information.Objects.Activator:
                    //如果是受击是触发能力，则把activator（攻击者）作为条件对象
                    //其他情况下，把自己作为条件对象

                    ReasonObjects = new CharacterInGame[1];
                    ReasonObjects[0] = activator;

                    break;

                /// 任何情况下都会可以 ，不进行后续判断，直接运行Result所定义的能力，且RegardActivatorAsResultObject=false
                case Information.Objects.Any:
                    Result.RegardActivatorAsResultObject = false;
                    AbilityResultAnalyze();
                    return;


                //己方上一位卡牌
                case Information.Objects.Last:
                    //只有一张卡，不执行
                    if (GameState.CardOnSpot[playerId].Count == 1)
                    {
                        ReasonObjects = null;
                        return;
                    }

                    ReasonObjects = new CharacterInGame[1];
                    ReasonObjects[0] =
                        GameState.CardOnSpot[playerId][GameState.whichCardPerforming[playerId] == 1 ? 6 : -1];
                    break;
                //己方下一位卡牌
                case Information.Objects.Next:
                    //只有一张卡，不执行
                    if (GameState.CardOnSpot[playerId].Count == 1)
                    {
                        ReasonObjects = null;
                        return;
                    }

                    ReasonObjects = new CharacterInGame[1];
                    ReasonObjects[0] =
                        GameState.CardOnSpot[playerId][GameState.whichCardPerforming[playerId] == 6 ? 1 : +1];
                    break;

                case Information.Objects.Self:
                    ReasonObjects = new CharacterInGame[1];
                    ReasonObjects[0] = this;
                    break;

                //己方场上所有的角色卡牌
                case Information.Objects.AllInTeam:
                    ReasonObjects = CommonTools.ListArrayConversion(GameState.CardOnSpot[playerId]);
                    break;

                //敌方场上所有的角色卡牌
                case Information.Objects.AllOfEnemy:
                    ReasonObjects = CommonTools.ListArrayConversion(GameState.CardOnSpot[playerId == 1 ? 0 : 1]);
                    break;

                //场上所有角色卡牌
                case Information.Objects.AllOnSpot:
                    ReasonObjects = new CharacterInGame[GameState.CardOnSpot[0].Count + GameState.CardOnSpot[1].Count];
                    for (int i = 0; i < ReasonObjects.Length; i++)
                    {
                        if (i < GameState.CardOnSpot[0].Count)
                        {
                            ReasonObjects[i] = GameState.CardOnSpot[0][i];
                        }
                        else
                        {
                            ReasonObjects[i] = GameState.CardOnSpot[1][i - GameState.CardOnSpot[0].Count];
                        }
                    }

                    break;

                // 己方场上随机一位角色
                case Information.Objects.RandomInTeam:

                    ReasonObjects = new CharacterInGame[1];
                    ReasonObjects[0] =
                        GameState.CardOnSpot[playerId][rd.Next(1, GameState.CardOnSpot[playerId].Count + 1)];
                    break;

                // 地方方场上随机一位角色
                case Information.Objects.RandomOfEnemy:
                    ReasonObjects = new CharacterInGame[1];
                    ReasonObjects[0] =
                        GameState.CardOnSpot[playerId == 1 ? 0 : 1][
                            rd.Next(1, GameState.CardOnSpot[playerId].Count + 1)];
                    break;


                //对家主持/主席/部长
                case Information.Objects.ChiefOfEnemy:
                    chief = GameState.chiefs[playerId == 0 ? 1 : 0];
                    break;

                //自家主持/主席/部长
                case Information.Objects.OurChief:
                    chief = GameState.chiefs[playerId];
                    break;
            }

            #endregion

            //判断的参数
            string[] parameterValues;  //获取要判断的参数的值
            if (chief != null)
            {
                parameterValues = new string[1];
            }
            else
            {
                parameterValues  = new string[ReasonObjects.Length];
            }
          

            #region 获取判断的参数的值
            switch (Reason.ReasonParameter)
            {
                //部长/主席/主持的金币数量
                case Information.Parameter.Coin:
                    if (chief != null) parameterValues[0] = chief.coin.ToString();
                    else throw new Exception($"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断部长金币数，但是能力原因的条件对象不是chief");
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
                    else throw new Exception($"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断角色卡攻击力，但是能力原因的条件对象不是角色卡");
                    break;
                  
                //角色卡是否被静默
                case Information.Parameter.Silence:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            parameterValues[i] = ReasonObjects[i].silence.ToString();//0 1 2...
                        }
                    }
                    else throw new Exception($"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断角色卡是否被沉默，但是能力原因的条件对象不是角色卡");
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
                    else throw new Exception($"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断角色卡状态，但是能力原因的条件对象不是角色卡");
                    break;
                
                //tag对比
                case Information.Parameter.Tag:
                    if (chief == null)
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            foreach (var tag in ReasonObjects[i].tags)
                            {
                                parameterValues[i] = $"{parameterValues[i]}={tag}";//最终的效果就是，每一个角色卡记录的tags:=SOS=coward，即每个tag间都有个=连接，第一个标签前有一个=
                            }
                        }
                    }
                    else throw new Exception($"{FriendlyCardName}(内部名称：{CardName} 所属：{BundleName})想要判断角色卡标签，但是能力原因的条件对象不是角色卡");
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

            
            //根据判断方法，进行数值计算
            string[] values;

            #region 根据判断方法，进行数值计算

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
            
            
        }


        /// <summary>
        /// 能力结果解析
        /// </summary>
        private void AbilityResultAnalyze()
        {
        }
    }
}