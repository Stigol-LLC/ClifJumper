using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using FuseNativeAPI;


public class FuseAPI_UnityEditor : FuseAPI
{
	public bool logging = false;
	public static bool debugOutput = false;

	#region Session Creation
	[DllImport("__Internal")]
	private static extern void FuseAPI_StartSession(string gameId);

	void Awake()
	{
		if (logging)
		{
			FuseAPI_UnityEditor.debugOutput = true;
		}
	}

	new public static void StartSession(string gameId)
	{
		// set callbacks for native wrapper
		FuseNative.SessionStartReceived += _SessionStartReceived;
		FuseNative.SessionLoginError += _SessionLoginError;
		FuseNative.GameConfigurationReceived += _GameConfigurationReceived;

		FuseLog("StartSession(" + gameId + ")");
		FuseNative.StartSession(gameId);
	}

	private static void _SessionStartReceived()
	{
		FuseLog("SessionStartReceived()");

		OnSessionStartReceived();
	}

	private static void _SessionLoginError(int error)
	{
		FuseLog("SessionLoginError(" + error + ")");

		OnSessionLoginError(error);
	}

	#if UNITY_ANDROID
	new public static void SetupPushNotifications(string gcmProjectID)
	{
		FuseLog("SetupPushNotifications(" + gcmProjectID + ")");
	}
	#endif

	#endregion

	#region Analytics Event
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterEvent(string message);
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterEventStart();
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterEventKeyValue(string entryKey, double entryValue);
	[DllImport("__Internal")]
	private static extern int FuseAPI_RegisterEventEnd(string name, string paramName, string paramValue);
	[DllImport("__Internal")]
	private static extern int FuseAPI_RegisterEventVariable(string name, string paramName, string paramValue, string variableName, double variableValue);
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterEventWithDictionary(string message, string[] keys, string[] values, int numEntries);

	new public static void RegisterEvent(string message)
	{
		FuseLog("RegisterEvent(" + message + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterEvent(message);
		}
	}

