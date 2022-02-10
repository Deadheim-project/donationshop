using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Configuration;
using Jotunn.Utils;
using HarmonyLib;
using Jotunn.Managers;

namespace DonationShop
{
    [BepInPlugin(PluginGUID, PluginGUID, Version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class DonationShop : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.DonationShop";
        public const string Name = "DonationShop";
        public const string Version = "1.0.0";

        public static bool IsBuying = false;

        public static string PlayerName = "";

        public static Dictionary<string, GameObject> menuItems = new Dictionary<string, GameObject>();

        Harmony harmony = new Harmony(PluginGUID);

        public static GameObject Menu;

        public static ConfigEntry<KeyCode> KeyboardShortcut;
        public static ConfigEntry<string> ShopItems;


        public void Awake()
        {
           InitConfigs();

            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                if (attr.InitialSynchronization)
                {
                    Jotunn.Logger.LogMessage("Initial Config sync event received");
                }
                else
                {
                    Jotunn.Logger.LogMessage("Config sync event received");
                }
            };

            harmony.PatchAll();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyboardShortcut.Value))
            {
                Player localPlayer = Player.m_localPlayer;
                if (!localPlayer || localPlayer.IsDead() || (localPlayer.InCutscene() || localPlayer.IsTeleporting()))
                    return;

                GUI.ToggleMenu();
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "GetGoldServer", new ZPackage());
            }
        }

        [HarmonyPatch("ChickenBoo.Patches+SpawnPatch, ChickenBoo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Prefix")]
        public static class ChickenBooSpawnSystemPatch
        {
            [HarmonyPriority(Priority.First)]
            public static bool Prefix()
            {
                return false;
            }
        }

        public void InitConfigs()
        {
            Config.SaveOnConfigSet = true;

            KeyboardShortcut = Config.Bind("Client config", "KeyboardShortcutConfig",
                KeyCode.Home,
                    new ConfigDescription("Client side KeyboardShortcut", null, null,
                    new ConfigurationManagerAttributes { IsAdminOnly = false }));

            ShopItems = Config.Bind("Server config", "ShopItems", "prefabs=Blueberriesamount=50;price=150|prefab=Raspberry;amount=50;price=150|prefab=Thistle;amount=50;price=200|prefab=Cloudberry;amount=50;price=100|prefab=Wood;amount=50;price=100|prefab=Stone;amount=50;price=100|prefab=RoundLog;amount=50;price=100|prefab=FineWood;amount=50;price=150|prefab=IronNails;amount=10;price=110|prefab=IronOre;amount=50;price=700|prefab=SilverOre;amount=50;price=1000|prefab=GreydwarfEye;amount=500;price=250|prefab=SurtlingCore;amount=100;price=100|prefab=PortalToken;amount=1;price=750|prefab=ResetToken;amount=1;price=250|prefab=Coins;amount=1000;price=500",
    new ConfigDescription("ShopItems", null,
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

        }
    }
}
