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

namespace MonoMate
{
	[ObjectiveCClass]
	class BuildMessage : NSObject, IComparable
	{
		public BMCategory Category;
		public string FullPath;
		public int Line;
		public int Col;
		public string Message;
		public string Code;

		public BuildMessage() {}
		public BuildMessage(IntPtr np) : base(np) {}
		
		public static BuildMessage Create(string str)
		{
			try
			{
				int pos = str.IndexOf("[csc] /");
				int pos2 = str.IndexOf(":");
				if ((pos == -1) || (pos2 == -1)) return null;
				
				if (str.IndexOf(": (") != -1) return null;
				
				pos += 6;
				
				int pos3 = str.LastIndexOf("(", pos2);
				int posComma = str.IndexOf(",", pos3);
				int pos4 = str.IndexOf(":", pos2+1);
				int pos5 = str.LastIndexOf(" ", pos4);
				
				BuildMessage bm = new BuildMessage();

				bm.FullPath = str.Substring(pos, pos3-pos);
				bm.Line = Int32.Parse(str.Substring(pos3+1, posComma-pos3-1));
				bm.Col = Int32.Parse(str.Substring(posComma+1, pos2-posComma-2));
				
				bm.Code = str.Substring(pos5+1, pos4-pos5-1);
				
				string Category = str.Substring(pos2+2, pos5-pos2 - 2);
				if (Category == "error") bm.Category = BMCategory.Error;
				if (Category == "warning") bm.Category = BMCategory.Warning;
				
				bm.Message = str.Substring(pos4+2, str.Length - pos4 -2);
				
				return bm;
			}
			catch (Exception e)
			{
				Console.WriteLine("PARSING ERROR FOR: " + str + " \n\nMessage:" + e.Message + "\nStack:\n" + e.StackTrace);
				return null;
			}
		}

		public int CompareTo(object obj)
		{
			BuildMessage msg = (BuildMessage) obj;
			if (this.Category == msg.Category) 
			{
				if (this.Category == BMCategory.Warning)
				{
					bool imp1 = Builder.IsImportantWarning(this);
					bool imp2 = Builder.IsImportantWarning(msg);
					if (imp1 != imp2)
					{
						if (imp1) return -1;
						return +1;
					}
				}
				
				if (this.FullPath != msg.FullPath)
					return this.FullPath.CompareTo(msg.FullPath);
				
				if (this.Line != msg.Line)
					return this.Line.CompareTo(msg.Line);
			}
			if (this.Category == BMCategory.Error) return -1000;
			return +1000;
		}
		
		[ObjectiveCMessage("copyWithZone:")]
		public Id CopyWithZone(Id zone)
		{
			return this;
		}
	}
	
	public enum BMCategory
	{
		Error,
		Warning
	}
}