using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using Sonic4.PlayerSonicModern;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sonic4;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public override void Load()
    {
        // Plugin startup logic
        Patcher.DoPatching();
        
        ClassInjector.RegisterTypeInIl2Cpp<PlayerSonicModern2D>();
        ClassInjector.RegisterTypeInIl2Cpp<PlayerUniqueComponentSonicModern>();
        ClassInjector.RegisterTypeInIl2Cpp<PlActionBounceSonic>();
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
