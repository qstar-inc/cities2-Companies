//using Colossal.Logging;
using Colossal.UI;
using Game;
using Game.Modding;
using Game.SceneFlow;
using System.IO;

namespace RealWorldPetrochemicals
{
    public class Mod : IMod
    {
        public static string uiHostName = "starq-rwcompanies-petrochemicals";
        //public static ILog log = LogManager.GetLogger($"{nameof(RealWorldPetrochemicals)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            //log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                UIManager.defaultUISystem.AddHostLocation(uiHostName, Path.Combine(Path.GetDirectoryName(asset.path), "thumbs"), false);
            //log.Info($"Current mod asset at {asset.path}");

        }

        public void OnDispose()
        {
            UIManager.defaultUISystem.RemoveHostLocation(uiHostName);
            //log.Info(nameof(OnDispose));
        }
    }
}
