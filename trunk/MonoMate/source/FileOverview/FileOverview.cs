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
using System.Runtime.InteropServices;
using Monobjc;
using Monobjc.Cocoa;

namespace MonoMate
{
	[ObjectiveCClass]
	partial class FileOverview : NSViewController 
	{
		public FileOverview() {}
		public FileOverview(IntPtr np) : base(np) {}
		public FileOverview(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}
		
		[ObjectiveCField]
		public NSScrollView viewScroll;
		
		[ObjectiveCField]
		public NSOutlineView viewOutline;
		
		[ObjectiveCField]
		public NSButton buttonRefresh;
		
		[ObjectiveCField]
		public NSTextField labelTitle;
		
		[ObjectiveCField]
		public NSMenuItem menuReveal;
		
		public string Path;
		public OFile Root;
		
		public event EventHandler Picked;
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			this.viewOutline.DataSource = this;
			this.viewOutline.Delegate = this;
			this.viewOutline.OutlineTableColumn.DataCell = new OCell();
		}
		
		public NSRect Frame
		{
			set
			{
				this.View.Frame = value;
				NSRect sframe = value;
				sframe.origin = new NSPoint(0, 30);
				sframe.size.height -= sframe.origin.y + 30;
				this.viewScroll.Frame = sframe;
				
				NSRect lframe = this.labelTitle.Frame;
				lframe.origin.y = sframe.origin.y + sframe.size.height + 7;
				this.labelTitle.Frame = lframe;
				
				NSRect bframe = this.buttonRefresh.Frame;
				bframe.origin.y = 5;
				bframe.origin.x = sframe.size.width - bframe.size.width - 10;
				this.buttonRefresh.Frame = bframe;
			}
		}
		
		[ObjectiveCMessage("reveal:")]
		public void Reveal(Id sender)
		{
			NSWorkspace.SharedWorkspace.SelectFileInFileViewerRootedAtPath(new NSString(this.CurrentSelection.FullPath), new NSString(this.CurrentSelection.ParentPath));		}
		
		[ObjectiveCMessage("rename:")]
		public void Rename(Id sender)
		{
			ItemRenamer ir = ItemRenamer.Create();
			ir.OriginalName = this.CurrentSelection.Title;
			NSApplication.SharedApplication.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(
				ir.Window,
				this.View.Window,
				RenameFinished, 
				IntPtr.Zero);
		}
		public void RenameFinished(NSWindow sheet, int returnCode, IntPtr contextInfo)
		{
			sheet.OrderOut(this);
			if (returnCode != 0) return;
			string name = ((ItemRenamer)sheet.WindowController).textName.StringValue;
			if (name.Trim() == "") return;
			
			((ProjectWindow)this.View.Window.WindowController).Close(this.CurrentSelection.FullPath);
			NSString srcPath = this.CurrentSelection.FullPath;
			NSString destPath = srcPath.StringByDeletingLastPathComponent.StringByAppendingPathComponent(new NSString(name));
			
			NSFileManager.DefaultManager.MovePathToPathHandler(srcPath, destPath, null);
			this.CurrentSelection.Parent.Rescan();
		}
		
		[ObjectiveCMessage("remove:")]
		public void Remove(Id sender)
		{
			OFile cur = this.CurrentSelection;
			
			NSAlert alert = new NSAlert();
			alert.MessageText = new NSString("Remove "+cur.Title+" ?");
			alert.InformativeText = new NSString("Full Path: " + cur.FullPath);
			alert.AddButtonWithTitle(new NSString("Remove"));
			alert.AddButtonWithTitle(new NSString("Cancel"));
			int result = alert.RunModal();
			
			if (result == 1001) return;
			if (result == 1000) 
			{
				((ProjectWindow)this.View.Window.WindowController).Close(cur.FullPath);
				NSFileManager.DefaultManager.RemoveFileAtPathHandler(new NSString(cur.FullPath), null);
				cur.Parent.Rescan();
			}
		}
		
