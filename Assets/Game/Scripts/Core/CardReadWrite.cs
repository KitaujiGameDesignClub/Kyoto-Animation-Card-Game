using System;
using System.Collections;
using KitaujiGameDesignClub.GameFramework;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Text;
using KitaujiGameDesignClub.GameFramework.UI;
using UnityEngine;
using UnityEngine.Networking;
using YamlDotNet.Core.Tokens;

namespace Core
{

    /// <summary>
    /// 卡牌读写，卡牌资源读写
    /// </summary>
    public class CardReadWrite
    {
        #region 游戏特有的yaml资源（字典文件）读写

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
#if UNITY_EDITOR
            if (File.Exists(Information.TagsIO.FullPath()))
            {
                File.Delete(Information.TagsIO.FullPath());
            }
#endif
            
            Information.tags = YamlReadWrite.Read(Information.TagsIO, Information.tags,out int errorCode,false);
           //错误代码小于0，你这字典有问题啊
            if (errorCode < 0)
            {
                CreateDictionaryFilesWithClassification(Information.tags,Information.TagsIO);
            }
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
#if UNITY_EDITOR
            if (File.Exists(Information.characterNameIO.FullPath()))
            {
                File.Delete(Information.characterNameIO.FullPath());
            }
#endif
            
            //这个的话，如果不存在不使用默认的方法创建新的文件，用自己封装的一个方法
            Information.characterNamesList =
                YamlReadWrite.Read(Information.characterNameIO, Information.characterNamesList,out int errorCode,false);
            
            //错误代码小于0，你这字典有问题啊
            if (errorCode < 0)
            {
                CreateDictionaryFilesWithClassification(Information.characterNamesList,Information.characterNameIO);
            }
        }
        
        /// <summary> 
        /// 重新读取字典内容（tag anime cv列表，从本地文件中读取）
        /// </summary>
        public static void ReloadDictionaries()
        {
            ReadAnimeList();
            ReadTags();
            ReadCV();
            ReadCharacterNames();
        }

        private static void CreateDictionaryFilesWithClassification(string[] strings,DescribeFileIO io)
        {
            YamlDotNet.Serialization.Serializer serializer = new();
            var content =  serializer.Serialize(strings);
            //初始化要保存的文本，并加上开头需要的东西 就是说，这个是修正后的文本
              StringBuilder fixedContent = new();
              fixedContent.AppendLine($"# UTF-8\n{io.Note}");
              fixedContent.AppendLine();
              
              //在每个分类符号之前，都加一个空行
            StringReader stringReader = new(content);
            while (true)
            {
                var line = stringReader.ReadLine();
                //到最后了
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                //这一行存在着内容

                if (line.Contains("%"))
                {
                    //加2行空行（现在看起来效果还行）
                    fixedContent.AppendLine(System.Environment.NewLine);
                }

                //加上这行该有的东西
                fixedContent.AppendLine(line);
            }
            
            YamlReadWrite.WriteString(io,fixedContent.ToString());

        }

        #endregion

        #region 保存/另存为用

        /// <summary>
        /// 创建卡包清单文件
        /// </summary>
        /// <param name="BundlesManifest">保存的内容</param>
        /// <param name="manifestFullPathToSave">清单保存的完整路径（含文件和拓展名）</param>
        /// <param name="newImageFullPath">新图片的完整路径，将图片复制到卡包目录下</param>
        /// <returns></returns>
        public static async UniTask CreateBundleManifestFile(CardBundlesManifest cardBundlesManifest, string manifestFullPathToSave,
            string newImageFullPath)
        {
            var directory =
                Path.GetDirectoryName(manifestFullPathToSave); //最终，应当形如“D:\Kyoto Animation Card game\bundles\test\test.kabmanifest”
            var fileNameWithExtension = Path.GetFileName(manifestFullPathToSave);

            //清单文件
            var io = new DescribeFileIO(fileNameWithExtension,
                $"-{directory}",
                "# card bundle manifest.\n# It'll tell you the summary of the bundle.");
            await YamlReadWrite.WriteAsync(io, cardBundlesManifest);
           
            //复制封面图片
            if (!string.IsNullOrEmpty(newImageFullPath) && newImageFullPath != $"{directory}\\{cardBundlesManifest.ImageName}")
            {
                File.Copy(newImageFullPath, $"{directory}\\{cardBundlesManifest.ImageName}", true);
            }

            //创建一个cards文件夹
            Directory.CreateDirectory($"{directory}/cards");

            Debug.Log($"“{cardBundlesManifest.FriendlyBundleName}”已成功保存在{directory}");
        }


