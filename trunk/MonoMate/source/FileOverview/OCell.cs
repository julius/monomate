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
	class OCell : NSCell 
	{
		public OCell() {}
		public OCell(IntPtr np) : base(np) {}

		[ObjectiveCMessage("drawInteriorWithFrame:inView:")]
		public void Draw(NSRect rect, NSView view)
		{
			/* 
			NSColor.WhiteColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();/**/
			
			OFile file = (OFile)this.ObjectValue;
			if (file == null) return;
			
			
			// Draw Icon
			NSRect recti = rect;
			recti.size.width = recti.size.height;
			recti.origin.x += 4;
			file.Icon.IsFlipped = true;
			file.Icon.DrawInRectFromRectOperationFraction(recti, new NSRect(0, 0, file.Icon.Size.width, file.Icon.Size.height), NSCompositingOperation.NSCompositeSourceOver, 1.0f);
			
			NSArray keys = new NSMutableArray();
			NSArray vals = new NSMutableArray();
			NSAttributedString str;
			NSRect rectc;
			
			// Draw Title
			keys.Clear();
			vals.Clear();
			keys.Add(new NSString("NSFont"));
			
			vals.Add(NSFont.SystemFontOfSize(12));
			
			keys.Add(new NSString("NSColor"));
			vals.Add(NSColor.BlackColor);
			rectc = rect;
			rectc.origin.x += 24;
			//rectc.origin.y -= 2;
			
			str = new NSAttributedString(new NSString(file.Title), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);
		}		
	}

}