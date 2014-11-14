#region Usings

using UnityEngine;

#endregion

internal struct CubicCurveData {
    #region Properties

    /// <summary>
    ///     Curve control points.
    /// </summary>
    public Vector3 P0, P1, P2, P3;

    /// <summary>
    ///     Prev control points.
    /// </summary>
//    public Vector3 B0, B1, B2, B3;
//
//    /// <summary>
//    ///     Bezier base constants.
//    /// </summary>
//    public Vector3 A, B, C;

    #endregion

    #region Constructor

    public CubicCurveData( Vector3 start, Vector3 finish, Vector3 firstControl, Vector3 secondControl ) {
        P0 = start;
        P1 = finish;
        P2 = firstControl;
        P3 = secondControl;
//        B0 = Vector3.zero;
//        B1 = Vector3.zero;
//        B2 = Vector3.zero;
//        B3 = Vector3.zero;
//        A = Vector3.zero;
//        B = Vector3.zero;
//        C = Vector3.zero;
    }

    #endregion
}


internal class Bezier {
    #region Properties

    private CubicCurveData _curveData;

    #endregion

    #region Constructor

    public Bezier( Vector3 start, Vector3 finish, Vector3 firstControl, Vector3 secondControl )
            : this( new CubicCurveData( start, finish, firstControl, secondControl ) ) {
    }

    public Bezier( CubicCurveData curveData ) {
        _curveData = curveData;
    }

    #endregion

    #region Methods

    public void SetEndPoint( Vector3 finish ) {
        _curveData.P1 = finish;
    }


    /// <summary>
    ///     Sets bezier base constants.
    /// </summary>
//    private void SetConstant() {
//        _curveData.C.x = 3 * ( ( _curveData.P0.x + _curveData.P1.x ) - _curveData.P0.x );
//        _curveData.B.x = 3 * ( ( _curveData.P3.x + _curveData.P2.x ) - ( _curveData.P0.x + _curveData.P1.x ) ) -
//                         _curveData.C.x;
//        _curveData.A.x = _curveData.P3.x - _curveData.P0.x - _curveData.C.x - _curveData.B.x;
//        _curveData.C.y = 3 * ( ( _curveData.P0.y + _curveData.P1.y ) - _curveData.P0.y );
//        _curveData.B.y = 3 * ( ( _curveData.P3.y + _curveData.P2.y ) - ( _curveData.P0.y + _curveData.P1.y ) ) -
//                         _curveData.C.y;
//        _curveData.A.y = _curveData.P3.y - _curveData.P0.y - _curveData.C.y - _curveData.B.y;
//        _curveData.C.z = 3 * ( ( _curveData.P0.z + _curveData.P1.z ) - _curveData.P0.z );
//        _curveData.B.z = 3 * ( ( _curveData.P3.z + _curveData.P2.z ) - ( _curveData.P0.z + _curveData.P1.z ) ) -
//                         _curveData.C.z;
//        _curveData.A.z = _curveData.P3.z - _curveData.P0.z - _curveData.C.z - _curveData.B.z;
//    }


    /// <summary>
    ///     Check if control points have changed.
    /// </summary>
//    private void CheckConstant() {
//        if ( _curveData.P0 != _curveData.B0 ||
//             _curveData.P1 != _curveData.B1 ||
//             _curveData.P2 != _curveData.B2 ||
//             _curveData.P3 != _curveData.B3 ) {
//            SetConstant();
//            _curveData.B0 = _curveData.P0;
//            _curveData.B1 = _curveData.P1;
//            _curveData.B2 = _curveData.P2;
//            _curveData.B3 = _curveData.P3;
//        }
//    }


    /// <summary>
    ///     Gets the bezier point at clamped time range.
    /// </summary>
    /// <param name="time">The time between 0 and 1.</param>
    /// <returns></returns>
    public Vector3 GetBezierPointAtTime( float time ) {
//        CheckConstant();


        float t2 =  time * time;
        float t3 =  time * time * time;

        float t1m = 1 - time;
        float t2m = (1 - time) * (1 - time) ;
        float t3m = (1 - time) * (1 - time) * (1 - time) ;

        float x = _curveData.P0.x * t3m + _curveData.P2.x * 3 * t2m * time + _curveData.P3.x * 3 * t1m * t2 + _curveData.P1.x * t3;
        float y = _curveData.P0.y * t3m + _curveData.P2.y * 3 * t2m * time + _curveData.P3.y * 3 * t1m * t2 + _curveData.P1.y * t3;
        float z = _curveData.P0.z * t3m + _curveData.P2.z * 3 * t2m * time + _curveData.P3.z * 3 * t1m * t2 + _curveData.P1.z * t3;

        return ( new Vector3( x, y, z ) );
    }

    #endregion
}