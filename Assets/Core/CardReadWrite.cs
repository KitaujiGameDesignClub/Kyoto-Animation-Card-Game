using System;
using System.Collections;
using KitaujiGameDesignClub.GameFramework.Tools;
using Core;
using SimpleFileBrowser;


/// <summary>
/// 卡牌读写
/// </summary>
public class CardReadWrite
{
   /// <summary>
   /// 所有的卡包
   /// </summary>
   public CardBundlesManifest[] AllBundles = null;


  /// <summary>
  /// 创建新的卡包（编辑器创建用）
  /// </summary>
  /// <returns>创建好后，要进行编辑的卡包</returns>
   public static CardBundlesManifest CreateNewBundle()
   {
      var io = new DescribeFileIO("BundleManifest.yaml", "Temp/newBundle","# card bundle manifest.\n# It'll tell you the summary of the bundle.");
      var manifest = new CardBundlesManifest();
      manifest.io = io;
      
      YamlReadWrite.Write(io,manifest);
      return manifest;


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
   /// <returns></returns>
   public static IEnumerator GetBundle()
   {
       yield break;
   }
   
   
}
