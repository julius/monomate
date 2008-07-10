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
	partial class FilePicker : NSViewController 
	{
		public FilePicker() {}
		public FilePicker(IntPtr np) : base(np) {}
		public FilePicker(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}
		
		[ObjectiveCField]
		public FileSearcher search;
		
		[ObjectiveCField]
		public NSTableView table;
		
		public event EventHandler Picked;
		
		public List<PFile> PickList;
		public FilePickQuery fpQuery;
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			this.View.IsHidden = true;
			this.search.LostFocus += new EventHandler(LostFocus);
			this.search.Delegate = this;
			
			this.table.DataSource = this;
			this.table.Delegate = this;
			this.table.TableColumnWithIdentifier(new NSString("PRIMARY")).DataCell = new FilePickResultCell();
			this.table.RowHeight = 15;
		}
		
		// Table - Datasource
		[ObjectiveCMessage("numberOfRowsInTableView:")]
		public int NumOfRows(NSTableView tview)
		{
			if (fpQuery == null) return 0;
			return fpQuery.Count;
		}
		
		[ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
		public Id TableObject(NSTableView tview, NSTableColumn col, int row)
		{
			if (fpQuery == null) return null;
			return fpQuery.FileAtRow(row);
		}
		
		
		// Search Event
		[ObjectiveCMessage("search:")]
		public void Search(Id sender)
		{
			this.fpQuery = new FilePickQuery(this.PickList, this.search.StringValue);
			this.table.ReloadData();

			if ((fpQuery == null) || (fpQuery.Count < 1)) return;
			this.table.SelectRowIndexesByExtendingSelection(new NSIndexSet(new NSRange(0,1)), false);
		}		
		
		
		// Key Events in Searchbox
		
		[DllImport("libobjc.dylib", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		public static extern IntPtr sel_registerName([MarshalAs(UnmanagedType.LPStr)] String str);

		[ObjectiveCMessage("control:textView:doCommandBySelector:")]
		public bool ControlCommand(NSControl control, NSTextView textView, IntPtr cmdSel)
		{
			IntPtr selNewLine = sel_registerName("insertNewline:");
			IntPtr selEscape = sel_registerName("cancelOperation:");
			IntPtr selMoveDown = sel_registerName("moveDown:");
			IntPtr selMoveUp = sel_registerName("moveUp:");
			
			
			
			if (control == search)
			{
				if (cmdSel == selNewLine) 
				{
					if ((fpQuery == null) || (fpQuery.Count < 1)) 
					{
						this.search.StringValue = new NSString("");
						this.Search(this);
						return true;
					}
					this.Hide();
					if (this.Picked != null) this.Picked(this, new EventArgs());
					return true;
				}
				
				if (cmdSel == selEscape) 
				{
					if (((string)this.search.StringValue).Length > 0) return false;
					this.Hide();
					return true;
				}
				
				if (cmdSel == selMoveUp) 
				{
					if ((fpQuery == null) || (fpQuery.Count < 1)) return true;
					
					int curSelection = this.table.SelectedRow;
					if (curSelection-1 < 0) return true;
					
					this.table.SelectRowIndexesByExtendingSelection(new NSIndexSet(new NSRange((uint)curSelection-1,1)), false);
					this.table.ScrollRowToVisible(curSelection-1);
					return true;
				}/**/
				
				if (cmdSel == selMoveDown) 
				{
					if ((fpQuery == null) || (fpQuery.Count < 1)) return true;
					
					int curSelection = this.table.SelectedRow;
					if (curSelection+1 >= fpQuery.Count) return true;
					
					this.table.SelectRowIndexesByExtendingSelection(new NSIndexSet(new NSRange((uint)curSelection+1,1)), false);
					this.table.ScrollRowToVisible(curSelection+1);
					return true;
				}/**/
				
				Console.WriteLine("unknown");
			}
			
			return false;
		}	
		
		public PFile PickedFile
		{
			get
			{
				int curSelection = this.table.SelectedRow;
				return fpQuery.FileAtRow(curSelection);				
			}
		}
		
		
		// --------------- Show / Hide
		
		public void LostFocus(object sender, EventArgs args)
		{
			//this.Hide();
		}
		
		public void Hide()
		{
			this.View.IsHidden = true;
		}
		
		public void Show(List<PFile> pickList)
		{
			this.search.StringValue = new NSString("");
			this.PickList = pickList;
			this.Search(this);
			
			this.View.IsHidden = false;
			
			this.View.Window.MakeFirstResponder(search);
			
			NSRect fc = this.View.Window.ContentView.Frame;
			NSRect fs = this.View.Frame;
			
			fs.origin.x = fc.size.width/2 - fs.size.width/2;
			fs.origin.y = fc.size.height - fs.size.height;
			
			this.View.Frame = fs;
		}
		
		public static FilePicker Picker()
		{
			return new FilePicker(new NSString("FilePicker"), NSBundle.MainBundle);
		}		
		
	}
	
	[ObjectiveCClass]
	class PFile : NSObject, IComparable
	{
		public string Title;
		public string FullPath;
		public bool AlreadyOpen;
		
		public string Container
		{
			get
			{
				NSString str = (new NSString(FullPath)).StringByDeletingLastPathComponent;
				string result = str.LastPathComponent;
				result = str.StringByDeletingLastPathComponent.LastPathComponent + "/" + result;
				return result;
			}
		}
		
		public PFile() {}
		public PFile(IntPtr np) : base(np) {}
		
		[ObjectiveCMessage("copyWithZone:")]
		public Id CopyWithZone(Id zone)
		{
			return this;
		}

		public int CompareTo(object obj)
		{
			PFile pf = (PFile) obj;
			return this.AlreadyOpen.CompareTo(pf.AlreadyOpen)*10 - this.Title.CompareTo(pf.Title);
		}
	}

}