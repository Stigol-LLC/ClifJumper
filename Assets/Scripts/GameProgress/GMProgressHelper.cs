using System.Collections.Generic;

using UnityEngine;


public class GMProgressHelper : MonoBehaviour {


    [SerializeField] private string _localProgressFileName;

    public string LocalProgressFileName {
        get {
            if ( ( _localProgressFileName == null ) ||
                 ( _localProgressFileName.Length == 0 ) )
                return "GMMergedLocalProgress.xml";

            return _localProgressFileName;
        }

        set { _localProgressFileName = value; }
    }



 

    // Use this for initialization

}

