using System;
using System.IO;
using System.Threading;
using System.Runtime.Remoting.Messaging;

#if UNITY_IPHONE

using Social;
#endif
using UnityEngine;

using System.Collections.Generic;
using System.Reflection;
using UIEditor.Util;

public class GMSyncHelper : MonoBehaviour {

 
    public enum syncType {
        betterValue,
        newerValue,
        encreaseValue,
        configuredValue,
        dontMerge,
    }

    enum mergeProgressResult
    {
        firstResult,
        secondResult,
        firstChanged,
        secondChanged,
        notChanged,
        bothChanged,
        skipMerge,
        badMergeResult
    }

    public string[] syncTypeNames = new string[5] {
            "Better value",
            "Newer value",
            "Increase value",
            "Server configured",
            "Not server synced",
        };

    private bool shouldConfiguerValuesFromServer = true;

    [SerializeField]
    public bool enableServerSync;

    [SerializeField]
    private string _serverProgressFileName ;
    public string ServerProgressFileName
    {
        get {
            if ((_serverProgressFileName == null) || (_serverProgressFileName.Length == 0))
                return "ServerProgress.dat";
       
            
            return _serverProgressFileName;
        }

        set { _serverProgressFileName = value; }
    }
    private GMProgressMngr _progress = null;
    private GMProgressMngr Progress
    {
        get { return _progress != null ? _progress : _progress = gameObject.GetComponent<GMProgressMngr>(); }
    }


    public void enabledLogs( bool val ) {
#if UNITY_IPHONE
        //ICloud.Instance().enableLogs( val ); ///
#endif
    }

     Dictionary<string, object> getDictionaryFromProgress(GMProgress gProgres)
    {
        Dictionary<string, object> progressDictionary = new Dictionary<string, object>();

        foreach (FieldInfo field in Reflection.FindOnlyPublicFields(gProgres))
        {
            progressDictionary.Add(field.Name, field.GetValue(gProgres));
        }

        return progressDictionary;
    }


     Dictionary<string, GMFieldMetaData> getDictionaryFromFieldsMetadata()
    {
        Dictionary<string, GMFieldMetaData> metaDataDictionary = new Dictionary<string, GMFieldMetaData>();

        foreach (GMFieldMetaData fieldMetaData in GMDataMngr.ProgressMngr.FieldsMetaData)
        {
            metaDataDictionary.Add(fieldMetaData.name, fieldMetaData);
        }

        return metaDataDictionary;
    }

     GMProgress getProgressFromDictionary(Dictionary<string, object> pDictionary)
    {
        GMProgress progress = new GMProgress();

        foreach (FieldInfo field in Reflection.FindOnlyPublicFields(progress))
        {
            field.SetValue(progress, pDictionary[field.Name]);

         //   Debug.Log("Set value - " + pDictionary[field.Name]  + " for name - " + field.Name);
        }

        return progress;
    }

    public void ConfigureProgressFromServer() {
     
       Debug.Log( "start config" );

   //   Dictionary<string,string> appConfig =  FuseBoxx.GetAppGameConfig();


        if ( shouldConfiguerValuesFromServer == true ) {

            List<string> configVariableList = getConfiguredValuesNames();
         
            if ( configVariableList.Count > 0 ) {
             
                foreach ( string varName in configVariableList ) {
                   string configString = FuseBoxx.GetAppGameConfig( varName );

//                   string configString = "[Score :  3000]";
                    Debug.Log( "Config string - " + configString );
                    Dictionary<string, string> varConfigDictionary = new Dictionary<string, string>();

                    

                    if ( 
                        FuseBoxx.GetGameConfigDictionary( configString, out varConfigDictionary ) == 
                        false ) {
                        log( "Received bad config dictionary" );
                    } else {


                        foreach ( KeyValuePair<string, string> kvPair in varConfigDictionary ) {
                               Debug.Log( "Key - " + kvPair.Key + "  Value - " + kvPair.Value );
                           }

                        if (varConfigDictionary.Count != 1)
                        {
                            log( "Bad config dictionary parameters count" );
                        } else {



                            Debug.Log( "Config dict string - " + configString );
                            Debug.Log("Var name - " + varName);
                            if ( varConfigDictionary.ContainsKey( varName )) {
                                 string valString = varConfigDictionary[ varName ];

                                if ( GMDataMngr.ProgressMngr.setValueObject( varName, valString ) ) {
                                    log( varName + " the value set - " + valString );
                                   
                                } else {
                                    log( "Value not set" );
                                }

                            } else {
                                log( "There is any value in config dictionary for key - " + varName );
                            }
                        }
                    }

                    
                }


            } else {
                shouldConfiguerValuesFromServer = false;
            }
        }

        
    }



