using System;
using System.Collections.Generic;
using UnityEngine;


public enum GMProductType {
    nobuying,
    consumable,
    nonconsumable,
    
}

[Serializable]
public class GMProductInfo {
    public string localProductID = "";
    public string serverProductID = "";
    public string name = "";
    public string description = "";
    public float price;
    public float discount;
    public float bonus;
    public int count = 1;
    public bool locked = true;
    public bool enabled = true;
    public bool isActive = false;
    public GMProductType productType ; // 0 - consumable, 1 - non concumable, 2 - not buying item
    public int slotNumber ;
    public int category;
}

[Serializable]
public class GMPurchaseProduct : MonoBehaviour {

    public List<GMProductInfo> productsList;




    private List<bool> productShow;
        
        GMPurchaseProduct() {
       
     
        if (productsList == null)
        {
            productsList = new List<GMProductInfo>();

            string[] enumStrings = Enum.GetNames(typeof(GMPurchaseEnum));

            for (int i = 0; i < enumStrings.Length; i++)
            {
                GMProductInfo pInfo  = new GMProductInfo();
                pInfo.localProductID = enumStrings[i];
                pInfo.name = "Product" + (i + 1);
                productsList.Add( pInfo );
            }
        }
    }

    public void clearServerIDs() {
        foreach ( GMProductInfo pInfo in productsList ) {
            pInfo.serverProductID = "";
        }
    }

    public void setServerID( string localID, string serverID ) {
        Debug.Log( "trying to set" );
        foreach ( GMProductInfo pInfo in productsList ) {
            if ( pInfo.localProductID == localID ) {
                pInfo.serverProductID = serverID;
                Debug.Log( "SET!" );
            }
        }
    }

    public GMProductInfo getProductInfo( string localID ) {
        foreach (GMProductInfo pInfo in productsList) {
            if ( pInfo.localProductID == localID )
                return pInfo;
        }

        return null;
    }

	void Start () {
        GMDataMngr.LoadProductsData();
	}
	

}
