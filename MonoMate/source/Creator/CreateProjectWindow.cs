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
	[ObjectiveCClass]
	class CreateProjectWindow : NSWindowController 
	{
		public CreateProjectWindow() {}
		public CreateProjectWindow(IntPtr np) : base(np) {}
		public CreateProjectWindow(NSString nibName) : base(nibName) {}
		
		[ObjectiveCField]
		public NSTextField textName;
		
		[ObjectiveCField]
		public NSTextField textPath;
		
		[ObjectiveCField]
		public NSTextField textError;
		
		public event EventHandler Success;
		
		public string FullPath
		{
			get { return (string)(new NSString(this.textPath.StringValue)).StringByExpandingTildeInPath; }
		}
		
		[ObjectiveCMessage("awakeFromNib")]
		public void Awake()
		{
			this.Window.MakeKeyAndOrderFront(this);
			textName.Delegate = this;
		}	
		
		[ObjectiveCMessage("controlTextDidChange:")]
		public void NameChanged(NSNotification notif)
		{
			string path = textPath.StringValue;
			int pos = path.LastIndexOf("/");
			if (pos == -1) path = "";
			else path = path.Substring(0, pos+1);
			textPath.StringValue = new NSString(path + textName.StringValue);
		}
		
		[ObjectiveCMessage("cancel:")]
		public void Cancel(Id sender)
		{
			this.Window.Close();
		}
		
		[ObjectiveCMessage("create:")]
		public void Create(Id sender)
		{
			ProjectCreator pc = new ProjectCreator(textPath.StringValue, textName.StringValue);
			pc.Created += new EventHandler(CreationSuccess);
			pc.Failed += new EventHandler(CreationFailed);
			
			pc.Create();
		}
		
		public void CreationSuccess(object sender, EventArgs args)
		{
			if (this.Success != null) this.Success(this, new EventArgs());
			this.Window.Close();
		}
		
		public void CreationFailed(object sender, EventArgs args)
		{
			this.textError.IsHidden = false;
			this.textError.StringValue = ((ProjectCreator)sender).Error;
		}
		
		public static CreateProjectWindow Create()
		{
			CreateProjectWindow cpw = new CreateProjectWindow(new NSString("CreateProject"));
			cpw.ShowWindow(cpw);
			return cpw;
		}		
	}
}