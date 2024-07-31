using Colossal.UI;
using Game.Modding;
using Game.SceneFlow;
using Game;
using System.IO;

namespace RealWorldBrand_ParadoxInteractive
{
    public class Mod : IMod
    {
        //public static ILog log = LogManager.GetLogger($"{nameof(RealWorldBrand_ParadoxInteractive)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static string uiHostName = "starq-rwb-ParadoxInteractive";

        public void OnLoad(UpdateSystem updateSystem)
        {
            //log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                    //log.Info($"Current mod asset at {asset.path}");
                    UIManager.defaultUISystem.AddHostLocation(uiHostName, Path.Combine(Path.GetDirectoryName(asset.path), "Thumbs"), false);

        }

        public void OnDispose()
        {
            //log.Info(nameof(OnDispose));
            UIManager.defaultUISystem.RemoveHostLocation(uiHostName);
        }
    }
}
