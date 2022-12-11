
namespace Core
{
    /// <summary>
    /// 完整的卡包（卡组）
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

