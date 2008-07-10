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
using System.Collections.Generic;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate 
{
	[ObjectiveCClass]
	partial class Controller : NSObject
	{
		public Controller() {}
		public Controller(IntPtr np) : base(np) {}
		
		[ObjectiveCField]
		public NSWindow mainWindow;
		
		public ProjectWindow currentWindow
		{
			get
			{
				NSWindow cWnd = NSApplication.SharedApplication.KeyWindow;
				if (cWnd == null) return null;
				
				try 
				{
					return (ProjectWindow) cWnd.Delegate;
				}
				catch
				{
					return null;
				}
			}
		}
		
		public TabView tabs
		{
			get
			{
				return this.currentWindow.tabs;
			}
		}
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			OpenProject(this);
		}
		
		[ObjectiveCMessage("applicationShouldTerminate:")]
		public NSApplicationTerminateReply ShouldTerminate(NSApplication sender)
		{
			foreach (ProjectWindow pw in pwList)
				if (pw.FullClose() == false) return NSApplicationTerminateReply.NSTerminateCancel;
				
			return NSApplicationTerminateReply.NSTerminateNow;
		}
		
		[ObjectiveCMessage("openProject:")]
		public void OpenProject(Id sender)
		{
			NSOpenPanel openPanel = new NSOpenPanel();
			openPanel.CanChooseFiles = false;
			openPanel.CanChooseDirectories = true;
			openPanel.AllowsMultipleSelection = false;
			int result = openPanel.RunModal();
			
			if (result == 1)
			{
				//Console.WriteLine("Folder: " + openPanel.Filename);
				this.OpenProjectOfPath(openPanel.Filename);	
			}
			openPanel.Release();
		}
		
		public List<ProjectWindow> pwList = new List<ProjectWindow>();
		public void OpenProjectOfPath(string path)
		{
			if (!NSFileManager.DefaultManager.FileExistsAtPath(new NSString(path))) return;
			ProjectWindow pw = ProjectWindow.Project(path, this);
			pwList.Add(pw);
		}
		
		[ObjectiveCMessage("createClass:")]
		public void CreateClass(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.ViewMode = ProjectViewMode.Creator;
		}
		
		[ObjectiveCMessage("findProject:")]
		public void FindProject(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.ViewMode = ProjectViewMode.Find;
		}
		
		[ObjectiveCMessage("createFolder:")]
		public void CreateFolder(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.CreateFolder();
		}
		
		[ObjectiveCMessage("showBuildMessages:")]
		public void BuildMessages(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.ViewMode = ProjectViewMode.Code;
			this.currentWindow.bmPanel.Activate();
		}
		
		[ObjectiveCMessage("focusOverview:")]
		public void OFocus(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.ViewMode = ProjectViewMode.Code;
			this.currentWindow.filer.Focus();
		}
		
		[ObjectiveCMessage("rescan:")]
		public void Rescan(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.filer.CurrentDirectory.Rescan();
		}
		
		[ObjectiveCMessage("find:")]
		public void Find(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.Find();
		}
		
		[ObjectiveCMessage("pickFile:")]
		public void PickFile(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.Pick();
		}
		
		[ObjectiveCMessage("viewCode:")]
		public void ViewCode(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.ViewMode = ProjectViewMode.Code;
		}
		
		[ObjectiveCMessage("viewTerminal:")]
		public void ViewTerminal(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.ViewMode = ProjectViewMode.Terminal;
		}
		
		[ObjectiveCMessage("closeCurrent:")]
		public void CloseCurrent(Id sender)
		{
			if (this.currentWindow == null) 
			{
				if (NSApplication.SharedApplication.KeyWindow != null) NSApplication.SharedApplication.KeyWindow.Close();
				return;
			}
			if (this.tabs.CurrentItem == null)
			{
				this.currentWindow.Window.Close();
			}
			else
				this.CloseCurrent(this.tabs.CurrentItem.FullPath);
		}
		
		[ObjectiveCMessage("buildCurrent:")]
		public void BuildCurrent(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.Build("build.sh", "-compile"); 
		}
		
		[ObjectiveCMessage("buildRunCurrent:")]
		public void BuildRunCurrent(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.Build("build.sh", "-run"); 
		}
		
		[ObjectiveCMessage("buildCurrentFull:")]
		public void BuildCurrentFull(Id sender)
		{
			if (this.currentWindow == null) return;
			this.currentWindow.Build("build.sh", "-build"); 
		}
		
		[ObjectiveCMessage("saveCurrent:")]
		public void SaveCurrent(Id sender)
		{
			if (this.currentWindow == null) return; 
			if (this.tabs.CurrentItem == null) return;

			this.currentWindow.SaveItem(this.tabs.CurrentItem.FullPath);
		}
		
		public void CloseCurrent(string path)
		{
			this.currentWindow.Close(path);
		}
		
		[ObjectiveCMessage("navLeft:")]
		public void NavLeft(Id sender)
		{
			if (this.currentWindow == null) return;
			this.tabs.NavigateLeft();
		}
		
		[ObjectiveCMessage("navRight:")]
		public void NavRight(Id sender)
		{
			if (this.currentWindow == null) return;
			this.tabs.NavigateRight();
		}
		
		[ObjectiveCMessage("createProject:")]
		public void CreateProject(Id sender)
		{
			CreateProjectWindow cpw = CreateProjectWindow.Create();
			cpw.Success += new EventHandler(ProjectCreated);
		}
		
		public void ProjectCreated(object sender, EventArgs args)
		{
			this.OpenProjectOfPath(((CreateProjectWindow)sender).FullPath);
		}
		
		[ObjectiveCMessage("windowDidResize:")]
		public void WindowResize(NSNotification notif)
		{
		}
	}
}