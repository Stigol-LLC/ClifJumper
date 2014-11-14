#import "ICloudHelper.h"
#include <dispatch/dispatch.h>
#pragma mark - NOTE

@interface Note : UIDocument
@property (strong) NSString * noteContent;
@property (strong,nonatomic) NSData * data;
-(void) setData:(NSData *) _data;
-(NSData *) getData;
@end

@implementation Note
@synthesize noteContent;
@synthesize data = m_data;
// Called whenever the application reads data from the file system
- (BOOL)loadFromContents:(id)contents
                  ofType:(NSString *)typeName
                   error:(NSError * __autoreleasing *)outError {

    if ([contents length] > 0) {
        self.data = contents;
    } else {
        // When the note is first created, assign some default content
        self.noteContent = @"Empty";
        self.data = nil;
        return FALSE;
    }
    [self.data retain];
    //NSLog(@"noteContent::loadFromContents %d", [self.data length]);
    return YES;
}

- (id)contentsForType:(NSString *)typeName error:(NSError **)outError {
    if ([self.data length] == 0) {
        self.noteContent = @"Empty";
        return [NSData dataWithBytes:[self.noteContent UTF8String]
                              length:[self.noteContent length]];
    }
    
    //NSLog(@"contentsForType %d", m_data.length);
    return m_data;
}

- (void)setData:(NSData *)_data {
    m_data = _data;
    //NSLog(@"setData %d", m_data.length);
};

- (NSData *)getData {
   // NSLog(@"getData %d", [self.data length]);
    return m_data;
};
@end

static ICloudHelper *m_ICloudHelper = NULL;
static NSMetadataQuery *icloud_query = nil;
static NSString *  m_fileFullName = nil;
static NSString *  m_folderLocationB = nil;

#pragma mark - ICLOUD_QUERY
@interface ICloudQuery : NSObject
+ (void)queryDidFinishGathering:(NSNotification *)notification;
+ (void)queryDidFinishGatheringList:(NSNotification *)notification;
+ (void)loadData:(NSMetadataQuery *)query;
+ (void)loadDataList:(NSMetadataQuery *)query;
+ (void)loadDocument:(NSString *)withIcloudFileName  toFullFileName:(NSString *)fileName;
+ (void)loadDocuments:(NSString *)file_ext toFolder:(NSString *)folder;
@end

@implementation ICloudQuery

+(void)loadDocuments:(NSString *)file_extentions toFolder:(NSString *)folder {
    
    ICloudHelper::Instance().iCloudLog([NSString stringWithFormat:@"loading documents with extetions %@ to %@", file_extentions, folder]);

    NSMetadataQuery *query = [[NSMetadataQuery alloc] init];
    
    if (icloud_query != nil)
        [icloud_query release];
    
    icloud_query = query;
    
    if ( m_folderLocationB != NULL )
        [m_folderLocationB release];
    m_folderLocationB = [[NSString stringWithFormat:@"%@",folder ] retain];
    //SCOPE
    [icloud_query setSearchScopes:[NSArray arrayWithObject:NSMetadataQueryUbiquitousDocumentsScope]];
    //PREDICATE
    NSPredicate *pred = [NSPredicate predicateWithFormat:@"%K ENDSWITH %@", NSMetadataItemFSNameKey, file_extentions ];
    [icloud_query setPredicate:pred];
    //FINISHED?
    dispatch_async(dispatch_get_main_queue(), ^(void) {
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(queryDidFinishGatheringList:) name:NSMetadataQueryDidFinishGatheringNotification object:icloud_query];
        [icloud_query startQuery];
    });
}

+ (void)loadDocument:(NSString *)withIcloudFileName  toFullFileName:(NSString *)fileName{
    
    ICloudHelper::Instance().iCloudLog([NSString stringWithFormat:@"loading document with icloud name - %@ to local file name - %@", withIcloudFileName, fileName]);
    
    if (icloud_query != nil)
        [icloud_query release];
        
    icloud_query = [[NSMetadataQuery alloc] init];
    
    if(m_fileFullName!= nil)
        [m_fileFullName release];
    
    m_fileFullName = [[NSString stringWithFormat:@"%@",fileName ] retain];
    
    [icloud_query setSearchScopes:[NSArray arrayWithObject: NSMetadataQueryUbiquitousDocumentsScope]];
    NSPredicate *pred = [NSPredicate predicateWithFormat:
                         @"%K == %@", NSMetadataItemFSNameKey, withIcloudFileName];
    [icloud_query setPredicate:pred];
    
    
    dispatch_async(dispatch_get_main_queue(), ^(void) {
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(queryDidFinishGathering:) name:NSMetadataQueryDidFinishGatheringNotification object:icloud_query];
        [icloud_query startQuery];
    });
};

