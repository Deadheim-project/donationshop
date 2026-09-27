using Deadheim.Vanilla;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DonationShop
{
    class GUI
    {
        public static void ToggleMenu()
        {
            if (!DonationShop.Menu && Player.m_localPlayer)
            {
                LoadMenu();
            }

            bool state = !DonationShop.Menu.activeSelf;

            DonationShop.Menu.SetActive(state);
        }

        public static void DestroyMenu()
        {
            DonationShop.Menu.SetActive(false);
        }

        /// <summary>
        /// Descarta o painel montado para o proximo ToggleMenu montar de novo com o
        /// ShopItems atual (cfg recarregado ou valor novo vindo do servidor).
        /// </summary>
        public static void InvalidateMenu()
        {
            if (DonationShop.Menu) UnityEngine.Object.Destroy(DonationShop.Menu);
            DonationShop.Menu = null;
        }

        public static void LoadMenu()
        {
            if (Player.m_localPlayer == null) return;

            // Montagem nova (primeira vez, relog ou lista trocada): as referencias antigas
            // apontam para objetos destruidos, e os Add abaixo recusariam chave repetida.
            DonationShop.menuItems.Clear();

            DonationShop.Menu = Ui.CreateWoodpanel(
                                                                       parent: Ui.Front.transform,
                                                                       anchorMin: new Vector2(0.5f, 0.5f),
                                                                       anchorMax: new Vector2(0.5f, 0.5f),
                                                                       position: new Vector2(0, 0),
                                                                       width: 600,
                                                                       height: 700,
                                                                       draggable: true);
            DonationShop.Menu.SetActive(false);

            GameObject scrollView = Ui.CreateScrollView(parent: DonationShop.Menu.transform,
                    showHorizontalScrollbar: false,
                    showVerticalScrollbar: true,
                    handleSize: 8f,
                    handleColors: Ui.ValheimScrollbarHandleColorBlock,
                    handleDistanceToBorder: 50f,
                    slidingAreaBackgroundColor: new Color(0.1568628f, 0.1019608f, 0.0627451f, 1f),
                    width: 500f,
                    height: 450f
                );

            var tf = (RectTransform)scrollView.transform;
            tf.anchoredPosition = new Vector2(0, 25);
            scrollView.SetActive(true); 

            GameObject coinTextObject = Ui.CreateText(
                text: "Deadcoins : 0",
                parent: DonationShop.Menu.transform,
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(-85f, -100f),
                font: Ui.AveriaSerifBold,
                fontSize: 25,
                color: Ui.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: 350f,
                height: 80f,
                addContentSizeFitter: false);
                DonationShop.menuItems.Add("coinText", coinTextObject);           

            CreateItems(scrollView);

            scrollView.transform.Find("Scroll View").GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

            GameObject buttonObject = Ui.CreateButton(
                text: "Close",
                parent: DonationShop.Menu.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0, -300f),
                width: 170,
                height: 45f);
            buttonObject.SetActive(true);

            GameObject errorText = Ui.CreateText(
                text: "",
                parent: DonationShop.Menu.transform,
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(0f, -600f),
                font: Ui.AveriaSerifBold,
                fontSize: 14,
                color: Color.red,
                outline: true,
                outlineColor: Color.black,
                width: 500f,
                height: 30f,
                addContentSizeFitter: false);

            DonationShop.menuItems.Add("errorText", errorText);

            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(DestroyMenu);
        }

        private static void CreateItems(GameObject scrollView)
        {
            GameObject x = Ui.CreateText(
                text: "\n",
             parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(0f, 0f),
                font: Ui.AveriaSerifBold,
                fontSize: 10,
                color: Ui.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: 150f,
                height: 30f,
                addContentSizeFitter: false);

            foreach (string array in DonationShop.ShopItems.Value.Trim(' ').Split('|'))
            {
                var splitedArray = array.Split(';');
                string prefab = splitedArray[0].Split('=')[1];
                string amount = splitedArray[1].Split('=')[1];
                string price = splitedArray[2].Split('=')[1];

                GameObject originalPrefab = Prefabs.Get(prefab);

                if (originalPrefab is null)
                {
                    Debug.LogError("prefab cagado" + prefab);
                    continue;
                }
                
                GameObject prefabName = Ui.CreateText(
                    text: prefab.ToString(),
                 parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                    anchorMin: new Vector2(0.5f, 1f),
                    anchorMax: new Vector2(0.5f, 1f),
                    position: new Vector2(0f, 0f),
                    font: Ui.AveriaSerifBold,
                    fontSize: 14,
                    color: Ui.ValheimOrange,
                    outline: true,
                    outlineColor: Color.black,
                    width: 150f,
                    height: 18f,
                    addContentSizeFitter: false);

                GameObject amountText = Ui.CreateText(
                  text: amount + "x",
             parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                  anchorMin: new Vector2(0.5f, 1f),
                  anchorMax: new Vector2(0.5f, 1f),
                  position: new Vector2(0, 0f),
            font: Ui.AveriaSerifBold,
                  fontSize: 14,
                  color: Ui.ValheimOrange,
                  outline: true,
                  outlineColor: Color.black,
                  width: 60f,
                  height: 18f,
                  addContentSizeFitter: false);

                GameObject priceText = Ui.CreateText(
                      text: price + " Deadcoins",
                 parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                      anchorMin: new Vector2(0.5f, 1f),
                      anchorMax: new Vector2(0.5f, 1f),
                      position: new Vector2(0, 0f),
            font: Ui.AveriaSerifBold,
                      fontSize: 14,
                      color: Ui.ValheimOrange,
                      outline: true,
                      outlineColor: Color.black,
                      width: 250f,
                      height: 18f,
                      addContentSizeFitter: false);

                GameObject buttonObject2 = Ui.CreateButton(
                          text: " Comprar ",
                 parent: prefabName.transform,
                      anchorMin: new Vector2(0.5f, -0.8f),
                    anchorMax: new Vector2(0.5f, -0.8f),
                          position: new Vector2(220, 0),
                          width: 80f,
                          height: 50f);
                buttonObject2.SetActive(true);

                Button button2 = buttonObject2.GetComponent<Button>();
                button2.onClick.AddListener(delegate { Shopper.BuyItem(originalPrefab, Convert.ToInt32(amount), Convert.ToInt32(price)); });

                DonationShop.menuItems.Add(prefab + "Text", priceText);

                GameObject spacador = Ui.CreateText(
                    text: "",
                 parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                    anchorMin: new Vector2(0.5f, -5f),
                    anchorMax: new Vector2(0.5f, -5f),
                    position: new Vector2(0f, -20f),
                    font: Ui.AveriaSerifBold,
                    fontSize: 10,
                    color: Ui.ValheimOrange,
                    outline: true,
                    outlineColor: Color.black,
                    width: 150f,
                    height: 10f,
                    addContentSizeFitter: false);
            }
        }
    }
}
