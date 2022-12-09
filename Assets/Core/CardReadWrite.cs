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
    /// 所有的卡包
    /// </summary>
    public CardBundlesManifest[] AllBundles = null;


    /// <summary>
    /// 创建卡包清单文件（编辑器创建用）
    /// </summary>
    /// <param name="cardBundlesManifest"></param>
    /// <param name="fullPath">保存的完整路径</param>
    /// <param name="imageFullPath">新图片的完整路径，将图片复制到卡包目录下</param>
    /// <returns></returns>
    public static async UniTask CreateBundleManifestFile(CardBundlesManifest cardBundlesManifest, string fullPath,
        string imageFullPath)
    {
        //清单文件
        var io = new DescribeFileIO($"{cardBundlesManifest.BundleName}.kabmanifest",
            $"-{fullPath}/{cardBundlesManifest.BundleName}",
            "# card bundle manifest.\n# It'll tell you the summary of the bundle.");
        await YamlReadWrite.WriteAsync(io, cardBundlesManifest);
        //复制封面图片
        if (imageFullPath != String.Empty)
        {
            File.Copy(imageFullPath, $"{fullPath}/{cardBundlesManifest.BundleName}/{Path.GetFileName(imageFullPath)}",true);
        }
    }

    /// <summary>
    /// 创建卡牌文件
    /// </summary>
    /// <param name="onlyCard"></param>
    /// <returns></returns>
    public static async UniTask CreateCardFile(CharacterCard characterCard)
    {
    }

    public static string GetAnimeChinsesName(Information.Anime anime)
    {
        switch (anime)
        {
            case Information.Anime.AIR:
                return "AIR";

            case Information.Anime.AmagiBrilliantPark:
                return "甘城光辉游乐园";

            case Information.Anime.BajaStudio:
                return "巴加的工作室";

            case Information.Anime.BeyondTheBoundary:
                return "境界的彼方";

            case Information.Anime.CLANNAD:
                return "CLANNAD";

            case Information.Anime.Free:
                return "free!";

            case Information.Anime.FullMetalPanic:
                return "全金属狂潮第二季（校园篇）";

            case Information.Anime.Hyouka:
                return "冰菓";

            case Information.Anime.Kanon:
                return "KANON";

            case Information.Anime.KazemaiKoukouKyuudoubu:
                return "弦音 -风舞高中弓道部-";

            case Information.Anime.Kon:
                return "轻音少女";

            case Information.Anime.LoveChunibyoAndOtherDelusions:
                return "中二病也要谈恋爱";

            case Information.Anime.LuckyStar:
                return "Lucky☆Star";

            case Information.Anime.MissKobayashiDragonMaid:
                return "小林家的龙女仆";

            case Information.Anime.MyriadColorsPhantomWorld:
                return "无彩限的怪灵世界";

            case Information.Anime.SoundEuphonium:
                return "吹响吧!上低音号";

            case Information.Anime.TamakoMarket:
                return "玉子市场";

            case Information.Anime.TheMelancholyOfHaruhiSuzumiya:
                return "凉宫春日的忧郁";
            
            case Information.Anime.VioletEvergarden:
                return "紫罗兰永恒花园";

            default:
                return "通用卡牌";
        }
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
    public static CardBundlesManifest GetBundle(string fullPath, out int fileCodeVersion)
    {
        fileCodeVersion = 1;
        return null;
    }
}