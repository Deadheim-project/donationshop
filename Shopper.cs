using HarmonyLib;
using System;
using UnityEngine;
using Jotunn.Managers;
using System.IO;
using BepInEx;
using UnityEngine.UI;

namespace DonationShop
{
    class Shopper
    {
        public static void BuyItem(GameObject prefab, int amount, int price)
        {

            if (DonationShop.menuItems.TryGetValue("errorText", out var value))
            {
                value.GetComponent<Text>().text = "";
            }

            if (DonationShop.IsBuying)
            {
                value.GetComponent<Text>().text = "Aguarde o processamento da última compra.";
            }

            DonationShop.IsBuying = true;

            ZPackage pkg = new ZPackage();
            pkg.Write(price + "," + amount + "," + prefab.name + "," + PlayFabManager.m_customId);
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "BuyItemServer", pkg);
        }
    }
}
