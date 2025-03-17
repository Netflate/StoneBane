using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using REPOLib.Modules;
using UnityEngine;

namespace StoneBane;

[BepInPlugin("Netflate.StoneBane", "StoneBane", "1.0.0")]
[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
public class StoneBane : BaseUnityPlugin
{
    internal static StoneBane Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    private void Awake()
    {
        Instance = this;
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        
        Logger.LogInfo("Patching StoneBane...");
        
        Logger.LogInfo("Loading assets...");
        LoadAssets();
        
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }
    
    private static void LoadAssets()
    {
        AssetBundle StoneBaneAssetBundle = LoadAssetBundle("StoneBane");

        Logger.LogInfo("Loading StoneBane setup...");
        EnemySetup StoneBaneSetup = StoneBaneAssetBundle.LoadAsset<EnemySetup>("Assets/REPO/Mods/plugins/StoneBane/Enemy - StoneBane.asset");
        
        Enemies.RegisterEnemy(StoneBaneSetup);
        
        Logger.LogDebug("Loaded StoneBane enemy!");
    }
    
    public static AssetBundle LoadAssetBundle(string name)
    {
        Logger.LogDebug("Loading Asset Bundle: " + name);
        AssetBundle bundle = null;
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), name);
        bundle = AssetBundle.LoadFromFile(path);
        return bundle;
    }
}