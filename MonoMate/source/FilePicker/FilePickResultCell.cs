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
	class FilePickResultCell : NSCell 
	{
		public FilePickResultCell() {}
		public FilePickResultCell(IntPtr np) : base(np) {}

		[ObjectiveCMessage("drawInteriorWithFrame:inView:")]
		public void Draw(NSRect rect, NSView view)
		{
			/* 
			NSColor.WhiteColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();/**/
			
			PFile file = (PFile)this.ObjectValue;
			if (file == null) return;
			
			NSArray keys = new NSMutableArray();
			NSArray vals = new NSMutableArray();
			NSAttributedString str;
			NSRect rectc;
			
			// Draw Title
			keys.Clear();
			vals.Clear();
			keys.Add(new NSString("NSFont"));
			
			if (file.AlreadyOpen)
				vals.Add(NSFont.BoldSystemFontOfSize(12));
			else
				vals.Add(NSFont.SystemFontOfSize(12));
			
			keys.Add(new NSString("NSColor"));
			vals.Add(NSColor.BlackColor);
			rectc = rect;
			rectc.origin.x += 10;
			rectc.origin.y -= 2;
			
			str = new NSAttributedString(new NSString(file.Title), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);

			// Draw Folder
			keys.Clear();
			vals.Clear();
			keys.Add(new NSString("NSFont"));
			
			vals.Add(NSFont.SystemFontOfSize(12));
			
			keys.Add(new NSString("NSColor"));
			vals.Add(NSColor.GrayColor);
			rectc = rect;
			float plus = 180;
			if (str.Size.width > plus) plus = str.Size.width + 60;
			rectc.origin.x += plus;
			rectc.origin.y -= 2;
			
			str = new NSAttributedString(new NSString(file.Container), new NSDictionary(vals, keys));
			str.DrawInRect(rectc);
		}		
	}

}