+ (void)queryDidFinishGatheringList:(NSNotification *)notification {
    NSMetadataQuery *query = [notification object];
    [query disableUpdates];
    [query stopQuery];
    [[NSNotificationCenter defaultCenter] removeObserver:self name:NSMetadataQueryDidFinishGatheringNotification object:query];
    [ICloudQuery loadDataList:query];
};

+ (void)queryDidFinishGathering:(NSNotification *)notification {
    NSMetadataQuery *query = [notification object];
    [query disableUpdates];
    [query stopQuery];
    [[NSNotificationCenter defaultCenter] removeObserver:self name:NSMetadataQueryDidFinishGatheringNotification object:query];
    [ICloudQuery loadData:query];
};

+ (void)loadDataList:(NSMetadataQuery *)query {
    if ([query resultCount] == 0) {
        ICloudHelper::Instance().iCloudLog (@"No files to load from cloud");
        ICloudHelper::Instance().runCallback(1, "No file");
        
    }
    else{
        for(int i =0; i< [query resultCount]; ++i){
            NSMetadataItem *item = [query resultAtIndex:i];
            NSURL *url = [item valueForAttribute:NSMetadataItemURLKey];
            NSString *ex_fn = [item valueForAttribute:NSMetadataItemFSNameKey];
            
            ICloudHelper::Instance().iCloudLog ([NSString stringWithFormat:@"loading from cloud - %@ URL: %@", ex_fn, url]);
       
            
            Note *icloud_doc = [[[Note alloc] initWithFileURL:url] autorelease];
            [icloud_doc openWithCompletionHandler:^(BOOL success) {
                
                NSString * full_fn = [NSString stringWithFormat:@"%@%@",m_folderLocationB,ex_fn];
                if (success) {

                    ICloudHelper::Instance().iCloudLog ([NSString stringWithFormat:@"writing to - %@", full_fn]);
                    [[icloud_doc getData] writeToFile:full_fn atomically:YES];
                }
                else
                {
                    ICloudHelper::Instance().iCloudLog ([NSString stringWithFormat:@"writing error to - %@", full_fn]);
                }
            }];
        }
        ICloudHelper::Instance().runCallback(0, "iCloud document opened");
    }
};

+ (void)loadData:(NSMetadataQuery *)query {
    if ([query resultCount] == 1) {
        NSMetadataItem *item = [query resultAtIndex:0];
        NSURL *url = [item valueForAttribute:NSMetadataItemURLKey];
        Note *icloud_doc = [[[Note alloc] initWithFileURL:url] autorelease];
        [icloud_doc openWithCompletionHandler:^(BOOL success) {
            if (success) {
                [[icloud_doc getData] writeToFile:m_fileFullName atomically:YES];
                ICloudHelper::Instance().iCloudLog(@"iCloud document opened");
                ICloudHelper::Instance().runCallback(0, "iCloud document opened");
            } else {
                ICloudHelper::Instance().iCloudLog(@"failed opening document from iCloud");
                ICloudHelper::Instance().runCallback(1, "Failed opening document from iCloud");
            }
        }];
    }else{
        ICloudHelper::Instance().iCloudLog(@"No file");
        ICloudHelper::Instance().runCallback(1, "No file");
    }
};
@end
#pragma mark - ICLOUD_HELPER
ICloudHelper::ICloudHelper() {
    
};

void ICloudHelper::runCallback(int error,const char *msg)
{
    m_func(__func__,error,msg);
}

ICloudHelper &ICloudHelper::Instance() {
    if (!m_ICloudHelper) {
        m_ICloudHelper = new ICloudHelper;
    }
    return *m_ICloudHelper;
};

void ICloudHelper::iCloudLog (NSString * log)
{
    if (enableLogs == true)
        NSLog(@"iCloudLog: %@", log);
}

void ICloudHelper::IC_EnableLogs (bool val)
{
    enableLogs = val;
}

void ICloudHelper::IC_setCallback(funcResult _func) {
    m_func = _func;
};



