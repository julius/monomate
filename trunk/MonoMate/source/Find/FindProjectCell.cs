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


using System;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate
{
	[ObjectiveCClass]
	class FindProjectCell : NSCell
	{
		public FindProjectCell() {}
		public FindProjectCell(IntPtr np) : base(np) {}

		[ObjectiveCMessage("drawInteriorWithFrame:inView:")]
		public void Draw(NSRect rect, NSView view)
		{
			FindProjectResult result = (FindProjectResult)this.ObjectValue;
			if (result == null) return;
			
			NSArray keys = new NSMutableArray();
			NSArray vals = new NSMutableArray();
			NSAttributedString str;
			NSRect rectc;
			
			// Draw Title
			keys.Clear();
			vals.Clear();
			keys.Add(new NSString("NSFont"));
			
			vals.Add(NSFont.BoldSystemFontOfSize(10));
			
			keys.Add(new NSString("NSColor"));
			vals.Add(NSColor.BlackColor);
			rectc = rect;
			rectc.origin.x += 10;
			rectc.origin.y += 4;
			
			str = new NSAttributedString(new NSString(result.File.Title), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);
			
			// Draw InFile Gray Areas
			keys.Clear();
			vals.Clear();
			keys.Add(new NSString("NSFont"));
			
			vals.Add(NSFont.SystemFontOfSize(10));
			
			keys.Add(new NSString("NSColor"));
			vals.Add(NSColor.GrayColor);
			rectc = rect;
			rectc.origin.x += 10;
			rectc.origin.y += 18;
			
			str = new NSAttributedString(new NSString(result.BeforeLine), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);

			rectc.origin.y += 15;
			str = new NSAttributedString(new NSString(result.TargetLineBefore), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);
			rectc.origin.x += str.Size.width + 0;

			str = new NSAttributedString(new NSString(result.TargetLineMain), new NSDictionary(vals, keys));
			rectc.origin.x += str.Size.width + 0;
			
			str = new NSAttributedString(new NSString(result.TargetLineAfter), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);
			
			rectc.origin.y += 15;
			rectc.origin.x = rect.origin.x + 10;
			str = new NSAttributedString(new NSString(result.AfterLine), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);
			
			// Draw Main Part
			keys.Clear();
			vals.Clear();
			keys.Add(new NSString("NSFont"));
			
			vals.Add(NSFont.SystemFontOfSize(10));
			
			keys.Add(new NSString("NSColor"));
			vals.Add(NSColor.BlackColor);
			rectc = rect;
			rectc.origin.x += 10;
			rectc.origin.y += 18 + 15;
			str = new NSAttributedString(new NSString(result.TargetLineBefore), new NSDictionary(vals, keys));
			rectc.origin.x += str.Size.width + 0;
			
			str = new NSAttributedString(new NSString(result.TargetLineMain), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);
		}		
	}
}

