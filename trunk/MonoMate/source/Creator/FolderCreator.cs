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
	class FolderCreator : NSWindowController 
	{
		public FolderCreator() {}
		public FolderCreator(IntPtr np) : base(np) {}
		public FolderCreator(NSString nibName) : base(nibName) {}
		
		[ObjectiveCField]
		public NSTextField textName;

		[ObjectiveCMessage("cancel:")]
		public void Cancel(Id sender)
		{
			NSApplication.SharedApplication.EndSheetReturnCode(this.Window, 1);
		}
		
		[ObjectiveCMessage("create:")]
		public void Create(Id sender)
		{
			NSApplication.SharedApplication.EndSheetReturnCode(this.Window, 0);
		}
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
		}

		public static FolderCreator Create()
		{
			FolderCreator fc = new FolderCreator(new NSString("FolderCreator"));
			return fc;
		}		
	}
}


