
namespace Core
{
    /// <summary>
    /// �����Ŀ��������飩
    /// </summary>
    /// 
    public class Bundle
    {
        public CardBundlesManifest manifest;
        public CharacterCard[] cards;

        public Bundle()
        {
            manifest = new();
            cards = null;
        }
    }
}

