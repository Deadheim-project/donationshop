
using UnityEngine;
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
            if (!Player.m_localPlayer.m_inventory.CanAddItem(prefab, amount))
            {
                value.GetComponent<Text>().text = "Sem espaço no inventário";
                DonationShop.IsBuying = false;
                return;
            }

            ZPackage pkg = new ZPackage();
            pkg.Write(price + "," + amount + "," + prefab.name);
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "BuyItemServer", pkg);

        }
    }
}
