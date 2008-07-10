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
using System.Xml;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate
{
	class CCNormal : ClassCreator
	{
		private string identifier = "DAMAGED TEMPLATE";
		private NSImage icon = null;
		private string superClassStd = "--";
		private string templatePath = "--";
		private bool superClassChoosable = false;
		private bool supportsNib = false;
		private string[] csFiles;
		
		public CCNormal(NSString path, NSString resourcePath) : base(resourcePath)
		{
			this.templatePath = path.StringByDeletingLastPathComponent.LastPathComponent;
			
			List<string> fileList = new List<string>();
			XmlTextReader xml = null;
			
			try {
				xml = new XmlTextReader(path);
				
				while (xml.Read())
				{
					if (xml.NodeType != XmlNodeType.Element) continue;
					
					// Load Template
					if (xml.Name == "Template")
					{
						this.superClassStd = xml.GetAttribute("superclass");
						this.superClassChoosable = (xml.GetAttribute("superclasschoosable") == "true") ? true : false;
						this.supportsNib = (xml.GetAttribute("supportsnib") == "true") ? true : false;
						string iconPath = xml.GetAttribute("icon");
						this.icon = new NSImage(this.TemplateRealPath.StringByAppendingPathComponent(new NSString(iconPath)));
						this.identifier = xml.GetAttribute("identifier");
					}
					
					// Load Tasks
					if (xml.Name == "CSFile")
					{
						fileList.Add(xml.GetAttribute("path"));
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to load Template: " + path + "\nProblem: " + e.Message);
			}
			finally
			{
				if (xml != null) xml.Close();
			}
			
			this.csFiles = fileList.ToArray();
		}
		
		
		public override string Identifier
		{
			get { return identifier; }
		}
		
		public override NSImage Icon
		{
			get { return icon; }
		}
		
		public override string SuperClassStd
		{
			get { return superClassStd; }
		}
		
		public override bool SuperClassChoosable
		{
			get { return superClassChoosable; }
		}
		
		public override bool SupportsNib
		{
			get { return supportsNib; }
		}
		
		public override string TemplatePath
		{
			get { return templatePath; } 
		}
		
		public override string[] CSFiles
		{
			get { return csFiles; }
		}
	}
}


