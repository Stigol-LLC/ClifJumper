using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
[CustomEditor(typeof(GMStatistics))]
public class GMStatisticsEditor : Editor {

    public override void OnInspectorGUI() {
        int eventCount = GMDataMngr.Statistics.StatisticEventsList.Count;

        GUILayout.Space( 20 );

        GUILayout.BeginVertical(  );
        Color prevColor = GUI.color;
        GUI.color = Color.red;
        GUILayout.Label( "Events sending delegates" );
        GUILayout.Space(10);

        GUI.color = prevColor;
        for (int i = 0; i < eventCount; i++ ) {

            GUILayout.BeginHorizontal(  );
            prevColor = GUI.color;
            GUI.color = Color.yellow;
            GUILayout.Label("Event " + (i + 1).ToString() + ":", GUILayout.Width(120));
            GUI.color = prevColor;
            GMDataMngr.Statistics.StatisticEventsList[i].eventName = GUILayout.TextArea(GMDataMngr.Statistics.StatisticEventsList[i].eventName, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            if (GMDataMngr.Statistics.StatisticEventsList[i].eventName != ""){
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Method:", GUILayout.Width(100));
                GMDataMngr.Statistics.StatisticEventsList[i].eventDelegateIndex = EditorGUILayout.Popup(GMDataMngr.Statistics.StatisticEventsList[i].eventDelegateIndex, GMDataMngr.Statistics.delegateNamesList, GUILayout.Width(160));
                GUILayout.EndHorizontal();

                if (GMDataMngr.Statistics.StatisticEventsList[i].eventDelegateIndex != 0){
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Parameter var:", GUILayout.Width(100));
                    GMDataMngr.Statistics.StatisticEventsList[i].parameterVarIndex = EditorGUILayout.Popup(GMDataMngr.Statistics.StatisticEventsList[i].parameterVarIndex, GMDataMngr.progressVariablesList, GUILayout.Width(120));
                    GUILayout.EndHorizontal();

                    if (GMDataMngr.Statistics.StatisticEventsList[i].parameterVarIndex != 0){
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label("Parameter type:", GUILayout.Width(100));
                        GMDataMngr.Statistics.StatisticEventsList[i].parameterTypeIndex = EditorGUILayout.Popup(GMDataMngr.Statistics.StatisticEventsList[i].parameterTypeIndex, GMDataMngr.progressVariablesList, GUILayout.Width(120));
                        GUILayout.EndHorizontal();

                        if (GMDataMngr.Statistics.StatisticEventsList[i].parameterTypeIndex == 2){

                            if (GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges == null){
                                GMDataMngr.Statistics.addEmptyRange(i);
                            }
                            int rangesCount = GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges.Count;
                            if (rangesCount > 0){
                                
                            } else {
                                GMStatisticsRange range = new GMStatisticsRange();
                                range.rangeName = "";
                                GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges = new List<GMStatisticsRange>();
                                GMDataMngr.Statistics.StatisticEventsList[i].parameterEndRange = range;
                                GMDataMngr.Statistics.addEmptyRange(i);
                            }

                            for (int j = 0; j < rangesCount; j++ ) {
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(40);
                                GUILayout.Label("Range "+(j+1)+" Name:", GUILayout.Width(90));
                                GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges[j].rangeName = GUILayout.TextArea(GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges[j].rangeName, GUILayout.Width(100));
                                if (j > 0){
                                    GUILayout.Label(GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges[j-1].rangeValue+" - ", GUILayout.Width(40));
                                }
                                else{
                                    GUILayout.Label("to:", GUILayout.Width(40));
                                }
                                
                                GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges[j].rangeValue = EditorGUILayout.FloatField(GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges[j].rangeValue, GUILayout.Width(50));
                                GUILayout.EndHorizontal();
                            } 

                            GUILayout.BeginHorizontal();
                            GUILayout.Space(40);
                            GUILayout.Label("Range "+(rangesCount+1)+" Name:", GUILayout.Width(90));
                            GMDataMngr.Statistics.StatisticEventsList[i].parameterEndRange.rangeName = GUILayout.TextArea(GMDataMngr.Statistics.StatisticEventsList[i].parameterEndRange.rangeName, GUILayout.Width(100));
                            if (rangesCount > 0){
                                GUILayout.Label("after:   "+GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges[rangesCount-1].rangeValue, GUILayout.Width(80));
                            } else { 
                                GUILayout.Label("after:   "+0, GUILayout.Width(80));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal(  );
                            GUILayout.Space(140);
                            if ( GUILayout.Button( "+" , GUILayout.Width(50)) ) {
                                GMDataMngr.Statistics.addEmptyRange(i);
                            }
                            if ( GUILayout.Button( "-", GUILayout.Width(50) ) ) {
                                GMDataMngr.Statistics.removeLastRange(i);
                            }
                            GUILayout.EndHorizontal();
                        } else{
                            //GMDataMngr.Statistics.StatisticEventsList[i].parameterRanges = null;
                        }
                    } else {
                        GMDataMngr.Statistics.StatisticEventsList[i].parameterTypeIndex = 0;
                    }
                } else {
                    GMDataMngr.Statistics.StatisticEventsList[i].parameterVarIndex = 0;
                }

            } else {
                GMDataMngr.Statistics.StatisticEventsList[i].eventDelegateIndex = 0;
            }
            
           
            GUILayout.Space( 10 );
        }

        GUILayout.EndVertical();


        GUILayout.BeginHorizontal(  );
        if ( GUILayout.Button( "Add" ) ) {
            GMDataMngr.Statistics.addEmptyEventInfo();
        }

        if (GUILayout.Button("Remove"))
        {
            GMDataMngr.Statistics.removeLastEventInfo();
        }


        GUILayout.EndHorizontal();

        if ( GUI.changed ) {
            EditorUtility.SetDirty( target );


//            Debug.Log("progress vars - " + GMDataMngr.Statistics.progressVariablesList.Length);
//            Debug.Log("delegates list - " + GMDataMngr.Statistics.delegateNamesList.Length);
//            Debug.Log( "Event count - " + eventCount );
            for (int i = 0; i < eventCount; i++ ) {

                GMDataMngr.Statistics.StatisticEventsList[i].parameterVarName = GMDataMngr.progressVariablesList[
                        GMDataMngr.Statistics.StatisticEventsList[ i ].parameterVarIndex ];

                GMDataMngr.Statistics.StatisticEventsList[i].eventDelegateName = GMDataMngr.Statistics.delegateNamesList[
                        GMDataMngr.Statistics.StatisticEventsList[i].eventDelegateIndex];

            }
        }

    }
}
