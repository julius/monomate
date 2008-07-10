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
	abstract class BottomPanel : NSViewController 
	{
		public BottomPanel() {}
		public BottomPanel(IntPtr np) : base(np) {}
		public BottomPanel(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}
		
		public BottomView bView;
		
		public bool Visible
		{
			get { return !this.View.IsHidden; }
			set
			{
				this.View.IsHidden = !value;
				if (value) this.Focus();
			}
		}
		
		public void Activate()
		{
			this.bView.Activate(this);
		}
		
		public void Deactivate()
		{
			this.Visible = false;
			this.bView.UpdateUI();	
		}
		
		public abstract void Focus();
		public virtual NSRect Frame
		{
			set
			{
				this.View.Frame = value;	
			}
			get { return this.View.Frame; }
		}
	}
}