	new public static void RegisterEvent(string message, Hashtable values)
	{
		FuseLog("RegisterEvent(" + message + ", [variables])");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			int numValues = values.Keys.Count;
			string[] keys = new string[numValues];
			string[] attributes = new string[numValues];
			keys.Initialize();
			attributes.Initialize();
			int numEntries = 0;
			foreach (DictionaryEntry entry in values)
			{
				string entryKey = entry.Key as string;
				string entryValue = entry.Value as string;

				keys[numEntries] = entryKey;
				attributes[numEntries] = entryValue;
				numEntries++;
			}
			FuseAPI_RegisterEventWithDictionary(message, keys, attributes, numEntries);
		}
	}

	new public static int RegisterEvent(string name, string paramName, string paramValue, Hashtable variables)
	{
		FuseLog("RegisterEvent(" + name + "," + paramName + "," + paramValue + ", [variables])");

		//if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			//FuseAPI_RegisterEventStart();

			foreach (DictionaryEntry entry in variables)
			{
				string entryKey = entry.Key as string;
				try
				{
					//double entryValue = // commented out to remove a 'variable never used' warning
					Convert.ToDouble(entry.Value);
				}
				catch
				{
					string entryString = (entry.Value == null) ? "" : entry.Value.ToString();
					Debug.LogWarning("Key/value pairs in FuseAPI::RegisterEvent must be String/Number");
					Debug.LogWarning("For Key: " + entryKey + " and Value: " + entryString);
				}
				//FuseAPI_RegisterEventKeyValue(entryKey, entryValue);
			}

			return -1;
			//return FuseAPI_RegisterEventEnd(name, paramName, paramValue);
		}

		//return -1;
	}

	new public static int RegisterEvent(string name, string paramName, string paramValue, string variableName, double variableValue)
	{
		FuseLog("RegisterEvent(" + name + "," + paramName + "," + paramValue + "," + variableName + "," + variableValue + ")");
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return FuseAPI_RegisterEventVariable(name, paramName, paramValue, variableName, variableValue);
		}

		return -1;
	}
	#endregion

	#region In-App Purchase Logging
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterInAppPurchaseListStart();
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterInAppPurchaseListProduct(string productId, string priceLocale, float price);
	[DllImport("__Internal")]
	private static extern int FuseAPI_RegisterInAppPurchaseListEnd();

	new public static void RegisterInAppPurchaseList(Product[] products)
	{
		FuseLog("RegisterInAppPurchaseList(" + products.Length + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterInAppPurchaseListStart();

			foreach (Product product in products)
			{
				FuseAPI_RegisterInAppPurchaseListProduct(product.productId, product.priceLocale, product.price);
			}

			FuseAPI_RegisterInAppPurchaseListEnd();
		}
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterInAppPurchase(string productId, string transactionId, byte[] transactionReceiptBuffer, int transactionReceiptLength, int transactionState);

#if UNITY_IPHONE
	new public enum TransactionState { PURCHASING, PURCHASED, FAILED, RESTORED }
#else
	public enum TransactionState { PURCHASING, PURCHASED, FAILED, RESTORED }
#endif

	public static void RegisterInAppPurchase(string productId, string transactionId, byte[] transactionReceipt, TransactionState transactionState)
	{
		FuseLog("RegisterInAppPurchase(" + productId + "," + transactionReceipt.Length + "," + transactionState + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterInAppPurchase(productId, transactionId, transactionReceipt, transactionReceipt.Length, (int)transactionState);
		}
		else
		{
			_PurchaseVerification(true, "", "");
		}
	}

	#if UNITY_ANDROID
	// Android purchase notification
	new public static void RegisterInAppPurchase(PurchaseState purchaseState, string notifyId, string productId, string orderId, DateTime purchaseTime, string developerPayload, double price, string currency)
	{
		FuseLog("RegisterInAppPurchase");
	}
	new public static void RegisterInAppPurchase(PurchaseState purchaseState, string notifyId, string productId, string orderId, long purchaseTime, string developerPayload, double price, string currency)
	{
		FuseLog("RegisterInAppPurchase");
	}
	#endif

	private static void _PurchaseVerification(bool verified, string transactionId, string originalTransactionId)
	{
		FuseLog("PurchaseVerification(" + verified + "," + transactionId + "," + originalTransactionId + ")");

		OnPurchaseVerification(verified, transactionId, originalTransactionId);
	}

	#endregion

	#region Fuse Interstitial Ads
	[DllImport("__Internal")]
	private static extern void FuseAPI_CheckAdAvailable(string adZone);
	[DllImport("__Internal")]
	private static extern void FuseAPI_ShowAd(string adZone);

	new public static void PreLoadAd(string adZone)
	{
		Debug.Log("In editor");
	}

	new public static void CheckAdAvailable(string adZone)
	{
		FuseLog("CheckAdAvailable()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_CheckAdAvailable(adZone);
		}
		else
		{
			_AdAvailabilityResponse(0,0);
		}
	}

	new public static void ShowAd(string adZone)
	{
		FuseLog("ShowAd()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_ShowAd(adZone);
		}
		else
		{
			_AdWillClose();
		}
	}

	private static void _AdAvailabilityResponse(int available, int error)
	{
		FuseLog("AdAvailabilityResponse(" + available + "," + error + ")");

		OnAdAvailabilityResponse(available, error);
	}

	private static void _AdWillClose()
	{
		FuseLog("AdWillClose()");

		OnAdWillClose();
	}

	#endregion

	#region Notifications
	[DllImport("__Internal")]
	private static extern void FuseAPI_DisplayNotifications();

	new public static void FuseAPI_RegisterForPushNotifications()
	{
		FuseLog("RegisterForNotifications()");
	}

	new public static void DisplayNotifications()
	{
		FuseLog("DisplayNotifications()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_DisplayNotifications();
		}
	}

    new public static bool IsNotificationAvailable()
    {
        return false;
    }

	private static void _NotificationAction(string action)
	{
		FuseLog("NotificationAction(" + action + ")");

		OnNotificationAction(action);
	}

    #endregion

#region More Games
	[DllImport("__Internal")]
	private static extern void FuseAPI_DisplayMoreGames();

	new public static void DisplayMoreGames()
	{
		FuseLog("DisplayMoreGames()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_DisplayMoreGames();
		}
		else
		{
			_OverlayWillClose();
		}
	}

	private static void _OverlayWillClose()
	{
		FuseLog("OverlayWillClose()");
		OnOverlayWillClose();
	}

    #endregion

#region Gender
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterGender(int gender);

	new public static void RegisterGender(Gender gender)
	{
		FuseLog("RegisterGender(" + gender + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterGender((int)gender);
		}
	}
    #endregion

#region Account Login

	[DllImport("__Internal")]
	private static extern void FuseAPI_GameCenterLogin();

	new public static void GameCenterLogin()
	{
		FuseLog("GameCenterLogin()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_GameCenterLogin();
		}
		else
		{
			_AccountLoginComplete(AccountType.GAMECENTER, "");
		}
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_FacebookLogin(string facebookId, string name, string accessToken);

	new public static void FacebookLogin(string facebookId, string name, string accessToken)
	{
		FuseLog("FacebookLogin(" + facebookId + "," + name + "," + accessToken + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_FacebookLogin(facebookId, name, accessToken);
		}
		else
		{
			_AccountLoginComplete(AccountType.FACEBOOK, facebookId);
		}
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_FacebookLoginGender(string facebookId, string name, Gender gender, string accessToken);

	new public static void FacebookLogin(string facebookId, string name, Gender gender, string accessToken)
	{
		FuseLog("FacebookLogin(" + facebookId + "," + name + "," + gender + "," + accessToken + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_FacebookLoginGender(facebookId, name, gender, accessToken);
		}
		else
		{
			_AccountLoginComplete(AccountType.FACEBOOK, facebookId);
		}
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_TwitterLogin(string twitterId);

	new public static void TwitterLogin(string twitterId)
	{
		FuseLog("TwitterLogin(" + twitterId + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_TwitterLogin(twitterId);
		}
		else
		{
			_AccountLoginComplete(AccountType.TWITTER, twitterId);
		}
	}

	new public static void DeviceLogin(string alias)
	{
		FuseLog("DeviceLogin(" + alias + ")");

		_AccountLoginComplete(AccountType.DEVICE_ID, alias);
	}


	[DllImport("__Internal")]
	private static extern void FuseAPI_OpenFeintLogin(string openFeintId);

	new public static void OpenFeintLogin(string openFeintId)
	{
		FuseLog("OpenFeintLogin(" + openFeintId + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_OpenFeintLogin(openFeintId);
		}
		else
		{
			_AccountLoginComplete(AccountType.OPENFEINT, openFeintId);
		}
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_FuseLogin(string fuseId, string alias);

	new public static void FuseLogin(string fuseId, string alias)
	{
		FuseLog("FuseLogin(" + fuseId + "," + alias + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_FuseLogin(fuseId, alias);
		}
		else
		{
			_AccountLoginComplete(AccountType.USER, fuseId);
		}
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_GooglePlayLogin(string alias, string token);

	new public static void GooglePlayLogin(string alias, string token)
	{
		FuseLog("GooglePlayLogin(" + alias + "," + token + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_GooglePlayLogin(alias, token);
		}
		else
		{
			_AccountLoginComplete(AccountType.GOOGLE_PLAY, alias);
		}
	}

	[DllImport("__Internal")]
	private static extern string FuseAPI_GetOriginalAccountAlias();

	new public static string GetOriginalAccountAlias()
	{
		FuseLog("GetOriginalAccountAlias()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string accountAlias = FuseAPI_GetOriginalAccountAlias();

			return accountAlias;
		}
		else
		{
			return "";
		}
	}

	[DllImport("__Internal")]
	private static extern string FuseAPI_GetOriginalAccountId();

	new public static string GetOriginalAccountId()
	{
		FuseLog("GetOriginalAccountId()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string accountId = FuseAPI_GetOriginalAccountId();

			return accountId;
		}
		else
		{
			return "";
		}
	}

	[DllImport("__Internal")]
	private static extern AccountType FuseAPI_GetOriginalAccountType();

	new public static AccountType GetOriginalAccountType()
	{
		FuseLog("GetOriginalAccountType()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			AccountType accountType = FuseAPI_GetOriginalAccountType();

			return accountType;
		}
		else
		{
			return AccountType.NONE;
		}
	}

	private static void _AccountLoginComplete(AccountType type, string accountId)
	{
		FuseLog("AccountLoginComplete(" + type + "," + accountId + ")");

		OnAccountLoginComplete(type, accountId);
	}

    #endregion

#region Miscellaneous
	[DllImport("__Internal")]
	private static extern int FuseAPI_GamesPlayed();

	new public static int GamesPlayed()
	{

		FuseLog("GamesPlayed()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			int gamesPlayed = FuseAPI_GamesPlayed();

			return gamesPlayed;
		}
		else
		{
			return 0;
		}
	}

	[DllImport("__Internal")]
	private static extern string FuseAPI_LibraryVersion();

	new public static string LibraryVersion()
	{
		FuseLog("LibraryVersion()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string libraryVersion = FuseAPI_LibraryVersion();

			return libraryVersion;
		}
		else
		{
			return "1.22";
		}
	}
	[DllImport("__Internal")]
	private static extern bool FuseAPI_Connected();

	new public static bool Connected()
	{
		FuseLog("Connected()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			bool connected = FuseAPI_Connected();

			return connected;
		}
		else
		{
			return true;
		}
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_TimeFromServer();

	new public static void TimeFromServer()
	{
		FuseLog("TimeFromServer()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_TimeFromServer();
		}
		else
		{
			_TimeUpdated((DateTime.UtcNow - unixEpoch).Ticks / TimeSpan.TicksPerSecond);
		}
	}

	private static void _TimeUpdated(long timestamp)
	{
		FuseLog("TimeUpdated(" + timestamp + ")");

		if (timestamp >= 0)
		{
			OnTimeUpdated(unixEpoch + TimeSpan.FromTicks(timestamp * TimeSpan.TicksPerSecond));
		}
	}

	private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	[DllImport("__Internal")]
	private static extern bool FuseAPI_NotReadyToTerminate();

	new public static bool NotReadyToTerminate()
	{
		FuseLog("NotReadyToTerminate()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			bool notReadyToTerminate = FuseAPI_NotReadyToTerminate();

			return notReadyToTerminate;
		}
		else
		{
			return false;
		}
	}

	new public static void FuseLog(string str)
	{
		if(debugOutput)
		{
			Debug.Log(" " + str);
		}
	}

    #endregion

#region Data Opt In/Out
	[DllImport("__Internal")]
	private static extern void FuseAPI_EnableData(bool enable);

	new public static void EnableData(bool enable)
	{
		FuseLog("EnableData(" + enable + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_EnableData(enable);
		}
	}

	[DllImport("__Internal")]
	private static extern bool FuseAPI_DataEnabled();

	new public static bool DataEnabled()
	{
		FuseLog("DataEnabled()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			bool enabled = FuseAPI_DataEnabled();

			return enabled;
		}
		else
		{
			return true;
		}
	}
    #endregion

#region User Game Data
	[DllImport("__Internal")]
	private static extern void FuseAPI_SetGameDataStart(string key, bool isCollection, string fuseId);
	[DllImport("__Internal")]
	private static extern void FuseAPI_SetGameDataKeyValue(string key, string value, bool isBinary);
	[DllImport("__Internal")]
	private static extern int FuseAPI_SetGameDataEnd();

	new public static int SetGameData(Hashtable data)
	{
		return SetGameData("", data, false, GetFuseId());
	}

	new public static int SetGameData(string key, Hashtable data)
	{
		return SetGameData(key, data, false, GetFuseId());
	}

	new public static int SetGameData(string key, Hashtable data, bool isCollection)
	{
		return SetGameData(key, data, isCollection, GetFuseId());
	}

	new public static int SetGameData(string key, Hashtable data, bool isCollection, string fuseId)
	{
		FuseLog("SetGameData(" + key + "," + isCollection + "," + fuseId + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_SetGameDataStart(key, isCollection, fuseId);

			foreach (DictionaryEntry entry in data)
			{
				string entryKey = entry.Key as string;
				byte[] buffer = entry.Value as byte[];
				if (buffer != null)
				{
					string entryValue = Convert.ToBase64String(buffer);
					FuseAPI_SetGameDataKeyValue(entryKey, entryValue, true);
				}
				else
				{
					string entryValue = entry.Value.ToString();
					FuseAPI_SetGameDataKeyValue(entryKey, entryValue, false);
				}
			}

			return FuseAPI_SetGameDataEnd();
		}
		else
		{
			_GameDataSetAcknowledged(-1);

			return -1;
		}
	}

	private static void _GameDataError(int error, int requestId)
	{
		FuseLog("GameDataError(" + error + "," + requestId + ")");

		OnGameDataError(error, requestId);
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_GetGameDataStart(string key, string fuseId);
	[DllImport("__Internal")]
	private static extern void FuseAPI_GetGameDataKey(string key);
	[DllImport("__Internal")]
	private static extern int FuseAPI_GetGameDataEnd();

	new public static int GetGameData(string[] keys)
	{
		return GetFriendGameData("", "", keys);
	}

	new public static int GetGameData(string key, string[] keys)
	{
		return GetFriendGameData("", key, keys);
	}

	new public static int GetFriendGameData(string fuseId, string[] keys)
	{
		return GetFriendGameData(fuseId, "", keys);
	}

	new public static int GetFriendGameData(string fuseId, string key, string[] keys)
	{
		FuseLog("GetGameData(" + fuseId + "," + key + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_GetGameDataStart(key, fuseId);

			foreach (string entry in keys)
			{
				FuseAPI_GetGameDataKey(entry);
			}

			return FuseAPI_GetGameDataEnd();
		}
		else
		{
			_GameDataReceivedStart(fuseId, key, -1);
			_GameDataReceivedEnd();

			return -1;
		}
	}

	private static void _GameDataSetAcknowledged(int requestId)
	{
		FuseLog("GameDataSetAcknowledged(" + requestId + ")");

		OnGameDataSetAcknowledged(requestId);
	}

	private static void _GameDataReceivedStart(string fuseId, string key, int requestId)
	{
		FuseLog("GameDataReceivedStart(" + fuseId + "," + key + ")");

		_gameDataFuseId = fuseId;
		_gameDataKey = key;
		_gameData = new Hashtable();
		_gameDataRequestId = requestId;
	}

	private static void _GameDataReceivedKeyValue(string key, string value, bool isBinary)
	{
		FuseLog("GameDataReceivedKeyValue(" + key + "," + value + "," + isBinary + ")");

		if (isBinary)
		{
			byte[] buffer = Convert.FromBase64String(value);
			_gameData.Add(key, buffer);
		}
		else
		{
			_gameData.Add(key, value);
		}
	}

	private static void _GameDataReceivedEnd()
	{
		FuseLog("GameDataReceivedEnd()");

		OnGameDataReceived(_gameDataFuseId, _gameDataKey, _gameData, _gameDataRequestId);
		_gameDataRequestId = -1;
		_gameData = null;
		_gameDataKey = "";
		_gameDataFuseId = "";
	}

	[DllImport("__Internal")]
	private static extern string FuseAPI_GetFuseId();

	new public static string GetFuseId()
	{
		FuseLog("GetFuseId()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string fuseId = FuseAPI_GetFuseId();

			return fuseId;
		}
		else
		{
			return "";
		}
	}

	private static string _gameDataKey = "";
	private static string _gameDataFuseId = "";
	private static Hashtable _gameData = null;
	private static int _gameDataRequestId = -1;
    #endregion

#region Friend List
	new public static void AddFriend(string fuseId)
	{
		OnFriendAdded(fuseId, (int)FuseAPI.FriendErrors.FUSE_FRIEND_NO_ERROR);
	}
	new public static void RemoveFriend(string fuseId)
	{
		OnFriendRemoved(fuseId, (int)FuseAPI.FriendErrors.FUSE_FRIEND_NO_ERROR);
	}
	new public static void AcceptFriend(string fuseId)
	{
		OnFriendAccepted(fuseId, (int)FuseAPI.FriendErrors.FUSE_FRIEND_NO_ERROR);
	}
	new public static void RejectFriend(string fuseId)
	{
		OnFriendRejected(fuseId, (int)FuseAPI.FriendErrors.FUSE_FRIEND_NO_ERROR);
	}
	
	new public static void MigrateFriends(string fuseId)
	{
		OnFriendsMigrated(fuseId, (int)FuseAPI.FuseMigrateFriendErrors.FUSE_MIGRATE_FRIENDS_NOT_CONNECTED);
	}		
	
	[DllImport("__Internal")]
	private static extern void FuseAPI_UpdateFriendsListFromServer();

	new public static void UpdateFriendsListFromServer()
	{
		FuseLog("UpdateFriendsListFromServer()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_UpdateFriendsListFromServer();
		}
		else
		{
			_FriendsListUpdatedStart();
			_FriendsListUpdatedEnd();
		}
	}

	private static void _FriendsListUpdatedStart()
	{
		FuseLog("FriendsListUpdatedStart()");

		_friendsList = new List<Friend>();
	}

	private static void _FriendsListUpdatedFriend(string fuseId, string accountId, string alias, bool pending)
	{
		FuseLog("FriendsListUpdatedFriend(" + fuseId + "," + accountId + "," + alias + "," + pending + ")");

		Friend friend = new Friend();
		friend.fuseId = fuseId;
		friend.accountId = accountId;
		friend.alias = alias;
		friend.pending = pending;

		_friendsList.Add(friend);
	}

	private static void _FriendsListUpdatedEnd()
	{
		FuseLog("FriendsListUpdatedEnd()");

		OnFriendsListUpdated(_friendsList);

		_friendsList = null;
	}

	private static void _FriendsListError(int error)
	{
		FuseLog("FriendsListError(" + error + ")");

		OnFriendsListError(error);
	}

	private static List<Friend> _friendsList = null;

	[DllImport("__Internal")]
	private static extern int FuseAPI_GetFriendsListCount();
	[DllImport("__Internal")]
	private static extern string FuseAPI_GetFriendsListFriendFuseId(int index);
	[DllImport("__Internal")]
	private static extern string FuseAPI_GetFriendsListFriendAccountId(int index);
	[DllImport("__Internal")]
	private static extern string FuseAPI_GetFriendsListFriendAlias(int index);
	[DllImport("__Internal")]
	private static extern bool FuseAPI_GetFriendsListFriendPending(int index);

	new public static List<Friend> GetFriendsList()
	{
		FuseLog("GetFriendsList()");

		List<Friend> friendsList = new List<Friend>();

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			int friendCount = FuseAPI_GetFriendsListCount();

			for (int index = 0; index < friendCount; index++)
			{
				Friend friend = new Friend();
				friend.fuseId = FuseAPI_GetFriendsListFriendFuseId(index);
				friend.accountId = FuseAPI_GetFriendsListFriendAccountId(index);
				friend.alias = FuseAPI_GetFriendsListFriendAlias(index);
				friend.pending = FuseAPI_GetFriendsListFriendPending(index);

				friendsList.Add(friend);
			}
		}

		return friendsList;
	}
    #endregion

#region Chat List
    #endregion

#region User-to-User Push Notifications
	[DllImport("__Internal")]
	private static extern void FuseAPI_UserPushNotification(string fuseId, string message);

	new public static void UserPushNotification(string fuseId, string message)
	{
		FuseLog("UserPushNotification(" + fuseId +"," + message + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_UserPushNotification(fuseId, message);
		}
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_FriendsPushNotification(string message);

	new public static void FriendsPushNotification(string message)
	{
		FuseLog("FriendsPushNotification(" + message + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_FriendsPushNotification(message);
		}
	}
    #endregion

#region Gifting
	[DllImport("__Internal")]
	private static extern void FuseAPI_GetMailListFromServer();
	[DllImport("__Internal")]
	private static extern void FuseAPI_GetMailListFriendFromServer(string fuseId);

	new public static void GetMailListFromServer()
	{
		FuseLog("GetMailListFromServer()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_GetMailListFromServer();
		}
		else
		{
			_MailListReceivedStart("");
			_MailListReceivedEnd();
		}
	}

	new public static void GetMailListFriendFromServer(string fuseId)
	{
		FuseLog("GetMailListFriendFromServer(" + fuseId + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_GetMailListFriendFromServer(fuseId);
		}
		else
		{
			_MailListReceivedStart(fuseId);
			_MailListReceivedEnd();
		}
	}

	private static void _MailListReceivedStart(string fuseId)
	{
		FuseLog("MailListReceivedStart()");

		_mailListFuseId = fuseId;
		_mailList = new List<Mail>();
	}

	private static void _MailListReceivedMail(int messageId, long timestamp, string alias, string message, int giftId, string giftName, int giftAmount)
	{
		FuseLog("MailListReceivedMail(" + messageId + "," + timestamp + "," + alias + "," + message + "," + giftId + "," + giftName + "," + giftAmount + ")");

		Mail mail = new Mail();
		mail.messageId = messageId;
		mail.timestamp = unixEpoch + TimeSpan.FromTicks(timestamp * TimeSpan.TicksPerSecond);
		mail.alias = alias;
		mail.message = message;
		mail.giftId = giftId;
		mail.giftName = giftName;
		mail.giftAmount = giftAmount;

		_mailList.Add(mail);
	}

	private static void _MailListReceivedEnd()
	{
		FuseLog("MailListReceivedEnd()");

		OnMailListReceived(_mailList, _mailListFuseId);

		_mailList = null;
		_mailListFuseId = "";
	}

	private static void _MailListError(int error)
	{
		FuseLog("MailListError(" + error + ")");

		OnMailListError(error);
	}

	private static List<Mail> _mailList = null;
	private static string _mailListFuseId = "";

	[DllImport("__Internal")]
	private static extern int FuseAPI_GetMailListCount(string fuseId);
	[DllImport("__Internal")]
	private static extern int FuseAPI_GetMailListMailMessageId(string fuseId, int index);
	[DllImport("__Internal")]
	private static extern long FuseAPI_GetMailListMailTimestamp(string fuseId, int index);
	[DllImport("__Internal")]
	private static extern string FuseAPI_GetMailListMailAlias(string fuseId, int index);
	[DllImport("__Internal")]
	private static extern string FuseAPI_GetMailListMailMessage(string fuseId, int index);
	[DllImport("__Internal")]
	private static extern int FuseAPI_GetMailListMailGiftId(string fuseId, int index);
	[DllImport("__Internal")]
	private static extern string FuseAPI_GetMailListMailGiftName(string fuseId, int index);
	[DllImport("__Internal")]
	private static extern int FuseAPI_GetMailListMailGiftAmount(string fuseId, int index);

	new public static List<Mail> GetMailList(string fuseId)
	{
		FuseLog("GetMailList()");

		List<Mail> mailList = new List<Mail>();

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			int mailCount = FuseAPI_GetMailListCount(fuseId);

			for (int index = 0; index < mailCount; index++)
			{
				Mail mail = new Mail();
				mail.messageId = FuseAPI_GetMailListMailMessageId(fuseId, index);
				mail.timestamp = unixEpoch + TimeSpan.FromTicks(FuseAPI_GetMailListMailTimestamp(fuseId, index) * TimeSpan.TicksPerSecond);
				mail.alias = FuseAPI_GetMailListMailAlias(fuseId, index);
				mail.message = FuseAPI_GetMailListMailMessage(fuseId, index);
				mail.giftId = FuseAPI_GetMailListMailGiftId(fuseId, index);
				mail.giftName = FuseAPI_GetMailListMailGiftName(fuseId, index);
				mail.giftAmount = FuseAPI_GetMailListMailGiftAmount(fuseId, index);

				mailList.Add(mail);
			}
		}

		return mailList;
	}

	[DllImport("__Internal")]
	private static extern void FuseAPI_SetMailAsReceived(int messageId);

	new public static void SetMailAsReceived(int messageId)
	{
		FuseLog("SetMailAsReceived(" + messageId + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_SetMailAsReceived(messageId);
		}
	}

	[DllImport("__Internal")]
	private static extern int FuseAPI_SendMail(string fuseId, string message);

	new public static int SendMail(string fuseId, string message)
	{
		FuseLog("SendMail(" + fuseId + "," + message + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return FuseAPI_SendMail(fuseId, message);
		}
		else
		{
			_MailAcknowledged(-1, fuseId, -1);
			return -1;
		}
	}

	[DllImport("__Internal")]
	private static extern int FuseAPI_SendMailWithGift(string fuseId, string message, int giftId, int giftAmount);

	new public static int SendMailWithGift(string fuseId, string message, int giftId, int giftAmount)
	{
		FuseLog("SendMailWithGift(" + fuseId + "," + message + "," + giftId + "," + giftAmount + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return FuseAPI_SendMailWithGift(fuseId, message, giftId, giftAmount);
		}
		else
		{
			_MailAcknowledged(-1, fuseId, -1);
			return -1;
		}
	}

	private static void _MailAcknowledged(int messageId, string fuseId, int requestID)
	{
		FuseLog("MailAcknowledged()");

		OnMailAcknowledged(messageId, fuseId, requestID);
	}

	private static void _MailError(int error)
	{
		FuseLog("MailError(" + error + ")");

		OnMailError(error);
	}

    #endregion

#region Game Configuration Data
	[DllImport("__Internal")]
	private static extern string FuseAPI_GetGameConfigurationValue(string key);

	new public static string GetGameConfigurationValue(string key)
	{
		FuseLog("GetGameConfigurationValue(" + key + ")");

		return FuseNative.GetGameConfigurationValue(key);
	}

	new public static Dictionary<string, string> GetGameConfig()
	{
		FuseLog("GetGameConfig()");

		return FuseNative.GetCameConfiguration();
	}

	private static void _GameConfigurationReceived()
	{
		FuseLog("GameConfigurationReceived()");

		OnGameConfigurationReceived();
	}

    #endregion

#region Specific Event Registration
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterLevel(int level);
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterCurrency(int type, int balance);
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterFlurryView();
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterFlurryClick();
	[DllImport("__Internal")]
	private static extern void FuseAPI_RegisterTapjoyReward(int amount);

	new public static void RegisterLevel(int level)
	{
		FuseLog("RegisterLevel(" + level + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterLevel(level);
		}
	}

	new public static void RegisterCurrency(int type, int balance)
	{
		FuseLog("RegisterCurrency(" + type + "," + balance + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterCurrency(type, balance);
		}
	}

	new public static void RegisterFlurryView()
	{
		FuseLog("RegisterFlurryView()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterFlurryView();
		}
	}

	new public static void RegisterFlurryClick()
	{
		FuseLog("RegisterFlurryClick()");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterFlurryClick();
		}
	}

	new public static void RegisterTapjoyReward(int amount)
	{
		FuseLog("RegisterTapjoyReward(" + amount + ")");

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FuseAPI_RegisterTapjoyReward(amount);
		}
	}

	new public static void RegisterAge(int age)
	{
		FuseLog("RegisterAge(" + age + ")");
	}
	
	new public static void RegisterBirthday(int year, int month, int day)
	{
		FuseLog("RegisterBirthday(" + year + ", " + month + ", " + day + ")");
	}
#endregion

}
#endif
