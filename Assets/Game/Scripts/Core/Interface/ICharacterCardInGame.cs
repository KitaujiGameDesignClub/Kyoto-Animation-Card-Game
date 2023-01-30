using Cysharp.Threading.Tasks;

namespace Core.Interface
{
  /// <summary>
  /// 卡牌的逻辑接口
  /// </summary>
    public interface ICharacterCardInGame
    {
        /*
         * 对于各类数值的调整同步执行就行，反正也是瞬间完成
         * 动画或者其他效果之类的，要等待他播放完成，用每个方法对应的异步（CardPanel专属）
        */

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">正数:减血</param>
        /// <param name="activator">受到的伤害是谁造成的</param>
        public void GetDamaged(int damage, CharacterInGame activator);

        /// <summary>
        /// 攻击力提升
        /// </summary>
        /// <param name="value">正数：提高数值</param>
        /// <param name="activator">受到的伤害是谁造成的</param>
        public void PowerUp(int value, CharacterInGame activator);

        /// <summary>
        /// 修改血量和攻击力
        /// </summary>
        /// <param name="changeHealth">要修改生命值吗</param>
        /// <param name="value1">正数:减血</param>
        /// <param name="changePower">要修改攻击力吗</param>
        /// <param name="value2">正数：提高攻击力数值</param>
        /// <param name="Activator">是谁触发了这个函数</param>
        public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2, CharacterInGame Activator);

        /// <summary>
        /// 改变此卡状态
        /// </summary>
        public void ChangeState(Information.CardState cardState);

        /// <summary>
        /// 登场执行
        /// </summary>
        public void OnDebut();

        /// <summary>
        /// 每次轮到该卡都执行的攻击逻辑
        /// </summary>
        public void Attack(CharacterInGame target);

        /// <summary>
        /// 退场时执行
        /// </summary>
        public void Exit();

        /// <summary>
        /// 被击时执行
        /// </summary>
        /// <param name="activator">是谁触发了这个函数（谁打我了）</param>
        public void OnHurt(CharacterInGame activator);


    }
}