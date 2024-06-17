using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.PSI.Environment;
using Colossal.UI;
using Game.Modding;
using Game.Prefabs;
using Game.PSI;
using Game.SceneFlow;
using Game;
using StreamReader = System.IO.StreamReader;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using static Game.Rendering.Debug.RenderPrefabRenderer;

namespace CompanyModParadoxInteractive
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(CompanyModParadoxInteractive)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static string uiHostName = "starq-companies-paradox";
        public static Mod instance;

        public void OnLoad(UpdateSystem updateSystem)
        {
            //log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                //log.Info($"Current mod asset at {asset.path}");
                UIManager.defaultUISystem.AddHostLocation(uiHostName, Path.Combine(Path.GetDirectoryName(asset.path), "Thumbs"), false);

            //updateSystem.UpdateAt<CompanyModLoader>(SystemUpdatePhase.MainLoop);
        }

        public static void ModInstance()
        {
            instance = new Mod();
        }

        public void OnDispose()
        {
            UIManager.defaultUISystem.RemoveHostLocation(uiHostName);
        }
    }
    
}
