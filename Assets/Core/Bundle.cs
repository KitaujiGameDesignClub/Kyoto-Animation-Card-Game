
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// �����Ŀ��������飩�������ſ���ӱ����ļ���ȡ�ĸ�����Ϣ
    /// </summary>
    /// 
    public class Bundle
    {
        public CardBundlesManifest manifest;
        public CharacterCard[] cards = new CharacterCard[0];

        public Bundle()
        {
            manifest = new();
            cards = new CharacterCard[0];
        }
    }
}

