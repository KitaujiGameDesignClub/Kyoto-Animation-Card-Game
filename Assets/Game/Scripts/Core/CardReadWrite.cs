using System;
using System.Collections;
using KitaujiGameDesignClub.GameFramework;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Text;
using KitaujiGameDesignClub.GameFramework.UI;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Networking;
using System.IO.Compression;
using System.Linq;
using YamlDotNet.Serialization;

namespace Core
{

    /// <summary>
    /// 卡牌读写，卡牌资源读写
    /// </summary>
    public class CardReadWrite
    {
        #region 创建缓存

        public static async UniTask LoadBundleAndSaveInCache(string manifestDirectoryPath)
        {
            DirectoryInfo   directory = new DirectoryInfo(Path.Combine(manifestDirectoryPath,"cards"));

           var allCards = directory.GetFiles(Information.CardFileName, SearchOption.AllDirectories);


            foreach (var card in allCards)
            {
                await LoadCardAndSaveInCache(card.DirectoryName);
            }

        }



        /// <summary>
        /// 加载一个卡牌，并存到缓存中（不产生cardPanel）
        /// </summary>
        /// <param name="cardDiectoryPath">卡牌路径（仅文件夹，null时不会加载资源文件）</param>
        /// <param name="loadedCard">预设已经加载的卡牌</param>
        /// <returns>uuid</returns>
        public static async UniTask<string> LoadCardAndSaveInCache(string cardDiectoryPath, CharacterCard loadedCard = null)
        {
            if (cardDiectoryPath == null && loadedCard == null)
            {
                throw new Exception("参数全空？");
            }

            //读yml
            loadedCard ??= await GetOneCard(Path.Combine(cardDiectoryPath, Information.CardFileName), false);

            //没给路径，不加载这些
            if (!string.IsNullOrEmpty(cardDiectoryPath))
            {
                //图片加载              
                //加载图片，如果加载失败的话，就用预设自带的默认图片了（panel到时候会被实例化，自带一个默认图片）
                var texture = await CoverImageLoader(Path.Combine(cardDiectoryPath, loadedCard.ImageName));
                Sprite image = null;
                if (texture != null)
                {
                    image = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one / 2);

                }
                //音频加载
                var (debut, ability, defeat, exit) = await UniTask.WhenAll(CardAudioLoader($"{cardDiectoryPath}/{loadedCard.voiceDebutFileName}"),
                                                         CardAudioLoader($"{cardDiectoryPath} / {loadedCard.voiceAbilityFileName}"),
                                                         CardAudioLoader($"{cardDiectoryPath} / {loadedCard.voiceDefeatFileName}"),
                                                         CardAudioLoader($"{cardDiectoryPath}  /  {loadedCard.voiceExitFileName}"));


                //创建缓存
                GameState.cardCaches.Add(new CardCache(loadedCard,ref debut,ref defeat,ref exit,ref ability,ref image));
            }
            else GameState.cardCaches.Add(new CardCache(loadedCard));

            return loadedCard.UUID;
        }
      
        /// <summary>
        /// 获取某一个卡牌在缓存中的序号
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static int GetCardIndexFromCache(string uuid)
        {
            //先找缓存
           var  cacheIndex = -1;
            for (int i = 0; i < GameState.cardCaches.Count; i++)
            {
                if (GameState.cardCaches[i].UUID == uuid)
                {
                    cacheIndex = i;
                    break;
                }
            }

            if (cacheIndex < 0)
            {
                Notify.notify.CreateBannerNotification(null, "卡牌创建发生错误，已终止。");
                Debug.LogError($"卡牌uuid“{uuid}”无法在缓存中找到，原因未知");
            }

            return cacheIndex;
        }
        
        #endregion


        #region 游戏特有的yaml资源（字典文件）读写

        public static void ExportDictionary(string directoryName, string fileName)
        {
            //先创建一个文件，以便知到在那里保存
            var fullPath = FileBrowserHelpers.CreateFileInDirectory(directoryName, $"{fileName}");

#if UNITY_EDITOR || UNITY_STANDALONE
            //android甚至不用删除，或者说不能删除。。
            File.Delete(fullPath);
#endif
            FileBrowserHelpers.CopyFile($"{YamlReadWrite.UnityButNotAssets}/saves/{fileName}", fullPath);
        }

