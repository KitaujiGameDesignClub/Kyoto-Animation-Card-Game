namespace Core
{
  /// <summary>
  /// 社团主持/部长
  /// </summary>
  public class Chief
  {
    public int healthPoint = 40;
    public int coin = 23;
    
    /// <summary>
    /// 角色名称（无则空）
    /// </summary>
    public string CharacterName;

    /// <summary>
    /// 部长的名字
    /// </summary>
    public string ChiefName;

    public Chief(CharacterCard characterCard)
    {
        CharacterName = characterCard.CharacterName;
        ChiefName = characterCard.FriendlyCardName;
    }
  }
}
