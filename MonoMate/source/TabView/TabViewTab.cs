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
	class TabViewTab : NSView 
	{
		public TabViewTab() {}
		public TabViewTab(IntPtr np) : base(np) {}
		
		public TabViewTab(NSRect rect) : base(rect)
		{
		}
		
		private TabView tabView;
		private TabViewItem item;
		public NSButton CloseButton;
		
		public void Init(TabView tview, TabViewItem item)
		{
			this.tabView = tview;
			this.item = item;
			this.item.Tab = this;
			
			this.item.Editor.Changed += new EventHandler(EditorChanged);
			
			NSButton btn = new NSButton();
			btn.SetButtonType(NSButtonType.NSToggleButton);
			btn.IsBordered = false;
			btn.BezelStyle = NSBezelStyle.NSRegularSquareBezelStyle;
			btn.Image = this.tabView.closeImage;
			btn.FocusRingType = NSFocusRingType.NSFocusRingTypeNone;
			this.AddSubview(btn);
			
			this.CloseButton = btn;
			this.CloseButton.ActionEvent += new ActionEventHandler(CloseButtonClick);
		}
		
		public void CloseButtonClick(Id sender)
		{
			this.item.Editor.controller.CloseCurrent(this.item.FullPath);
		}
		
		public void EditorChanged(object sender, EventArgs args)
		{
			this.UpdateUI();
		}
		
		[ObjectiveCMessage("mouseDown:")]
		public override void MouseDown(NSEvent evt)
		{
			this.tabView.NavigateItem(this.item);
		}		
		
		public NSAttributedString String
		{
			get
			{
				NSColor tabText = NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.1f, 0.1f, 0.1f, 1.0f);
				NSFont font = NSFont.SystemFontOfSize(12);			
				NSMutableDictionary dict = new NSMutableDictionary();
				dict.Add(new NSString("NSFont"), font);
				dict.Add(new NSString("NSColor"), tabText);
				NSMutableParagraphStyle pstyle = new NSMutableParagraphStyle();
				pstyle.SetAlignment(NSTextAlignment.NSCenterTextAlignment);
				pstyle.SetLineBreakMode(NSLineBreakMode.NSLineBreakByTruncatingTail);
				dict.Add(new NSString("NSParagraphStyle"), pstyle);
				
				return new NSAttributedString(item.Title, dict); 
			}
		}
		
		public void UpdateUI()
		{
			// Update CloseButton
			this.CloseButton.Frame = new NSRect(6, 3, 20, 20);
			
			if (item.Editor.HasChanged) 
			{
				this.CloseButton.Image = this.tabView.closeImage2;
				this.CloseButton.AlternateImage = this.tabView.closeImage2A;
			}
			else 
			{
				this.CloseButton.Image = this.tabView.closeImage;
				this.CloseButton.AlternateImage = this.tabView.closeImageA;
			}
		}
		
		[ObjectiveCMessage("drawRect:")]
		public void Draw(NSRect rect)
		{
			rect = new NSRect(0, 0, this.Frame.size.width, this.Frame.size.height);
			// Draw Background
			NSColor.ClearColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();
			
			//if (rect.size.width < 50) return;
			// activate shadow
			NSShadow s = new NSShadow();
			s.ShadowColor = NSColor.BlackColor.ColorWithAlphaComponent(0.5f);
			s.ShadowBlurRadius = 4.0f;
			s.ShadowOffset = new NSSize(0, 0);
			
			NSGraphicsContext.CurrentContext.SaveGraphicsState();
			s.Set();
			
			// Draw Tab
			NSColor tabNormal = NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.7f, 0.7f, 0.7f, 1.0f);
			NSColor tabSelected = NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.9f, 0.9f, 0.9f, 1.0f);
			NSColor tabBorder = NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.3f, 0.3f, 0.3f, 1.0f);

			NSColor tabBG = tabNormal;
			if (item == this.tabView.CurrentItem) tabBG = tabSelected;

			// create text
			NSAttributedString text = this.String;
			
			NSRect tabRect = rect;
			tabRect.origin.y -= 20;
			tabRect.size.height += 17;
			tabRect.origin.x += 5;
			tabRect.size.width -= 10;  
				
			// Draw Tab
			tabBorder.SetFill();
			path = new NSBezierPath();
			path.AppendBezierPathWithRoundedRectXRadiusYRadius(tabRect, 4.5f, 4.5f);
			path.Fill();

			NSGraphicsContext.CurrentContext.RestoreGraphicsState();

			tabBG.SetFill();
			tabRect.origin.x += 0.8f;
			tabRect.size.width -= 1.6f;
			tabRect.origin.y -= 0.8f;
			path = new NSBezierPath();
			path.AppendBezierPathWithRoundedRectXRadiusYRadius(tabRect, 4.5f, 4.5f);
			path.Fill();
				
			// Draw Text
			NSRect trect = tabRect;
			trect.origin.x += 15;
			trect.size.width -= 15;
			trect.origin.y -= 4;
			text.DrawInRect(trect);
		}
		
	}

}