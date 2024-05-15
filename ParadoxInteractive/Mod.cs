using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.PSI.Environment;
using Colossal.UI;
using Game;
using Game.Modding;
using Game.Prefabs;
using Game.SceneFlow;
using System.Collections.Generic;
using System.IO;
using StreamReader = System.IO.StreamReader;

namespace CompanyModParadoxInteractive
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(CompanyModParadoxInteractive)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static string uiHostName = "starq-companies-paradox";
        private static PrefabSystem prefabSystem;

        public void OnLoad(UpdateSystem updateSystem)
        {
            //log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                //log.Info($"Current mod asset at {asset.path}");
                UIManager.defaultUISystem.AddHostLocation(uiHostName, Path.Combine(Path.GetDirectoryName(asset.path), "Thumbs"), false);
            prefabSystem = updateSystem.World.GetOrCreateSystemManaged<PrefabSystem>();

            Dictionary<string, List<FileInfo>> modAssets = [];
            var modDir = Path.GetDirectoryName(asset.path);
            var assetDir = new DirectoryInfo(Path.Combine(modDir, "Assets-StarQ"));

            List<FileInfo> files = [];
            ProcessDirectory(assetDir, files);

            foreach (var file in files)
            {
                var relativePath = file.FullName.Replace('\\', '/').Replace(EnvPath.kUserDataPath + "/", "");
                relativePath = relativePath.Substring(0, relativePath.LastIndexOf('/'));
                var fileName = Path.GetFileNameWithoutExtension(file.FullName);
                var extension = Path.GetExtension(file.FullName);

                var path = AssetDataPath.Create(relativePath, fileName);

                var cidFilename = EnvPath.kUserDataPath + $"/{relativePath}/{fileName}{extension}.cid";
                using StreamReader sr = new(cidFilename);
                var guid = sr.ReadToEnd();
                sr.Close();

                if (extension == ".Prefab")
                {
                    AssetDatabase.user.AddAsset<PrefabAsset>(path, guid);
                }
                else if (extension == ".Geometry")
                {
                    AssetDatabase.user.AddAsset<GeometryAsset>(path, guid);
                }
                else if (extension == ".Surface")
                {
                    AssetDatabase.user.AddAsset<SurfaceAsset>(path, guid);
                }
                else if (extension == ".Texture")
                {
                    AssetDatabase.user.AddAsset<TextureAsset>(path, guid);
                }

            }
        }

        static void ProcessDirectory(DirectoryInfo directory, List<FileInfo> files)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                if (IsDesiredExtension(file.Extension))
                {
                    files.Add(file);
                }
            }

            foreach (DirectoryInfo subdirectory in directory.GetDirectories())
            {
                ProcessDirectory(subdirectory, files);
            }
        }

        static bool IsDesiredExtension(string extension)
        {
            return extension == ".Prefab" || extension == ".Geometry" || extension == ".Surface" || extension == ".Texture";
        }

        public void OnDispose()
        {
            //log.Info(nameof(OnDispose));
            UIManager.defaultUISystem.RemoveHostLocation(uiHostName);
        }
    }
}
