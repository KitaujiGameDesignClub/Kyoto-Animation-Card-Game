using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace KitaujiGameDesignClub.GameFramework.UI
{
    [RequireComponent(typeof(Image))]
    public class AtlasRead : MonoBehaviour
    {
        private Image imageRender;
        public SpriteAtlas spriteAtlas;
        public string spriteName;

        public bool destroyWhenGetSprite = true;
        private bool isimageRenderNotNull;

        private void Start()
        {
            if(Application.isPlaying && destroyWhenGetSprite) Destroy(this);
        }

        [ContextMenu("获取图片")]
        public virtual void Awake()
        {
       
            imageRender = GetComponent<Image>();
        
            isimageRenderNotNull = imageRender != null;
            GetSpriteFromAtlas(spriteName); 
        
       
        }

        /// <summary>
        /// 从图集中得到图片
        /// </summary>
        public void GetSpriteFromAtlas(string nameOfSprite = null)
        {
            if (nameOfSprite == null)
            {
                nameOfSprite = spriteName;
            }
        
            if (isimageRenderNotNull)
            {
                imageRender.sprite = spriteAtlas.GetSprite(nameOfSprite);
            }

        }
    
    
    


    }
}
