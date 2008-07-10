//
//  MMEffects.m
//  MMEffects
//
//  Created by Julius Eckert on 08.06.08.
//  Copyright 2008 __MyCompanyName__. All rights reserved.
//

#import "MMEffects.h"

#import "CGSPrivate.h"

CGSConnection cgs;
int cgs_handle;
float lastDuration;

void TransitionEffectStart(int wid, int type, int option, float duration) {
	cgs_handle=-1;
	lastDuration = duration;
	CGSTransitionSpec spec;
	
	spec.unknown1=0;
	spec.type=type;
	spec.option=option | (1<<7);
	spec.backColour=0;
	spec.wid=wid;
	
	cgs= _CGSDefaultConnection();
	
	CGSNewTransition(cgs, &spec, &cgs_handle);
	
	CGSInvokeTransition(cgs, cgs_handle, duration);
}

void TransitionEffectFinish() {
	if (cgs_handle == 0) return;
	usleep((useconds_t)(1000000*lastDuration));
	
	cgs= _CGSDefaultConnection();
	CGSReleaseTransition(cgs, cgs_handle);
	cgs_handle=0;
}
	
@implementation MMEffects

@end
