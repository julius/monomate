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
	partial class FindProject
	{
		private FindProjectQuery fpQuery = null;
		
		public void Awake()
		{
			this.View.IsHidden = true;
			this.textSearch.Delegate = this;
			
			this.table.DataSource = this;
			this.table.Delegate = this;
			this.table.TableColumnWithIdentifier(new NSString("PRIMARY")).DataCell = new FindProjectCell();
			this.table.RowHeight = 65;
		}
		
		public void Search()
		{
			OFile root = ((ProjectWindow) this.View.Window.WindowController).filer.Root;
			this.buttonSearch.IsEnabled = false;
			fpQuery = new FindProjectQuery(root, this.textSearch.StringValue);
			
			this.table.SelectRowIndexesByExtendingSelection(new NSIndexSet(new NSRange(0,1)), false);
			this.table.ReloadData();
		}
		
		public void TextDidChange()
		{
			this.buttonSearch.IsEnabled = true;
		}
		
		public void Cancel()
		{
			((ProjectWindow) this.View.Window.WindowController).ViewMode = ProjectViewMode.Code;
		}
		
		
		// Table - Datasource
		public int NumOfRows()
		{
			if (fpQuery == null) return 0;
			return fpQuery.Count;
		}
		
		public Id TableObject(int row)
		{
			if (fpQuery == null) return null;
			return fpQuery.ResultAtRow(row);
		}
		
		private void OpenSelected()
		{
			int row = this.table.SelectedRow;
			FindProjectResult fpr = fpQuery.ResultAtRow(row);
			((ProjectWindow)this.View.Window.WindowController).OpenFileAtRange(fpr.File.FullPath, fpr.Range);
		}
		
		public void TableClick()
		{
			if ((fpQuery == null) || (fpQuery.Count < 1)) return;
			this.OpenSelected();
		}
		
		// Key Events in Searchbox
		[ObjectiveCMessage("control:textView:doCommandBySelector:")]
		public bool ControlCommand(NSControl control, NSTextView textView, IntPtr cmdSel)
		{
			IntPtr selNewLine = sel_registerName("insertNewline:");
			IntPtr selMoveDown = sel_registerName("moveDown:");
			IntPtr selMoveUp = sel_registerName("moveUp:");
			
			
			
			if (control == textSearch)
			{
				if (cmdSel == selNewLine) 
				{
					if (this.buttonSearch.IsEnabled) return false;
					if ((fpQuery == null) || (fpQuery.Count < 1))
						return false;
					this.OpenSelected();
					return false;
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
		
		
		public NSRect Frame
		{
			get { return this.View.Frame; }
			set
			{
				this.View.Frame = value;
			}
		}

		public void Focus()
		{
			this.View.Window.MakeFirstResponder(this.textSearch);
		}
	}
}