    List<string> getConfiguredValuesNames() {

        List<string> configuredValuesList = new List<string>();

        foreach ( GMFieldMetaData metaData in GMDataMngr.ProgressMngr.FieldsMetaData ) {
            if ( metaData.fieldSyncType == syncType.configuredValue) {
                configuredValuesList.Add( metaData.name );

                Debug.Log( "!Add" + metaData.name );
            }
        }

       return configuredValuesList;
    }

    public void SyncProgress() {
        Thread syncProgressThread = new Thread( SyncProgressAsync );
        syncProgressThread.Start();

    }

    public void SyncGameProgress()
    {
        SyncLocalProgress(true);
    }

    void SyncProgressAsync() {
        SyncLocalProgress(false);
        SyncServerProgress();
    }



   private void SyncLocalProgress(bool shouldUpdateGameProgress) {
        log( "SYNC: Start sync progress" );
        GMProgress mProgress = null;

        GMProgress localProgress = GMDataMngr.LoadLocalProgress();

        if ( localProgress == null ) { // no saved
            GMDataMngr.SaveLocalProgress( GMDataMngr.GameProgress );
            log( "SYNC: Saved game progress to local progress" );
        } else {

            mergeProgressResult progressResult = MergeProgress( GMDataMngr.GameProgress, localProgress, out mProgress );
           
            if ( progressResult != mergeProgressResult.badMergeResult ) {
                if ( shouldUpdateGameProgress == true ) {
                    log( "SYNC: Update game progress and local progress!" );
                    setGameProgress( mProgress );
                } else {
                    log( "SYNC: Update local progress!" );
                }

                setLocalProgress( mProgress );
            } else {
                log( "SYNC: Bad merge result" );
            }

        }

    }

    private void saveToICloud(GMProgress progress) {
 
        GMDataMngr.SaveServerProgress( progress );
#if UNITY_IPHONE
        ICloud.Instance().SaveFile( getServerLocalFilePath(), ServerProgressFileName, saveToICloudCallback );
#endif
    }

    private void saveToICloudCallback(string callBackResult) {
        if ( string.IsNullOrEmpty( callBackResult ) ) {
            log( "Saved successfully to cloud" );
        } else {
            log( "Not saved to cloud. Error message - " + callBackResult );
           
        }
    }

    private void SyncServerProgress() {

        

        if ( enableServerSync ) {
            log( "Start sync with Icloud" );
#if UNITY_IPHONE
            if ( ICloud.Instance().AccessAllowed() ) {
                ICloud.Instance().FileExistsInIcloud( GMDataMngr.SyncTool.ServerProgressFileName, fileExistsCallback );
            } else {
                log( "ICloud acess is not allowed" );
            }
#endif
        } else {
            log( "Sync is not enabled" );
        }
    }

    private void clearCloudProgress()
    {
        File.Delete(getServerLocalFilePath());
    }

    public string getServerLocalFilePath() {
        return Path.Combine(GMDataMngr.persistentDataPath, ServerProgressFileName);
    }

   

    private void fileExistsCallback(string callBackResult) {
        log( "File exists callback - " + callBackResult );
        if ( string.IsNullOrEmpty( callBackResult ) ) { // file exists
            log("File exists in ICloud");
            clearCloudProgress();
#if UNITY_IPHONE
            ICloud.Instance().LoadFile(GMDataMngr.SyncTool.ServerProgressFileName, getServerLocalFilePath(), loadCloudFileCallback);
#endif
        } else {
            setServerProgress(GMDataMngr.LoadLocalProgress());
            log("File doesn't exists in ICloud. Message - " + callBackResult);
        }

    }

