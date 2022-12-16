
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 完整的卡包（卡组），储存着卡组从本地文件读取的各类信息
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

