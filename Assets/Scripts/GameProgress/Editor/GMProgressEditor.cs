using System;

using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GMProgressMngr))]
public class GMProgressEditor : Editor {


    public override void OnInspectorGUI()
    {

        GUILayout.BeginHorizontal();
        GUILayout.Label("Local progress file name - ", GUILayout.Width(200));
        GMDataMngr.ProgressMngr.LocalProgressFileName = GUILayout.TextArea(GMDataMngr.ProgressMngr.LocalProgressFileName, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        foreach (GMFieldMetaData fieldMetaData in GMDataMngr.ProgressMngr.FieldsMetaData)
        {

            GUILayout.BeginHorizontal(  );

            Color prevColor = GUI.color;
            GUI.color = Color.magenta;
            GUILayout.Label(fieldMetaData.name);


            GUI.color = Color.yellow;


            object fieldValue = GMDataMngr.ProgressMngr.getValueObjectForName(fieldMetaData.name);
            if (fieldValue != null)
                GUILayout.Label(fieldValue.ToString(), GUILayout.Width(50));


            if (fieldMetaData.isSerialized)
            {
                GUI.color = Color.green;
                GUILayout.Label("Serialized", GUILayout.Width(100));
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("Not serialized", GUILayout.Width(100));
            }
            GUI.color = prevColor;

            string[] splitType = fieldMetaData.typeString.Split('.');
            GUILayout.Label("(" + splitType[splitType.Length - 1] + ")", GUILayout.Width( 70 ));

            fieldMetaData.isCrypted = GUILayout.Toggle(fieldMetaData.isCrypted, "Crypted", GUILayout.Width(70));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
           GUILayout.Space( 180 );
           GUILayout.Label("Sync type - ", GUILayout.Width(80));

            if ( GMDataMngr.SyncTool != null ) {
                if (((int)fieldMetaData.fieldSyncType >= 0) &&
                ((int)fieldMetaData.fieldSyncType < GMDataMngr.SyncTool.syncTypeNames.Length))
                    GUILayout.Label(GMDataMngr.SyncTool.syncTypeNames[(int)fieldMetaData.fieldSyncType], GUILayout.Width(120));
            else
                {
                    GUILayout.Label( "Bad sync value" , GUILayout.Width( 120 ));
                }
            } else {
                GUILayout.Label("Sync is not enabled", GUILayout.Width(120));
            }

            GUILayout.EndHorizontal();
        }
//
//        if ( GUILayout.Button( "Config values" ) ) {
//            GMDataMngr.SyncTool.ConfigureProgressFromServer();
//        }

            EditorUtility.SetDirty(target);
       
    }

    }

    

