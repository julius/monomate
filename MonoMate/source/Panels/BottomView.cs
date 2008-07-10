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
	class BottomView : NSView 
	{
		public BottomView() {}
		public BottomView(IntPtr np) : base(np) {}
		
		public event EventHandler UpdatedUI;
		public List<BottomPanel> panelList = new List<BottomPanel>();
		
		public BottomView(NSRect rect) : base(rect)
		{
		}
		
		public BottomPanel ActivePanel
		{
			get
			{
				foreach (BottomPanel p in panelList)
					if (p.Visible) return p;
				return null;
			}
		}
		
		public void Activate(BottomPanel panel)
		{
			foreach (BottomPanel p in panelList)
			{
				if (p == panel)
				{
					p.Visible = true;
					p.Focus();
				}
				else
				{
					p.Visible = false;
				}
			}
			
			this.UpdateUI();
		}
		
		public void UpdateUI()
		{
			BottomPanel panel = this.ActivePanel;
			if (panel == null) 
			{
				this.IsHidden = true;
				((ProjectWindow)this.Window.WindowController).tabs.Focus();
			}
			else
			{
				this.IsHidden = false;
				this.ResizeUI();
			}
			if (this.UpdatedUI != null) this.UpdatedUI(this, new EventArgs());
		}
		
		public void ResizeUI()
		{
			BottomPanel panel = this.ActivePanel;
			if (panel == null) return;

			NSRect fw = this.Window.ContentView.Frame;
			NSRect ff = ((ProjectWindow)this.Window.WindowController).filer.View.Frame;
			fw.origin.x = ff.size.width;
			fw.size.width -= fw.origin.x;
			
			NSRect fp = panel.Frame;
			
			fp.origin = new NSPoint(0,0);
			fp.size.width = fw.size.width;
			
			fw.size.height = fp.size.height;
			this.Frame = fw;
			panel.Frame = fp;
		}
		
		public void Add(BottomPanel panel)
		{
			this.AddSubview(panel.View);
			this.panelList.Add(panel);
			
			panel.bView = this;
			panel.Visible = false;
		}
		
	}
}