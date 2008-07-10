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
	abstract class ClassCreator
	{
		public string Error = "Unknown Problem";
		public event EventHandler Failed;
		public event EventHandler Created;
		
		public abstract string Identifier { get; }
		public abstract NSImage Icon { get; }
		public abstract string SuperClassStd { get; }
		public abstract bool SuperClassChoosable { get; }
		public abstract bool SupportsNib { get; }
		
		public abstract string TemplatePath { get; }
		public abstract string[] CSFiles { get; }
		
		public string FileToOpen = null;
		public NSString ResourcePath;
		
		public ClassCreator(NSString resourcePath)
		{
			this.ResourcePath = (NSString)resourcePath.Retain();
		}
		
		public NSString TemplateRealPath
		{
			get
			{
				NSString templPath = NSBundle.MainBundle.ResourcePath.StringByAppendingPathComponent(new NSString("Templates/Classes"));
				return templPath.StringByAppendingPathComponent(new NSString(this.TemplatePath));
			}
		}

		public void Create(string path, string name, string ns, string nib, string super)
		{
			// check if informations are correct
			if (!Check(path, name, ns, nib, super)) return;
			
			// Correct the superclass if necessary
			if (this.SuperClassChoosable)
			{
				if (super.Trim() == "") super = this.SuperClassStd;
			}
			else super = this.SuperClassStd;
			
			
			if (!CopyCSFiles(path, name, ns, nib, super)) return;
			
			if (this.SupportsNib)
				if (!CopyNib(path, name, nib)) return;
			
			if (this.Created != null) this.Created(this, null);
		}
		
		protected bool CopyCSFiles(string path, string name, string ns, string nib, string super)
		{
			FileToOpen = null;
			foreach (string csfile in this.CSFiles)
			{
				NSString srcPath = this.TemplateRealPath.StringByAppendingPathComponent(new NSString(csfile));
				NSString destPath = new NSString(path).StringByAppendingPathComponent(new NSString(csfile.Replace("Class", name)));

				bool cpResult = NSFileManager.DefaultManager.CopyPathToPathHandler(srcPath, destPath, null);
				if (!cpResult)
				{
					Fail("Creation failed, during copy process.");
					return false;
				}
				this.Configure(destPath, name, ns, nib, super);
				if (FileToOpen == null) FileToOpen = destPath;
			}
			
			return true;
		}
		
		protected bool CopyNib(string path, string name, string nib)
		{
			NSString srcPath = this.TemplateRealPath.StringByAppendingPathComponent(new NSString("UI.xib"));
			NSString destPath = this.ResourcePath.StringByAppendingPathComponent(new NSString(nib + ".xib"));

			bool cpResult = NSFileManager.DefaultManager.CopyPathToPathHandler(srcPath, destPath, null);
			if (!cpResult)
			{
				Fail("Creation failed, during copy process.");
				return false;
			}
			this.ConfigureNib(destPath, name, nib);
			return true;
		}
		
		protected void Configure(string path, string name, string ns, string nib, string super)
		{
			Console.WriteLine("Configuring "+path+" ...");
			System.Text.Encoding enc = System.Text.Encoding.UTF8;
			//if (path.IndexOf("build.sh") != -1 ) enc = System.Text.Encoding.Default;
			
			// Load the file
			TextReader txt = new StreamReader(path, enc);
			string text = txt.ReadToEnd();
			txt.Close();
			
			// Configure the file
			text = text.Replace("{TEMPLATE.VAR:NAME}", name);
			text = text.Replace("{TEMPLATE.VAR:NAMESPACE}", ns);
			text = text.Replace("{TEMPLATE.VAR:NIB}", nib);
			
			if (super == "object") super = "";
			if (super != "")
				text = text.Replace("{TEMPLATE.VAR:SUPERCLASS}", ": " + super);
			else
				text = text.Replace("{TEMPLATE.VAR:SUPERCLASS}", "");
			
			// Save the configured file
			TextWriter txt2 = new StreamWriter(path, false, enc);
			txt2.Write(text);
			txt2.Close();
		}
		
		protected void ConfigureNib(string path, string name, string nib)
		{
			Console.WriteLine("Configuring Nib "+path+" ...");
			System.Text.Encoding enc = System.Text.Encoding.UTF8;
			//if (path.IndexOf("build.sh") != -1 ) enc = System.Text.Encoding.Default;
			
			// Load the file
			TextReader txt = new StreamReader(path, enc);
			string text = txt.ReadToEnd();
			txt.Close();
			
			// Configure the file
			text = text.Replace("TEMPLATE_VAR_NAME", name);
			text = text.Replace("TEMPLATE_VAR_NIB}", nib);
			
			// Save the configured file
			TextWriter txt2 = new StreamWriter(path, false, enc);
			txt2.Write(text);
			txt2.Close();
		}
		
		
		protected bool Check(string path, string name, string ns, string nib, string super)
		{
			// Check the name
			if (name.Trim() == "")
			{
				Fail("Specify the Name of your Class.");
				return false;
			}
			
			// check nib
			if (this.SupportsNib)
			{
				if (nib.Trim() == "") 
				{
					Fail("Specify a Name for your Nib-File.");
					return false;
				}
			}
			
			// check path
			if (!NSFileManager.DefaultManager.FileExistsAtPath(new NSString(path)))
			{
				Fail("Destination '"+path+"' does not exist.");
				return false;
			}
			
			// check namespace
			if (ns.Trim() == "")
			{
				Fail("Specify the Namespace for your Class.");
				return false;
			}
			
			// check filebase and destinations (CSFiles)
			foreach (string csfile in this.CSFiles)
			{
				NSString srcPath = this.TemplateRealPath.StringByAppendingPathComponent(new NSString(csfile));
				if (!NSFileManager.DefaultManager.FileExistsAtPath(srcPath))
				{
					Fail("Template broken !!!");
					return false;
				}
				
				NSString destPath = new NSString(path).StringByAppendingPathComponent(new NSString(csfile.Replace("Class", name)));
				if (NSFileManager.DefaultManager.FileExistsAtPath(destPath))
				{
					Fail("Class File '"+destPath+"' already exists !");
					return false;
				}
			}

			// check filebase and destinations (Nib)
			if (this.SupportsNib)
			{
				Console.WriteLine("R: " + this.ResourcePath);
				NSString destPath = this.ResourcePath.StringByAppendingPathComponent(new NSString(nib + ".xib"));
				if (NSFileManager.DefaultManager.FileExistsAtPath(destPath))
				{
					Fail("Nib File '"+destPath+"' already exists !");
					return false;
				}
			}			
			
			return true;
		}
		
		protected void Fail(string msg)
		{
			this.Error = msg;
			if (this.Failed != null) this.Failed(this, new EventArgs());
		}
	}
}