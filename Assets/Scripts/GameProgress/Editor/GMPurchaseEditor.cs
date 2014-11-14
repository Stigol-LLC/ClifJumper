using System;
using System.Linq;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(GMPurchase))]
public class GMPurchaseEditor :Editor {

    private GMPurchase purchaseMngr;



    void Awake() {
        purchaseMngr = (GMPurchase) target;
      
    }

    public override void OnInspectorGUI() {


        foreach ( GMPurchaseInfo pInfo in purchaseMngr.PurchaseInfos ) {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "ID_Local", GUILayout.Width( 200 ) );
            EditorGUILayout.LabelField(
                    "ID_Server (" + SettingProject.Instance.PURCHASE_GC_PREFIX + ")",
                    GUILayout.Width( 200 ) );
            EditorGUILayout.LabelField( "Var_Name", GUILayout.Width( 150 ) );
            //   EditorGUILayout.LabelField("Var_Value", GUILayout.Width(50));

            if ( GUILayout.Button( "Del", GUILayout.Width( 40 ) ) ) {
                purchaseMngr.removePurchaseInfo( pInfo );
                return;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            pInfo.localIDIndex = EditorGUILayout.Popup(
                    pInfo.localIDIndex,
                    GMPurchase.getLocalIDs(),
                    GUILayout.Width( 200 ) );
            pInfo.serverIDIndex = EditorGUILayout.Popup(
                    pInfo.serverIDIndex,
                    SettingProject.Instance.SERVER_IAP_PRODUCTS,
                    GUILayout.Width( 200 ) );

            pInfo.varNameIndex = EditorGUILayout.Popup(
                    pInfo.varNameIndex,
                    GMDataMngr.progressVariablesList,
                    GUILayout.Width( 150 ) );

            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 20 );
        }

        if (GUILayout.Button("Add"))
        {
            purchaseMngr.addEmptyPurchaseInfo();
        }

        if (GUILayout.Button("Clear Purchase data file"))
        {
            GMDataMngr.DeletePurchaseData();
        }

        if ( GUI.changed ) {

            EditorUtility.SetDirty( target );
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();

        }

        GMDataMngr.ProductMngr.clearServerIDs();
        foreach (GMPurchaseInfo pInfo in purchaseMngr.PurchaseInfos)
        {

            if (pInfo.localIDIndex != 0)
                GMDataMngr.ProductMngr.setServerID(
                        GMPurchase.getLocalIDs()[pInfo.localIDIndex],
                        SettingProject.Instance.SERVER_IAP_PRODUCTS[pInfo.serverIDIndex]);
        }

        if (GMDataMngr.PurchaseMngr != purchaseMngr)
        {
            GMDataMngr.PurchaseMngr = purchaseMngr;
        }
    }
}
