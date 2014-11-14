using UnityEditor;
using UnityEngine;
using System.Collections;

[System.Serializable]
[CustomEditor (typeof(GMModulesMngr))]
public class GMModulesMngrEditor : Editor
{

    public override void OnInspectorGUI() {
        GMDataMngr.ModulesMngr.useFuseBoxx = GUILayout.Toggle(GMDataMngr.ModulesMngr.useFuseBoxx, "FuseBoxx");


        if (GMDataMngr.ModulesMngr.useFuseBoxx == true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("FuseBoxx api", GUILayout.Width(100));

            Color prevColor = GUI.color;
            GUI.color = Color.green;
            EditorGUILayout.LabelField(SettingProject.Instance.FUSEBOXX_ID, GUILayout.Width(250));
            GUI.color = prevColor;

            GUILayout.EndHorizontal();
        }


        EditorUtility.SetDirty(target);

    }
}
