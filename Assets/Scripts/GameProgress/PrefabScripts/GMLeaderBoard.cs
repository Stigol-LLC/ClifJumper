using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;


[Serializable]
public class GMLeaderBoardInfo {
    public string localID;
    public string serverID;
    public string varName;

    public int localIDIndex;
    public int serverIDIndex;
    public int varNameIndex;
}

[Serializable]
public class GMLeaderBoard : MonoBehaviour {


    private Stack<GMLeaderBoardInfo> reportScoreStack; 
    public List<GMLeaderBoardInfo> LeaderBoardInfos;

    private string[] localEnumNames;

	// Use this for initialization

    public  GMLeaderBoard () {
        string[] leaderBoardEnums = Enum.GetNames(typeof(GMLeaderBoardEnum));
        localEnumNames = new string[leaderBoardEnums.Length + 1];

        localEnumNames[0] = "None";

        for (int i = 1; i < localEnumNames.Length; i++)
        {
            localEnumNames[i] = leaderBoardEnums[i - 1];

        }
    }

    public string[] getLocalIDs() 
    {
        return localEnumNames;
    }


	void Start () {
	  
        reportScoreStack = new Stack<GMLeaderBoardInfo>();
        GMDataMngr.LoadLeaderBoardsData();
	    authenticateUser( null );
	}

   
    public void removeLeaderBoardInfo(GMLeaderBoardInfo lInfo) {
        LeaderBoardInfos.Remove( lInfo );
    }

    public void addEmptyPurchaseInfo()
    {
        GMLeaderBoardInfo newInfo = new GMLeaderBoardInfo();
        LeaderBoardInfos.Add(newInfo);
    }

    public void updateLeaderboardData()
    {
        foreach (GMLeaderBoardInfo lInfo in LeaderBoardInfos)
        {
            lInfo.localID = GMDataMngr.LeaderBoardMngr.getLocalIDs()[lInfo.localIDIndex];
            lInfo.serverID = SettingProject.Instance.SERVER_GC_IDs[lInfo.serverIDIndex];
            lInfo.varName = GMDataMngr.progressVariablesList[lInfo.varNameIndex];
        }
    }



    public void reportScore( GMLeaderBoardEnum reportEnum ) {

        string localID = reportEnum.ToString();
        foreach ( GMLeaderBoardInfo lInfo in LeaderBoardInfos ) {
            if ( lInfo.localID == localID ) {
                reportScore( lInfo );
                return;
                
            }
        }
        log( "Failed to report score: local id not found" );
    }

    void reportScore(GMLeaderBoardInfo lInfo) {
        
            if ( lInfo.serverID != null ) {

                if ( lInfo.varName != null ) {  
                  reportScoreStack.Push( lInfo );
                  Debug.Log("Report score - " + lInfo.localID + "  with server id - " + SettingProject.Instance.PURCHASE_GC_PREFIX + lInfo.serverID + "  var name - " + lInfo.varName);
                  authenticateUser( reportScoreAuthCheck );
                    
            } else {
                log("server id is not set");
               
            }
        } else {
            log("var name is not set");
            
        }

    }

    void reportScoreAuthCheck(bool result) {
        if ( result ) {
            GMLeaderBoardInfo lInfo = reportScoreStack.Pop();

            int varValue = (int)GMDataMngr.ProgressMngr.getValueObjectForName(lInfo.varName);
            UnityEngine.Social.ReportScore(varValue, SettingProject.Instance.PURCHASE_GC_PREFIX + lInfo.serverID, reportScoreCallBack);
        }
    }

    void reportScoreCallBack( bool result ) {
        log( "score reported with result " + result.ToString() );
    }


    void log (string message)
    {
        Debug.Log( "GMLeaderBoard: " + message );
    }

    void authenticateUser( Action<bool> authCallBack ) {
        
        if ( !UnityEngine.Social.localUser.authenticated ) {
            if ( authCallBack == null )
                UnityEngine.Social.localUser.Authenticate( callback => Debug.Log( "User authenticate " + callback ) );
            else {
                UnityEngine.Social.localUser.Authenticate( authCallBack );
            }
        } else {
            if ( authCallBack != null ) {
                authCallBack( true );
            }
        }
    }

}


