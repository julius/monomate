#import <Foundation/Foundation.h>
#import <Notifier.h>

int main (int argc, const char * argv[]) {
    NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];
    NSLog(@"Checking...");
	
	bool monoAvailable = false;
	if([[NSFileManager defaultManager] fileExistsAtPath:@"/Library/Frameworks/Mono.framework/Versions/Current"]) monoAvailable = true;
		
	if (monoAvailable) {
		NSLog(@"Mono installed.");
	} else {
		
		[NSApplication sharedApplication];
		
		ProcessSerialNumber* psn;
		GetCurrentProcess(psn);
		TransformProcessType(psn, kProcessTransformToForegroundApplication);
		SetFrontProcess(psn);
		
		[NSBundle loadNibNamed:@"MonoChecker" owner:NSApp];
		[NSApp run];
		
	}
	
    [pool drain];
    return 0;
}