        public static void ImportDictionary(string fullPath, Information.DictionaryType dictionaryType)
        {
            FileBrowserHelpers.CopyFile(fullPath,$"{YamlReadWrite.UnityButNotAssets}/saves/{dictionaryType}.yml");
           
          
        }
        
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

        /// <summary>
        /// 创建有分类标识的字典文件（每类之间都有空行隔开了）
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="io"></param>
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

        #region 保存 另存为 导出

        /// <summary>
        /// 导入卡组
        /// </summary>
        /// <param name="zipFullPath"></param>
        public static void ImportBundleZip(string zipFullPath)
        {
            //saf修正（顺便直接复制到resCache里了）
            zipFullPath = FixedLoadedPathDueToSAF(zipFullPath);

            //检测是不是符合要求的
            ZipArchive zipArchive = ZipFile.OpenRead(zipFullPath);
            //获取Manifest文件
            ZipArchiveEntry manifestEntry = zipArchive.GetEntry(Information.ManifestFileName);
            //确实存在这么个文件
            if (manifestEntry != null)
            {
                try
                {
                    //读一下试试
                    StreamReader streamReader = new(manifestEntry.Open());
                    var content = streamReader.ReadToEnd();
                    streamReader.Close();
                    streamReader.Dispose();
                    zipArchive.Dispose();

                    //看看能不能加载出manifest来
                    Deserializer deserializer = new();

                    var manifest = (CardBundlesManifest)deserializer.Deserialize(content, typeof(CardBundlesManifest));

                    if (manifest != null)
                    {
                        if (!Directory.Exists($"{Information.bundlesPath}/{manifest.UUID}"))
                        {
                            //解压缩此压缩包到游戏目录
                            ZipFile.ExtractToDirectory(zipFullPath, $"{Information.bundlesPath}/{manifest.UUID}", true);

                            Notify.notify.CreateBannerNotification(null, $"“{FileBrowserHelpers.GetFilename(FileBrowser.Result[0])}”导入成功");
                        }
                        else
                        {
                            Notify.notify.CreateBannerNotification(null, $"“{manifest.FriendlyBundleName}”已存在");
                        }
                    }
                }
                catch (Exception)
                {
                    Debug.LogError($"“{Path.GetFileName(zipFullPath)}”读取失败，可能不是合规的卡组");
                }

            }

        }

        /// <summary>
        /// 导出卡组
        /// </summary>
        /// <param name="saveDirectoryPath"></param>
        /// <param name="manifestFullPath"></param>
        public static void ExportBundleZip(string saveDirectoryPath,string fileNameWithExtension,string manifestFullPath)
        {
            var tempZip = $"{Application.temporaryCachePath}/temp.zip";
            //先检查缓存中是否存在这个
            if(FileBrowserHelpers.FileExists(tempZip)) FileBrowserHelpers.DeleteFile(tempZip);

            //在缓存中创建好压缩包
            ZipFile.CreateFromDirectory(Path.GetDirectoryName(manifestFullPath),tempZip);
          
            //创建个文件，以便知道导出到哪里（SAF的格式太迷惑了）
          var fullPath = FileBrowserHelpers.CreateFileInDirectory(saveDirectoryPath, fileNameWithExtension);

#if UNITY_EDITOR || UNITY_STANDALONE
            //android甚至不用删除，或者说不能删除。。
            File.Delete(fullPath);
#endif


            //然后移动过去
            FileBrowserHelpers.MoveFile($"{Application.temporaryCachePath}/temp.zip",fullPath);
        }

