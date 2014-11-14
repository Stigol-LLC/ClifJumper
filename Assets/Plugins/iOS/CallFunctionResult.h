#ifndef CallFunctionResult_h
#define CallFunctionResult_h

//#ifdef DEBUG
//#   define NSLog(...) NSLog(__VA_ARGS__)
//#else
//#   define NSLog(...)
//#endif

typedef void (*funcResult)(const char *method,const char *error,const char *msg);
class CallBackResult{
    funcResult m_func;
public:
    CallBackResult(){
        m_func = NULL;
    }
    void set(funcResult _func){
        m_func = _func;
    }
    void operator=(funcResult _func){
        set(_func);
    }
    void operator()(const char * method,const char *error,const char* msg){
        call(method,error,msg);
    }
    void operator()(const char * method,int error,const char* msg){
        char str[3];
        sprintf(str, "%d", error);
        call(method,str,msg);
    }
    void call(const char * method,const char *error,const char* msg){
        if(m_func){
            m_func(method,error,msg);
        }
    };
};

#endif
