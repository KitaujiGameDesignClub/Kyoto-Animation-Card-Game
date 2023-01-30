using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework.Tools;
using Random = System.Random;
using Core.Interface;

namespace Core
{
    /// <summary>
    /// 角色卡在游戏中的状态（里状态）
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
        public CharacterInGame(CharacterCard characterCard, int teamId)
        {
            profile = characterCard;
            silence = 0;
            ridicule = 0;
            actualPower = characterCard.BasicPower;
            actualHealthPoint = characterCard.BasicHealthPoint;
            State = Information.CardState.Present;
           // connectEnabled = false;
            this.teamId = teamId;
        }


        public void GetDamaged(int damage, CharacterInGame activator) =>
            ChangeHealthAndPower(true, damage, false, 0, activator);


        public void PowerUp(int value, CharacterInGame activator) =>
            ChangeHealthAndPower(false, 0, true, value, activator);


     
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
           
        }

        /// <summary>
        /// 每次轮到该卡都执行的攻击逻辑
        /// </summary>
        public void Attack(CharacterInGame target)
        {
            //顺带分析一波能力
           

            //扣血逻辑之类的...
            target.GetDamaged(actualPower, this);
        }

        /// <summary>
        /// 退场时执行
        /// </summary>
        public void Exit()
        {
            
        }

        /// <summary>
        /// 被击时执行
        /// </summary>
        /// <param name="activator">是谁触发了这个函数（谁打我了）</param>
        public void OnHurt(CharacterInGame activator)
        {
          
            
            
        }

    

    }
}