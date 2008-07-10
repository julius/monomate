//
// Author:
//   Julius Eckert
//

//
// Copyright (C) 2008 Julius Eckert (http://www.julius-eckert.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


/*
	This file implements private APIs.
	It is possible that Apple changes them so that they stop working !
*/

using System;
using System.Runtime.InteropServices;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate 
{
	class CGSPrivate 
	{
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern int _CGSDefaultConnection();
		
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern int CGSNewCIFilterByName(int cid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof (IdMarshaler<NSString>))] NSString filterName, ref IntPtr outFilter);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern int CGSAddWindowFilter(int cid, int wid, IntPtr filter, int flags);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern int CGSRemoveWindowFilter(int cid, int wid, IntPtr filter);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern int CGSReleaseCIFilter(int cid, IntPtr filter);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern int CGSSetCIFilterValuesFromDictionary(int cid, IntPtr filter, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof (IdMarshaler<NSDictionary>))] NSDictionary filterValues);
		
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern int CGSGetWindowTransform(int cid, int wid, ref CGAffineTransform outTransform);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern int CGSSetWindowTransform(int cid, int wid, CGAffineTransform transform);

		[DllImport("MMEffects.dylib")]
		public static extern void TransitionEffectStart(int wid, int type, int option, float duration);
		
		[DllImport("MMEffects.dylib")]
		public static extern void TransitionEffectFinish();
		
		
		public static void StartTransitionEffect(int wid, int type, int option, float duration)
		{
			TransitionEffectStart(wid,type,option,duration);
			/*
			NSThread.NSThreadRunner runner = delegate(Id argument)
                            {
                                NSAutoreleasePool pool = new NSAutoreleasePool();
 								
	 								TransitionEffect(wid,type,option,duration);
 
                                pool.Release();
                            };			
			NSThread.DetachNewThreadSelectorToTargetWithObject(runner, null);
			/**/
		}
		
		
		public static void FinishTransitionEffect()
		{
			TransitionEffectFinish();
		}
		
		
		public static void AlphaSwitch(NSWindow window, float duration)
		{
			NSDate d = NSDate.Date;
			float startAlpha = window.AlphaValue;
			while (true)
			{
				float fraction = (float)(-d.TimeIntervalSinceNow) / duration;
				if (fraction > 1) break;
				
				window.AlphaValue = startAlpha + (1.0f-startAlpha) * fraction;
			}
			window.AlphaValue = 1.0f;
		}
	}
	
}