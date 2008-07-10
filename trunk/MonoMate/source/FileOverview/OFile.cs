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
	class OFile : NSObject, IComparable
	{
		public string FullPath;
		public string ParentPath;
		public NSImage Icon;
		
		public bool IsDirectory;
		public bool IsCodeFile;
		
		public event EventHandler Rescanned;
		
		public List<OFile> Children = new List<OFile>();
		public OFile Parent = null;

		public string Title
		{
			get 
			{
				return (new NSString(this.FullPath)).LastPathComponent;
			}
		}
		
		public OFile() {}
		public OFile(IntPtr np) : base(np) {}
		
		public OFile(string path)
		{
			this.FullPath = path;
			this.ParentPath = (new NSString(path)).StringByDeletingLastPathComponent;
			
			this.Icon = (NSImage)NSWorkspace.SharedWorkspace.IconForFile(new NSString(path)).Retain();
			
			NSFileManager.DefaultManager.FileExistsAtPathIsDirectory(new NSString(path), ref this.IsDirectory);
			if (this.IsDirectory && ((new NSString(this.FullPath)).PathExtension != "")) this.IsDirectory = false;
			
			this.IsCodeFile = ((new NSString(path)).PathExtension == "cs");
			
			this.Rescan();
		}
		
		public void Rescan()
		{
			if (!this.IsDirectory) return;
			this.Children.Clear();
			
			NSString npath = new NSString(this.FullPath);
			NSArray files = NSFileManager.DefaultManager.DirectoryContentsAtPath(npath);
			if (files == null) return;
			
			NSString file;
			
			
			for (int i=0; i<files.Count; i++)
			{
				file = ObjectiveCRuntime.SendMessage<NSString>(files, "objectAtIndex:", (uint)i);
				if (((string)file.LastPathComponent)[0] == '.') continue;
				
				OFile child = new OFile(npath.StringByAppendingPathComponent(file));
				child.Rescanned += new EventHandler(ChildRescanned);
				child.Parent = this;
				this.Children.Add(child);
			}
			
			this.Children.Sort();
			if (this.Rescanned != null) this.Rescanned(this, null);
		}
		
		public void ChildRescanned(object sender, EventArgs args)
		{
			if (this.Rescanned != null) this.Rescanned(sender, null);
		}
		
		public List<string> CSFiles
		{
			get
			{
				List<string> list = new List<string>();
				for (int i=0; i < this.Children.Count; i++)
				{
					if (this.Children[i].IsCodeFile) list.Add(this.Children[i].FullPath);
					if (this.Children[i].Children.Count > 0) list.AddRange(this.Children[i].CSFiles);
				}
				return list;
			}
		}
		
		public int CompareTo(object obj)
		{
			OFile o = (OFile) obj;
			return this.Title.CompareTo(o.Title);
		}
		
		[ObjectiveCMessage("copyWithZone:")]
		public Id CopyWithZone(Id zone)
		{
			return this;
		}
	}
}

