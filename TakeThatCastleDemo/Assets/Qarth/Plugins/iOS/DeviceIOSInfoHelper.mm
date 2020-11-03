#import <Foundation/Foundation.h>
#import <AdSupport/AdSupport.h>

#pragma mark - "C"
extern "C" {
    char* _GetIDFA () {        
        return AllocCString([[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString]);
    }
    
    bool _HasIDFA () {
        return [[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled];
    }
}
