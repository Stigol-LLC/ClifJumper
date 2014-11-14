using System;
using System.Collections.Generic;

using System.IO;

using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using System.Reflection;
using UIEditor.Util;


public  class GMDataMngr {


    public static string[] progressVariablesList;
    public static string persistentDataPath ;
    private static string purchaseFileName = "pInfo.dat";
    private static string leaderboardFileName = "lInfo.dat";
    private static string productsFileName = "productInfo.dat";

    public static GameObject progressGO = GameObject.Find("GameProgressMngr");
    public static GameObject purchaseGO = GameObject.Find("PurchaseMngr");
    public static GMStatistics Statistics = progressGO.GetComponent<GMStatistics>(); 
    private static GMStatisticEventsHandler _statisticEventsHandler;
    public static GMStatisticEventsHandler StatisticEventsHandler
    {
        get {
            if (Statistics == null)
                return null;

          return Statistics.EventsHandler;
        }
    }

    public static GMSyncProgress SyncTool = progressGO.gameObject.GetComponent<GMSyncProgress>();
    public static GMModulesMngr ModulesMngr = progressGO.gameObject.GetComponent<GMModulesMngr>();
    public static GMProgress GameProgress;
    public static GMProgressMngr ProgressMngr = progressGO.gameObject.GetComponent<GMProgressMngr>();
    public static GMLeaderBoard LeaderBoardMngr = progressGO.gameObject.GetComponent<GMLeaderBoard>();

    public static GMPurchase PurchaseMngr = purchaseGO.gameObject.GetComponent<GMPurchase>();
    public static GMPurchaseProduct ProductMngr = purchaseGO.gameObject.GetComponent<GMPurchaseProduct>();

    private static readonly GMDataMngr instance = new GMDataMngr();
    public static GMDataMngr Instance  {
        get {
            
            return instance;
        }
    }

    public GMDataMngr(){
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

        log("Load data manager");

        if (progressGO == null)
            Debug.LogError("Not found GameProgressMngr on scene. Please add game object with correct name on scene!");

        GameProgress = LoadProgress(Path.Combine(Application.persistentDataPath, ProgressMngr.LocalProgressFileName));

        if (GameProgress == null){
            GameProgress = new GMProgress();
            SaveLocalProgress(GameProgress);
        }

        
        UpdateFieldsMetadata();

        if (Statistics != null)
          Statistics.initValues();
    }

    static GMFieldMetaData getFieldMetaData( string fieldName ) {

        foreach ( GMFieldMetaData fMetaData in ProgressMngr.FieldsMetaData ) {
            if ( fMetaData.name == fieldName )
                return fMetaData;
        }

        return null;
    }


     public void UpdateFieldsMetadata() {


         if ( GameProgress == null ) {
             log( "Progress is null" );
             return;
             
         }

        foreach (GMFieldMetaData fMetaData in ProgressMngr.FieldsMetaData)
        {
            fMetaData.checkFlag = false;
        }


        foreach (FieldInfo field in Reflection.FindOnlyPublicFields(GameProgress)) {

            GMFieldMetaData fMetaData = getFieldMetaData( field.Name );

            if ( fMetaData == null ) {

                fMetaData = new GMFieldMetaData();
                fMetaData.name = field.Name;
                fMetaData.typeString = field.FieldType.ToString();
                fMetaData.checkFlag = true;
                fMetaData.type = field.FieldType;
                NonSerializedAttribute notSerializedAttribute =
                        (NonSerializedAttribute) Attribute.GetCustomAttribute( field, typeof (NonSerializedAttribute) );

                if ( notSerializedAttribute != null ) {
                    fMetaData.isSerialized = false;
                } else {
                    fMetaData.isSerialized = true;
                }


                ProgressMngr.FieldsMetaData.Add( fMetaData );
            } else {
                fMetaData.name = field.Name;
                fMetaData.typeString = field.FieldType.ToString();
                fMetaData.checkFlag = true;
                fMetaData.type = field.FieldType;
            }
        }

        progressVariablesList = GMDataMngr.ProgressMngr.getFieldNames();
    }


