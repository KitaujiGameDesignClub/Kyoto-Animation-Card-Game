

namespace Core
{
    public class Manager
    {






        public static void RandomShowThreeCardsAsChief()
        {
            //获取所有卡包
           // CardBundlesManifest[] allBundles = 
        }

        /// <summary>
        /// 给三个角色卡，从三个里选择一个作为主持/部长/英雄
        /// </summary>
        /// <param name="teamId">哪一队的</param>
        /// <param name="oneOfThreeCards">那三个卡里中选的那个</param>
        public static void SelectChief(int teamId,CharacterInGame oneOfThreeCards)
        {
            
            
            
            
            GameState.chiefs[teamId] = new Chief(oneOfThreeCards);
        }
        
        /// <summary>
        /// 卡牌登场（只能在GameState.AllAvailableCards中调用）
        /// </summary>
        /// <param name="teamId">哪一队的</param>
        /// <param name="characterCard">要登场的卡牌</param>
        public static void CardDebut(int teamId, CharacterCard characterCard)
        {
            if (GameState.CardOnSpot[teamId].Count <= 5)
            {
                //要添加的卡牌
                var card = new CharacterInGame(characterCard, teamId);
                GameState.CardOnSpot[teamId].Add(card);
            }
        }
    }
}


