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
using System.Runtime.InteropServices;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate
{
	[ObjectiveCClass]
	partial class FindProject : NSViewController
	{
		public FindProject() {}
		public FindProject(IntPtr np) : base(np) {}
		public FindProject(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}
		
		[ObjectiveCField]
		public NSTextField textSearch;
		
		[ObjectiveCField]
		public NSButton buttonSearch;
		
		[ObjectiveCField]
		public NSTableView table;
		
		[ObjectiveCMessage("awakeFromNib")]
		public void awakeFromNib() { this.Awake(); }
		
		[ObjectiveCMessage("search:")]
		public void search(Id sender) { this.Search(); }
		
		[ObjectiveCMessage("cancel:")]
		public void cancel(Id sender) { this.Cancel(); }
		
		[ObjectiveCMessage("tableClick:")]
		public void tableClick(Id sender) { this.TableClick(); }
		
		[ObjectiveCMessage("numberOfRowsInTableView:")]
		public int numberOfRowsInTableView(NSTableView tview) { return this.NumOfRows(); }

		[ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
		public Id tableViewobjectValueForTableColumnrow(NSTableView tview, NSTableColumn col, int row)
		{ return TableObject(row); }
		
		[ObjectiveCMessage("controlTextDidChange:")]
		public void controlTextDidChange(NSNotification n) { this.TextDidChange(); }

		[DllImport("libobjc.dylib", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
       public static extern IntPtr sel_registerName([MarshalAs(UnmanagedType.LPStr)] String str);

		public static FindProject Create()
		{
			return new FindProject(new NSString("FindProject"), NSBundle.MainBundle);
		}		
		
	}
}

