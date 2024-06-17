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
using static Colossal.AssetPipeline.Diagnostic.Report;

internal readonly struct StarQ_Company_Paradox : IAssetDatabaseDescriptor
{
    public string name => "starq-company-paradox";

    public IAssetFactory assetFactory => AssetDatabase.user.dataSource.assetFactory;

    public IDataSourceProvider dataSourceProvider => AssetDatabase.user.dataSource;
}

namespace CompanyModParadoxInteractive
{
    public partial class CompanyModLoader : GameSystemBase
    {
        public static string uiHostName = "starq-companies-paradox";
        private static PrefabSystem prefabSystem;

        //public string RetrieveAssetPath(IMod modInstance)
        //{
        //    GameManager.instance.modManager.TryGetExecutableAsset(modInstance, out var asset);
        //    string assetPath = asset.path;
        //    Mod.log.Info(assetPath);
        //    return assetPath;
        //}
        protected override void OnCreate()
        {
            string assetPath;
            base.OnCreate();
            Mod.log.Info("OnCreate");
            try
            {
                GameManager.instance.modManager.TryGetExecutableAsset(Mod.instance, out var asset);
                assetPath = asset.path;
            }
            catch (Exception e)
                {
                Mod.log.Info(e);
            }
            assetPath = "";
            Mod.log.Info("dead");
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            var modDir = Path.GetDirectoryName(assetPath);
            StartProcess(modDir);
            Mod.log.Info(modDir);
        }

        private void StartProcess(string modDir)
        {
            int cidRestored = CheckCID(modDir);
            if (cidRestored == 0)
            {
                JustDoIt(modDir);
            }
        }

        private int CheckCID(string modDir, int cidRestored = 0)
        {
            string[] cidBackups = Directory.GetFiles(modDir);
            foreach (string cidBackup in cidBackups)
            {
                if (cidBackup.EndsWith(".cid.bak"))
                {
                    string cidFile = cidBackup.Substring(0, cidBackup.Length - 4);

                    if (!File.Exists(cidFile))
                    {
                        File.Copy(cidBackup, cidFile);
                        cidRestored++;
                        Mod.log.Info(cidRestored);
                    }
                }
            }

            string[] subdirs = Directory.GetDirectories(modDir);
            foreach (string subdir in subdirs)
            {
                cidRestored = CheckCID(subdir, cidRestored);
            }

            if (cidRestored > 0)
            {
                NotificationSystem.Push($"{nameof(CompanyModParadoxInteractive)}-RestoreCID",
                        title: "Restart Required",
                        text: $"{cidRestored} CID file(s) restored after changing playset...",
                        onClicked: () => Application.Quit(0)
                );
            }

            return cidRestored;
        }
        private void JustDoIt(string modDir)
        {
            AssetDatabase<StarQ_Company_Paradox> starq_company_paradox = new();

            var assetDir = new DirectoryInfo(Path.Combine(modDir, "Assets-StarQ"));

            List<FileInfo> files = [];
            ProcessDirectory(assetDir, files);

            foreach (var file in files)
            {
                Mod.log.Info(file);
                var relativePath = file.FullName.Replace('\\', '/').Replace(EnvPath.kUserDataPath + "/", "");
                relativePath = relativePath.Substring(0, relativePath.LastIndexOf('/'));
                var fileName = Path.GetFileNameWithoutExtension(file.FullName);
                var extension = Path.GetExtension(file.FullName);

                var path = AssetDataPath.Create(relativePath, fileName);
                try
                {
                    var cidFilename = EnvPath.kUserDataPath + $"/{relativePath}/{fileName}{extension}.cid";
                    using StreamReader sr = new(cidFilename);
                    var guid = sr.ReadToEnd();
                    sr.Close();
                    starq_company_paradox.AddAsset<PrefabAsset>(path, guid);
                }
                catch (Exception)
                {
                    NotificationSystem.Push($"{nameof(CompanyModParadoxInteractive)}-CIDException",
                        title: "SOMETHING WENT WRONG",
                        text: $"Company Mod can't find the correct CID file(s)...",
                        onClicked: () => NotificationSystem.Pop($"{nameof(CompanyModParadoxInteractive)}-CIDException")
                );
                }

            }

            foreach (PrefabAsset prefabAsset in starq_company_paradox.GetAssets<PrefabAsset>())
            {
                PrefabBase prefabBase = prefabAsset.Load() as PrefabBase;
                prefabSystem.AddPrefab(prefabBase);

                Mod.log.Info(prefabAsset.database.name);
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

        protected override void OnUpdate()
        {

        }
    }

}
