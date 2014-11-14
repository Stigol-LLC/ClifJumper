#if UNITY_IPHONE
#region usings

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using UnityEngine;

#endregion

namespace Social {

	 class ICloud: CallbackFunctionMngr<ICloud> {
#if UNITY_EDITOR
		private static void _IC_SaveFile( StringBuilder fullFileName, StringBuilder icloudFileName){}
		private static void _IC_LoadFile(StringBuilder fromIcloudFileName, StringBuilder toFullFileName){}
		private static void _IC_LoadAllFiles(StringBuilder file_ext ,StringBuilder folder_path) {}
		private static void _IC_FileExistsInIcloud(StringBuilder fileName){return;}
		private static void _IC_ClearOldCloudFiles(StringBuilder file_ext, StringBuilder folder_path){}
		//        private static IntPtr _IC_GetFileName(){ return IntPtr.Zero; }
		//        private static void _IC_SetFileName( StringBuilder name ){}
		private static void _IC_setCallback( callbackDelegate callback ){}
		private static bool _IC_AccessAllowed(){return false;}
		private static void _IC_EnableLogs (bool val){}
#elif UNITY_IPHONE || UNITY_XBOX360
        [DllImport( "__Internal" )]
		private static extern void _IC_SaveFile( StringBuilder fullFileName, StringBuilder icloudFileName);

        [DllImport( "__Internal" )]
		private static extern void _IC_LoadFile( StringBuilder fromIcloudFileName, StringBuilder toFullFileName);

		[DllImport( "__Internal" )]
		private static extern void _IC_LoadAllFiles(StringBuilder file_ext ,StringBuilder folder_path);

		[DllImport( "__Internal" )]
		private static extern void _IC_ClearOldCloudFiles(StringBuilder file_ext, StringBuilder folder_path);

		[DllImport( "__Internal" )]
		private static extern void _IC_FileExistsInIcloud(StringBuilder fileName);

		[DllImport( "__Internal" )]
		private static extern bool _IC_AccessAllowed();


        [DllImport( "__Internal" )]
        private static extern void _IC_setCallback( callbackDelegate callback );

		[DllImport( "__Internal" )]
		private static extern void _IC_EnableLogs (bool val);
#else

#endif

		[MonoPInvokeCallback( typeof (callbackDelegate) )]
		private static void ReceiveDeviceMessage(string method,string error,string result) {
			singleTonObject.Run(method,error,result);	
		}

//        private static string _dataPath = Finder.GetSandboxPath() + FileName;
//
//        public static string DataPath {
//            get { return _dataPath; }
//            set { _dataPath = value; }
//        }
//
//        public static string FileName {
//            get {
//                if ( !DeviceUtil.IsEditor ) {
//                    return Marshal.PtrToStringAnsi( _IC_GetFileName() );
//                }
//                return null;
//            }
//            set {
//                if ( !DeviceUtil.IsEditor ) {
//                    _IC_SetFileName( new StringBuilder( value ) );
//                }
//            }
//        }

        public void Initialize() { }
		public bool AccessAllowed(){
			return _IC_AccessAllowed();
		}
        public ICloud() {
              _IC_setCallback(ReceiveDeviceMessage);
        }
		//send file to icloud
		public void SaveFile( string fromFullFileName,string toIcloudFileName,Action<string> callback ) {
			if(!AccessAllowed()){
				Debug.Log("No Icloud access");
				return;
			}
			PushAction(callback);
			Debug.LogError("Save from: " + fromFullFileName + "  to   " + toIcloudFileName);
			_IC_SaveFile( new StringBuilder(fromFullFileName),new StringBuilder(toIcloudFileName) );
        }

		public void enableLogs(bool val)
		{
			_IC_EnableLogs (val);
		}

		//load all file with ext to folder
		public void ClearOldCloudFiles(string file_ext, string folder,Action<string> callback) {
			if(!AccessAllowed()){
				Debug.Log("No Icloud access");
				return;
			}

			PushAction(callback);
			Debug.LogError("Clear for: " + file_ext + "     " + folder);
			_IC_ClearOldCloudFiles(new StringBuilder(file_ext), new StringBuilder(folder));
		}

		//load all file with ext to folder
		public void LoadFiles(string file_ext,string folder,Action<string> callback) {
			if(!AccessAllowed()){
				Debug.Log("No Icloud access");
				return;
			}
			PushAction(callback);
			Debug.LogError("Load List for: " + file_ext + "     " + folder);
			_IC_LoadAllFiles(new StringBuilder(file_ext),new StringBuilder(folder));
		}

		//load file with icloud to sandbox
		public void LoadFile(string fromIcloudFileName,string toFullFileName,Action<string> callback) {
			if(!AccessAllowed()){
				Debug.Log("No Icloud access");
				return;
			}
			PushAction(callback);
			_IC_LoadFile(new StringBuilder(fromIcloudFileName),new StringBuilder(toFullFileName));
        }
		public void FileExistsInIcloud(string fileName,Action<string> callback){
			if(!AccessAllowed()){
				Debug.Log("No Icloud access");
				return;
			}
			PushAction (callback);
			 _IC_FileExistsInIcloud(new StringBuilder(fileName));
		}
//        public static bool IsBaseOlder( string tempDataPath, Predicate<string> diffComparer = null ) {
//            if ( diffComparer == null ) {
//                DateTime baseTime = File.GetLastWriteTime( _dataPath );
//                DateTime newTime = File.GetLastWriteTime( tempDataPath );
//
//                int result = DateTime.Compare( baseTime, newTime );
//                if ( result < 0 ) {
//                    return true;
//                }
//                return false;
//            }
//            return diffComparer( tempDataPath );
//        }
    }
}
#endif