    public static void SaveProgress(GMProgress progress, string progressPath)
    {
        BinaryFormatter bf = new BinaryFormatter();
        
       
        if (progress != null)
        {
            FileStream file = File.Create(progressPath);
            bf.Serialize(file, progress);
            log("Progress saved to "+ progressPath);
            file.Close();
        }
        else
        {
            log("Error: progress is not set");
        }

        
    }

   

    public static void SaveServerProgress(GMProgress progress)
    {
        SaveProgress( progress, SyncTool.getServerLocalFilePath() );
    }
   
    public static void ResetLocalProgress() {

         SaveProgress( new GMProgress(), ProgressMngr.getLocalProgressFilePath());
    }

    public static void SaveLocalProgress(GMProgress progress) {

         SaveProgress( progress, ProgressMngr.getLocalProgressFilePath());
    }

    public static GMProgress LoadLocalProgress() {
       GMProgress localProgress = LoadProgress(ProgressMngr.getLocalProgressFilePath());

       // return null;
          return localProgress;
    }

    public static GMProgress LoadServerProgress() {
        GMProgress serverProgress = LoadProgress(SyncTool.getServerLocalFilePath());

        return serverProgress;
    }

     static GMProgress LoadProgress(string progressFullPath) {
         Debug.Log( "Start load progress at - " + progressFullPath );

         if ( File.Exists( progressFullPath ) ) {
       
             
             BinaryFormatter bf = new BinaryFormatter();
             FileStream fs = File.Open( progressFullPath, FileMode.Open );
        
             GMProgress newProgress = (GMProgress) bf.Deserialize( fs );

             fs.Close();

             return newProgress;
         } else {
            log( "No progress on path - " + progressFullPath );
             return null;
         }

    }


     private static string getPurchaseFilePath() {
         return persistentCombine( purchaseFileName );
     }

     private static string getLeaderBoardsFilePath() {
         return persistentCombine( leaderboardFileName );
     }

     private static string getProductsFilePath()
     {
         return persistentCombine(productsFileName);
     }

    static string persistentCombine( string fileName ) {
        if (persistentDataPath == null || persistentDataPath == "")
        {

            return Path.Combine(Application.persistentDataPath, fileName);
        }


        return Path.Combine(persistentDataPath, fileName);
    }



    public static void LoadLeaderBoardsData()
    {
        LeaderBoardMngr.updateLeaderboardData();
        object resultObject = deserializeClass(getLeaderBoardsFilePath());

        if (resultObject == null)
        {
            saveLeaderBoardData();
        }
        else
        {
            LeaderBoardMngr.LeaderBoardInfos = (List<GMLeaderBoardInfo>)resultObject;
        }
    }

     public static void LoadPurchaseData()
     {
         PurchaseMngr.updatePurchaseData();
         object resultObject = deserializeClass( getPurchaseFilePath() );

         if ( resultObject == null ) {
             savePurchaseData();
         } else {
             PurchaseMngr.PurchaseInfos = (List<GMPurchaseInfo>) resultObject;
         }
     }

     public static void LoadProductsData()
     {
    
         object resultObject = deserializeClass(getProductsFilePath());

         if (resultObject == null)
         {
             SaveProductsData();
         }
         else
         {
             ProductMngr.productsList = (List<GMProductInfo>)resultObject;
         }
     }

     public static void DeletePurchaseData()
     {
         deleteFile( getPurchaseFilePath() );
     }

    public static void DeleteLeaderBoardData() {
        deleteFile( getLeaderBoardsFilePath() );
    }

     private static void savePurchaseData()
     {
         serializeClass( getPurchaseFilePath(), PurchaseMngr.PurchaseInfos );
     }

     private static void saveLeaderBoardData()
     {
         serializeClass(getLeaderBoardsFilePath(), LeaderBoardMngr.LeaderBoardInfos);
     }

     public static void SaveProductsData()
     {
         serializeClass(getProductsFilePath(), ProductMngr.productsList);
     }

    private static void deleteFile(string filePath) {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private static object deserializeClass(string filePath) {
        object resultObject = null;
        if (File.Exists(filePath))
        {

           // updatePurchaseData();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(filePath, FileMode.Open);
            resultObject = bf.Deserialize(fs);

            fs.Close();
        }

        return resultObject;
    }

    private static void serializeClass( string filePath, object classObject ) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, classObject);

        file.Close();
    }


 
     static void log( string message ) {
        Debug.Log( "Game progress mngr: " + message );
    }

}
