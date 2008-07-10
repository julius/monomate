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

namespace MonoMate 
{
	class FilePickQuery 
	{
		private List<PFile> ofileList; 
		private List<PFile> fileList; 
		
		public FilePickQuery(List<PFile> ofileList, string query)
		{
			this.ofileList = ofileList;
			this.fileList = new List<PFile>();
			
			foreach (PFile f in this.ofileList)
			{
				if (this.FileMatchesQuery(query, f))
					this.fileList.Add(f);
			}
			this.fileList.Sort();
		}
		
		private bool FileMatchesQuery(string query, PFile file)
		{
			query = query.ToLower();
			string title = file.Title.ToLower();
			
			int qpos = 0;
			int tpos = 0;
			while (true)
			{
				if (qpos >= query.Length) break;
				if (tpos >= title.Length) return false;
				
				char cur = query[qpos++];
				tpos = title.IndexOf(cur, tpos);
				if (tpos < 0) return false;
			}			
			return true;
		}
		
		public int Count
		{
			get
			{
				return fileList.Count;
			}
		}
		
		public PFile FileAtRow(int row)
		{
			return (PFile)fileList[this.Count - row - 1].Retain();
		}
	}

}