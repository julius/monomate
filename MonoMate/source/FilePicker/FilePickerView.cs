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
	partial class FilePickerView : NSView 
	{
		public FilePickerView() {}
		public FilePickerView(IntPtr np) : base(np) {}
		
		[ObjectiveCMessage("isFlipped")]
		public new bool IsFlipped()
		{
			return false;
		}
		
		[ObjectiveCMessage("drawRect:")]
		public void Draw(NSRect rect)
		{
			rect = new NSRect(0, 0, this.Frame.size.width, this.Frame.size.height);
			// Draw Background
			NSColor.ClearColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();
			
			// activate shadow
			NSShadow s = new NSShadow();
			s.ShadowColor = NSColor.BlackColor.ColorWithAlphaComponent(0.9f);
			s.ShadowBlurRadius = 6.0f;
			s.ShadowOffset = new NSSize(0, 0);
			
			NSGraphicsContext.CurrentContext.SaveGraphicsState();
			s.Set();
			
			NSRect tRect = rect;
			tRect.origin.y += 5;
			tRect.origin.x += 5;
			tRect.size.height -= 5;
			tRect.size.width -= 10;  

			NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.05f, 0.05f, 0.05f, 0.7f).SetFill();
			path = new NSBezierPath();
			path.AppendBezierPathWithRect(tRect);
			path.Fill();

			NSGraphicsContext.CurrentContext.RestoreGraphicsState();
		}
	}

}