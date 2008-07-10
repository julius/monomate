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
		int flipDown = 8;
		int flipUp = 4;
		int flipLeft = 6;
		int flipRight = 5;
		private void FlipEffect(int option)
		{
			int wid = this.Window.WindowNumber;
			CGSPrivate.StartTransitionEffect(wid, 7, option, 0.12f);
		}
		
		public ProjectViewMode ViewMode
		{
			set
			{
				if (value == this.ViewMode) return;
				int effect = flipUp;
				
				if (value == ProjectViewMode.Code)
				{
					if (this.ViewMode == ProjectViewMode.Terminal)
					{
						this.terminal.View.IsHidden = true;
						effect = flipUp;
					}
					else if (this.ViewMode == ProjectViewMode.Creator)
					{
						this.ccreator.View.IsHidden = true;
						effect = flipDown;
					}
					else if (this.ViewMode == ProjectViewMode.Find)
					{
						this.findProject.View.IsHidden = true;
						effect = flipRight;
					}
					
					this.filer.View.IsHidden = false;
					this.tabs.IsHidden = false;
					this.tabs.Focus();
					
				}
				else if (value == ProjectViewMode.Terminal)
				{
					if (this.ViewMode == ProjectViewMode.Code)
					{
						this.tabs.IsHidden = true;
						this.filer.View.IsHidden = true;
						effect = flipDown;
					}
					else if (this.ViewMode == ProjectViewMode.Creator)
					{
						this.ccreator.View.IsHidden = true;
						effect = flipDown;
					}
					else if (this.ViewMode == ProjectViewMode.Find)
					{
						this.findProject.View.IsHidden = true;
						effect = flipRight;
					}
					
					if (this.findLocal.Visible) this.findLocal.Deactivate();
					if (this.bmPanel.Visible) this.bmPanel.Deactivate();
					
					this.terminal.View.IsHidden = false;
				}
				else if (value == ProjectViewMode.Creator)
				{
					if (this.ViewMode == ProjectViewMode.Code)
					{
						this.tabs.IsHidden = true;
						this.filer.View.IsHidden = true;
						effect = flipUp;
					}
					else if (this.ViewMode == ProjectViewMode.Terminal)
					{
						this.terminal.View.IsHidden = true;
						effect = flipUp;
					}
					else if (this.ViewMode == ProjectViewMode.Find)
					{
						this.findProject.View.IsHidden = true;
						effect = flipRight;
					}
					
					if (this.findLocal.Visible) this.findLocal.Deactivate();
					if (this.bmPanel.Visible) this.bmPanel.Deactivate();
					
					this.ccreator.View.IsHidden = false;
					this.ccreator.Focus();
				}
				else if (value == ProjectViewMode.Find)
				{
					if (this.ViewMode == ProjectViewMode.Code)
					{
						this.tabs.IsHidden = true;
						this.filer.View.IsHidden = true;
						effect = flipLeft;
					}
					else if (this.ViewMode == ProjectViewMode.Terminal)
					{
						this.terminal.View.IsHidden = true;
						effect = flipLeft;
					}
					else if (this.ViewMode == ProjectViewMode.Creator)
					{
						this.ccreator.View.IsHidden = true;
						effect = flipLeft;
					}
					
					if (this.findLocal.Visible) this.findLocal.Deactivate();
					if (this.bmPanel.Visible) this.bmPanel.Deactivate();
					
					this.findProject.View.IsHidden = false;
					this.findProject.Focus();
				}
				this.UpdateUI();
				this.Window.AlphaValue = 0.8f;
				this.Window.Display();

				this.FlipEffect(effect);
				CGSPrivate.FinishTransitionEffect();
				
				//this.Window.AlphaValue = 0.6f;
				CGSPrivate.AlphaSwitch(this.Window, 0.05f);
			}
			get 
			{ 
				if (!this.terminal.View.IsHidden) return ProjectViewMode.Terminal;
				if (!this.ccreator.View.IsHidden) return ProjectViewMode.Creator;
				if (!this.findProject.View.IsHidden) return ProjectViewMode.Find;
				return ProjectViewMode.Code;
			} 
		}
		
		[ObjectiveCMessage("windowDidResize:")]
		public void WindowResize(NSNotification notif)
		{
			UpdateUI();
		}
		
		public void BViewUpdate(object sender, EventArgs args)
		{
			UpdateUI();
		}
		
		public void UpdateUI()
		{
			if (tabs == null) return;
			NSRect frame = this.Window.ContentView.Frame;
			
			if (this.ViewMode == ProjectViewMode.Terminal)
			{
				terminal.Frame = frame;
			}
			else if (this.ViewMode == ProjectViewMode.Code)
			{
				NSRect oframe = frame;
				oframe.size.width = 200;
				this.filer.Frame = oframe;
				this.filer.UpdateUI();
				
				frame.origin.x = oframe.size.width;
				frame.size.width -= frame.origin.x;
				
				if (this.bView.IsHidden == false)
				{
					float bheight = this.bView.Frame.size.height;
					frame.origin.y += bheight;
					frame.size.height -= bheight;
				}
				
				tabs.Frame = frame;
				tabs.UpdateUI();
				
				this.bView.ResizeUI();
			}
			else if (this.ViewMode == ProjectViewMode.Creator)
			{
				ccreator.Frame = frame;
			}
			else if (this.ViewMode == ProjectViewMode.Find)
			{
				findProject.Frame = frame;
			}
		}
	}
}

