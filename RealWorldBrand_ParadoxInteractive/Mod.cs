using Colossal.UI;
using Game.Modding;
using Game.SceneFlow;
using Game;
using System.IO;

namespace RealWorldBrand_ParadoxInteractive
{
    public class Mod : IMod
    {
        public static string Name = "Real World Brand: Paradox Interactive";
        public static string Version = "3.1.0";
        public static string Author = "StarQ";

        public static string uiHostName = "starq-rwb-paradox";

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
