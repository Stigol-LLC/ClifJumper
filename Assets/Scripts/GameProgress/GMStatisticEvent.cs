using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GMStatisticEvent {
    public string eventName ;
    public string eventDelegateName;
    public int eventDelegateIndex;
    public string parameterVarName;
    public int parameterVarIndex;
    public string parameterTypeName;
    public int parameterTypeIndex;
    public List<GMStatisticsRange> parameterRanges;
    public GMStatisticsRange parameterEndRange;
}
