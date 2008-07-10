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
using System.Collections.Generic;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate
{
	[ObjectiveCClass]
	class FileOverviewView : NSView 
	{
		public FileOverviewView() {}
		public FileOverviewView(IntPtr np) : base(np) {}
		
		[ObjectiveCMessage("drawRect:")]
		public void Draw(NSRect rect)
		{
			// Draw Background
			NSColor.WhiteColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();

			NSRect mrect = this.Frame;
			mrect.origin.x = mrect.size.width-1;
			mrect.size.width = 2.0f;
			mrect.size.height -= 40;
			NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.4f, 0.4f, 0.4f, 0.7f).SetFill();
			path = new NSBezierPath();
			path.AppendBezierPathWithRect(mrect);
			path.Fill();
		}
		
	}
	
}