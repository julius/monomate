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
using System.IO;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate
{
	class FindProjectQuery 
	{
		private List<FindProjectResult> resultList = new List<FindProjectResult>();
		
		public FindProjectQuery(OFile root, string term)
		{
			if (term.Trim() == "") return;
			
			this.Search(root, term);
		}
		
		private void Search(OFile file, string term)
		{
			if (file.IsCodeFile)
			{
				TextReader txt = new StreamReader(file.FullPath, System.Text.Encoding.UTF8);
				string text = txt.ReadToEnd();
				txt.Close();
				
				int pos = -1;
				while ((pos = text.IndexOf(term, pos+1)) != -1)
				{
					try
					{
						FindProjectResult fpr = new FindProjectResult();
						fpr.File = file;
						fpr.Range = new NSRange((uint)pos, (uint)term.Length);
						
						fpr.TargetLineMain = text.Substring(pos, term.Length);
						
						int pos1 = text.LastIndexOf("\n", pos);
						fpr.TargetLineBefore = text.Substring(pos1+1, pos-pos1-1);
						
						int pos2 = text.IndexOf("\n", pos);
						int epos = (pos2 == -1) ? text.Length-1 : pos2;
						int eapos = pos+term.Length;
						fpr.TargetLineAfter = text.Substring(eapos, epos-eapos);
						
						if (pos1 == -1)
						{
							fpr.BeforeLine = "";
						}
						else
						{
							int pos3 = text.LastIndexOf("\n", pos1-1);
							fpr.BeforeLine = text.Substring(pos3+1, pos1-pos3-1);
						}
						
						if (pos2 == -1)
						{
							fpr.AfterLine = "";
						}
						else
						{
							int pos4 = text.IndexOf("\n", epos+1);
							int epos2 = (pos4 == -1) ? text.Length-1 : pos4;
							fpr.AfterLine = text.Substring(epos+1, epos2-epos-1);
						}
						
						resultList.Add(fpr);
					}
					catch (Exception e)
					{
						Console.WriteLine("Problem while parsing '"+file.Title+"': "+e.Message+" \nStack:\n" + e.StackTrace);
					}
				}
			}
			
			foreach (OFile child in file.Children)
				this.Search(child, term);
		}
		
		public int Count
		{
			get
			{
				return resultList.Count;
			}
		}
		
		public FindProjectResult ResultAtRow(int row)
		{
			return (FindProjectResult)resultList[row].Retain();
		}
	}
}

