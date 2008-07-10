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

namespace AllMyDocs 
{
	[ObjectiveCClass]
	class FindLocalView : NSView
	{
		public FindLocalView() {}
		public FindLocalView(IntPtr np) : base(np) {}
	
		public NSImage imgBar;
		
		public void LoadImages()
		{
			if (imgBar == null) imgBar = new NSImage(NSBundle.MainBundle.PathForImageResource(new NSString("FindBG")));
		}
	
		[ObjectiveCMessage("drawRect:")]
		public override void DrawRect(NSRect rect)
		{
			this.LoadImages();
			
			this.imgBar.DrawInRectFromRectOperationFraction(
				new NSRect(0, -1, this.Frame.size.width, 31),
				new NSRect(0, 0, this.imgBar.Size.width, this.imgBar.Size.height),
				NSCompositingOperation.NSCompositeCopy,
				1.0f);
				
		}
	}
		
}