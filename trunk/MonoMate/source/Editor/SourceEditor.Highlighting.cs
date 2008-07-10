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
	partial class SourceEditor 
	{
		NSRange hrange;
		NSMutableAttributedString hString;
		bool hlBusy = false;
		
		public void UpdateHighlighting(bool fullUpdate)
		{
			try
			{
				_UpdateHighlighting(fullUpdate);
			}
			catch (Exception e)
			{
				Console.WriteLine("[SyntaxHighlighting] Exception raised: " + e.Message + "\n\nStack:\n" + e.StackTrace);
			}
		}
				
		private void _UpdateHighlighting(bool fullUpdate)
		{
			if (hlBusy) return;
			
			hlBusy = true;
			
			// calculate range which has changed and/or is affected by the changes
			hrange = new NSRange(0, (uint)this.Text.Length);
			if (!fullUpdate)
			{
				hrange = this.viewText.TextStorage.EditedRange;
				if (this.symbolList.Count > 0) ExtendRangeWithSymbols();
			}
			this.ProcessSymbols();
			if (!fullUpdate) ExtendRangeWithSymbols();
			
			hString = new NSMutableAttributedString(new NSString(this.Text.Substring((int)hrange.location, (int)(hrange.length)))); 

			// apply highlighting
			hString.AddAttributeValueRange("NSFont", NSFont.UserFixedPitchFontOfSize(13), ConvertRangeToHRange(hrange));
			NSMutableParagraphStyle pstyle = new NSMutableParagraphStyle();
			pstyle.SetMinimumLineHeight(15.0f);
			pstyle.SetMaximumLineHeight(15.0f);
			//hString.AddAttributeValueRange("NSParagraphStyle", pstyle, ConvertRangeToHRange(hrange));

			this.HighlightSymbols(SymbolType.Reserved, NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.2f, 0.2f, 0.6f, 1.0f));						
			this.HighlightSymbols(SymbolType.ReservedType, NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.5f, 0.5f, 1.0f, 1.0f));						
			this.HighlightSymbols(SymbolType.String, NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.6f, 0.5f, 0.2f, 1.0f));						
			this.HighlightSymbols(SymbolType.Comment, NSColor.ColorWithCalibratedRedGreenBlueAlpha(0.2f, 0.6f, 0.2f, 1.0f));

			this.viewText.TextStorage.ReplaceCharactersInRangeWithAttributedString(hrange, hString);
			
			// restore old selection
			if ((this.Selection.location >= 0) && (this.Selection.location + this.Selection.length < this.Text.Length))
				this.viewText.SelectedRange = this.Selection;
								
			hlBusy = false;
		}
		
		private NSRange ConvertRangeToHRange(NSRange normalRange)
		{
			return new NSRange(normalRange.location - hrange.location, normalRange.length);
		}
		
		private void ExtendRangeWithSymbols()
		{
			if (this.Text.Length < 1) return;
			// extend to complete lines
			NSRange lineStart = LineRangeAtIndex(hrange.location);
			NSRange lineEnd = LineRangeAtIndex(hrange.location + hrange.length);
			hrange = new NSRange(lineStart.location, (lineEnd.location + lineEnd.length) - lineStart.location);

			// extend by affected symbols
			this.symbolList.Sort();
			
			for (int i=0; i<this.symbolList.Count; i++)
			{
				NSRange range = this.symbolList[i].Range;
				if (range.location > hrange.location+hrange.length) break;
				if (range.location+range.length < hrange.location) continue;
				if ((range.location >= hrange.location) && (range.location + range.length <= hrange.location + hrange.length)) continue;
				
				if (range.location < hrange.location) 
				{
					hrange.length += (hrange.location-range.location);
					hrange.location = range.location;
				}
				
				if (range.location + range.length > hrange.location + hrange.length)
					hrange.length = range.location + range.length - hrange.location;
			}
			
			// fix range if necessary
			if (hrange.location < 0) hrange.location = 0;
			if (hrange.location + hrange.length >= this.Text.Length) hrange.length = (uint)(this.Text.Length - hrange.location);
			
		}
		
		private void ColorizeRange(NSColor color, NSRange range)
		{
			if (range.location > hrange.location+hrange.length) return;
			if (range.location+range.length < hrange.location) return;
			
			if (range.location < 0) range.location = 0;
			if (range.location + range.length >= this.Text.Length) range.length = (uint)(this.Text.Length - range.location);
			
			hString.AddAttributeValueRange("NSColor", color, ConvertRangeToHRange(range));
			//this.viewText.TextStorage.AddAttributeValueRange("NSColor", color, range);
		}
		
		private void HighlightSymbols(SymbolType type, NSColor color)
		{
			foreach (Symbol s in symbolList)
			{
				if (s.Type != type) continue;
				this.ColorizeRange(color, s.Range);
			}
		}
		
	}

}