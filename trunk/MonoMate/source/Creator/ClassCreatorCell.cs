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
	class ClassCreatorCell : NSCell 
	{
		public ClassCreatorCell() {}
		public ClassCreatorCell(IntPtr np) : base(np) {}
		
		[ObjectiveCMessage("drawInteriorWithFrame:inView:")]
		public void Draw(NSRect rect, NSView view)
		{
			ClassCreatorItem cci = (ClassCreatorItem)this.ObjectValue;
			if (cci == null) return;
			
			NSArray keys = new NSMutableArray();
			NSArray vals = new NSMutableArray();
			NSAttributedString str;
			NSRect rectc;
			
			// Draw Title
			keys.Clear();
			vals.Clear();
			keys.Add(new NSString("NSFont"));
			vals.Add(NSFont.SystemFontOfSize(16));
			
			keys.Add(new NSString("NSColor"));
			vals.Add(NSColor.BlackColor);
			rectc = rect;
			rectc.origin.x += 60;
			rectc.origin.y += 12;
			
			str = new NSAttributedString(new NSString(cci.Creator.Identifier), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);
			
			// Draw Icon
			NSImage img = cci.Creator.Icon;
			NSRect recti = rect;
			recti.size.width = recti.size.height - 10;
			recti.size.height -= 10;
			recti.origin.x += 10;
			recti.origin.y += 5;
			img.IsFlipped = true;
			img.DrawInRectFromRectOperationFraction(recti, new NSRect(0, 0, img.Size.width, img.Size.height), NSCompositingOperation.NSCompositeSourceOver, 1.0f);
		}		
	}

}