using System;
using UnityEngine;

namespace Admin_Commands
{
    public class Loader
    {
        public static GameObject game;

        static Loader()
        {
            Loader.game = null;
        }


        public static void hook()
        {
            if (Loader.game == null)
            {
                Loader.game = new GameObject();
                Loader.game.AddComponent<Hook>();
                UnityEngine.Object.DontDestroyOnLoad(Loader.game);
            }
        }


    }
}