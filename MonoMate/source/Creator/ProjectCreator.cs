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
using System.IO;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate
{
	class ProjectCreator
	{
		private string Name;
		private string Path;
		
		public string Error = "Unknown Problem";
		public event EventHandler Failed;
		
		public event EventHandler Created;
		
		public ProjectCreator(string path, string name)
		{
			this.Name = name;
			this.Path = (string)(new NSString(path)).StringByExpandingTildeInPath;;
		}
		
		public void Create()
		{
			// Check the name
			if (this.Name.Trim() == "")
			{
				Fail("Specify the Name of your Project.");
				return;
			}
			
			// Check if ParentPath exists
			NSString destParentPath = (new NSString(this.Path)).StringByDeletingLastPathComponent;
			if (!NSFileManager.DefaultManager.FileExistsAtPath(destParentPath))
			{
				Fail("Folder '"+destParentPath+"' does not exist.");
				return;
			}
			
			// Check full path
			NSString fullPath = new NSString(this.Path);
			if (NSFileManager.DefaultManager.FileExistsAtPath(fullPath))
			{
				Fail("Folder '"+fullPath+"' already exists.");
				return;
			}
				
			// Copy the ProjectTemplate
			NSString templPath = NSBundle.MainBundle.ResourcePath.StringByAppendingPathComponent(new NSString("Templates"));
			templPath = templPath.StringByAppendingPathComponent(new NSString("MonobjcProject"));
			if (!NSFileManager.DefaultManager.FileExistsAtPath(templPath))
			{
				Fail("MonoMate filebase is broken !!!");
				return;
			}
			
			bool cpResult = NSFileManager.DefaultManager.CopyPathToPathHandler(templPath, fullPath, null);
			if (!cpResult)
			{
				Fail("Creation failed, during copy process.");
				return;
			}
			
			
			// Configure Files
			string[] projFiles = { "build.sh", 
				"essentials/Info.plist", 
				"essentials/nomono/NoMonoMessage.html", 
				"source/Program.cs", 
				"source/Controller.cs",
				"source/Controller.objc.cs",
				"resources/MainMenu.xib" 
			};
			
			foreach (string cfile in projFiles)
			{
				string cpath = fullPath.StringByAppendingPathComponent(new NSString(cfile));
				this.Configure(cpath);
			}
			
			
			if (this.Created != null) this.Created(this, new EventArgs());
		}
		
		private void Configure(string path)
		{
			Console.WriteLine("Configuring "+path+" ...");
			System.Text.Encoding enc = System.Text.Encoding.UTF8;
			if (path.IndexOf("build.sh") != -1 ) enc = System.Text.Encoding.Default;
			
			// Load the file
			TextReader txt = new StreamReader(path, enc);
			string text = txt.ReadToEnd();
			txt.Close();
			
			// Configure the file
			text = text.Replace("{TEMPLATE.VAR:NAME}", this.Name);
			
			// Save the configured file
			TextWriter txt2 = new StreamWriter(path, false, enc);
			txt2.Write(text);
			txt2.Close();
		}
		
		private void Fail(string msg)
		{
			this.Error = msg;
			if (this.Failed != null) this.Failed(this, new EventArgs());
		}
	}
}