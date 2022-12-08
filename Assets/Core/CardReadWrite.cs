using System;
using System.Collections;
using Codice.Client.Common.Threading;
using KitaujiGameDesignClub.GameFramework.Tools;
using Core;
using Cysharp.Threading.Tasks;
using System.IO;

/// <summary>
/// 卡牌读写
/// </summary>
public class CardReadWrite
{

    /// <summary>
    /// 新卡包缓存路径（保存时，会将此文件夹打包）
    /// </summary>
    public const string newBundleTempPath = "Temp/newBundle";

    /// <summary>
    /// 新卡牌的缓存路径（仅创建新卡牌时使用）
    /// </summary>
    public const string newCardTempPath = "Temp/newCard";
    
    
   /// <summary>
   /// 所有的卡包
   /// </summary>
   public CardBundlesManifest[] AllBundles = null;


    /// <summary>
    /// 创建卡包清单文件（编辑器创建用）
    /// </summary>
    /// <param name="cardBundlesManifest"></param>
    /// <param name="fullPath">保存的完整路径</param>
    /// <returns></returns>
    public static async UniTask CreateBundleManifestFile(CardBundlesManifest cardBundlesManifest,string fullPath,string imageFullPath)
   {

        //清单文件
        var io = new DescribeFileIO($"{cardBundlesManifest.BundleName}.kabundle", $"-{fullPath}/{cardBundlesManifest.BundleName}",
            "# card bundle manifest.\n# It'll tell you the summary of the bundle.");
        await YamlReadWrite.WriteAsync(io, cardBundlesManifest);
        //封面图片
        if (cardBundlesManifest.ImageName != string.Empty)
        {
            File.Copy(imageFullPath, $"{fullPath}/{cardBundlesManifest.BundleName}/{Path.GetFileName(imageFullPath)}");


        }


    }

    /// <summary>
    /// 创建新卡牌
    /// </summary>
    /// <param name="onlyCard"></param>
    /// <returns></returns>
    public static CharacterCard CreateNewCard(bool onlyCard)
   {
       string path;
       //仅仅是创建一个卡牌
       if (onlyCard)
       {
           path = newCardTempPath;
       }
       //隶属于卡包的新卡牌
       else
       {
           path = $"{newBundleTempPath}/cards";
       }

       var io = new DescribeFileIO("CharacterCard.yml", path,
           "# character card profile.\n# It'll tell you all the information of character card.");

       var card = new CharacterCard();
       
       YamlReadWrite.Write(io, card);
       return card;
   }

  /// <summary>
  /// 编辑已有卡包（发布格式为KgdCardBundles的一个zip文件）
  /// </summary>
  /// <returns></returns>
  public static CardBundlesManifest CardBundleEdit()
  {
      return null;
  }
   
   
   /// <summary>
   /// 发布（保存）卡包，创建文件（做好了之后，把temp里的临时卡包文件打包成KgdCardBundles文件
   /// </summary>
   /// <param name="cardBundlesManifest">内存中的卡包配置</param>
   /// <param name="BundleIO">往哪里创建文件</param>
   public static void PublishBundles(CardBundlesManifest cardBundlesManifest,DescribeFileIO BundleIO)
   {
      
   }

   /// <summary>
   /// 获取所有的卡包
   /// </summary>
   /// <returns></returns>
   public static IEnumerator GetAllBundles()
   {
      yield break;
   }

  /// <summary>
  /// 获取某一个卡包
  /// </summary>
  /// <param name="fullPath">卡包的完整路径</param>
  /// <param name="fileCodeVersion">得到这个卡包的编码版本</param>
  /// <returns></returns>
   public static CardBundlesManifest GetBundle(string fullPath,out int fileCodeVersion)
  {
      fileCodeVersion = 1;
      return null;
  }
   

   
   
}
