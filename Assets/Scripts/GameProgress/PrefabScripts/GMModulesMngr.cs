using UnityEngine;


[System.Serializable]
public class GMModulesMngr : MonoBehaviour {


    public bool useFuseBoxx;
    public string FuseBoxxApiString;
	// Use this for initialization
	void Start () {
	    if ( useFuseBoxx == true ) {
	        Debug.Log( "Try to start session!" );

	        FuseBoxxApiString = SettingProject.Instance.FUSEBOXX_ID;

	        FuseBoxx.enableLogs = true;
            FuseBoxx.SessionStartAction += GMDataMngr.SyncTool.ConfigureProgressFromServer;
	        FuseBoxx.StartSession( FuseBoxxApiString ); 
	    }
	}
	
	
}