    private void loadCloudFileCallback( string callBackResult ) {
        if ( File.Exists( getServerLocalFilePath() ) ) {
            log("SYNC: Start cloud sync progress");
            GMProgress mProgress = null;



            mergeProgressResult mResult = MergeProgress(GMDataMngr.LoadLocalProgress(), GMDataMngr.LoadServerProgress(), out mProgress);

            if (mResult == mergeProgressResult.bothChanged)
            {
                setServerProgress( mProgress );
                setLocalProgress(mProgress);
                log("SYNC: Update cloud progress and local progress!");
            }
            else if (mResult == mergeProgressResult.firstChanged)
            {
                setLocalProgress(mProgress);
                log("SYNC: Update local progress");
            }
            else if (mResult == mergeProgressResult.secondChanged)
            {
                setServerProgress(mProgress);
                log("SYNC: Update server progress");
            }
            else
            {
                log("SYNC: No changes");
            }
        } else {
            log( "File didn't load from iCloud" );
        }
    }

    public void setGameProgress (GMProgress progress) {
        GMDataMngr.GameProgress = progress;
   
    }

    public void setLocalProgress(GMProgress progress)
    {
     
        GMDataMngr.SaveLocalProgress( progress );
    }

    public void setServerProgress(GMProgress progress)
    {
        saveToICloud( progress );
    }

    private  mergeProgressResult MergeProgress(GMProgress p1, GMProgress p2, out GMProgress mProgress)
    {
        Dictionary<string, object> p1Dictionary = getDictionaryFromProgress(p1);
        Dictionary<string, object> p2Dictionary = getDictionaryFromProgress(p2);
        Dictionary<string, GMFieldMetaData> metaDataDictionary = getDictionaryFromFieldsMetadata();

        Dictionary<string, object> mDictionary = new Dictionary<string, object>();

        mergeProgressResult mResult = mergeProgressResult.notChanged;
        mProgress = null;

       
        foreach (KeyValuePair<string, GMFieldMetaData> metaDataKV in metaDataDictionary)
        {
            string fieldName = metaDataKV.Value.name;

            object obj1 = null;
            object obj2 = null;

            if ( p1Dictionary.ContainsKey( fieldName ) ) {
                obj1 = p1Dictionary[fieldName];
            }

            if ( p2Dictionary.ContainsKey( fieldName ) ) {
                obj2 = p2Dictionary[fieldName];
            }

            if (obj1 == null)
            {
                if (obj2 != null) // localObj == null pObj != null
                {
                    log("Merge error: bad field name - " + fieldName);
                    return mergeProgressResult.badMergeResult;
                }
                else // localObj == null pObj == null
                {
                    log("Merge error: bad field name - " + fieldName);
                    return mergeProgressResult.badMergeResult;
                }
            }
            else
            {
                if (obj2 == null) // localObj != null pObj == null
                {
                    log("Merge: field " + fieldName + " added to progress");
                    mDictionary.Add(fieldName, obj1);
                }
                else // localObj != null pObj != null
                {
                    object mergedObject = new object();
                    mergeProgressResult mObjectResult = mergeObjects(obj1, obj2, out mergedObject, metaDataKV.Value);

                   
                    if ( mObjectResult == mergeProgressResult.firstResult ) {
                        mObjectResult = mergeProgressResult.secondChanged;
                    }

                    if (mObjectResult == mergeProgressResult.secondResult)
                    {
                        mObjectResult = mergeProgressResult.firstChanged;
                    }

                   
                    if ( mObjectResult == mergeProgressResult.badMergeResult ) {
                        return mergeProgressResult.badMergeResult;
                    }


                    mResult = getMergeResult( mResult, mObjectResult );


                    if (mObjectResult != mergeProgressResult.skipMerge)
                         log("Merge result - " + mResult);
                    else 
                        log("Merge result - " + "skipped");
                    

                    mDictionary.Add(fieldName, mergedObject);

                    p2Dictionary.Remove( fieldName );
                }
            }
        }


        
        if ( p2Dictionary.Count != 0 ) {
            log( "Error - some parameters are not set to support the old version" );
            return mergeProgressResult.badMergeResult;
        }

     

        mProgress = getProgressFromDictionary( mDictionary );

        return mResult;
    }

