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
	class Builder
	{
		public Terminal terminal;
		public string Path;
		public event EventHandler BuildFinished;
		private NSTask CurrentBuild = null;
		
		public List<BuildMessage> bmList = new List<BuildMessage>();
		
		public Builder(Terminal t, string path)
		{
			this.terminal = t;
			this.Path = path;
		}
		
		public void Build(string script, string arg)
		{
			if (CurrentBuild != null) 
			{
				CurrentBuild.Terminate();
				if (this.BuildFinished != null) this.BuildFinished(this, new EventArgs());
				return;
			}
			
			string buildPath = (new NSString(this.Path)).StringByAppendingPathComponent(new NSString(script));
			Console.WriteLine("building "+ buildPath);
			
			bmList.Clear();
			
			CurrentBuild = new NSTask();
			CurrentBuild.LaunchPath = new NSString("/bin/sh");
			CurrentBuild.CurrentDirectoryPath = new NSString(this.Path);
			CurrentBuild.Arguments = NSArray.ArrayWithObjects(new NSString(buildPath), new NSString(arg), null);
			
			NSPipe pipe = new NSPipe();
			CurrentBuild.StandardError = pipe;
			CurrentBuild.StandardOutput = pipe;
			
			NSThread.DetachNewThreadSelectorToTargetWithObject(TerminalThread, pipe.FileHandleForReading);

			CurrentBuild.Launch();
		}

		public void TerminalThread(Id argument)
		{
			NSAutoreleasePool pool = new NSAutoreleasePool();
			
			NSFileHandle handle = argument.CastTo<NSFileHandle>();
			NSData data;
			
			while (true)
			{
				data = handle.AvailableData;
				if (data.Length <= 0) break;
				
				NSString str = new NSString(data, NSStringEncoding.NSASCIIStringEncoding);
				this.terminal.AddText(str);
			}
			
			this.terminal.AddText("\n\n----------------------------------\nFinished !");
			
			ReceiveBuildMessages();
			if (this.BuildFinished != null) this.BuildFinished(this, new EventArgs());
			CurrentBuild = null;
			
			pool.Release();
		}
		
		private void ReceiveBuildMessages()
		{
			string text = this.terminal.Text;
			
			int pos = 0;
			string current = "";
			while (pos < text.Length)
			{
				if (text[pos] == '\n')
				{
					BuildMessage bm = BuildMessage.Create(current);
					if (bm != null) bmList.Add(bm);
					current = "";
				}
				else
				{
					current += text[pos];
				}
				pos++;
			}
			
			//this.CleanList();
			bmList.Sort();
		}
		
		/*
		private void CleanList()
		{
			bool HasImportantMessages = false;
			foreach (BuildMessage msg in bmList)
			{
				if (msg.Category == BMCategory.Error)
				{
					HasImportantMessages = true;
					break;
				}
				if (msg.Category == BMCategory.Warning)
				{
					if (IsImportantWarning(msg)) 
					{
						HasImportantMessages = true;
						break;
					}
				}
			}
			
			if (HasImportantMessages)
			{
				Console.WriteLine("Has important");
				List<BuildMessage> tmpList = new List<BuildMessage>();
				foreach (BuildMessage msg in bmList)
				{
					if (msg.Category == BMCategory.Error)
						tmpList.Add(msg);
	
					if (msg.Category == BMCategory.Warning)
						if (IsImportantWarning(msg)) tmpList.Add(msg);
				}
				this.bmList = tmpList;
			}
		}/**/
		
		public static bool IsImportantWarning(BuildMessage msg)
		{
			string[] uCodes = { "CS0114", "CS0108" };
			foreach (string uc in uCodes) 
				if (msg.Code == uc) return false;
			return true;
		}
		
	}
}