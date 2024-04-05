using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using Tomlyn;
using Tomlyn.Model;

namespace SuperstarsTestMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public override void Load()
    {
        // Plugin startup logic
        Patcher.DoPatching();
        
        ClassInjector.RegisterTypeInIl2Cpp<PlActionBounceSonic>();
        ClassInjector.RegisterTypeInIl2Cpp<PlActionWSpinAttackSonic>();
        
        LoadConfig();
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
    
    public enum ESonicExtraActionType : byte
    {
        None,
        BounceAttack,
        WSpinAttack
    }

    public static ESonicExtraActionType SonicExtraActionType = ESonicExtraActionType.None;
    public static bool SonicEnableDropDash;
    
    private static string FindDLLPath()
    {
        // Get the path of the DLL that injected the code
        Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        return (from assembly in loadedAssemblies where assembly.FullName!.Contains("SuperstarsTestMod") select Path.GetDirectoryName(assembly.Location)).FirstOrDefault();
    }

    private void LoadConfig()
    {
        string path = FindDLLPath();

        if (path is null)
        {
            Log.LogError($"Could not find the assembly directory for this mod! Did you rename the .DLL?");
            return;
        }

        var configFile = Path.Combine(path, "config.toml");
        if (!File.Exists(configFile))
        {
            Log.LogError($"Could not find config.toml in the assembly directory! Ensure that it exists.");
            return;
        }
        
        var toml = File.ReadAllText(configFile);
        var model = Toml.ToModel(toml);
        SonicExtraActionType = (string)((TomlTable)model["sonic"]!)["sonic_extra_action_type"] switch
        {
            "WSpinAttack" => ESonicExtraActionType.WSpinAttack,
            "BounceAttack" => ESonicExtraActionType.BounceAttack,
            _ => ESonicExtraActionType.None
        };
        SonicEnableDropDash = (bool)((TomlTable)model["sonic"]!)["sonic_enable_drop_dash"];
    }
}
