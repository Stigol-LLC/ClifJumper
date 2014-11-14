using System;
using System.Collections.Generic;
using Social;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class GMPurchaseInfo  {
    public string localID;
 //   public int increaseValue;
    public string varName;

    public int localIDIndex;
    public int serverIDIndex;
    public int varNameIndex;
}

[Serializable]
public class GMPurchase : MonoBehaviour {
    
   
    public List <GMPurchaseInfo> PurchaseInfos;
    private Action<string> buyExternCallBack ;
    private static string[] localEnumNames;


      public static string[] getLocalIDs()
      {
          if (  localEnumNames == null ) {
              string[] boardEnums = Enum.GetNames(typeof(GMPurchaseEnum));
              localEnumNames = new string[boardEnums.Length + 1];

              localEnumNames[0] = "None";

              for (int i = 1; i < localEnumNames.Length; i++)
              {
                  localEnumNames[i] = boardEnums[i - 1];

              }
          }


          return localEnumNames;
      }

    void Start() {
        Debug.Log( "start load purchase data" );
        GMDataMngr.LoadPurchaseData();

        Debug.Log("start initialize products");

        string[] productsListWithPrefix = new string[SettingProject.Instance.SERVER_IAP_PRODUCTS.Length];
        for ( int i = 0; i < productsListWithPrefix.Length; i++ ) {
            productsListWithPrefix[i] = SettingProject.Instance.PURCHASE_GC_PREFIX + SettingProject.Instance.SERVER_IAP_PRODUCTS[i]
            ;
            Debug.Log( "init - " + productsListWithPrefix[i] );
        }

        Debug.Log("start load products");
        //InAppPurchase.Instance().LoadProduct(loadProductsFinished);
        
    }

    public void updatePurchaseData()
    {

      
        foreach (GMPurchaseInfo pInfo in PurchaseInfos)
        {
            pInfo.localID = localEnumNames[pInfo.localIDIndex];
            pInfo.varName = GMDataMngr.progressVariablesList[pInfo.varNameIndex];
        }


    }


    void loadProductsFinished(string result) {
        Debug.Log( "Load finished with result - " + result );
    }

    public void addEmptyPurchaseInfo()
    {
        GMPurchaseInfo newInfo = new GMPurchaseInfo();
        PurchaseInfos.Add(newInfo);
    }

    public void removePurchaseInfo(GMPurchaseInfo pInfo)
    {
        PurchaseInfos.Remove(pInfo);
    }

    GMPurchaseInfo getPurchaseInfo( string localProductID ) {
    
        foreach ( GMPurchaseInfo pInfo in PurchaseInfos ) {
     
            if ( localProductID == pInfo.localID ) {
                return pInfo;
            }
        }

        return null;
    }
    public void buyProduct( string localProductID, Action<string> callBack  ) {
    
        GMPurchaseInfo pInfo = getPurchaseInfo( localProductID );
        buyExternCallBack = callBack;
        if ( pInfo == null ) {
            buyProductOperationComplete( "Can't find product info with local ID - " + localProductID );
            return;
        }

        GMProductInfo productInfo = GMDataMngr.ProductMngr.getProductInfo( pInfo.localID );

        if ( productInfo == null ) {
            buyProductOperationComplete( "Cant find product with local ID - " + pInfo.localID );
            return;
        }

        if ( String.IsNullOrEmpty( productInfo.serverProductID ) ) {
            buyProductOperationComplete("Product server ID is not set");
            return;
        }

        if (String.IsNullOrEmpty(pInfo.varName)) 
        {
            buyProductOperationComplete("Variable is not set");
            return;
        }

        Debug.Log("BUYING - " + ( SettingProject.Instance.PURCHASE_GC_PREFIX + productInfo.serverProductID));
       // InAppPurchase.Instance().Buy( SettingProject.Instance.PURCHASE_GC_PREFIX + pInfo.storeID, buyProductOperationComplete );
    }

    void buyProductOperationComplete( string result ) {
        log( "Buy result - " + result );
       // FuseAPI.register
        buyExternCallBack.Invoke( result );
    }


    void log( string message ) {
        Debug.Log( "GMPurchase: " + message );
    }
  
}
