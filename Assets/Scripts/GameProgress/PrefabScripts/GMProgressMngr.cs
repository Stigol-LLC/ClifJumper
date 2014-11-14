
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UIEditor.Util;

[System.Serializable]
[RequireComponent (typeof(GMSyncProgress))]
public class GMProgressMngr : MonoBehaviour {

    public List<GMFieldMetaData> FieldsMetaData;

    private  GameObject progressGO;


    private string _localProgressFileName;
    public string LocalProgressFileName {
        get {
            if ( ( _localProgressFileName == null ) ||
                 ( _localProgressFileName.Length == 0 ) )
                return "LocalProgress.dat";

            return _localProgressFileName;
        }
        set { _localProgressFileName = value; }

    }


    void Start() {  
        GMDataMngr.persistentDataPath = Application.persistentDataPath;
        GMDataMngr.SyncTool.SyncProgress();
        GMDataMngr.SyncTool.ConfigureProgressFromServer();
         // default sync with iCloud on load
    }


    public GMFieldMetaData getFieldMetaDataWithName( string fieldName ) {
        foreach ( GMFieldMetaData fmData in FieldsMetaData ) {

        //    Debug.Log("FDATA NAME - " + fmData.name + "  Field Name - " + fieldName);

            if ( fmData.name == fieldName )
                return fmData;
        }

        return null;
    }


    public object getValueObjectForName(string fieldName)
    {
        foreach ( FieldInfo field in Reflection.FindOnlyPublicFields( GMDataMngr.GameProgress ) ) {
            if ( fieldName ==  field.Name) {
                return field.GetValue( GMDataMngr.GameProgress );
            }
        }
        return null;
    }

    public bool setValueObject(string fieldName, object fieldValue) {

        GMFieldMetaData mData = getFieldMetaDataWithName( fieldName );

        if ( mData == null )
            return false;

        object cFieldValue = convertToType(fieldValue, mData.type);
        
        foreach (FieldInfo field in Reflection.FindOnlyPublicFields(GMDataMngr.GameProgress))
        {
            if (fieldName == field.Name) {
               field.SetValue(GMDataMngr.GameProgress, cFieldValue);
               Debug.Log("SET VALUE - " + fieldValue + " FOR FIELD - " + fieldName);

                return true;

            }
        }
        return false;
    }

    public object convertToType(object obj, Type objType)
    {
        try
        {
            if (objType == typeof(int))
            {
                return Convert.ToInt32(obj);
            }

            if (objType == typeof(float))
            {
                return Convert.ToSingle(obj);
            }

            if (objType == typeof(string))
            {
                return obj;
            }

        }
        catch (Exception)
        {
           

            return null;
        }

        return null;
    }


    public string getLocalProgressFilePath(){
        if (GMDataMngr.persistentDataPath == null || GMDataMngr.persistentDataPath == ""){
            return Path.Combine(Application.persistentDataPath, LocalProgressFileName);
        }
        return Path.Combine(GMDataMngr.persistentDataPath, LocalProgressFileName);
    }


    public string[] getFieldNames() {
        List<string> fList = new List<string>();

        foreach ( GMFieldMetaData fData in FieldsMetaData ) {
            fList.Add( fData.name );
        }

        string[] resStrings = new string[fList.Count + 1];
        resStrings[ 0 ] = "Not defined";

        int i = 1;
        foreach ( string fName in fList ) {
            resStrings[ i ] = fName;
            i++;
        }

        return resStrings;
    }



    public void setSomeValues() {
//        progressContainer.val1 = 1;
//        progressContainer.val2 = 2;
//        progressContainer.val3 = 3;
//        progressContainer.val4 = 4;
    }


    public string FindValueRange(GMStatisticEvent eventInfo, object value){
        for (int i = 0; i < eventInfo.parameterRanges.Count; i++){
            if ((float) value <= eventInfo.parameterRanges[i].rangeValue){
                return eventInfo.parameterRanges[i].rangeName;
            }
        }
        if ((float) value > eventInfo.parameterEndRange.rangeValue){
            return eventInfo.parameterEndRange.rangeName;
        }
        return "";
    }

    void OnApplicationPause(bool paused) {
        if ( !paused ) {
            Debug.Log( "paused" );
         
           // GMDataMngr.SyncTool.SyncProgress();
        }
    }
}

	

