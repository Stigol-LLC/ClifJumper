using UnityEngine;
using System;

[Serializable]
public class GMFieldMetaData
{
    public string name;
    public string typeString;
    public Type type;
    public bool isSerialized;
    public GMSyncHelper.syncType fieldSyncType = GMSyncHelper.syncType.dontMerge;
    public bool isCrypted = false;

    [NonSerialized]
    public bool checkFlag;
    [NonSerialized]
    public bool serverConfigured = false;

}