    mergeProgressResult getMergeResult(mergeProgressResult prevResult, mergeProgressResult objectMergeResult) {

        if ( objectMergeResult == mergeProgressResult.skipMerge ) 
            return prevResult;

        if ((objectMergeResult != mergeProgressResult.firstChanged) && 
            (objectMergeResult != mergeProgressResult.secondChanged) &&
            (objectMergeResult != mergeProgressResult.notChanged)) // validation parameter
            return mergeProgressResult.badMergeResult;

        if ( prevResult == mergeProgressResult.bothChanged )
            return mergeProgressResult.bothChanged;

        if (objectMergeResult == mergeProgressResult.notChanged)
            return prevResult;

        if ( prevResult == mergeProgressResult.notChanged )
            return objectMergeResult;

        if ( prevResult == objectMergeResult )
            return objectMergeResult;

        if (((prevResult == mergeProgressResult.firstChanged) && (objectMergeResult == mergeProgressResult.secondChanged)) ||
            ((prevResult == mergeProgressResult.secondChanged) && (objectMergeResult == mergeProgressResult.firstChanged)))
            return mergeProgressResult.bothChanged;
        
        return mergeProgressResult.badMergeResult; // not known result

    }

    private mergeProgressResult mergeObjects(object obj1, object obj2, out object mObject, GMFieldMetaData metaData)
    {

       // log( "merge " + obj1 + " with " + obj2 + "  Sync type - " + metaData.fieldSyncType);

        mObject = null;



        switch (metaData.fieldSyncType)
        {
            case GMSyncHelper.syncType.newerValue:
                
                break;
                

            case GMSyncHelper.syncType.dontMerge:
                mObject = obj1;
                return mergeProgressResult.skipMerge;
                

            case GMSyncHelper.syncType.encreaseValue:
                
                break;

            case GMSyncHelper.syncType.betterValue:


                int compareResult = compareObjects( obj1, obj2, metaData.type );

                if (compareResult == 1)
                {
                    mObject = obj1;
                    return mergeProgressResult.firstResult;

                }
                else if (compareResult == -1)
                {
                    mObject = obj2;
                    return mergeProgressResult.secondResult;
                }
                else if (compareResult == 0)
                {
                    mObject = obj1;
                    return mergeProgressResult.notChanged;
                }
            break;

            case GMSyncHelper.syncType.configuredValue:
                mObject = obj1;
                return mergeProgressResult.skipMerge;
               
        }

        return mergeProgressResult.badMergeResult;
    }



    private int compareObjects(object obj1, object obj2, Type objType)
    { //0 - same , 1 - obj1 greater , -1 - obj2 greater, -2 - error
        try
        {


            if (objType == typeof(int))
            {

                int iObj1 = checked((int)obj1);
                int iObj2 = checked((int)obj2);



                return compareObjectI(iObj1, iObj2);
            }


            if (objType == typeof(float))
            {
                float fObj1 = checked((float)obj1);
                float fObj2 = checked((float)obj2);

                return compareObjectF(fObj1, fObj2);

            }

            if (objType == typeof(string))
            {
                string sObj1 = checked((string)obj1);
                string sObj2 = checked((string)obj2);

                return compareObjectS(sObj1, sObj2);
            }

        }
        catch (Exception)
        {
            log("Object type error!");

            return -2;
        }

        return -2;
    }

    private int compareObjectI(int obj1, int obj2)
    {
        
        return compareObjectF(obj1, obj2);
    }

    private  int compareObjectF(float obj1, float obj2)
    {

    

        if (obj1 > obj2)
            return 1;

        if (obj2 > obj1)
            return -1;

        if (obj1 == obj2)
            return 0;

        return -2;
    }


    private  int compareObjectS(string obj1, string obj2)
    {
        return string.Compare(obj1, obj2);
    }

    private void log(string message)
    {
        Debug.Log("Sync helper: " + message);
    }
 
}
