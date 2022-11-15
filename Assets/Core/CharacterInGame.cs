using System;
using System.Collections.Generic;
using KitaujiGameDesignClub.GameFramework.Tools;

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
        public CharacterInGame(CharacterCard characterCard,int playerId)
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
        public void GetDamaged(int damage,CharacterInGame activator) => ChangeHealthAndPower(true, damage, false, 0,activator);

        /// <summary>
        /// 攻击力提升
        /// </summary>
        /// <param name="value">正数</param>
        public void PowerUp(int value,CharacterInGame activator) => ChangeHealthAndPower(false, 0, true, value,activator);


        /// <summary>
        /// 修改血量和攻击力
        /// </summary>
        /// <param name="changeHealth">要修改生命值吗</param>
        /// <param name="value1">对血量修改，加法运算</param>
        /// <param name="changePower">要修改攻击力吗</param>
        /// <param name="value2">对攻击力修改，加法运算</param>
        /// <param name="Activator">是谁触发了这个函数</param>
        public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2,CharacterInGame Activator)
        {
            if (changeHealth)
            {
                BasicHealthPoint += value1;

                //受到伤害
                if (value1 < 0)
                {
                    OnHurt(Activator);
                }
                
                if(BasicHealthPoint <= 0)
                {
                    //do something....
                }
            }
            
            
            if (changePower)
            {
                BasicPower += value2;
                
                if(BasicPower <= 0)
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
        public  void OnDebut()
        {
            if (AbilityType == Information.CardAbilityTypes.Debut)
            {
                AbilityExecution(this);
            }
        }

        /// <summary>
        /// 每次轮到该卡都执行
        /// </summary>
        public  void Normal()
        {
            if (AbilityType == Information.CardAbilityTypes.Normal)
            {
                AbilityExecution(this);
            }
        }

        /// <summary>
        /// 退场时执行
        /// </summary>
        public  void Exit()
        {
            if (AbilityType == Information.CardAbilityTypes.Exit)
            {
                AbilityExecution(this);
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
                AbilityExecution(activator);
            }
        }


        /// <summary>
        /// 能力逻辑的实现
        /// </summary>
        public void AbilityExecution(CharacterInGame activator)
        {
            //确定条件对象们
            CharacterInGame[] ReasonObjects;//确定范围内的对象

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
                
                case Information.Objects.Any:
                    break;
                
                //己方上一位卡牌
                case Information.Objects.Last:
                    //只有一张卡，不执行
                    if (GameState.CardOnSpot[playerId].Count == 1)
                    {
                        ReasonObjects = null;
                        return;
                    }
                    ReasonObjects = new CharacterInGame[1];
                    ReasonObjects[0] = GameState.CardOnSpot[playerId][GameState.whichCardPerforming[playerId] == 1 ? 6 : -1];
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
                    ReasonObjects[0] = GameState.CardOnSpot[playerId][GameState.whichCardPerforming[playerId] == 6 ? 1 : +1];
                    break;
                
                case Information.Objects.Self:
                    ReasonObjects = new CharacterInGame[1];
                    ReasonObjects[0] = this;
                    break;
                
                //己方场上所有的卡牌
                case Information.Objects.AllInTeam:
                    ReasonObjects = CommonTools.ListArrayConversion(GameState.CardOnSpot[playerId]);
                    break;
                
                //敌方场上所有的卡牌
                case Information.Objects.AllOfEnemy:
                    ReasonObjects = CommonTools.ListArrayConversion(GameState.CardOnSpot[playerId == 1 ? 0:1]);
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
                    ReasonObjects[0] =GameState.CardOnSpot[playerId == 1 ? 0:1][rd.Next(1, GameState.CardOnSpot[playerId].Count + 1)];
                    break;
            

          
          
                
          
                    
                    
            }
            #endregion

            //判断的参数
            string[] parameterValues;//参数值
            #region 判断的参数



            #endregion
        }
        
    }
}
