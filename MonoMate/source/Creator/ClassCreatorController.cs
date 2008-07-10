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
	class ClassCreatorController : NSViewController 
	{
		public ClassCreatorController() {}
		public ClassCreatorController(IntPtr np) : base(np) {}
		public ClassCreatorController(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}
		
		public event EventHandler Created;
		public event EventHandler Canceled;

		[ObjectiveCField]
		public NSTextField labelError;
		[ObjectiveCField]
		public NSTextField labelNib;
		[ObjectiveCField]
		public NSTextField labelSuper;
		[ObjectiveCField]
		public NSTextField textName;
		[ObjectiveCField]
		public NSTextField textNib;
		[ObjectiveCField]
		public NSTextField textPath;
		[ObjectiveCField]
		public NSTextField textSuper;
		[ObjectiveCField]
		public NSTextField textNSpace;
		[ObjectiveCField]
		public NSTableView table;
		
		public List<ClassCreatorItem> cciList = new List<ClassCreatorItem>();
		public ClassCreator CurrentCreator = null;
		
		[ObjectiveCMessage("cancel:")]
		public void Cancel(Id sender)
		{
			if (Canceled != null) this.Canceled(this, null);
		}
		
		[ObjectiveCMessage("create:")]
		public void Create(Id sender)
		{
			CurrentCreator.Create(this.textPath.StringValue, this.textName.StringValue, this.textNSpace.StringValue, this.textNib.StringValue, this.textSuper.StringValue);
		}
		
		public NSRect Frame
		{
			get { return this.View.Frame; }
			set
			{
				this.View.Frame = value;
			}
		}
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			this.View.IsHidden = true;
			
			this.table.Delegate = this;
			this.table.DataSource = this;
			this.table.RowHeight = 45;
			this.table.TableColumnWithIdentifier(new NSString("PRIMARY")).DataCell = new ClassCreatorCell();
		}
		
	
		private NSString resourcePath;
		public NSString ResourcePath
		{
			set
			{
				this.resourcePath = value;
				this.LoadCreators();
				table.ReloadData();
			}
		}
		
		
		public void LoadCreators()
		{
			NSString templPath = NSBundle.MainBundle.ResourcePath.StringByAppendingPathComponent(new NSString("Templates/Classes"));
			NSDirectoryEnumerator e = NSFileManager.DefaultManager.EnumeratorAtPath(templPath);
			NSString file;
			
			while ((file = ObjectiveCRuntime.SendMessage<NSString>(e, "nextObject")) != null)
			{
				if (((string)file.LastPathComponent) == "Template.xml")
				{
					CCNormal creator = new CCNormal(templPath.StringByAppendingPathComponent(file), this.resourcePath);
					creator.Created += new EventHandler(CreationSuccess);
					creator.Failed += new EventHandler(CreationFailed);
					cciList.Add(new ClassCreatorItem(creator));
				}
			}
		}
		
		
		[ObjectiveCMessage("tableView:shouldSelectRow:")]
		public bool ShouldSelect(NSTableView tview, int row)
		{
			ClassCreatorItem cci = cciList[row];
			this.SelectCreator(cci.Creator);
			return true;
		}
		
		
		// Table - Datasource
		[ObjectiveCMessage("numberOfRowsInTableView:")]
		public int NumOfRows(NSTableView tview)
		{
			return cciList.Count;
		}
		
		[ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
		public Id TableObject(NSTableView tview, NSTableColumn col, int row)
		{
			return cciList[row].Retain();
		}

		public void Focus()
		{
			this.View.Window.MakeFirstResponder(this.table);
			this.labelError.IsHidden = true;
			
			this.table.SelectRowIndexesByExtendingSelection(new NSIndexSet(new NSRange(0,1)), false);
			this.SelectCreator(cciList[0].Creator);
		}
		
		public void SelectCreator(ClassCreator cc)
		{
			CurrentCreator = cc;
			this.textName.StringValue = new NSString("");
			this.textNSpace.StringValue = new NSString(((ProjectWindow)this.View.Window.WindowController).Path).LastPathComponent;
			
			if (cc.SuperClassChoosable)
			{
				this.textSuper.IsHidden = false;
				this.labelSuper.IsHidden = false;
				this.textSuper.StringValue = new NSString(cc.SuperClassStd);
			}
			else
			{
				this.textSuper.IsHidden = true;
				this.labelSuper.IsHidden = true;
			}
			
			if (cc.SupportsNib)
			{
				this.textNib.IsHidden = false;
				this.labelNib.IsHidden = false;
				this.textNib.StringValue = new NSString("");
			}
			else
			{
				this.textNib.IsHidden = true;
				this.labelNib.IsHidden = true;
			}
			
			this.textPath.StringValue = new NSString(((ProjectWindow)this.View.Window.WindowController).filer.CurrentDirectory.FullPath);
		}
		
		public void CreationSuccess(object sender, EventArgs arg)
		{
			if (Created != null) this.Created(this, null);
		}
		
		public void CreationFailed(object sender, EventArgs arg)
		{
			this.labelError.StringValue = ((ClassCreator) sender).Error;
			this.labelError.IsHidden = false;
		}
		
		public static ClassCreatorController Create()
		{
			return new ClassCreatorController(new NSString("CreateClass"), NSBundle.MainBundle);
		}		
	}

}