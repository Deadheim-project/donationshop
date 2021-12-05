using Jotunn.Managers;
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
                if (GUIManager.Instance == null)
                {
                    Debug.LogError("GUIManager instance is null");
                    return;
                }

                if (!GUIManager.CustomGUIFront)
                {
                    Debug.LogError("GUIManager CustomGUI is null");
                    return;
                }

                LoadMenu();
            }

            bool state = !DonationShop.Menu.activeSelf;

            DonationShop.Menu.SetActive(state);
        }

        public static void DestroyMenu()
        {
            DonationShop.Menu.SetActive(false);
        }

        public static void LoadMenu()
        {
            if (Player.m_localPlayer == null) return;

            DonationShop.Menu = GUIManager.Instance.CreateWoodpanel(
                                                                       parent: GUIManager.CustomGUIFront.transform,
                                                                       anchorMin: new Vector2(0.5f, 0.5f),
                                                                       anchorMax: new Vector2(0.5f, 0.5f),
                                                                       position: new Vector2(0, 0),
                                                                       width: 600,
                                                                       height: 700,
                                                                       draggable: true);
            DonationShop.Menu.SetActive(false);

            GameObject scrollView = GUIManager.Instance.CreateScrollView(parent: DonationShop.Menu.transform,
                    showHorizontalScrollbar: false,
                    showVerticalScrollbar: true,
                    handleSize: 8f,
                    handleColors: GUIManager.Instance.ValheimScrollbarHandleColorBlock,
                    handleDistanceToBorder: 50f,
                    slidingAreaBackgroundColor: new Color(0.1568628f, 0.1019608f, 0.0627451f, 1f),
                    width: 500f,
                    height: 450f
                );

            var tf = (RectTransform)scrollView.transform;
            tf.anchoredPosition = new Vector2(0, 25);
            scrollView.SetActive(true); 

            GameObject coinTextObject = GUIManager.Instance.CreateText(
                text: "Deadcoins : 0",
                parent: DonationShop.Menu.transform,
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(-85f, -100f),
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: 25,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: 350f,
                height: 80f,
                addContentSizeFitter: false);
                DonationShop.menuItems.Add("coinText", coinTextObject);           

            CreateItems(scrollView);

            scrollView.transform.Find("Scroll View").GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

            GameObject buttonObject = GUIManager.Instance.CreateButton(
                text: "Close",
                parent: DonationShop.Menu.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0, -300f),
                width: 170,
                height: 45f);
            buttonObject.SetActive(true);

            GameObject errorText = GUIManager.Instance.CreateText(
                text: "",
                parent: DonationShop.Menu.transform,
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(0f, -600f),
                font: GUIManager.Instance.AveriaSerifBold,
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
            GameObject x = GUIManager.Instance.CreateText(
                text: "\n",
             parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(0f, 0f),
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: 10,
                color: GUIManager.Instance.ValheimOrange,
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

                GameObject originalPrefab = PrefabManager.Instance.GetPrefab(prefab);

                if (originalPrefab is null)
                {
                    Debug.LogError("prefab cagado" + prefab);
                    continue;
                }
                
                GameObject prefabName = GUIManager.Instance.CreateText(
                    text: prefab.ToString(),
                 parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                    anchorMin: new Vector2(0.5f, 1f),
                    anchorMax: new Vector2(0.5f, 1f),
                    position: new Vector2(0f, 0f),
                    font: GUIManager.Instance.AveriaSerifBold,
                    fontSize: 14,
                    color: GUIManager.Instance.ValheimOrange,
                    outline: true,
                    outlineColor: Color.black,
                    width: 150f,
                    height: 18f,
                    addContentSizeFitter: false);

                GameObject amountText = GUIManager.Instance.CreateText(
                  text: amount + "x",
             parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                  anchorMin: new Vector2(0.5f, 1f),
                  anchorMax: new Vector2(0.5f, 1f),
                  position: new Vector2(0, 0f),
            font: GUIManager.Instance.AveriaSerifBold,
                  fontSize: 14,
                  color: GUIManager.Instance.ValheimOrange,
                  outline: true,
                  outlineColor: Color.black,
                  width: 60f,
                  height: 18f,
                  addContentSizeFitter: false);

                GameObject priceText = GUIManager.Instance.CreateText(
                      text: price + " Deadcoins",
                 parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                      anchorMin: new Vector2(0.5f, 1f),
                      anchorMax: new Vector2(0.5f, 1f),
                      position: new Vector2(0, 0f),
            font: GUIManager.Instance.AveriaSerifBold,
                      fontSize: 14,
                      color: GUIManager.Instance.ValheimOrange,
                      outline: true,
                      outlineColor: Color.black,
                      width: 250f,
                      height: 18f,
                      addContentSizeFitter: false);

                GameObject buttonObject2 = GUIManager.Instance.CreateButton(
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

                GameObject spacador = GUIManager.Instance.CreateText(
                    text: "",
                 parent: scrollView.transform.Find("Scroll View/Viewport/Content"),
                    anchorMin: new Vector2(0.5f, -5f),
                    anchorMax: new Vector2(0.5f, -5f),
                    position: new Vector2(0f, -20f),
                    font: GUIManager.Instance.AveriaSerifBold,
                    fontSize: 10,
                    color: GUIManager.Instance.ValheimOrange,
                    outline: true,
                    outlineColor: Color.black,
                    width: 150f,
                    height: 10f,
                    addContentSizeFitter: false);
            }
        }
    }
}
