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

namespace MonoMate.Editor 
{
	[ObjectiveCClass]
	partial class SourceEditor : NSViewController 
	{
		public SourceEditor() {}
		public SourceEditor(IntPtr np) : base(np) {}
		public SourceEditor(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}
		
		[ObjectiveCField]
		public NSScrollView viewScroll;
		
		[ObjectiveCField]
		public NSTextView viewText;
		
		[ObjectiveCField]
		public NSTextField labelStatus;
		
		public Controller controller = null;

		public NSRect Frame
		{
			get { return this.View.Frame; }
			set
			{
				this.View.Frame = value;
				
				/*
				this.viewScroll.Frame = new NSRect(value.origin.x, value.origin.y + 25, value.size.width, value.size.height-25);
				this.labelStatus.Frame = new NSRect(value.origin.x+20, value.origin.y + 6, value.size.width-40, 17);
				/**/
				this.viewScroll.Frame = new NSRect(value.origin.x, value.origin.y + 0, value.size.width, value.size.height-0);
				
				this.viewText.TextContainerInset = new NSSize(50, 5);
			}
		}
		
		[ObjectiveCMessage("becomeFirstResponder")]
		public new bool BecomeFirstResponder()
		{
			this.View.Window.MakeFirstResponder(this.viewText);
			return true;
		}
		
		public void SetConfig()
		{
			NSSize bigSize = new NSSize( 10000000, 10000000 );

			this.viewScroll.HasHorizontalScroller = true;
			
			this.viewText.TextContainer.ContainerSize = bigSize;
			this.viewText.TextContainer.WidthTracksTextView = false;
			this.viewText.TextContainer.HeightTracksTextView = false;
			
			//NSRect frame = new NSRect(0, 0, this.viewScroll.ContentSize.width, this.viewScroll.ContentSize.height);
			
			this.viewText.IsRichText = false;
			this.viewText.MaxSize = bigSize;
			this.viewText.IsHorizontallyResizable = true; 
			this.viewText.IsVerticallyResizable = true; 
			
			this.viewText.TextStorage.Delegate = this;
		}
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			this.SetConfig();
		}
		
		public static SourceEditor Editor()
		{
			return new SourceEditor(new NSString("EditorView"), NSBundle.MainBundle);
		}		
	}

}