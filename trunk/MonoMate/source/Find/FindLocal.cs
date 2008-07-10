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
	partial class FindLocal : BottomPanel
	{
		public FindLocal() {}
		public FindLocal(IntPtr np) : base(np) {}
		public FindLocal(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}
		
		[ObjectiveCField]
		public NSSearchField search;
		
		[ObjectiveCField]
		public NSTextField statusLabel;
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			this.search.Delegate = this;
			this.Status = FindStatus.Clear;
		}
		
		public override void Focus()
		{
			this.View.Window.MakeFirstResponder(search);
		}
		
		private MonoMate.Editor.SourceEditor CurrentEditor
		{
			get
			{
				TabViewItem item = ((ProjectWindow)this.View.Window.WindowController).tabs.CurrentItem;
				if (item != null) return item.Editor;
				return null;
			}
		}
		
		public FindStatus Status
		{
			set
			{
				if (value == FindStatus.NotFound)
				{
					this.statusLabel.StringValue = "NOT FOUND !";
				}
				else if (value == FindStatus.Found)
				{
					this.statusLabel.StringValue = "Use Up- and Down-Keys to navigate through the results.";
				}
				else
				{
					this.statusLabel.StringValue = "";
				}
			}
		}
		
		public string lastQuery = "";
		public int lastIndex = 0;
		public void FindNext()
		{
			if ((lastQuery == "") || (this.CurrentEditor == null))
			{
				this.Status = FindStatus.Clear;
				return;
			}
			
			
			string text = this.CurrentEditor.Text;
			if (lastIndex >= text.Length) lastIndex = 0;
			int pos = text.IndexOf(lastQuery, lastIndex);
			
			if (pos == -1) 
			{
				pos = text.IndexOf(lastQuery, 0);
				
				if (pos == -1)
				{
					this.Status = FindStatus.NotFound;
					return;
				}
			}
			
			this.lastIndex = pos+1;
			this.CurrentEditor.PresentRange(new NSRange((uint)pos, (uint)lastQuery.Length));
			this.Status = FindStatus.Found;
		}
		
		public void FindPrevious()
		{
			if ((lastQuery == "") || (this.CurrentEditor == null))
			{
				this.Status = FindStatus.Clear;
				return;
			}
			
			string text = this.CurrentEditor.Text;
			int pos = text.LastIndexOf(lastQuery, lastIndex-1);
			
			if (pos == -1) 
			{
				pos = text.LastIndexOf(lastQuery, text.Length-1);
				
				if (pos == -1)
				{
					this.Status = FindStatus.NotFound;
					return;
				}
			}
			
			this.lastIndex = pos+1;
			this.CurrentEditor.PresentRange(new NSRange((uint)pos, (uint)lastQuery.Length));
			this.Status = FindStatus.Found;
		}
		
		// Search Event
		[ObjectiveCMessage("search:")]
		public void Search(Id sender)
		{
			lastQuery = this.search.StringValue;
			lastIndex = 0;
			this.FindNext();
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
					this.FindNext();
					return true;
				}
				
				if (cmdSel == selEscape) 
				{
					this.Status = FindStatus.Clear;
					//if (((string)this.search.StringValue).Length > 0) return false;
					this.Deactivate();
					return true;
				}
				
				if (cmdSel == selMoveUp) 
				{
					this.FindPrevious();
					return true;
				}/**/
				
				if (cmdSel == selMoveDown) 
				{
					this.FindNext();
					return true;
				}/**/
				
				//Console.WriteLine("unknown");
			}
			
			return false;
		}
	
		public static FindLocal Create()
		{
			return new FindLocal(new NSString("FindLocalView"), NSBundle.MainBundle);
		}		
	
	}
	
	public enum FindStatus
	{
		Clear,
		Found,
		NotFound
	}
}
