#region usings


using UnityEngine;

#endregion

public class SettingProject : ScriptableObject {
	public static string LinkFile = @"Assets\Resourses\Settings.asset";

	static SettingProject _inctance = null;
	public static  SettingProject SetScritableObject{
		get{
			return _inctance;
		}
		set{
			_inctance = value;
		}

	}

	static public SettingProject Instance{
		get{
            
			if(_inctance == null){

#if UNITY_EDITOR
				_inctance = (SettingProject)UnityEditor.AssetDatabase.LoadAssetAtPath(SettingProject.LinkFile, typeof(SettingProject));
				if(_inctance == null){
					_inctance = new SettingProject();
					UnityEditor.AssetDatabase.CreateAsset(_inctance, SettingProject.LinkFile);
					Debug.Log("Create SettingProject");

                    Debug.Log("Settings instance - " + _inctance);
					return _inctance;
				}

#else

#endif
                
				SettingProject settinsProject = Resources.Load("Settings") as SettingProject;
				if(settinsProject != null){
                    Debug.Log( "NOT NULL SETTINGS" );
					_inctance = settinsProject;
				}else{
                    Debug.Log( "NULL SETTINGS" );
					_inctance = new SettingProject();
				}
			    
			}

//            Debug.Log( "Settings instance - " + _inctance );

			return _inctance;
		}
	}

    public string PURCHASE_GC_PREFIX = "com.stigol.";
    public string FUSEBOXX_ID = "b9134f4a-ae96-4fa0-892b-518ed11a6807";
    public string STIGOL_TWITTER_PAGE = "stigolliketest";
    public string STIGOL_TWITTER_ID = "1512633530";
	
	public string TWEET_FOLLOW =  "StigolApptest";//StigolApptest
	public string TWEET_FOLLOW_CHBT =  "tst160";
	
    public string STIGOL_FACEBOOK_ID = "540066399384495"; //App Secret:	2ac574f1b5e185eb8a24436982ba1224
	public string FACEBOOK_APPID = "153551704824520";
	public string[] FACEBOOK_PERMISSIONS = new string[]{"publish_actions","user_likes"};

	public string CHBT_FACEBOOK_PAGEID = "175118882596443";
		
    public string ITUNES_LINK = "https://itunes.apple.com/vg/app/g15/id641029291";
	public string APPLE_ID = "641029291";


    public string[] SERVER_IAP_PRODUCTS = new string[]
    {
        "2048idle.savescore",
        "2048idle.hint"
    };

//    public string[] LOCAL_IAP_PRODUCTS = new string[]
//    {
//        "hint_purchase1",
//        "hint_purchase2"
//    };

    public string[] SERVER_GC_IDs = new string[]
    {
        "bralocks.totalbraopened"
    };

   


}