using System;
using UnityEngine;

namespace Admin_Commands
{
    public class Hook : MonoBehaviour
    {
        private AdminCommands plugin;

        private void OnGUI()
        {
            this.plugin.OnGUI();
        }

        private void Start()
        {
            UnityEngine.Object.DontDestroyOnLoad(this);
            this.plugin = new AdminCommands();
            UnityEngine.Object.DontDestroyOnLoad(this.plugin);
            this.plugin.Start();
        }

        private void Update()
        {
            this.plugin.Update();
        }
    }
}