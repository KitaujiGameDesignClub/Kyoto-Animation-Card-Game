namespace Core
{
    /// <summary>
    /// 角色卡在游戏中的表现和状态
    /// </summary>
    public class CharacterInGame : CharacterCard
    {

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
        /// 受到伤害
        /// </summary>
        /// <param name="damage">正数</param>
        public void GetDamaged(int damage) => ChangeHealthAndPower(true, damage, false, 0);

        /// <summary>
        /// 攻击力提升
        /// </summary>
        /// <param name="value">正数</param>
        public void PowerUp(int value) => ChangeHealthAndPower(false, 0, true, value);


        /// <summary>
        /// 修改血量和攻击力
        /// </summary>
        /// <param name="changeHealth">要修改生命值吗</param>
        /// <param name="value1">对血量修改，加法运算</param>
        /// <param name="changePower">要修改攻击力吗</param>
        /// <param name="value2">对攻击力修改，加法运算</param>
        public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2)
        {
            if (changeHealth)
            {
                BasicHealthPoint += value1;
                
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
        public virtual void OnDebut()
        {
            
        }

        /// <summary>
        /// 每次轮到该卡都执行
        /// </summary>
        public virtual void Normal()
        {
            
        }

        /// <summary>
        /// 退场时执行
        /// </summary>
        public virtual void Exit()
        {
            
        }
    }
}
