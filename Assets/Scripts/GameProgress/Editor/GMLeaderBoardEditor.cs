using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (GMLeaderBoard))]
public class GMLeaderBoardEditor : Editor {

    private GMLeaderBoard leaderBoardMngr;
        void Awake() {
            leaderBoardMngr = (GMLeaderBoard) target;
        }


    public override void OnInspectorGUI() {

        foreach ( GMLeaderBoardInfo lInfo in leaderBoardMngr.LeaderBoardInfos ) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "ID_Local", GUILayout.Width( 200 ) );
            EditorGUILayout.LabelField(
                    "ID_Server (" + SettingProject.Instance.PURCHASE_GC_PREFIX + ")",
                    GUILayout.Width( 200 ) );
            EditorGUILayout.LabelField( "Var_Name", GUILayout.Width( 150 ) );


            if ( GUILayout.Button( "Del", GUILayout.Width( 40 ) ) ) {
                leaderBoardMngr.removeLeaderBoardInfo( lInfo );
                return;
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            lInfo.localIDIndex = EditorGUILayout.Popup(
            lInfo.localIDIndex,
        GMDataMngr.LeaderBoardMngr.getLocalIDs(),
        GUILayout.Width(200));



            lInfo.serverIDIndex = EditorGUILayout.Popup(
                    lInfo.serverIDIndex,
                    SettingProject.Instance.SERVER_GC_IDs,
                    GUILayout.Width(200));

            lInfo.varNameIndex = EditorGUILayout.Popup(
                    lInfo.varNameIndex,
                    GMDataMngr.progressVariablesList,
                    GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);
        }

        if (GUILayout.Button("Add"))
        {
            leaderBoardMngr.addEmptyPurchaseInfo();
        }

        if (GUILayout.Button("Clear Leaderboard data file"))
        {
            GMDataMngr.DeleteLeaderBoardData();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
	
}
