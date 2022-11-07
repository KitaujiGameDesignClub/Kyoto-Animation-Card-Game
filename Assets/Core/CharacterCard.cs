namespace Core
{
    public abstract class Card
    {
     public enum CardState
      {
          //默认的，没有在手牌、场上、部长位，可招募卡牌中出现
          None,
          /// <summary>
          /// 在可招募卡牌中出现
          /// </summary>
          Available,
          /// <summary>
          /// 是手牌
          /// </summary>
          Hand,
          /// <summary>
          /// 登场出战
          /// </summary>
          Present,
          /// <summary>
          /// 部长位
          /// </summary>
          Chief,
      }

      /// <summary>
      /// 此卡状态
      /// </summary>
      public CardState State = CardState.None;

      /// <summary>
      /// 生命值
      /// </summary>
      public int HealthPoint
      {
          get
          {
              return _healthPoint;
          }
      }


      /// <summary>
      /// 攻击力
      /// </summary>
      public int Power
      {
          get
          {
              return _power;
          }
      }
      

      /// <summary>
      /// 生命值
      /// </summary>
      private int _healthPoint;

      /// <summary>
      /// 攻击力
      /// </summary>
      private int _power;

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
          if (changeHealth) _healthPoint += value1;
          if (changePower) _power += value2;
      }

    }
}
