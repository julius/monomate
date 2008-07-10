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
	class TabView : NSView 
	{
		public TabView() {}
		public TabView(IntPtr np) : base(np) {}
		
		public TabView(NSRect rect) : base(rect)
		{
			this.closeImage = new NSImage(NSBundle.MainBundle.PathForImageResource(new NSString("CloseTab")));
			this.closeImage2 = new NSImage(NSBundle.MainBundle.PathForImageResource(new NSString("CloseTabModified")));
			this.closeImageA = new NSImage(NSBundle.MainBundle.PathForImageResource(new NSString("CloseTabPressed")));
			this.closeImage2A = new NSImage(NSBundle.MainBundle.PathForImageResource(new NSString("CloseTabPressedModified")));
		}
		
		public NSImage closeImage;
		public NSImage closeImage2;
		public NSImage closeImageA;
		public NSImage closeImage2A;
		public int Selection = 0;
		public List<TabViewItem> itemList = new List<TabViewItem>();
		
		public void AddTab(TabViewItem item)
		{
			itemList.Add(item);
			this.AddSubview(item.Editor.View);
			
			item.Editor.View.NextKeyView = this;
			
			TabViewTab tvt = new TabViewTab(new NSRect(0, 0, 100, 20));
			this.AddSubview(tvt);
			tvt.Init(this, item);
			
			this.Focus();
		}
		
		public TabViewItem ItemByPath(string path)
		{
			foreach (TabViewItem item in itemList)
				if (item.FullPath == path) return item;
			return null;
		}
		
		public TabViewItem CurrentItem
		{
			get 
			{
				if (this.itemList.Count < 1) return null;
				if (this.Selection < 0) return null;
				if (this.Selection >= this.itemList.Count) return null;
				
				return this.itemList[this.Selection];
			}
		}
		
		public void Remove(TabViewItem item)
		{
			this.itemList.Remove(item);
			item.Remove();
			
			if (CurrentItem == null) this.Selection--;
			if (this.Selection < 0) this.Selection = 0;
			
			this.UpdateUI();
		}
		
		public void Focus()
		{
			TabViewItem item = this.CurrentItem;
			if (item == null) 
			{
				((ProjectWindow) this.Window.WindowController).filer.Focus();
				return; 
			}
			this.Window.MakeFirstResponder(item.Editor);
		}
		
		public void NavigateLeft()
		{
			this.Selection--;
			if (this.Selection < 0) this.Selection = this.itemList.Count -1;
			this.UpdateUI();
			this.Focus();
		}
		
		public void NavigateRight()
		{
			this.Selection++;
			if (this.Selection >= this.itemList.Count) this.Selection = 0;
			this.UpdateUI();
			this.Focus();
		}
		
		public void NavigateItem(TabViewItem item)
		{
			for (int i=0; i<itemList.Count; i++)
			{
				TabViewItem tvi = itemList[i];
				if (tvi == item)
				{
					Selection = i;
					this.UpdateUI();
					this.Focus();
					return;
				}
			}
		}
		
		public void UpdateUI()
		{
			NSRect frame = this.Frame;
			frame.size.height -= 30;
			frame.origin = new NSPoint(0,0);
			int lastX = -3;
			for (int i=0; i<itemList.Count; i++)
			{
				TabViewItem item = itemList[i];
				item.Editor.Frame = frame;
				
				if (i == this.Selection)
					item.Editor.View.IsHidden = false;
				else
					item.Editor.View.IsHidden = true;
					
				item.Tab.Frame = new NSRect(lastX, 0 + this.Frame.size.height-30, item.Tab.String.Size.width+40, 29);
				lastX += (int)item.Tab.Frame.size.width + -5;
				
				item.Tab.UpdateUI();
			}
			
			this.NeedsDisplay = true;
		}
		
		[ObjectiveCMessage("drawRect:")]
		public void Draw(NSRect rect)
		{
			//Console.WriteLine("draw");
			
			// Draw Background
			NSColor.WhiteColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();
		}
	}
}