using System;
using System.Collections;
using System.Diagnostics;
using Codice.Client.Common.Threading;
using KitaujiGameDesignClub.GameFramework.Tools;
using Core;
using Cysharp.Threading.Tasks;
using System.IO;
using Debug = UnityEngine.Debug;

/// <summary>
/// 卡牌读写，卡牌资源读写
/// </summary>
public class CardReadWrite
{
    #region 游戏特有的yaml资源读写

    /// <summary>
    /// 读取所有的Anime
    /// </summary>
    /// <returns></returns>
    public static void ReadAnimeList()
    {
        Information.AnimeList = YamlReadWrite.Read(Information.AnimeListIO, Information.AnimeList);
    }


    public static void ReadTags()
    {
        Information.tags = YamlReadWrite.Read(Information.TagsIO, Information.tags);
    }

    public static void ReadCV()
    {
        Information.CV = YamlReadWrite.Read(Information.cvIO, Information.CV);
        if (Information.CV != null && Information.CV[0] != "不设置声优")
        {
            Information.CV[0] = "不设置声优";
        }
    }

    public static void ReadCharacterNames()
    {
        Information.characterNamesList =
            YamlReadWrite.Read(Information.characterNameIO, Information.characterNamesList);
    }


    /// <summary> 
    /// 刷新yaml资源（tag anime cv列表，从本地文件中读取）
    /// </summary>
    public static void refreshYamlResFromDisk()
    {
        ReadAnimeList();
        ReadTags();
        ReadCV();
        ReadCharacterNames();
    }

    #endregion

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
            File.Copy(imageFullPath, $"{fullPath}/{cardBundlesManifest.BundleName}/{Path.GetFileName(imageFullPath)}",
                true);
        }
    }


    /// <summary>
    /// 创建卡牌文件
    /// </summary>
    /// <param name="onlyCard"></param>
    /// <returns></returns>
    public static async UniTask CreateCardFile(string bundleName, CharacterCard characterCard, string fullPath,
        string imageFullPath, string[] voiceFileFullPath)
    {
        //卡牌配置文件
        var io = new DescribeFileIO($"{characterCard.CardName}.kabcard",
            $"-{fullPath}/{characterCard.CardName}",
            "# card detail.\n# It'll tell you all the information of the card,but it can't work independently.");
        await YamlReadWrite.WriteAsync(io, characterCard);

        //复制封面图片
        if (imageFullPath != String.Empty)
        {
            File.Copy(imageFullPath, $"{fullPath}/{characterCard.CardName}/{Path.GetFileName(imageFullPath)}", true);
        }

        //复制音频资源
        for (int i = 0; i < voiceFileFullPath.Length; i++)
        {
            if (voiceFileFullPath[i] != string.Empty)
            {

                File.Copy(voiceFileFullPath[i],
                    $"{fullPath}/{characterCard.CardName}/{Path.GetFileName(voiceFileFullPath[i])}", true);
                
                
            }
        }
    }


    /// <summary>
    /// 在规定的游戏目录下获取所有的卡包
    /// </summary>
    /// <returns></returns>
    public static async UniTask<Bundle[]> GetAllBundles()
    {
        Bundle[] bundles = null;

        await UniTask.RunOnThreadPool(async delegate()
        {
            //有规定的文件夹吗
            //有的话，尝试读取所有的卡组
            if (Directory.Exists(Information.bundlesPath))
            {
                ArrayList allBundles = new();

                //卡组文件夹的子文件夹
                var allDirectories =
                    Directory.GetDirectories(Information.bundlesPath, "*", SearchOption.TopDirectoryOnly);

                for (int i = 0; i < allDirectories.Length; i++)
                {
                    //试试读取，合规的清单文件是有值的
                    var manifest =
                        await YamlReadWrite.ReadAsync<CardBundlesManifest>(
                            new DescribeFileIO("*.kabmanifes", $"-{allDirectories[i]}"), null, false);

                    //是合规的清单文件，就把他加进来
                    //并对后续卡牌进行获取
                    if (manifest != null)
                    {
                        //清单加进来了（加入到对应的卡包中）
                        var bundle = new Bundle();
                        bundle.manifest = manifest;

                        //看看这个卡包内有多少卡牌文件
                        var allCardsFilesInThisBundle = Directory.GetFiles($"{allDirectories[i]}/cards", "*.kabcard");
                        ArrayList allCards = new();
                        //试试读取，合规的卡牌文件是有值的
                        for (int j = 0; j < allCardsFilesInThisBundle.Length; j++)
                        {
                            var card = await YamlReadWrite.ReadAsync<CharacterInGame>(
                                new DescribeFileIO(Path.GetFileName(allCardsFilesInThisBundle[j]),
                                    $"-{allDirectories[i]}/cards"), null, false);
                            //是合规的卡牌文件，把他加进来
                            allCards.Add(card);
                        }

                        //把所有合规的卡牌文件，加入到对应的卡包中
                        bundle.cards = (CharacterCard[])allCards.ToArray(typeof(CharacterCard));

                        //把读取好卡牌和清单的卡组（bundle）传递出去
                        allBundles.Add(bundle);
                    }
                }

                bundles = (Bundle[])allBundles.ToArray(typeof(Bundle));
            }
            //没有这个路径，初始化，并返回空值
            else
            {
                Directory.CreateDirectory(Information.bundlesPath);
                StreamWriter streamWriter = new StreamWriter($"{Information.bundlesPath}/readme.txt", true,
                    System.Text.Encoding.UTF8);
                await streamWriter.WriteAsync("将卡组放在此文件夹中。使用编辑器创建的卡组基本上均可以。以后如果存在不兼容的情况，会有自动修复机制尝试修复（挖了个坑）");
            }
        });


        return bundles;
    }

    /// <summary>
    /// 读取某一个卡包（清单+卡牌）
    /// </summary>
    /// <param name="fullPath">卡组清单的完整路径</param>
    /// <returns></returns>
    public static async UniTask<Bundle> GetBundle(string fullPath)
    {
        Bundle load = null;

        try
        {
            load = await YamlReadWrite.ReadAsync(new DescribeFileIO(Path.GetFileName(fullPath), $"-{fullPath}"),
                new Bundle());
        }
        catch (Exception e)
        {
            //先暂时这样，这里应该是提示玩家进行修复，之后用YamlFixer修复
            throw e;
        }

        return load;
    }


    /// <summary>
    /// yaml文件修复（对于清单文件和卡牌文件）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="yamlFilesText"></param>
    /// <returns></returns>
    public static async UniTask<T> YamlFixer<T>(T yamlFilesText)
    {
        var type = yamlFilesText.GetType();

        if (type == typeof(CardBundlesManifest))
        {
            return default;
        }
        else if (type == typeof(CharacterCard))
        {
            return default;
        }
        else
        {
            throw new Exception($"yaml修复器不支持{type}类型，此修复器只能接受{typeof(CardBundlesManifest)}和{typeof(CharacterCard)}");
        }
    }
}