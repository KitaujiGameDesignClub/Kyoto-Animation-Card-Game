using UnityEngine;

namespace Core
{
    /// <summary>
    /// 角色卡在游戏中的数据
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
        public int teamId;
        /// <summary>
        /// 这一组内第几个卡牌（从0开始）
        /// </summary>
        public int cardId;

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
        /// 这一轮游戏这个卡牌已经干过活了
        /// </summary>
        public bool thisRoundHasActiviated = false;

        /// <summary>
        /// 音效资源
        /// </summary>
        public AudioClip voiceDebut;
        public AudioClip voiceDefeat;
        public AudioClip voiceExit;
        public AudioClip voiceAbility;
        //图片资源
        public Sprite CoverImage;

        /*羁绊暂时取消了，以后采取另外一种形式实现
        /// <summary>
        /// 羁绊被激活
        /// </summary>
        public bool connectEnabled = false;
        */



        /// <summary>
        /// 按照角色的配置文件，创建游戏中可用的角色卡（登场时调用）（不加载有关资源）
        /// </summary>
        /// <param name="characterCard">角色卡配置</param>
        /// <param name="teamId">属于哪个玩家？ 0=A 1=B</param>
        public CharacterInGame(CharacterCard characterCard)
        {
            profile = characterCard;
            silence = 0;
            ridicule = 0;
            actualPower = characterCard.BasicPower;
            actualHealthPoint = characterCard.BasicHealthPoint;
            State = Information.CardState.Present;
           // connectEnabled = false;
        }
     

     
        public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2,
            CharacterInGame Activator)
        {
            if (changeHealth)
            {
                //受到伤害
                actualHealthPoint -= value1;

               
               //没血了
                if (actualHealthPoint <= 0)
                {
                    //do something....
                }
            }


            if (changePower)
            {
               actualPower += value2;

             
            }
        }


      

      
    

    }
}