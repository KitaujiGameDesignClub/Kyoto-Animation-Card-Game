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
    public Information.CharacterName CharacterName;
    
    /// <summary>
    /// 所用声优
    /// </summary>
    public Information.CV CV ;
    
    public void ConvertToChief(CharacterCard characterCard)
    {
    
    }
  }
}