void ICloudHelper::IC_FileExistsInIcloud(const char *iCloudFileName) {
    
    NSString *fileName = [NSString stringWithUTF8String:iCloudFileName];
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^(void){
        @try {
            
            
            NSFileManager *fm = [[NSFileManager alloc] init];
            NSURL *ubiq = [fm URLForUbiquityContainerIdentifier:nil];
            
            if (ubiq) 
            {
                NSURL *ubiquitousPackage = [[ubiq URLByAppendingPathComponent: @"Documents"] URLByAppendingPathComponent:fileName];
        
                NSString *pathString = [ubiquitousPackage path];
                
                
                bool fileExists = [fm fileExistsAtPath:pathString];
        
                if (fileExists)
                {
                    m_func(__func__, 0, nil);
                    iCloudLog([NSString stringWithFormat: @"file exists at %@",pathString]);
                }
                else
                {
                    m_func(__func__, 1, "File doesnt exist");
                    iCloudLog([NSString stringWithFormat: @"file doesn't exist at %@",pathString]);
                }
                
                
                
            }
            else {
                iCloudLog(@"iCloud is not enabled");
                m_func(__func__,2,"iCloud is not enabled");
            }
        
            [fm release];
        
        }
        @catch (NSException *exception) {
            showException(@"file exists", exception);
        }
    
        });
};

void ICloudHelper::showException(NSString *funcName, NSException *exception)
{
    NSLog(@"iCloud exception in %@", funcName);
    NSLog(@"Name - %@", exception.name);
    NSLog(@"Reason - %@", exception.reason);
    NSLog(@"Descriprion - %@", exception.description);
}

bool ICloudHelper::IC_accessAllowed(){
 
    NSFileManager *fm = [[[NSFileManager alloc] init] autorelease];
    if  ([fm ubiquityIdentityToken])
    {
        return true;
    }
    else
    {
        iCloudLog(@"iCloud is not enabled");
        return false;
    }
  
}



void ICloudHelper::IC_ClearOldCloudFiles(const char* _fileExt, const char* _folder){
    
    NSString *file_ext = [NSString stringWithUTF8String:_fileExt];
    NSString *folder = [NSString stringWithUTF8String:_folder];
    
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^(void){
        @try {
            NSFileManager *fm = [[[NSFileManager alloc] init] autorelease];
        
            NSArray *directoryContent = [fm contentsOfDirectoryAtPath:folder error:NULL];
        
            NSURL *ubiq = [fm URLForUbiquityContainerIdentifier:nil];
            if (!ubiq) {
                return;
                }
        
            for (int i = 0; i < (int)[directoryContent count]; ++i)
            {
               
            
                NSString *path = [folder stringByAppendingPathComponent:[directoryContent objectAtIndex:i] ];
            
                if(![[[directoryContent objectAtIndex:i] pathExtension] isEqualToString:file_ext]){
             
                    iCloudLog([NSString stringWithFormat:@"File %d ------ ignored: %@", i, path]);
                    continue;
                }
            
                NSURL *ubiquitousPackage = [[ubiq URLByAppendingPathComponent: @"Documents"] URLByAppendingPathComponent:[directoryContent objectAtIndex:i]];
                
                iCloudLog([NSString stringWithFormat:@"File %d ------ del: %@    url :  %@", i, path, ubiquitousPackage]);
           
            
                if ( [fm fileExistsAtPath:[ubiquitousPackage path]] ){
                    [fm removeItemAtURL:ubiquitousPackage error:nil];
                }
            
                NSError *error = nil;
                // delete local
                [fm removeItemAtPath:path error:&error];
            
                if (error != nil){
                    NSLog(@"File %d ------ del error: %@", i, error);
                }
            };
        }
        @catch (NSException *exception) {
            showException(@"clear old cloud files", exception);
        }
    });
}


