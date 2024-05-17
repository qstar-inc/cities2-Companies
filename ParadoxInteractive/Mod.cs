using Colossal.IO.AssetDatabase;
//using Colossal.Logging;
using Colossal.PSI.Environment;
using Colossal.UI;
using Game.Modding;
using Game.Prefabs;
using Game.SceneFlow;
using Game;
using StreamReader = System.IO.StreamReader;
using System.Collections.Generic;
using System.IO;

internal readonly struct StarQ_Company_Paradox : IAssetDatabaseDescriptor
{
    public string name => "starq-company-paradox";

    public IAssetFactory assetFactory => AssetDatabase.user.dataSource.assetFactory;

    public IDataSourceProvider dataSourceProvider => AssetDatabase.user.dataSource;
}

namespace CompanyModParadoxInteractive
{
    public class Mod : IMod
    {
        //public static ILog log = LogManager.GetLogger($"{nameof(CompanyModParadoxInteractive)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static string uiHostName = "starq-companies-paradox";
        private static PrefabSystem prefabSystem;

        public void OnLoad(UpdateSystem updateSystem)
        {
            //log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                //log.Info($"Current mod asset at {asset.path}");
                UIManager.defaultUISystem.AddHostLocation(uiHostName, Path.Combine(Path.GetDirectoryName(asset.path), "Thumbs"), false);

            prefabSystem = updateSystem.World.GetOrCreateSystemManaged<PrefabSystem>();
            JustDoIt(asset.path);
        }

        private void JustDoIt(string assetpath) {
            AssetDatabase<StarQ_Company_Paradox> starq_company_paradox = new();
            
            var modDir = Path.GetDirectoryName(assetpath);
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
                starq_company_paradox.AddAsset<PrefabAsset>(path, guid);
            }

            foreach (PrefabAsset prefabAsset in starq_company_paradox.GetAssets<PrefabAsset>())
            {
                PrefabBase prefabBase = prefabAsset.Load() as PrefabBase;
                prefabSystem.AddPrefab(prefabBase);
            }
        }

        private void ProcessDirectory(DirectoryInfo directory, List<FileInfo> files)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Extension == ".Prefab")
                {
                    files.Add(file);
                }
            }

            foreach (DirectoryInfo subdirectory in directory.GetDirectories())
            {
                ProcessDirectory(subdirectory, files);
            }
        }

        public void OnDispose()
        {
            UIManager.defaultUISystem.RemoveHostLocation(uiHostName);
        }
    }
    
}
