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
	class BMTableCell : NSCell 
	{
		public BMTableCell() {}
		public BMTableCell(IntPtr np) : base(np) {}
		
		public BMPanel bmPanel;
		
		public BMTableCell(BMPanel panel) : base()
		{
			this.bmPanel = panel;	
		}

		[ObjectiveCMessage("drawInteriorWithFrame:inView:")]
		public void Draw(NSRect rect, NSView view)
		{
			/* 
			NSColor.WhiteColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();/**/
			
			NSImage imgError = bmPanel.ErrorIcon;
			NSImage imgWarning = bmPanel.WarningIcon;
			
			BuildMessage msg = (BuildMessage)this.ObjectValue;
			if (msg == null) return;
			
			NSImage img = imgWarning;
			if (msg.Category == BMCategory.Error) img = imgError;
			
			NSRect recti = rect;
			recti.size.width = img.Size.width;
			recti.size.height = img.Size.height;
			recti.origin.x += 4;
			recti.origin.y += 3;
			img.IsFlipped = true;
			img.DrawInRectFromRectOperationFraction(recti, new NSRect(0, 0, img.Size.width, img.Size.height), NSCompositingOperation.NSCompositeSourceOver, 1.0f);
		}		
	}

}