using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;

namespace DonationShop
{
    [BepInPlugin(PluginGUID, PluginGUID, Version)]
    public class DonationShop : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.DonationShop";
        public const string Name = "DonationShop";
        public const string Version = "2.0.0";

        // No lugar do NetworkCompatibility e do IsAdminOnly do Jotunn: a lista da loja
        // que vale e a do servidor, e cliente sem o mod (ou abaixo de 2.0) e recusado.
        private static readonly ConfigSync ServerConfigSync = new ConfigSync(PluginGUID)
        {
            DisplayName = Name,
            CurrentVersion = Version,
            MinimumRequiredVersion = "2.0.0",
            ModRequired = true,
            IsLocked = true
        };

        public static bool IsBuying = false;

        public static string PlayerName = "";

        /// <summary>
        /// Identidade do jogador local, usada como chave do saldo.
        /// No Valheim 1.0 PlayFabManager.m_customId deixou de ser string e virou
        /// Splatform.PlatformUserID, tipo que ZPackage.Write nao aceita. O id passa
        /// a viajar sempre como texto, e sempre por aqui, para que cliente e servidor
        /// nunca usem representacoes diferentes da mesma pessoa.
        /// </summary>
        public static string LocalPlayerId
        {
            get { return PlayFabManager.m_customId.ToString(); }
        }

        public static Dictionary<string, GameObject> menuItems = new Dictionary<string, GameObject>();

        Harmony harmony = new Harmony(PluginGUID);

        public static GameObject Menu;

        public static ConfigEntry<KeyCode> KeyboardShortcut;
        public static ConfigEntry<string> ShopItems;


        public void Awake()
        {
           InitConfigs();


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
                ZPackage pkgToSend = new ZPackage();
                pkgToSend.Write(LocalPlayerId);
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "GetGoldServer", pkgToSend);
            }
        }

        public void InitConfigs()
        {
            Config.SaveOnConfigSet = true;

            KeyboardShortcut = Config.Bind("Client config", "KeyboardShortcutConfig",
                KeyCode.Home,
                    new ConfigDescription("Client side KeyboardShortcut"));

            ShopItems = Config.Bind("Server config", "ShopItems", "prefabs=Blueberriesamount=50;price=150|prefab=Raspberry;amount=50;price=150|prefab=Thistle;amount=50;price=200|prefab=Cloudberry;amount=50;price=100|prefab=Wood;amount=50;price=100|prefab=Stone;amount=50;price=100|prefab=RoundLog;amount=50;price=100|prefab=FineWood;amount=50;price=150|prefab=IronNails;amount=10;price=110|prefab=IronOre;amount=50;price=700|prefab=SilverOre;amount=50;price=1000|prefab=GreydwarfEye;amount=500;price=250|prefab=SurtlingCore;amount=100;price=100|prefab=PortalToken;amount=1;price=750|prefab=ResetToken;amount=1;price=250|prefab=Coins;amount=1000;price=500",
    new ConfigDescription("ShopItems"));
            ServerConfigSync.AddConfigEntry(ShopItems).SynchronizedConfig = true;

        }
    }
}
