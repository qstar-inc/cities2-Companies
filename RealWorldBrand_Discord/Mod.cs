using Colossal.UI;
using Game.Modding;
using Game.SceneFlow;
using Game;
using System.IO;

namespace RealWorldBrand_Discord
{
    public class Mod : IMod
    {
        public static string Name = "Real World Brand: Discord";
        public static string Version = "1.1.0";
        public static string Author = "StarQ";

        public static string uiHostName = "starq-rwb-discord";

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