void ICloudHelper::IC_ClearCloudFile(const char* _file_path){
    
    
    NSString *filePath = [NSString stringWithUTF8String:_file_path];
     dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^(void){
         @try {
             NSFileManager *fm = [[[NSFileManager alloc] init] autorelease];
             NSURL *ubiq = [fm URLForUbiquityContainerIdentifier:nil];
             if (!ubiq) {
                 return;
             }
        
                    NSURL *ubiquitousPackage = [[ubiq URLByAppendingPathComponent: @"Documents"] URLByAppendingPathComponent:filePath];
                
                    iCloudLog([NSString stringWithFormat:@"File  ------ del: %@    url :  %@",  filePath, ubiquitousPackage]);
            
            
                    if ( [fm fileExistsAtPath:[ubiquitousPackage path]] ){
                        [fm removeItemAtURL:ubiquitousPackage error:nil];
                    }
            
                    NSError *error = nil;
                    // delete local
                    [fm removeItemAtPath:filePath error:&error];
            
                    if (error != nil){
                        NSLog(@"File ------ del error: %@", error);
                    }
       
         }
            @catch (NSException *exception) {
                showException(@"clear cloud file", exception);
            }
        });
}


void ICloudHelper::IC_saveFileToIcloud(const char *_path,const char *_icloudFileName) {
    
    
    NSString *path = [NSString stringWithUTF8String:_path];
    NSString *icloudFileName = [NSString stringWithUTF8String:_icloudFileName];
    
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^(void){
    
    @try {
        
        NSFileManager *fm = [[[NSFileManager alloc] init] autorelease];
        NSURL *ubiq = [fm URLForUbiquityContainerIdentifier:nil];
        if (!ubiq) {
            iCloudLog(@"No iCloud access");
            m_func(__func__,1,"No iCloud access");
            return;
        }
        NSURL *ubiquitousPackage = [[ubiq URLByAppendingPathComponent:
                                 @"Documents"] URLByAppendingPathComponent:icloudFileName];
   
        if (![fm fileExistsAtPath:path]) {
            iCloudLog([NSString stringWithFormat: @"file not exist at: %@", path]);
        
            m_func(__func__,1,"file not exist");
            return;
        } else {
            Note *doc = [[[Note alloc] initWithFileURL:ubiquitousPackage] autorelease];
            [doc setData:[[[NSData alloc] initWithContentsOfFile:path] autorelease]];
            [doc saveToURL:ubiquitousPackage forSaveOperation:UIDocumentSaveForOverwriting completionHandler:^(BOOL success) {
                if (success) {
                    iCloudLog(@"Saved for overwriting");
                    m_func(__func__,0,"Saved for overwriting");
                } else {
                    iCloudLog(@"Not saved for overwriting");
                    m_func(__func__,1,"Not saved for overwriting");
                }
            }];
        }
        
        }
        @catch (NSException *exception) {
            showException(@"save file", exception);
        }
    });
};

void ICloudHelper::IC_LoadFromIcloud(const char* from ,const char *toFullFileName) {
    [ICloudQuery loadDocument:[NSString stringWithUTF8String:from] toFullFileName:[NSString stringWithUTF8String:toFullFileName]];
};

void ICloudHelper::IC_LoadAllFromIcloud(const char* file_ext ,const char *folder_path) {
    [ICloudQuery loadDocuments:[NSString stringWithUTF8String:file_ext] toFolder:[NSString stringWithUTF8String:folder_path]];
};

extern "C"{
    
    void _IC_EnableLogs (bool val)
    {
        ICloudHelper::Instance().IC_EnableLogs(val);
    }
    
    
    void _IC_ClearCloudFile(const char* file_path)
    {
        ICloudHelper::Instance().IC_ClearCloudFile(file_path);
    }
    
    void _IC_ClearOldCloudFiles(const char* file_ext, const char *folder_path){
        ICloudHelper::Instance().IC_ClearOldCloudFiles(file_ext, folder_path);
    }
    
    bool _IC_AccessAllowed(){
        return ICloudHelper::Instance().IC_accessAllowed();
    }
    void _IC_SaveFile(const char *path,const char *icloudName) {
        ICloudHelper::Instance().IC_saveFileToIcloud(path,icloudName);
    }
    
    void _IC_LoadAllFiles(const char* file_ext, const char *folder_path) {
        ICloudHelper::Instance().IC_LoadAllFromIcloud(file_ext, folder_path);
    }
    
    void _IC_LoadFile(const char* from ,const char *toFullFileName) {
        ICloudHelper::Instance().IC_LoadFromIcloud(from,toFullFileName);
    }
    void _IC_FileExistsInIcloud(const char *fileName) {
        ICloudHelper::Instance().IC_FileExistsInIcloud(fileName);
    }
    
    void _IC_setCallback(funcResult _func) {
        ICloudHelper::Instance().IC_setCallback(_func);
    }
}