		[ObjectiveCMessage("validateMenuItem:")]
		public bool ValidateMenuItem(NSMenuItem mitem)
		{
			if (mitem == menuReveal) return true;
			return (this.CurrentSelection != this.Root);
		}				
		
		public void Focus()
		{
			this.View.Window.MakeFirstResponder(this.viewOutline);
		}
		
		public void CreateFolder(string name)
		{
			if (name.Trim() == "") return;
			NSString fullPath = (new NSString(this.CurrentDirectory.FullPath)).StringByAppendingPathComponent(new NSString(name));
			NSFileManager.DefaultManager.CreateDirectoryAtPathAttributes(fullPath, null);
			
			this.CurrentDirectory.Rescan();
		}
		
		public void Rescan()
		{
			this.Root = new OFile(this.Path);
			this.Root.Rescanned += new EventHandler(ItemRescanned);
			this.viewOutline.ReloadData();
			this.viewOutline.ExpandItem(this.viewOutline.ItemAtRow(0));
		}
		
		public void ItemRescanned(object sender, EventArgs args)
		{
			OFile file = (OFile) sender;
			this.viewOutline.ReloadItemReloadChildren(file, true);
			Console.WriteLine("Rescanned: " + file.FullPath);
		}

		[ObjectiveCMessage("refresh:")]
		public void Refresh(Id sender)
		{
			this.CurrentDirectory.Rescan();
		}
		
		public void UpdateUI()
		{
			this.labelTitle.StringValue = (new NSString(this.Path)).LastPathComponent;
		}
		
		public List<string> CSFiles
		{
			get { return this.Root.CSFiles; }
		}

		public OFile CurrentSelection
		{
			get 
			{
				int row = this.viewOutline.SelectedRow;
				return (OFile) this.viewOutline.ItemAtRow(row);
			}
		}
		
		public OFile CurrentDirectory
		{
			get
			{
				OFile cur = this.CurrentSelection;
				if (cur.IsDirectory) return cur;
				return (OFile) this.viewOutline.ParentForItem(cur);
			}
		}
		
		[ObjectiveCMessage("outlineClick:")]
		public void OutlineClick(Id sender)
		{
			if (this.CurrentSelection == null) return;
			if (this.viewOutline.SelectedRow < 0) return;
			if (this.CurrentSelection.IsDirectory) return;
			
			if (this.CurrentSelection.IsCodeFile)
			{
				if (this.Picked != null) this.Picked(this, new EventArgs());
			}
			else if (sender == null)   // sender is null on a keypress
			{
				//Console.WriteLine("View: " + this.CurrentSelection.FullPath);
				//NSWorkspace.SharedWorkspace.SelectFileInFileViewerRootedAtPath(new NSString(this.CurrentSelection.FullPath), new NSString(this.CurrentSelection.ParentPath));
			}
		}
		
		// ------------------ Data Source ------------------------
		[ObjectiveCMessage("outlineView:numberOfChildrenOfItem:")]
		public int NumberOfChildren(NSOutlineView oview, Id item)
		{
			if (item == null) return 1;
			return ((OFile)item).Children.Count;
		}
		
		[ObjectiveCMessage("outlineView:child:ofItem:")]
		public Id ChildOfItem(NSOutlineView oview, int index, Id item)
		{
			if (item == null) return this.Root.Retain();
			return ((OFile)item).Children[index].Retain();
		}
		
		[ObjectiveCMessage("outlineView:isItemExpandable:")]
		public bool IsItemExpandable(NSOutlineView oview, Id item)
		{
			if (item == null) return false;
			return (((OFile)item).Children.Count > 0);
		}
		
		[ObjectiveCMessage("outlineView:objectValueForTableColumn:byItem:")]
		public Id ObjectForColumnOfItem(NSOutlineView oview, NSTableColumn col, Id item)
		{
			return ((OFile)item).Retain();
		}
		
		// -------------------------------------------------------

		public static FileOverview Create(string path)
		{
			FileOverview fo = new FileOverview(new NSString("FileOverview"), NSBundle.MainBundle);
			fo.Path = path;
			return fo;
		}
	}
}