using HarmonyLib;
using System;
using UnityEngine;
using Jotunn.Managers;
using System.IO;
using BepInEx;
using UnityEngine.UI;

namespace DonationShop
{
    [HarmonyPatch]
    public class RPC
    {
        [HarmonyPatch(typeof(Game), "Start")]
        [HarmonyPrefix]
        public static void Prefix()
        {
            ZRoutedRpc.instance.Register("GetGoldServer", new Action<long, ZPackage>(RPC_GetGoldServer));
            ZRoutedRpc.instance.Register("GetGoldClient", new Action<long, ZPackage>(RPC_GetGoldClient));
            ZRoutedRpc.instance.Register("BuyItemServer", new Action<long, ZPackage>(RPC_BuyItemServer));
            ZRoutedRpc.instance.Register("BuyItemClient", new Action<long, ZPackage>(RPC_BuyItemClient));
            ZRoutedRpc.instance.Register("DontHaveGold", new Action<long, ZPackage>(RPC_DontHaveGold));
        }

        public static string Folder = "/DonationShop/";

        public static void RPC_GetGoldServer(long sender, ZPackage pkg)
        {
            if (!ZNet.instance.IsServer()) return;

            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            Debug.LogError("RPC_GetGoldServer");

            if (peer != null)
            {
                string steamId = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                string playerName = peer.m_playerName;
                string directory = Paths.ConfigPath + Folder + playerName + "-" + steamId + ".json";

                ZPackage pkgToSend = new ZPackage();

                if (!File.Exists(directory))
                {
                    Directory.CreateDirectory(Paths.ConfigPath + Folder);
                    File.WriteAllText(directory, "0");
                    pkgToSend.Write("0");
                }

                pkgToSend.Write((File.ReadAllText(directory)));

                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "GetGoldClient", pkgToSend);
            }
        }

        public static void RPC_GetGoldClient(long sender, ZPackage pkg)
        {
            if (DonationShop.menuItems.TryGetValue("coinText", out var value))
            {
                value.GetComponent<Text>().text = "Deadcoins: " + pkg.ReadString();
            }
        }

        public static void RPC_DontHaveGold(long sender, ZPackage pkg)
        {
            if (!Player.m_localPlayer) return;

            if (DonationShop.menuItems.TryGetValue("errorText", out var value))
            {
                value.GetComponent<Text>().text = "Erro: você não tem deadcoins suficientes";
            }

            DonationShop.IsBuying = false;
        }

        public static void RPC_BuyItemServer(long sender, ZPackage pkg)
        {
            if (!ZNet.instance.IsServer()) return;
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            string steamId = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
            string playerName = peer.m_playerName;
            string directory = Paths.ConfigPath + Folder + playerName + "-" + steamId + ".json";

            string[] splited = pkg.ReadString().Split(',');
            int price = Convert.ToInt32(splited[0]);
            int amount = Convert.ToInt32(splited[1]);
            string name = (splited[2]);
            int balance = Convert.ToInt32(File.ReadAllText(directory));

            if (price > balance)
            {
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "DontHaveGold", new ZPackage());
                return;
            } else
            {
                balance -= price;
                File.WriteAllText(directory, balance.ToString());
                Directory.CreateDirectory(Paths.ConfigPath + Folder + "/log/");
                File.AppendAllText(Paths.ConfigPath + Folder + "/log/log.txt", steamId + "-" + playerName + "buyed " + amount + " " + name + "for " + price + " coins");
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "BuyItemClient", pkg);          
            }
        }

        public static void RPC_BuyItemClient(long sender, ZPackage pkg)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "GetGoldServer", new ZPackage());

            string[] splited = pkg.ReadString().Split(',');
            int price = Convert.ToInt32(splited[0]);
            int amount = Convert.ToInt32(splited[1]);
            string name = (splited[2]);

            GameObject prefab = PrefabManager.Instance.GetPrefab(name);
            if (prefab.GetComponent<ItemDrop>() != null)
            {
                Player.m_localPlayer.m_inventory.AddItem(prefab, amount);
            }
            else
            {
                UnityEngine.Object.Instantiate<GameObject>(prefab, Player.m_localPlayer.transform.position, Quaternion.identity);
            }


            DonationShop.IsBuying = false;
        }
    }
}
