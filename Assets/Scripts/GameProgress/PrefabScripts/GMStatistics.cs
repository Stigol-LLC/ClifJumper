using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UIEditor.Util;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[RequireComponent(typeof(GMProgressMngr))]
[Serializable]
public class GMStatistics : MonoBehaviour {

  
    public bool isStatisticsSetEnabled = true;

    public string FuseBoxxApiString;
//
//    [SerializeField]
//    public List<GMStatisticEvent> StatisticEventsList;



    public List<GMStatisticEvent> StatisticEventsList;

 

   
    public string[] delegateNamesList;
    public string[] variablesTypesList = new []{"Value", "Count", "Group"};




    private GMStatisticEventsHandler _eventsHanlder;
    public GMStatisticEventsHandler EventsHandler {
        get {
            if ( _eventsHanlder != null ) {
                return _eventsHanlder;
            } else {
                _eventsHanlder = new GMStatisticEventsHandler();
                _eventsHanlder.statisticsDelegate = this;
                return _eventsHanlder;
            }
        }

    }

 

    public bool sendHandlerMethodEvent( string methodName, Hashtable varHashtable ) {

        int methodIndex = -1;

        for ( int i = 0; i < delegateNamesList.Length; i ++ ) {
            if ( delegateNamesList[ i ] == methodName ) {
                methodIndex = i;
                break;
            }
        }

        if ( methodIndex == -1 ) {
            log( "event method not found" );
            return false;
        } else {
            log( "start sending event from " + methodName );
        }

        List<GMStatisticEvent> methodNameEvents = getStatisticEvents( methodName );

        if ( methodNameEvents.Count == 0 ) {
            log( "event method is not set" );
            return false;
        }

        if ( methodNameEvents.Count > 1 ) {
            log( "Sending to multyple events" );
        }

        foreach ( GMStatisticEvent sEvent in methodNameEvents ) {
            string eventSentName = sendEvent( sEvent, varHashtable );

            if ( eventSentName == null ) {
                log( "Error bad event atributes" );
            } else {
                log( "Event sent with name " + eventSentName );
            }

        }

        return true;
    }





    private string sendEvent( GMStatisticEvent eventInfo, Hashtable varHashtable ) {
        string eventName = eventInfo.eventName;

        if ( eventName.Length == 0 )
            eventName = null;


        if ( eventInfo.eventName != delegateNamesList[ 0 ] ) {
            GMFieldMetaData fmData = GMDataMngr.ProgressMngr.getFieldMetaDataWithName( eventInfo.parameterVarName );

            if ( fmData == null ) {
                log( "Bad event parameter" );
                return null;
            } else {
                object fieldValue = GMDataMngr.ProgressMngr.getValueObjectForName( fmData.name );
                object value = GMDataMngr.ProgressMngr.convertToType( fieldValue, eventInfo.parameterEndRange.rangeValue.GetType() );
                if (eventInfo.parameterTypeIndex == 0) {

                    if (fieldValue != null){
                        FuseBoxx.RegisterEvent( eventName, fmData.name, fieldValue.ToString(), varHashtable );
                    }
                    else
                    {
                        log("Can't retrieve the value from field!");
                        return null;
                    }
                }

                if (eventInfo.parameterTypeIndex == 1) {
                    FuseBoxx.RegisterEvent( eventName, fmData.name, "1", varHashtable );
                }

                if (eventInfo.parameterTypeIndex == 2){
                    string rangeName = GMDataMngr.ProgressMngr.FindValueRange(eventInfo, value);
                    if (rangeName != ""){
                        log("Range name = " + rangeName);
                        FuseBoxx.RegisterEvent(eventName, fmData.name, rangeName, varHashtable);
                    }else{
                        log("NOT find range name");
                    }
                }
            }


        } else {

//            FuseBoxx.RegisterEvent( eventName );

        }


        return eventName;
    }



    public List<GMStatisticEvent> getStatisticEvents( string methodName ) {
        List<GMStatisticEvent> result = new List<GMStatisticEvent>();


        foreach ( GMStatisticEvent sEvent in StatisticEventsList ) {
        
            if ( sEvent.eventDelegateName == methodName ) {
                result.Add( sEvent );
             

            }
        }

        return result;
    }

    public void initValues() {

        clearEventLists();
        List<string> tNamesList = new List<string>();


        foreach ( MethodInfo mInfo in  Reflection.FindPublicMethods( EventsHandler ) ) {

            tNamesList.Add( mInfo.Name );
        }

        delegateNamesList = new string[ tNamesList.Count + 1 ];
        delegateNamesList[ 0 ] = "Not defined";
        int i = 1;
        foreach ( string eventName in tNamesList ) {
            delegateNamesList[ i ] = eventName;
            i++;
        }



    }


    public void removeLastEventInfo() {
        if ( StatisticEventsList.Count != 0 )
            StatisticEventsList.RemoveAt( StatisticEventsList.Count - 1 );

    }

    public void addEmptyEventInfo() {
        Debug.Log( "ADD EMPTY" );

        GMStatisticEvent newEvent = new GMStatisticEvent();
        newEvent.eventName = "";

        StatisticEventsList.Add( newEvent );
    

        if (StatisticEventsList == null)
            StatisticEventsList = new List<GMStatisticEvent>();

      //  Debug.Log( "STATISTICS EVENT LIST - " + StatisticEventsList );

    }

    private void clearEventLists() {
        //EventInfoList.Clear();

    }

    public void removeLastRange(int i) {
        if (StatisticEventsList[i].parameterRanges.Count > 1){
            StatisticEventsList[i].parameterRanges.RemoveAt(StatisticEventsList[i].parameterRanges.Count-1);
        }
    }

    public void addEmptyRange(int i) {
        GMStatisticsRange newRange = new GMStatisticsRange();
        newRange.rangeName = "";

        StatisticEventsList[i].parameterRanges.Add( newRange );

    }


    void log( string message ) {
        Debug.Log( "Statistics: " + message );
    }




}
