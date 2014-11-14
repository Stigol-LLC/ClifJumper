using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UIEditor.Util;
using System.Reflection;


[System.Serializable]
[CustomEditor(typeof(GMSyncProgress))]

public class GMSyncEditor : Editor {

    private int syncTypePopupIndex ;
    private GMProgressMngr _progressComponent = null;
    public GMProgressMngr ProgressComponent {
        get {
            if ( _progressComponent != null )
                return _progressComponent;
            else {
                GMSyncProgress gmSync = (GMSyncProgress) target;
                _progressComponent = gmSync.GetComponent<GMProgressMngr>();
                return _progressComponent;
            }
        }
    }

    

    public override void OnInspectorGUI() {
        GMDataMngr.SyncTool.enableServerSync = GUILayout.Toggle(GMDataMngr.SyncTool.enableServerSync, "Enable server sync");

        if ( GMDataMngr.SyncTool.enableServerSync ) {
            GUIFileNames();
            GUIValSettings();

        }
        if (GUI.changed)
            EditorUtility.SetDirty(target);

    }

    void GUIFileNames() {
       
        GUILayout.BeginHorizontal();
        GUILayout.Label("Server progress file name - ", GUILayout.Width(200));
        GMDataMngr.SyncTool.ServerProgressFileName = GUILayout.TextField(GMDataMngr.SyncTool.ServerProgressFileName, GUILayout.Width(200));
        GUILayout.EndHorizontal();
    }

    void GUIValSettings()
    {
        int i = 0;
        GUILayout.Space(10);
        Color prevColor = GUI.color;
        GUI.color = Color.red;
        GUILayout.Label("Values settings");
        GUI.color = prevColor;

        

        foreach (GMFieldMetaData fieldMetaData in GMDataMngr.ProgressMngr.FieldsMetaData)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label(fieldMetaData.name, GUILayout.Width(200));

            GUILayout.Label( "SyncType", GUILayout.Width( 90 ));
            fieldMetaData.fieldSyncType = (GMSyncHelper.syncType)EditorGUILayout.Popup((int)fieldMetaData.fieldSyncType, GMDataMngr.SyncTool.syncTypeNames, GUILayout.Width(120));
          
            GUILayout.EndHorizontal();

            i++;
        }

        if ( GUI.changed ) {
            EditorUtility.SetDirty( target );
        }
    }

 
}