        /// <summary>
        /// 创建卡牌文件
        /// </summary>
        /// <param name="characterCard">保存的内容</param>
        /// <param name="cardFullPathToSave">卡牌配置文件保存的完整路径（含文件和拓展名）</param>
        /// <param name="imageFullPath">新的封面图片的路径</param>
        /// <param name="newVoiceFileFullPath">新的音频文件的路径</param>
        /// <param name="voiceNamesWithoutExtension">语音文件的名字（不含拓展名）</param>
        /// <returns></returns>
        public static async UniTask CreateCardFile(CharacterCard characterCard, string cardFullPathToSave,
            string imageFullPath, string[] newVoiceFileFullPath, string[] voiceNamesWithoutExtension)
        {
            var directory = Path.GetDirectoryName(cardFullPathToSave);
            var fileNameWithExtension = Path.GetFileName(cardFullPathToSave);
            //上面二者最终应当形如“D:\Kyoto Animation Card game\bundles\test\cards\114514\114514.kbcard”

            //卡牌配置文件
            var io = new DescribeFileIO(fileNameWithExtension,
                $"-{directory}",
                "# card detail.\n# It'll tell you all the information of the card,but it can't work independently.");
            Debug.Log(io.FullPath());
            await YamlReadWrite.WriteAsync(io, characterCard);


            //复制封面图片
            if (!string.IsNullOrEmpty(imageFullPath) && imageFullPath != $"{directory}\\{characterCard.ImageName}")
            {
                File.Copy(imageFullPath, $"{directory}\\{characterCard.ImageName}", true);
            }

            //复制音频资源
            for (int i = 0; i < newVoiceFileFullPath.Length; i++)
            {
                //这个音频文件的目标路径
                var audioTargetPath = $"{directory}/{voiceNamesWithoutExtension[i]}{Path.GetExtension(newVoiceFileFullPath[i])}";

                if (!string.IsNullOrEmpty(newVoiceFileFullPath[i]) && audioTargetPath != newVoiceFileFullPath[i])
                {

                    File.Copy(newVoiceFileFullPath[i], audioTargetPath, true);
                }
            }

            //创建readme文件
            StreamWriter streamWriter = new($"{directory}/readme.txt", false, Encoding.UTF8);
            await streamWriter.WriteAsync("此文件夹内除了txt文件外，任何文件不能修改文件名");
            await streamWriter.DisposeAsync();
            streamWriter.Close();

            Debug.Log($"“{characterCard.FriendlyCardName}”已成功保存在{directory}");
        }


        #endregion

        #region 读取配置文件

        /// <summary>
        /// 获取此卡包下所有卡牌的友好名称
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static async UniTask<string[]> GetThisBundleAllCardsFriendlyNames(string bundleName)
        {
            var bundle = await GetOneBundleFromDesignatedDir(bundleName);

            string[] names = new string[bundle.cards.Length];

            for (int i = 0; i < bundle.cards.Length; i++)
            {
                names[i] = bundle.cards[i].FriendlyCardName;
            }

            return names;
        }

        /// <summary>
        /// 获取规定目录下此卡组内所有卡牌的识别名称
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static async UniTask<string[]> GetThisBundleAllCardsNames(string bundleName)
        {
            var bundle = await GetOneBundleFromDesignatedDir(bundleName);

            string[] names = new string[bundle.cards.Length];

            for (int i = 0; i < bundle.cards.Length; i++)
            {
                names[i] = bundle.cards[i].CardName;
            }

            return names;
        }


        /// <summary>
        /// 获取某一个指定清单文件所属的卡组内所有的卡牌配置
        /// </summary>
        /// <param name="manifestFullPath"></param>
        /// <returns></returns>
        public static async UniTask<CharacterCard[]> GetAllCardsOfOneBundle(string manifestFullPath)
        {
            var directoryName = Path.GetDirectoryName(manifestFullPath);
            if (Directory.Exists($"{directoryName}/cards"))
            {
                //此卡组内所有的卡牌
                ArrayList allCards = new();
                //此卡组文件夹的卡牌子文件夹
                var allDirectories =
                    Directory.GetDirectories($"{directoryName}/cards", "*", SearchOption.TopDirectoryOnly);

                for (int i = 0; i < allDirectories.Length; i++)
                {
                    //此卡存在且目录合法，则加进去
                    if (File.Exists($"{allDirectories[i]}/{Information.CardFileName}"))
                    {
                        var card = await YamlReadWrite.ReadAsync<CharacterCard>(
                       new DescribeFileIO($"{Information.CardFileName}", $"-{allDirectories[i]}"), null, false);

                        if (card != null) allCards.Add(card);
                    }


                }

                return (CharacterCard[])allCards.ToArray(typeof(CharacterCard));
            }
            else
            {
                Debug.Log($"{manifestFullPath}/cards中找不到任何卡牌文件");
                return null;
            }
        }

