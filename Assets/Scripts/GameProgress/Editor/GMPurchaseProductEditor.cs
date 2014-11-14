using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(GMPurchaseProduct))]
public class GMPurchaseProductEditor : Editor {

  
    private GMPurchaseProduct purchaseProductMngr;
    private List<bool> productShow;

    private string[] enumStrings;
    private string[] productTypeStrings;


    void Awake() {
        enumStrings = GMPurchase.getLocalIDs();
        
        purchaseProductMngr = (GMPurchaseProduct)target; 
    }

    public override void OnInspectorGUI() {

        if ( productShow == null ) {

            productShow = new List<bool>(enumStrings.Length);

            for (int i = 0; i < enumStrings.Length; i++)
                productShow.Add(false);
        }

        if ( productTypeStrings == null ) {
            productTypeStrings = Enum.GetNames(typeof(GMProductType));
        }


        for (int i = 0; i < enumStrings.Length - 1; i++)
        {

            Color prevColor = GUI.color;
            GUI.color = Color.yellow;
            //GMPurchaseLocalEnum lEnum = (GMPurchaseLocalEnum) Enum.Parse( typeof( GMPurchaseLocalEnum ), enumStrings[ i ] );
            productShow[i] = EditorGUILayout.Foldout(productShow[i], purchaseProductMngr.productsList[i].localProductID + "   (" + SettingProject.Instance.PURCHASE_GC_PREFIX + purchaseProductMngr.productsList[i].serverProductID + ")");
            GUI.color = prevColor;

            if ( productShow[ i ] ) {
              

                GUILayout.BeginHorizontal(  );
                GUILayout.Label( "Name", GUILayout.Width( 100 ) );
                purchaseProductMngr.productsList[i].name = GUILayout.TextArea( purchaseProductMngr.productsList[i].name);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal(  );
                GUILayout.Label("Description", GUILayout.Width(100));
                purchaseProductMngr.productsList[i].description = GUILayout.TextArea( purchaseProductMngr.productsList[i].description);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Price", GUILayout.Width(100));

                string newPriceString = GUILayout.TextArea( purchaseProductMngr.productsList[ i ].price.ToString() );
                if ( string.IsNullOrEmpty( newPriceString ) )
                    newPriceString = "0";
                
                purchaseProductMngr.productsList[i].price = float.Parse(newPriceString);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Discount", GUILayout.Width(100));
                string newDiscountString = GUILayout.TextArea(purchaseProductMngr.productsList[i].discount.ToString());
                if (string.IsNullOrEmpty(newDiscountString))
                    newDiscountString = "0";

                purchaseProductMngr.productsList[i].discount = float.Parse(newDiscountString);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Bonus", GUILayout.Width(100));
                string newBonusString = GUILayout.TextArea(purchaseProductMngr.productsList[i].bonus.ToString());
                if (string.IsNullOrEmpty(newBonusString))
                    newBonusString = "0";

                purchaseProductMngr.productsList[i].bonus = float.Parse(newBonusString);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Count", GUILayout.Width(100));
                string newCountString = GUILayout.TextArea(purchaseProductMngr.productsList[i].count.ToString());
                if (string.IsNullOrEmpty(newCountString))
                    newCountString = "0";

                purchaseProductMngr.productsList[i].count = int.Parse(newCountString);
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("Slot number", GUILayout.Width(100));
                string newSlotString = GUILayout.TextArea(purchaseProductMngr.productsList[i].slotNumber.ToString());
                if (string.IsNullOrEmpty(newSlotString))
                    newSlotString = "0";

                purchaseProductMngr.productsList[i].slotNumber = int.Parse(newSlotString);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Category", GUILayout.Width(100));
                string newCategoryString = GUILayout.TextArea(purchaseProductMngr.productsList[i].category.ToString());
                if (string.IsNullOrEmpty(newCategoryString))
                    newCategoryString = "0";

                purchaseProductMngr.productsList[i].category = int.Parse(newCategoryString);
                GUILayout.EndHorizontal();



                GUILayout.BeginHorizontal();
                purchaseProductMngr.productsList[i].productType = (GMProductType)EditorGUILayout.Popup((int)purchaseProductMngr.productsList[i].productType, productTypeStrings, GUILayout.Width(150));
                purchaseProductMngr.productsList[i].locked =
                        GUILayout.Toggle(purchaseProductMngr.productsList[i].locked, "Locked", GUILayout.Width(100));

                purchaseProductMngr.productsList[i].enabled =
                GUILayout.Toggle(purchaseProductMngr.productsList[i].enabled, "Enabled", GUILayout.Width(100));
                GUILayout.EndHorizontal();

            }


            if (GUI.changed)
            {

                EditorUtility.SetDirty(target);
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
            }

            if (GMDataMngr.ProductMngr != purchaseProductMngr)
            {
                GMDataMngr.ProductMngr = purchaseProductMngr;
            }

        }
    }
}
