using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public class FuseBoxx : MonoBehaviour {
    
    public static event Action SessionStartAction;

    public static bool enableLogs = false;
    private static string appKey = null;

    private static bool isSessionRunning;
	public static void StartSession (string theAppKey) {
	    RegisterActions();
	    appKey = theAppKey;
//        Thread fuseApiThread = new Thread( startSessionAsync );
//        fuseApiThread.Start();
        FuseAPI.StartSession(appKey);
	}

    static void startSessionAsync() {
        FuseAPI.StartSession(appKey);
    }

    public static void StopSession() {
        UnRegisterActions();
    }


    static void RegisterActions()
    {
        FuseAPI.SessionStartReceived += sessionStarted;
       
        // FuseAPI.AdAvailabilityResponse += adAvailabilityResponse;
        // FuseAPI.AdWillClose += adWillClose;
    }

    public static void RegisterEvent(string eventName) {
        if ( isSessionRunning == true ) {
            FuseBoxxLog( "Event sent - " + eventName );
            FuseAPI.RegisterEvent(eventName);
        } else {
            FuseBoxxLog("Can't send event - session not started");
        }
    }


    public static void RegisterEvent( string eventName, string paramName, string paramValue, string valueName, float val ) {

        if (isSessionRunning == true)
        {
            FuseBoxxLog("Event sent - " + eventName + "with parameter name - " + paramName + "and parameter value - " + paramValue + "and value name - " + valueName + " and value - " + val);
            FuseAPI.RegisterEvent(eventName, paramName, paramValue, valueName, val);
        }
        else
        {
            FuseBoxxLog("Can't send event - session not started");
        }
    }

    public static void RegisterEvent( string eventName, string paramName, string paramValue, Hashtable tableValues) {

        if ( isSessionRunning == true ) {
            FuseBoxxLog("Event sent - " + eventName + "with parameter name - " + paramName + "and parameter value - " + paramValue);
            FuseAPI.RegisterEvent( eventName, paramName, paramValue, tableValues );

            
        } else {
            FuseBoxxLog("Can't send event - session not started");
        }
    }

    

    public static Dictionary<string, string> GetAppGameConfig()
    {
        if ( isSessionRunning == true ) {
            Dictionary<string, string> configDictionary = FuseAPI.GetGameConfig();
            FuseBoxxLog( "App game config count - " + configDictionary.Count );
            return configDictionary;
        }
        else
        {
            FuseBoxxLog("Can't get app config - session not started");
            return null;
        }
    }

    public static string GetAppGameConfig( string configName ) {
        if ( isSessionRunning == true ) {
            string valueConfig = FuseAPI.GetGameConfigurationValue( configName );
            FuseBoxxLog( "Received config value - " + valueConfig + " for key - " + configName );
            return valueConfig;
        }
        else
        {
            FuseBoxxLog("Can't get app config - session not started");
            return null;
        }
    }

    public static bool GetGameConfigDictionary(string configString, out Dictionary<string, string> result)
    {
        
        result = null;
        if (string.IsNullOrEmpty(configString))
        {
            return false;
        }

        string fix_val = configString.Replace("[", "");
        fix_val = fix_val.Replace("]", "");
        string[] str_list = fix_val.Substring(0, fix_val.Length).Split(',');

        if (str_list == null || str_list.Length == 0)
        {
            FuseBoxxLog( "Cann't parse the config string. It is null or empty" );
            return false;
        }

        result = new Dictionary<string, string>(str_list.Length + 3);
        for (int i = 0; i < str_list.Length; ++i)
        {
            if (string.IsNullOrEmpty(str_list[i]))
            {
                FuseBoxxLog("Cann't parse the config string parameter.  It is null or empty");
                return false;
            }

            string fix_str = str_list[i].Replace(" ", "");
            string[] pairKV = fix_str.Substring(0, fix_str.Length).Split(':');
            
            if (pairKV.Length != 2)
            {

				FuseBoxxLog("Cann't parse the config string parameter.  Wrong format");
                return false;
            }

                result.Add(pairKV[0], pairKV[1]);
        }

        return true;
    }
    
    private static void sessionStarted() {
        FuseBoxxLog("Session started");
        isSessionRunning = true;
        SessionStartAction();
    }

    private static void adAvailabilityResponse(int isAdAvailable, int hasError)
    {
        
        if (isAdAvailable == 1)
        {
            FuseAPI.ShowAd();
        }
    }

    private static void adWillClose()
    {
        //Do Something...
    }

    private static void UnRegisterActions()
    {
        FuseAPI.SessionStartReceived -= sessionStarted;
       // FuseAPI.AdAvailabilityResponse -= adAvailabilityResponse;
      //  FuseAPI.AdWillClose -= adWillClose;
        isSessionRunning = false;
        FuseBoxxLog("Session stopped");
    }

    static void FuseBoxxLog( string message ) {
        if (enableLogs == false)
            return;

        Debug.Log( "FuseBoxx: " + message );
    }
  
}