        /// <summary>
        /// 创建卡包清单文件
        /// </summary>
        /// <param name="content">保存的内容</param>
        /// <param name="directoryToSave">清单保存的文件夹（这个文件夹里面就是yml了）</param>
        /// <param name="newImageFullPath">新图片的完整路径，将图片复制到卡包目录下</param>
        /// <returns></returns>
        public static async UniTask CreateBundleManifestFile(CardBundlesManifest content, string directoryToSave,
            string newImageFullPath)
        {
            Directory.CreateDirectory(directoryToSave);

            //清单文件
            var io = new DescribeFileIO(Information.ManifestFileName, $"-{directoryToSave}",
                "# card bundle manifest.\n# It'll tell you the summary of the bundle.");


            await YamlReadWrite.WriteAsync(io, content);


            //复制封面图片
            if (!string.IsNullOrEmpty(newImageFullPath) &&
                newImageFullPath != $"{directoryToSave}\\{content.ImageName}")
            {
                File.Copy(newImageFullPath, Path.Combine(directoryToSave,content.ImageName), true);
            }

            //创建一个cards文件夹

            Directory.CreateDirectory($"{directoryToSave}/cards");

            Debug.Log($"“{content.FriendlyBundleName}”已成功保存在{directoryToSave}");
        }


        /// <summary>
        /// 创建卡牌文件（保存用）
        /// </summary>
        /// <param name="characterCard">保存的内容</param>
        /// <param name="directoryToSave">卡牌配置文件保存的路径（仅文件夹）</param>
        /// <param name="imageFullPath">新的封面图片的路径</param>
        /// <param name="newVoiceFileFullPath">新的音频文件的路径</param>
        /// <param name="voiceNamesWithoutExtension">语音文件的名字（不含拓展名）</param>
        /// <returns></returns>
        public static async UniTask CreateCardFile(CharacterCard characterCard, string directoryToSave,
            string imageFullPath, string[] newVoiceFileFullPath, string[] voiceNamesWithoutExtension)
        {

            Directory.CreateDirectory(directoryToSave);


            //复制封面图片
                if (!string.IsNullOrEmpty(imageFullPath) &&
    imageFullPath != $"{directoryToSave}\\{characterCard.ImageName}")
                {
                    File.Copy(imageFullPath, Path.Combine(directoryToSave, characterCard.ImageName), true);
                }

            //检查图片大小
            var texture = await CoverImageLoader(Path.Combine(directoryToSave, characterCard.ImageName));
            if(texture != null)
            {
                //如果像素超过1024，就压缩
                if (texture.height > 1024)
                {
                    var image = new Texture2D(768, 1024, texture.format, false);

                    for (int w = 0; w < image.width; w++)
                    {
                        for (int h = 0; h < image.height; h++)
                        {
                            Color color = texture.GetPixelBilinear(w / (float)image.width, h / (float)image.height);
                            image.SetPixel(w, h, color);
                        }
                    }
                    image.Apply();

                    //删除原来的图片
                    File.Delete(Path.Combine(directoryToSave, characterCard.ImageName));
                    //创建新的图片
                    await File.WriteAllBytesAsync(Path.Combine(directoryToSave, "cover.png"), image.EncodeToPNG());
                    //修改配置文件，防止读不到图片
                    characterCard.ImageName = "cover.png";

                    Debug.Log($"“{characterCard.FriendlyCardName}”的图片已压缩");
                }
            }



            //复制音频资源
            for (int i = 0; i < newVoiceFileFullPath.Length; i++)
            {
                //这个音频文件的目标路径
                var audioTargetPath =
                    $"{directoryToSave}/{voiceNamesWithoutExtension[i]}{Path.GetExtension(FileBrowserHelpers.GetFilename(newVoiceFileFullPath[i]))}";

                if (!string.IsNullOrEmpty(newVoiceFileFullPath[i]) && audioTargetPath != newVoiceFileFullPath[i])
                {
                    File.Copy(newVoiceFileFullPath[i], audioTargetPath, true);
                }
            }


            //卡牌配置文件
            var io = new DescribeFileIO(Information.CardFileName,
                $"-{directoryToSave}",
                "# card detail.\n# It'll tell you all the information of the card,but it can't work independently.");

            await YamlReadWrite.WriteAsync(io, characterCard);


            Debug.Log($"“{characterCard.FriendlyCardName}”已成功保存在{directoryToSave}");
        }


#endregion

        #region 读取配置文件
        
