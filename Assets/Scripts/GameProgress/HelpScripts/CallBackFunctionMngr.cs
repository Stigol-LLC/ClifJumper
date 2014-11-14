#if UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Runtime.CompilerServices;

namespace Social {
/*	listener callback C function,and executer c# actions
 * 
 * 	Add	implementation in you class
 * [MonoPInvokeCallback( typeof (callbackDelegate) )]
 * private static void ReceiveDeviceMessage(string method,string error,string result) {
 * 		singleTonObject.Run(method,error,result);	
 * }
 *  PushAction(Action<string> callback) - insert this method to c# function,if you need
 * [Example]
 *  public void YouFunction(params ..., Action<string> action){
 * 		PushAction(action);
 * 		//do something
 * 		.......
 * }
 * */
	public class CallbackFunctionMngr<T> where T : class, new(){
		private static object lockingObject = new object();
    	protected static T singleTonObject;
		public static T Instance()
	    {
	        if(singleTonObject == null)
	        {
	             lock (lockingObject)
	             {
	                  if(singleTonObject == null)
	                  {
	                       singleTonObject = new T();
	                  }
	             }
	        }
	        return singleTonObject;
	    }
		
		protected delegate void callbackDelegate(string method,string error,string result );
		
		protected Action<string> callBackResult = null;//new List<Action<string>>();
		protected Queue < Action > functionQueue = new Queue<Action>();

		protected bool PushAction(Action<string> callback){
			if(callBackResult == null){
                Debug.Log(callBackResult);
				callBackResult = callback;
				return true;
			}else{
				Debug.Log("Error PushAction " + GetCurrentMethod());
			}
			return false;
		}

		protected void PushFunction(Action function){
			functionQueue.Enqueue(function);
		}

		private void ExecuteNextFunction(){
			if(functionQueue.Count > 0){
				Action func = functionQueue.Dequeue();
				if(func != null){
					func();
				}
			}
		}
		protected void Run(string method,string error, string message){
			Debug.Log("Run : callBackResult " + method + "," + error + "," + message);
			Action<string> tmpAct = callBackResult;
			callBackResult = null;
			if( error.Contains("0")){//no error no message. Everything ok!
              
                if (tmpAct != null)
                    tmpAct(null);
              
                Debug.LogError(message);
			}else{ // something happens!
                if (tmpAct != null)
                {
                    Debug.Log("Run : callBackResult " + message);
                    tmpAct(message);
                }
                else
                {
                    Debug.Log("Run : callBackResult null");
                }
			}
			//remove action what executed
			//ExecuteNextFunction();
		}	
		[MethodImpl(MethodImplOptions.NoInlining)]
		static protected string GetCurrentMethod()
		{
		    System.Diagnostics.StackTrace st = new  System.Diagnostics.StackTrace ();
		    System.Diagnostics.StackFrame sf = st.GetFrame (2);
		    return sf.GetMethod().Name;
		}
	}
}
#endif
