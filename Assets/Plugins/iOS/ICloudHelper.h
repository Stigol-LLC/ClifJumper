#ifndef ICloudHelper_h
#define ICloudHelper_h

#include "CallFunctionResult.h"

class ICloudHelper {
    ICloudHelper();
    CallBackResult m_func;
    bool enableLogs = false;

private:
    void        showException(NSString *funcName, NSException *exception);
    
public:
    void        IC_EnableLogs (bool val);
    bool        IC_accessAllowed();
    void        iCloudLog (NSString * log);
    void        runCallback(int error,const char *msg);
    void        IC_setCallback( funcResult _func );
    void        IC_saveFileToIcloud( const char* ,const char *);
    void        IC_LoadFromIcloud(const char* from ,const char *toFullFileName);
    void        IC_LoadAllFromIcloud(const char* file_ext ,const char *folder_path);
    void        IC_ClearCloudFile(const char* file_path);
    void        IC_ClearOldCloudFiles(const char* file_ext, const char* folder_path);
    void        IC_FileExistsInIcloud(const char *icloudFileName);
    static ICloudHelper& Instance();
};

#endif
