using System.Collections.Generic;
using KitaujiGameDesignClub.GameFramework.@interface;
using UnityEngine;

namespace KitaujiGameDesignClub.GameFramework.Tools
{
    public class UpdateManager : MonoBehaviour
    {
    
        private static List<IUpdate> Updates = new List<IUpdate>();
        private static List<ILateUpdate> lateUpdates = new List<ILateUpdate>();


        private void Awake()
        {

            Updates.Clear();
            lateUpdates.Clear();
  
       
        }

        public static void RegisterUpdate(IUpdate update) => Updates.Add(update);


        public static void RegisterLateUpdate(ILateUpdate update) => lateUpdates.Add(update);


        public static void Remove(IUpdate update) => Updates.Remove(update);


        public static void RemoveLateUpdate(ILateUpdate update) => lateUpdates.Remove(update);



        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < Updates.Count; i++)
            {
                Updates[i].FastUpdate();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < lateUpdates.Count; i++)
            {
                lateUpdates[i].BetterLateUpdate();
            }
        }
    }
}