        /// <summary>
        /// 根据清单文件路径获取某一个卡组，包含其清单文件和卡牌文件
        /// </summary>
        /// <param name="manifestFullPath">清单文件的完整路径</param>
        /// <returns></returns>
        public static async UniTask<Bundle> GetOneBundle(string manifestFullPath)
        {
            //读取清单文件
            var manifest =
                await YamlReadWrite.ReadAsync<CardBundlesManifest>(
                    new DescribeFileIO(Path.GetFileName(manifestFullPath), $"-{Path.GetDirectoryName(manifestFullPath)}"),
                    null, false);

            var cards = await GetAllCardsOfOneBundle(manifestFullPath);

            return new Bundle(manifest, cards,manifestFullPath);
        }


        /// <summary>
        /// 从指定目录中获取某一个卡组，包含其清单文件和卡牌文件
        /// </summary>
        /// <param name="bundleName">规定目录下清单文件的文件名（应当为识别名称）</param>
        /// <returns></returns>
        public static async UniTask<Bundle> GetOneBundleFromDesignatedDir(string bundleName)
        {
            CardBundlesManifest manifestToLoad = null;
            CharacterCard[] cards = null;
            Bundle bundle = new(manifestToLoad, cards);

            await UniTask.RunOnThreadPool(UniTask.Action(async () =>
            {
                //有规定的文件夹吗
                //有的话，尝试读取所有的卡组
                if (Directory.Exists(Information.bundlesPath))
                {
                    //卡组文件夹的子文件夹
                    var allDirectories =
                        Directory.GetDirectories(Information.bundlesPath, "*", SearchOption.TopDirectoryOnly);

                    //记录中选的卡组路径
                    string selectedPath = string.Empty;

                    for (int i = 0; i < allDirectories.Length; i++)
                    {
                        //试试读取，合规的清单文件是有值的
                        var manifest =
                            await YamlReadWrite.ReadAsync<CardBundlesManifest>(
                                new DescribeFileIO($"{Information.ManifestFileName}", $"-{allDirectories[i]}"), null,
                                false);

                        //检查是不是与要求的name相符
                        if (manifest.BundleName == bundleName)
                        {
                            manifestToLoad = manifest;
                            selectedPath = allDirectories[i];
                            break;
                        }
                    }

                    if (selectedPath == string.Empty)
                    {
                        Notify.notify.CreateBannerNotification(null, $"找不到卡组{bundleName}");
                    }
                    else
                    {
                        cards = await GetAllCardsOfOneBundle(selectedPath);
                    }
                }
            }));


            if (manifestToLoad != null)
            {
                bundle.manifest = manifestToLoad;
            }
            else
            {
                throw new Exception($"{bundleName}不存在");
            }

            if (cards != null)
            {
                bundle.cards = cards;
            }

            return bundle;
        }


