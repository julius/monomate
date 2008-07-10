//
//  Notifier.m
//  MonoChecker
//
//  Created by Julius Eckert on 26.04.08.
//  Copyright 2008 __MyCompanyName__. All rights reserved.
//

#import "Notifier.h"


@implementation Notifier

-(void) awakeFromNib {
	NSLog(@"No Mono at /Library/Frameworks/Mono.framework/Versions/Current !!");
	
	allowLoad = true;
	[[web mainFrame] loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:[[NSBundle mainBundle] pathForResource:@"NoMonoMessage" ofType:@"html"]]]];
}

- (void)webView:(WebView *)sender 
          decidePolicyForNavigationAction:(NSDictionary *)actionInformation request:(NSURLRequest *)request
		  frame:(WebFrame *)frame
          decisionListener:(id <WebPolicyDecisionListener>)listener {
	
	if (allowLoad) {
		[listener use];
		allowLoad = false;
		return;
	} else {
		[listener ignore];
		[[NSWorkspace sharedWorkspace] openURL:[request URL]];
	}
}


- (void)windowWillClose:(NSNotification *)notification {
	[NSApp terminate:self];
}

-(IBAction) close:(id)sender {
	[NSApp terminate:self];
}

@end
