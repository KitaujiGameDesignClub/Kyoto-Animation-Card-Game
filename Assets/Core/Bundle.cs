
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
        public List<CharacterCard> cards = new List<CharacterCard>();

        public Bundle()
        {
            manifest = new();
            cards = null;
        }
    }
}

