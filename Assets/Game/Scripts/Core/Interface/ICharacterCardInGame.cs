using System.Threading.Tasks;
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
        public void GetDamaged(int damage, CardPanel activator);

        /// <summary>
        /// 攻击力提升
        /// </summary>
        /// <param name="value">正数：提高数值</param>
        /// <param name="activator">是谁提高了我的攻击力</param>
        public void PowerUp(int value, CardPanel activator);

        
        public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2, CardPanel Activator);



        /// <summary>
        /// 每次轮到该卡都执行的攻击逻辑
        /// </summary>
        /// <param name="target">打谁</param>
        /// <returns></returns>
        public UniTask Attack(CardPanel target);



        /// <summary>
        /// 登场执行
        /// </summary>
        public void OnDebut();

        /// <summary>
        /// 退场时执行
        /// </summary>
        public void Exit();

        /// <summary>
        /// 被击时执行
        /// </summary>
        /// <param name="activator">是谁触发了这个函数（谁打我了）</param>
        public void OnHurt(CardPanel activator);

    }
}