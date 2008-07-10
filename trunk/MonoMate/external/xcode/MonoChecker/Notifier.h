//
//  Notifier.h
//  MonoChecker
//
//  Created by Julius Eckert on 26.04.08.
//  Copyright 2008 __MyCompanyName__. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import <WebKit/WebKit.h>

@interface Notifier : NSObject {
	IBOutlet WebView* web;
	bool allowLoad;
}

-(IBAction) close:(id)sender;

@end
