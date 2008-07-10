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
using System.Runtime.InteropServices;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate
{
	[ObjectiveCClass]
	partial class BMPanel : BottomPanel
	{
		public BMPanel() {}
		public BMPanel(IntPtr np) : base(np) {}
		public BMPanel(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}
		
		[ObjectiveCField]
		public BMTable table;
		
		[ObjectiveCField]
		public NSScrollView viewScroll;
		
		public Builder builder;
		public NSImage ErrorIcon;
		public NSImage WarningIcon;
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			table.bmPanel = this;
			table.DataSource = this;
			table.TableColumnWithIdentifier(new NSString("ICON")).DataCell = new BMTableCell(this);
			
			builder.BuildFinished += new EventHandler(TableReload);
			this.ErrorIcon = new NSImage(NSBundle.MainBundle.PathForImageResource(new NSString("Error")));
			this.WarningIcon = new NSImage(NSBundle.MainBundle.PathForImageResource(new NSString("Warning")));
		}
		
		[ObjectiveCMessage("tableAction:")]
		public void TableAction(Id sender)
		{
			int sel = this.table.SelectedRow;
			if ((sel < 0) || (sel >= builder.bmList.Count)) return;
			BuildMessage msg = builder.bmList[sel];
			
			((ProjectWindow)this.View.Window.WindowController).OpenFileAtLine(msg.FullPath, msg.Line);
		}
		
		public void TableReload(object sender, EventArgs args)
		{
			table.ReloadData();	
		}
		
		
		// Table - Datasource
		[ObjectiveCMessage("numberOfRowsInTableView:")]
		public int NumOfRows(NSTableView tview)
		{
			if (builder == null) return 0;
			return builder.bmList.Count;
		}
		
		[ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
		public Id TableObject(NSTableView tview, NSTableColumn col, int row)
		{
			if (builder == null) return null;
			if (row >= builder.bmList.Count) return  null;
			BuildMessage msg = builder.bmList[row];

			if (col == table.TableColumnWithIdentifier(new NSString("ICON")))
				return msg.Retain();
			
			if (col == table.TableColumnWithIdentifier(new NSString("FILE")))
				return (new NSString(msg.FullPath)).LastPathComponent;

			if (col == table.TableColumnWithIdentifier(new NSString("LINE")))
				return new NSString(""+msg.Line);

			if (col == table.TableColumnWithIdentifier(new NSString("MESSAGE")))
				return new NSString(msg.Message);

			if (col == table.TableColumnWithIdentifier(new NSString("CODE")))
				return new NSString(msg.Code);

			return new NSString("x");
		}

		
		public override void Focus()
		{
			this.View.Window.MakeFirstResponder(table);
		}
		
		public static BMPanel Create()
		{
			return new BMPanel(new NSString("BuildMessageView"), NSBundle.MainBundle);
		}		
	
		public override NSRect Frame
		{
			set
			{
				base.Frame = value;
				NSRect tFrame = this.viewScroll.Frame;
				tFrame.size.width = value.size.width;
				this.viewScroll.Frame = tFrame;
			}
			get { return base.Frame; }
		}
	}
}