        public static async UniTask<CharacterCard> GetOneCard(string cardFullPath,bool loadedFromFileBrowser)
        {
            try
            {
                var card = new CharacterCard();

                if (loadedFromFileBrowser)
                {
                    Deserializer deserializer = new();
                    card = deserializer.Deserialize<CharacterCard>(FileBrowserHelpers.ReadTextFromFile(cardFullPath));
                }
                else
                {
                    card  = await YamlReadWrite.ReadAsync(
                        new DescribeFileIO($"{Path.GetFileName(cardFullPath)}",
                            $"-{Path.GetDirectoryName(cardFullPath)}"), new CharacterCard(), false);
                }
                  
                return card;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\"{cardFullPath}\"无法读写。具体原因为\n{e}");
                throw;
            }
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

                foreach (var t in allDirectories)
                {
                    //此卡存在且目录合法，则加进去
                    if (File.Exists($"{t}/{Information.CardFileName}"))
                    {
                        var card = await YamlReadWrite.ReadAsync<CharacterCard>(
                            new DescribeFileIO($"{Information.CardFileName}", $"-{t}"), null, false);

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
            try
            {

                //读取清单文件
                CardBundlesManifest manifest =
                    await YamlReadWrite.ReadAsync<CardBundlesManifest>(
                        new DescribeFileIO(Path.GetFileName(manifestFullPath),
                            $"-{Path.GetDirectoryName(manifestFullPath)}"),
                        null, false);
                CharacterCard[] cards = await GetAllCardsOfOneBundle(manifestFullPath);


                return new Bundle(manifest, cards, manifestFullPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\"{Path.GetFullPath(manifestFullPath)}\"无法读写。具体原因为\n{e}");
                throw;
            }
        }
        

        /// <summary>
        /// 在规定的游戏目录下获取所有的卡组
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
        public static async UniTask<AudioClip> CardAudioLoader(string audioFullPath)
        {
            if (!FileBrowserHelpers.FileExists(audioFullPath))
            {
                return null;
            }

            if (FileBrowserHelpers.GetFilename(audioFullPath).Contains("："))
            {
                var warning = $"{audioFullPath}的文件名中，不应当含有中文引号";
                Debug.LogError(warning);
                return null;
            }

            //SAF修正
            audioFullPath = $"file://{FixedLoadedPathDueToSAF(audioFullPath)}";
            

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
                        allSupportedFormat =
                            $"{allSupportedFormat}、{Information.SupportedAudioExtension[i].Substring(1)}"; //ogg、wav、aif、mp3
                    }

                    Debug.LogError(
                        $"{Path.GetExtension(audioFullPath).ToLower()}是不受支持的格式，只接受以下格式：{allSupportedFormat}");
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
            if (!FileBrowserHelpers.FileExists(imageFullPath))
            {
                Debug.LogWarning($"图片文件“{imageFullPath}”不存在");             
                return null;
            }
            
            //saf修正
            imageFullPath =$"file://{FixedLoadedPathDueToSAF(imageFullPath)}";
            
            var handler = new DownloadHandlerTexture();
            UnityWebRequest unityWebRequest = new(imageFullPath, "GET", handler, null); 
            
            //试试能不能正常加载图片
            try
            {
                await unityWebRequest.SendWebRequest();

                //都等待了，不用isDone了
                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {


                    if (handler.texture.width % 4 == 0 && handler.texture.height % 4 == 0)
                    {
                        handler.texture.Compress(false);
                    }

                    unityWebRequest = null;
                    return handler.texture;

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
        /// 修复SAF限制的不可读性。（SAF的话会将这个文件转存到缓存中）
        /// </summary>
        /// <param name="readFileFullPathBySAF">选取文件的fullPath</param>
        /// <returns></returns>
        public static string FixedLoadedPathDueToSAF(string readFileFullPathBySAF)
        {

            if (readFileFullPathBySAF.Substring(0, 7) == "content")
            {
                var temp = $"{Application.temporaryCachePath}/{FileBrowserHelpers.GetFilename(readFileFullPathBySAF)}";

                FileBrowserHelpers.CopyFile(readFileFullPathBySAF,temp);

                return temp;
             
            }
           
            return readFileFullPathBySAF;

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
}
