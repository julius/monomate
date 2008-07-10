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
	partial class SourceView : NSTextView 
	{
		public SourceView() {}
		public SourceView(IntPtr np) : base(np) {}
		
		[ObjectiveCMessage("drawViewBackgroundInRect:")]
		public void DrawBG(NSRect rect)
		{
			//Console.WriteLine("bg");
			NSRect mrect = rect;
			mrect.origin.x = 0;
			mrect.size.width = 48;

			NSColor.WhiteColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();

			NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.2f, 0.2f, 0.2f, 1.0f).SetFill();
			path = new NSBezierPath();
			path.AppendBezierPathWithRect(mrect);
			path.Fill();
			
			mrect.origin.x += mrect.size.width;
			mrect.size.width = 1.0f;
			NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.0f, 0.0f, 0.0f, 0.9f).SetFill();
			path = new NSBezierPath();
			path.AppendBezierPathWithRect(mrect);
			path.Fill();
			
			mrect.origin.x ++;
			NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.0f, 0.0f, 0.0f, 0.6f).SetFill();
			path = new NSBezierPath();
			path.AppendBezierPathWithRect(mrect);
			path.Fill();
			
			int lineheight = 17;
			int linecount = (int)(this.Frame.size.height / lineheight);
			
			NSFont font = NSFont.SystemFontOfSize(12);			
			NSMutableDictionary dict = new NSMutableDictionary();
			dict.Add(new NSString("NSFont"), font);
			dict.Add(new NSString("NSColor"), NSColor.WhiteColor);
			NSMutableParagraphStyle pstyle = new NSMutableParagraphStyle();
			pstyle.SetAlignment(NSTextAlignment.NSCenterTextAlignment);
			dict.Add(new NSString("NSParagraphStyle"), pstyle);
			
			for (int i=0; i < linecount; i++)
			{
				NSRect lrect = new NSRect();
				lrect.origin.x = 8;
				lrect.origin.y = i*lineheight+5;
				lrect.size.width = 33;
				lrect.size.height = lineheight;
				
				if (lrect.origin.y + lrect.size.height < rect.origin.y) continue;
				if (lrect.origin.y > rect.origin.y + rect.size.height) break;
				
				if (rect.origin.x > lrect.origin.x + lrect.size.width) break;
				
				NSString text = new NSString(""+(i+1));
				text.DrawInRectWithAttributes(lrect, dict);
			}
		}
		
		/*
		[ObjectiveCMessage("performKeyEquivalent:")]
		public bool PerformKeyEquivalent(NSEvent evt)
		{
		 	return ObjectiveCRuntime.SendMessageSuper<bool>(this, NSTextViewClass, "performKeyEquivalent:", evt);
		}/**/

		//*
		[ObjectiveCMessage("keyUp:")]
		public override void KeyUp(NSEvent evt)
		{
			if ((evt.KeyCode == 48) && ((evt.ModifierFlags & (uint)NSEventMask.NSShiftKeyMask) == (uint)NSEventMask.NSShiftKeyMask))
				this.SendKeyCommand(KeyCommand.UnTab);
				
			ObjectiveCRuntime.SendMessageSuper(this, NSTextViewClass, "keyUp:", evt);
		}
		
		public void SendKeyCommand(KeyCommand cmd)
		{
			ObjectiveCRuntime.SendMessage(this.Delegate, "additionalKeyCommand:", cmd);
		}
	}
	
	public enum KeyCommand
	{
		UnTab
	}

}