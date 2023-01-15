using Cysharp.Threading.Tasks;

namespace Core.Interface
{
    public interface ICharacterCardInGame
    {
        /// <summary>
        /// �ܵ��˺�
        /// </summary>
        /// <param name="damage">ӦΪ����</param>
        /// <param name="activator">�ܵ����˺���˭��ɵ�</param>
        public void GetDamaged(int damage, CharacterInGame activator);

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="value">����</param>
        /// <param name="activator">�ܵ����˺���˭��ɵ�</param>
        public void PowerUp(int value, CharacterInGame activator);

        /// <summary>
        /// �޸�Ѫ���͹�����
        /// </summary>
        /// <param name="changeHealth">Ҫ�޸�����ֵ��</param>
        /// <param name="value1">��Ѫ���޸ģ��ӷ�����</param>
        /// <param name="changePower">Ҫ�޸Ĺ�������</param>
        /// <param name="value2">�Թ������޸ģ��ӷ�����</param>
        /// <param name="Activator">��˭�������������</param>
        public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2, CharacterInGame Activator);

        /// <summary>
        /// �ı�˿�״̬
        /// </summary>
        public void ChangeState(Information.CardState cardState);

        /// <summary>
        /// �ǳ�ִ��
        /// </summary>
        public void OnDebut();

        /// <summary>
        /// ÿ���ֵ��ÿ���ִ��
        /// </summary>
        public void Normal();

        /// <summary>
        /// �˳�ʱִ��
        /// </summary>
        public void Exit();

        /// <summary>
        /// ����ʱִ��
        /// </summary>
        /// <param name="activator">��˭���������������˭�����ˣ�</param>
        public void OnHurt(CharacterInGame activator);


    }
}
