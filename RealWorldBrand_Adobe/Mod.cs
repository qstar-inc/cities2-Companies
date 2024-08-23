//using Colossal.Logging;
using Colossal.UI;
using Game;
using Game.Modding;
using Game.SceneFlow;
using System.IO;

namespace RealWorldBrand_Adobe
{
    public class Mod : IMod
    {
        public static string Name = "Real World Brand: Adobe";
        public static string Version = "1.0.2";
        public static string Author = "StarQ";

        public static string uiHostName = "starq-rwb-adobe";

        public void OnLoad(UpdateSystem updateSystem)
        {
            GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset);

            UIManager.defaultUISystem.AddHostLocation(uiHostName, Path.Combine(Path.GetDirectoryName(asset.path), "thumbs"), false);
        }

        public void OnDispose()
        {
            UIManager.defaultUISystem.RemoveHostLocation(uiHostName);
        }
    }
}
