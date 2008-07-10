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

namespace MonoMate.Editor 
{
	partial class SourceEditor 
	{
		public void ChangeCurrentIndent(int change)
		{
			hlBusy = true;
			NSRange oldSelection = this.Selection;
			int line1 = this.LineAtIndex(Selection.location);
			int line2 = this.LineAtIndex(Selection.location + Selection.length - 1);
						
			int i = line1;
			while(true)
			{
				int indent = this.CurrentIndentOfLine(i);
				if (indent + change < 0)
				{
					i++;
					if (i > line2) break;
					continue;
				}
				this.SetIndentOfLine(i, indent + change);
					
				if (oldSelection.length == 0)
				{
					if (change + oldSelection.location < 0)
						oldSelection.location = 0;
					else
						oldSelection.location += (uint)change;
					break;
				}
				if (change + oldSelection.length < 0)
					oldSelection.length = 0;
				else 
					oldSelection.length += (uint)change;
				i++;
				if (i > line2) break;
			}				
			if ((oldSelection.location >= 0) && (oldSelection.location + oldSelection.length < this.Text.Length))
				this.viewText.SelectedRange = oldSelection;
			hlBusy = false;
		}
		
		public void SetIndentOfLine(int line, int count)
		{
			int index = IndexOfLine(line);
			
			// calculate replacement range
			int clen = this.CurrentIndentOfLine(line);
			NSRange replRange = new NSRange((uint)index, (uint)clen);
			
			// create replacement string
			string replString = "";
			for (int i=0; i<count; i++) replString += '\t';
			
			// replace and restore Selection
			this.viewText.TextStorage.ReplaceCharactersInRangeWithString(replRange, new NSString(replString));
		}
	
		public int CurrentIndentOfLine(int line)
		{
			int index = IndexOfLine(line);
			int pos = index;
			while ((pos < this.Text.Length) && (this.Text[pos] == '\t')) pos++;
			return pos - index; 
		}
		
		public int CorrectIndentOfLine(int line)
		{
			// start of line
			int index = IndexOfLine(line);
			
			// end of line
			NSRange lineRange = LineRangeAtIndex((uint)index);
			int index2 = (int)(lineRange.location + lineRange.length) - 1;
			
			
			// read blocks until start of line
			int pos = 0;
			int blocksOpen = 0;
			int lastBlockOpen = -1;
			int lastBlockClose = -1;
			Stack<int> blocksOpened = new Stack<int>();
			
			while (pos < index)
			{
				if (this.IsIndexInComment(pos))
				{
					pos++;
					continue;
				}
				char cur = this.Text[pos];
				
				if (cur == '{') 
				{
					blocksOpen++;
					lastBlockOpen = pos;
					blocksOpened.Push(pos);
				}
				if (cur == '}') 
				{
					blocksOpen--;
					lastBlockClose = pos;
					if (blocksOpened.Count > 0) blocksOpened.Pop();
				}
				
				pos++;
			}
			
			int contextIndent = 0;
			int contextStart = 0;
			if (lastBlockClose > lastBlockOpen)
			{
				if (blocksOpened.Count > 0) contextIndent = this.CurrentIndentOfLine(this.LineAtIndex((uint)blocksOpened.Peek())) + 1;
				contextStart = lastBlockClose;
			}
			else if (lastBlockOpen != -1)
			{
				contextIndent = this.CurrentIndentOfLine(this.LineAtIndex((uint)lastBlockOpen)) + 1;
				contextStart = lastBlockOpen;
			}
			
			// do extra parsing from the last { or } to start of line
			pos = contextStart + 1;
			int nlInc = 0;
			char lastc = ' ';
			int indent = contextIndent;
			while (pos < index)
			{
				if (this.IsIndexInComment(pos))
				{
					pos++;
					continue;
				}
				char cur = this.Text[pos];
				
				// is whitespace ?
				if ((cur == ' ') || (cur == '\t'))
				{
					pos++;
					continue;
				}
				
				// is new line ?
				if (cur == '\n') 
				{
					if (lastc != ' ')
					{
						if ((lastc != ',') && (lastc != ']') && (lastc != ';'))
						{
							nlInc ++;
							indent++;
						}
						
						lastc = ' ';
					}
					pos++;
					continue;
				}
				
				// is ;
				if (cur == ';')
				{
					if (nlInc > 0)
					{
						indent -= nlInc;
						nlInc = 0;
					}
				}
				
				lastc = cur;
				pos++;
			}
			
			// get the first char of the current line
			pos = index;
			char firstChar = ' ';
			while (pos < index2)
			{
				if (this.IsIndexInComment(pos))
				{
					pos++;
					continue;
				}
				char cur = this.Text[pos];
				if (cur == '\n') break;
				
				// is whitespace ?
				if ((cur == ' ') || (cur == '\t'))
				{
					pos++;
					continue;
				}
				
				firstChar = cur;
				break;
			}
			
			if (firstChar == '{') indent --;
			if (firstChar == '}') 
			{
				if (blocksOpened.Count > 0)
					indent = this.CurrentIndentOfLine(this.LineAtIndex((uint)blocksOpened.Peek())) + 0;
				else
					indent = 0;
			}
						
			return indent;
		}
		
		private bool IsIndexInComment(int index)
		{
			foreach (Symbol s in symbolList)
				if ((s.Type == SymbolType.Comment) && (index >= s.Range.location) && (index < s.Range.location + s.Range.length)) return true;
			return false;
		}
		
	}
}