        /// <summary>
        /// 在规定的游戏目录下获取所有的卡包
        /// </summary>
        /// <returns></returns>
        public static async UniTask<Bundle[]> GetAllBundles()
        {
            Bundle[] bundles = null;

            //不提前缓存一下，会提示getpath只能在main thread上运行，而且也有要求在awake start中调用之类的错误
            var bundlesPath = Information.bundlesPath;



            //根据平台兼容性，切换到线程池和单线程
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            await UniTask.SwitchToThreadPool();
#else
 await UniTask.SwitchToMainThread();
#endif

            //查询所有的manifest文件
            DirectoryInfo directoryInfo = new(bundlesPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles($"{Information.ManifestFileName}", SearchOption.AllDirectories);

            bundles = new Bundle[fileInfos.Length];
            //读取清单文件
            for (int i = 0; i < fileInfos.Length; i++)
            {
                bundles[i] = await GetOneBundle(fileInfos[i].FullName);
            }


            #region 废弃
            /*
             *  //有规定的文件夹吗
            //有的话，尝试读取所有的卡组
            if (Directory.Exists(bundlesPath))
            {
                ArrayList allBundles = new();

                //卡组文件夹的子文件夹
                var allDirectories =
                    Directory.GetDirectories(bundlesPath, "*", SearchOption.TopDirectoryOnly);


                for (int i = 0; i < allDirectories.Length; i++)
                {
                    //试试读取，合规的清单文件是有值的
                    var manifest =
                        await YamlReadWrite.ReadAsync<CardBundlesManifest>(
                            new DescribeFileIO($"*{Information.ManifestExtension}", $"-{allDirectories[i]}"), null,
                            false);

                    //是合规的清单文件，就把他加进来
                    //并对后续卡牌进行获取
                    if (manifest != null)
                    {
                        //清单加进来了（加入到对应的卡包中）
                        var bundle = new Bundle
                        {
                            manifest = manifest
                        };

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
                Directory.CreateDirectory(bundlesPath);
                StreamWriter streamWriter = new($"{bundlesPath}/readme.txt", true,
                    System.Text.Encoding.UTF8);
                await streamWriter.WriteAsync("将卡组放在此文件夹中。使用编辑器创建的卡组基本上均可以。以后如果存在不兼容的情况，会有自动修复机制尝试修复（挖了个坑）");
            }
             */
            #endregion


            //保证正常运行，回到主线程
            await UniTask.SwitchToMainThread();
            return bundles;
        }

        #endregion


        #region 资源文件加载

        /// <summary>
        /// 加载卡牌音频（加载失败返回null）
        /// </summary>
        /// <param name="audioFullPath"></param>
        /// <returns></returns>
        public static async UniTask<AudioClip> CardVoiceLoader(string audioFullPath)
        {
            if (!File.Exists(audioFullPath))
            {
                return null;
            }

            if (Path.GetFileNameWithoutExtension(audioFullPath).Contains("："))
            {
                var warning = $"{audioFullPath}的文件名中，不应当含有中文引号";
                Debug.LogError(warning);
                return null;
            }

            DownloadHandlerAudioClip handler = null;
            switch (Path.GetExtension(audioFullPath).ToLower())
            {
                case ".ogg":
                    handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.OGGVORBIS);
                    break;

                case ".mp3":
                    handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.MPEG);
                    break;

                case ".aif":
                    handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.AIFF);
                    break;

                case ".wav":
                    handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.WAV);
                    break;

                default:
                    //提示所选音频是不受支持的格式
                    var allSupportedFormat = Information.SupportedAudioExtension[0].Substring(1);
                    for (int i = 1; i < Information.SupportedAudioExtension.Length; i++)
                    {
                        allSupportedFormat = $"{allSupportedFormat}、{Information.SupportedAudioExtension[i].Substring(1)}";//ogg、wav、aif、mp3
                    }
                    Debug.LogError($"{Path.GetExtension(audioFullPath).ToLower()}是不受支持的格式，只接受以下格式：{allSupportedFormat}");
                    return null;
            }

            handler.streamAudio = true;
             var uwr = new UnityWebRequest(audioFullPath, "GET", handler, null);

            await uwr.SendWebRequest();

            var clip = handler.audioClip;            
            handler.Dispose();
            uwr.Dispose();
            return clip;
        }

        /// <summary>
        /// 封面图片加载（加载不到返回null）
        /// </summary>
        /// <param name="imageFullPath"></param>
        /// <returns></returns>
        public static async UniTask<Texture2D> CoverImageLoader(string imageFullPath)
        {
            //没有特意设置图片，用默认的
            if (string.IsNullOrEmpty(Path.GetExtension(imageFullPath)))
            {             
                return null;
            }

            //图片文件丢失了
            if (!File.Exists(imageFullPath))
            {
                Debug.LogWarning($"图片文件“{imageFullPath}”不存在");             
                return null;
            }


            //路径补全，防止产生cann't connect host错误
            imageFullPath = Path.GetFullPath(imageFullPath);

            if (!File.Exists(imageFullPath))
            {
               return null;
            }

            var handler = new DownloadHandlerTexture();
            UnityWebRequest unityWebRequest = new(imageFullPath, "GET", handler, null);
            
            //试试能不能正常加载图片
            try
            {
                await unityWebRequest.SendWebRequest();

                //都等待了，不用isDone了
                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    var image = handler.texture;
                    handler.Dispose();
                    unityWebRequest.Dispose();
                    return image;

                }
                //不存在或加载失败，返回空值
                else
                {
                    handler.Dispose();
                    unityWebRequest.Dispose();
                    return null;
                }
            }
            catch (Exception e)
            {
                //不行的话就抓取报错
                Debug.LogError(unityWebRequest.url);
                Debug.LogError(e.Message);
                return null;
            }
         
          

        }

        #endregion

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
}
