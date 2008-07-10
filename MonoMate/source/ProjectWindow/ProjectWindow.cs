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
	[ObjectiveCClass]
	partial class ProjectWindow : NSWindowController 
	{
		public ProjectWindow() {}
		public ProjectWindow(IntPtr np) : base(np) {}
		public ProjectWindow(NSString nibName) : base(nibName) {}

		public BottomView bView;
		public FindLocal findLocal;
		public BMPanel bmPanel;
		
		public FindProject findProject;
		public ClassCreatorController ccreator;
		public FileOverview filer;
		public Builder builder;
		public Terminal terminal;
		public TabView tabs;
		public FilePicker picker;
		public Controller controller;
		public string Path;
		public List<string> Files
		{
			get { return this.filer.CSFiles; }
		}
		
		public void CreateFolder()
		{
			FolderCreator fc = FolderCreator.Create();
			NSApplication.SharedApplication.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(
				fc.Window,
				this.Window,
				SheetFinished, 
				IntPtr.Zero);
		}
		
		public void SheetFinished(NSWindow sheet, int returnCode, IntPtr contextInfo)
		{
			sheet.OrderOut(this);
			if (returnCode != 0) return;
			string path = ((FolderCreator)sheet.WindowController).textName.StringValue;
			this.filer.CreateFolder(path);
		}
		
		public void ClassCreationSuccess(object sender, EventArgs arg)
		{
			this.ViewMode = ProjectViewMode.Code;
			this.LoadFile(this.ccreator.CurrentCreator.FileToOpen);
			this.filer.CurrentDirectory.Rescan();
		}
		
		public void ClassCreationCanceled(object sender, EventArgs arg)
		{
			this.ViewMode = ProjectViewMode.Code;
		}
		
		public void OpenFileAtLine(string path, int line)
		{
			if (!this.IsFileAlreadyOpen(path))
			{
				this.LoadFile(path);
				this.UpdateUI();
			}
			
			TabViewItem item = this.tabs.ItemByPath(path);
			if (item == null) return;
			
			this.tabs.NavigateItem(item);
			this.tabs.Focus();
			
			int index1 = item.Editor.IndexOfLine(line-1);
			int index2 = item.Editor.IndexOfLine(line);
			item.Editor.PresentRange(new NSRange((uint)index1, (uint)(index2-index1-1)));
		}
		
		public void OpenFileAtRange(string path, NSRange range)
		{
			this.ViewMode = ProjectViewMode.Code;
			if (!this.IsFileAlreadyOpen(path))
			{
				this.LoadFile(path);
				this.UpdateUI();
			}
			
			TabViewItem item = this.tabs.ItemByPath(path);
			if (item == null) return;
			
			this.tabs.NavigateItem(item);
			this.tabs.Focus();
			
			item.Editor.PresentRange(range);
		}
		
		public void Find()
		{
			//if (this.tabs.itemList.Count < 1) return;
			this.findLocal.Activate();
		
		}
		
		public void SaveItem(string path)
		{
			this.tabs.ItemByPath(path).Save();
		}
		
		public bool Close(string path)
		{ return Close (path, false); }
		
		public bool Close(string path, bool forfull)
		{
			TabViewItem item = this.tabs.ItemByPath(path);
			if (item == null) return true;
			if (item.Editor.HasChanged)
			{
				NSAlert alert = new NSAlert();
				alert.MessageText = new NSString("Save "+item.Title+" ?");
				alert.InformativeText = new NSString("You have made changes to this file, which will be lost without saving !");
				alert.AddButtonWithTitle(new NSString("Save"));
				alert.AddButtonWithTitle(new NSString("Cancel"));
				alert.AddButtonWithTitle(new NSString("Don't Save"));
				int result = alert.RunModal();
				
				if (result == 1001) return false;
				if (result == 1000) item.Save();
			} 
			if (!forfull) 
			{
				this.tabs.Remove(item);
				this.tabs.Focus();
			}
			return true;
		}
		
		public bool FullClose()
		{
			foreach (TabViewItem item in this.tabs.itemList)
				if (this.Close(item.FullPath, true) == false) return false;
			return true;
		}
		
		public void FilePicked(object sender, EventArgs args)
		{
			if (sender == (object)filer)
			{
				OFile f = this.filer.CurrentSelection;
				if (this.IsFileAlreadyOpen(f.FullPath))
					this.tabs.NavigateItem(this.tabs.ItemByPath(f.FullPath));
				else 
					this.LoadFile(f.FullPath);
			}
			else
			{
				PFile pf = this.picker.PickedFile;
				if (pf.AlreadyOpen)
					this.tabs.NavigateItem(this.tabs.ItemByPath(pf.FullPath));
				else 
					this.LoadFile(pf.FullPath);
			}
			UpdateUI();
		}
		
		public List<PFile> PickList
		{
			get
			{
				List<PFile> l = new List<PFile>();
				foreach (string file in Files)
				{
					PFile pf = new PFile();
					pf.FullPath = file;
					pf.AlreadyOpen = this.IsFileAlreadyOpen(file);
					pf.Title = (new NSString(file)).LastPathComponent;
					l.Add(pf);
				}
				return l;
			}
		}
		
		public void Pick()
		{
			this.picker.Show(this.PickList);
		}
		
		public bool IsFileAlreadyOpen(string path)
		{
			return (tabs.ItemByPath(path) != null);
		}

		public void LoadFile(string path)
		{
			MonoMate.Editor.SourceEditor editor = MonoMate.Editor.SourceEditor.Editor();
			editor.controller = this.controller;
			
			TabViewItem tvi = new TabViewItem();
			tvi.Editor = editor;
			tvi.FullPath = path;
			
			tabs.AddTab(tvi);
			tabs.NavigateItem(tvi);
			

			TextReader txt = new StreamReader(path, System.Text.Encoding.UTF8);
			string text = txt.ReadToEnd();
			txt.Close();
			editor.LoadCompleteText(text);
		
			this.ViewMode = ProjectViewMode.Code;
		}
		
		public void Build(string script, string arg)
		{
			this.terminal.Clear();
			this.ViewMode = ProjectViewMode.Terminal;
			builder.Build(script, arg);
		}
		
		public void BuildFinished(object sender, EventArgs args)
		{
			this.ViewMode = ProjectViewMode.Code;
			if (this.builder.bmList.Count > 0) this.bmPanel.Activate();
		}
	}


	public enum ProjectViewMode
	{
		Code,
		Terminal,
		Creator,
		Find
	}
}