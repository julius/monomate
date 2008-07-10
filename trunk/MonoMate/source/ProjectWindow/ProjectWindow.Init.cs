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
	partial class ProjectWindow
	{
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			// Init TabView
			tabs = new TabView(this.Window.ContentView.Frame);
			this.Window.ContentView.AddSubview(tabs);
			
			// Init FileOverview
			filer = FileOverview.Create(this.Path);
			filer.Picked += new EventHandler(FilePicked);
			this.Window.ContentView.AddSubview(filer.View);
			this.filer.Rescan();
			
			// Init Terminal
			terminal = Terminal.CreateTerminal();
			this.Window.ContentView.AddSubview(terminal.View);
			
			// Init Find in Project
			findProject = FindProject.Create();
			this.Window.ContentView.AddSubview(findProject.View);
			
			// Init Class Creator
			ccreator = ClassCreatorController.Create();
			this.Window.ContentView.AddSubview(ccreator.View);
			ccreator.ResourcePath = (new NSString(this.Path)).StringByAppendingPathComponent("resources");
			ccreator.Canceled += new EventHandler(ClassCreationCanceled);
			ccreator.Created += new EventHandler(ClassCreationSuccess);
			
			// Init Builder
			builder = new Builder(terminal, this.Path);
			builder.BuildFinished += new EventHandler(BuildFinished);
			
			// Init FilePicker
			picker = FilePicker.Picker();
			picker.Picked += new EventHandler(FilePicked);
			this.Window.ContentView.AddSubview(picker.View);
			
			// Init BottomView
			this.bView = new BottomView(this.Window.ContentView.Frame);
			this.Window.ContentView.AddSubview(this.bView);
			this.bView.UpdatedUI += new EventHandler(BViewUpdate);
			
			// Find Local Panel
			this.findLocal = FindLocal.Create();
			this.bView.Add(findLocal);
			
			// Build Message Panel
			this.bmPanel = BMPanel.Create();
			this.bmPanel.builder = this.builder;
			this.bView.Add(bmPanel);
			
			this.bView.UpdateUI();
			
			// Setup UI
			this.ViewMode = ProjectViewMode.Code;
			UpdateUI();
		}
		
		public void Init(string path)
		{
			this.Path = path;
			this.ShowWindow(this);
		}
		
		
		public static ProjectWindow Project(string path, Controller controller)
		{
			ProjectWindow pw = new ProjectWindow(new NSString("ProjectWindow"));
			pw.controller = controller;
			pw.Init(path);
			return pw;
		}		
	}
}

