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
		public event EventHandler Changed;
	
		public NSRange Selection;
		private bool _hasChanged = false;
		public bool HasChanged
		{
			set
			{
				_hasChanged = value;
				if (this.Changed != null) this.Changed(this, new EventArgs());
			}
			get { return _hasChanged; }
		}
		private List<int> lineList = new List<int>();

		private string internText;
		public string Text { get { return internText; } }
		private void RecacheText()
		{
			NSMutableString str = this.viewText.TextStorage.MutableString;
			internText = str;
			
			// get line breaks
			int pos = 0;
			lineList.Clear();
			while (pos < internText.Length)
				if (internText[pos++] == '\n') lineList.Add(pos);
		}
		
		public void LoadCompleteText(string text)
		{
			//this.viewText.TextStorage.ReplaceCharactersInRangeWithString(new NSRange(0,this.viewText.TextStorage.Length),new NSString(value));
			this.viewText.String = new NSString(text);
			string txt = this.Text;
			this.viewText.String = new NSString("");
			this.viewText.TextStorage.InsertAttributedStringAtIndex(new NSAttributedString(txt), 0);
			
			this.viewText.SelectedRange = new NSRange(0,0);
			HasChanged = false;
		}
		
		public int IndexOfLine(int line)
		{
			line--;
			if ((this.lineList.Count < 1) || (line < 0)) return 0;
			if (line >= this.lineList.Count) return 0;
			return this.lineList[line];
		}
		
		[ObjectiveCMessage("lineAtIndex:")]
		public int LineAtIndex(uint index)
		{
			for (int i=0; i<this.lineList.Count; i++)
				if (index < this.lineList[i]) return i;
			return this.lineList.Count;
		}
		
		public int ColumnAtIndex(uint index)
		{
			int line = this.LineAtIndex(index) - 1;
			int lineIndex = ((this.lineList.Count < 1) || (line < 0))? 0 : this.lineList[line]; 
			return (int) (index - lineIndex);
		}
		
		public NSRange LineRangeAtIndex(uint index)
		{
			int line = this.LineAtIndex(index) - 1;
			int loc = ((this.lineList.Count < 1) || (line < 0))? 0 : this.lineList[line];
			int loc2 = ((this.lineList.Count < 1) || (line+1 >= this.lineList.Count)) ? this.Text.Length : this.lineList[line+1];
			return new NSRange((uint)loc, (uint)(loc2-loc));
		}
		
		public void PresentRange(NSRange range)
		{
			this.viewText.SelectedRange = range;
			this.viewText.CenterSelectionInVisibleArea(this);
		}
		
		public void UpdateStatus()
		{
			/*
			int line = this.LineAtIndex(Selection.location) + 1;
			int col = this.ColumnAtIndex(Selection.location) + 1;
			NSRange lineRange = this.LineRangeAtIndex(Selection.location);
			int indent = this.CurrentIndentOfLine(line - 1);
			int indent2 = this.CorrectIndentOfLine(line - 1);
			this.labelStatus.StringValue = new NSString("Pos:"+line+":"+col+"  Indent:"+indent+":"+indent2+"  LineRange:"+lineRange.location+":"+lineRange.length+"  SelRange:"+Selection.location+":"+Selection.length);
			/**/
		}
		
		[ObjectiveCMessage("textView:willChangeSelectionFromCharacterRange:toCharacterRange:")]
		public NSRange SelectionChange(NSTextView tview, NSRange oldRange, NSRange newRange)
		{
			this.Selection = newRange;
			this.UpdateStatus();
			
			return newRange;
		}
		
		[ObjectiveCMessage("textView:shouldChangeTextInRange:replacementString:")]
		public bool ShouldChangeText(NSTextView tview, NSRange range, NSString str)
		{
			string nstr = str;
			
			if (nstr == "\n")
			{
				this.viewText.TextStorage.ReplaceCharactersInRangeWithString(this.Selection, new NSString("\n"));
				this.Selection = this.viewText.SelectedRange;
				
				int line = this.LineAtIndex(Selection.location);
				int correct = this.CorrectIndentOfLine(line+0);
				
				this.SetIndentOfLine(line+0, correct);
				this.viewText.ScrollRangeToVisible(this.Selection);
				return false;
			}
			
			if ((nstr == "{") || (nstr == "}"))
			{
				this.viewText.TextStorage.ReplaceCharactersInRangeWithString(this.Selection, str);
				this.RecacheText();

				int line = this.LineAtIndex(Selection.location);
				int correct = this.CorrectIndentOfLine(line);
				
				this.SetIndentOfLine(line, correct);
				return false;
			}

			if (nstr == "\t")
			{
				this.ChangeCurrentIndent(+1);
				return false;
			}

			//Console.WriteLine("str[" + nstr + "]");
			return true;
		}
		
		[ObjectiveCMessage("additionalKeyCommand:")]
		public void AdditionalKeyCommand(KeyCommand cmd)
		{
			if (cmd == KeyCommand.UnTab)
			{
				this.ChangeCurrentIndent(-1);
			}
		}
		
		[ObjectiveCMessage("textStorageDidProcessEditing:")]
		public void ProcessEditing(NSNotification notif)
		{
			HasChanged = true;
			this.RecacheText();
			this.UpdateHighlighting(false);
			//this.CorrectIndentation(this.viewText.TextStorage.EditedRange);
		}
		
	}

}