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
	partial class Controller
	{
		[ObjectiveCField]
		public NSMenuItem menuFileClass;
		[ObjectiveCField]
		public NSMenuItem menuFileClose;
		[ObjectiveCField]
		public NSMenuItem menuFileFolder;
		[ObjectiveCField]
		public NSMenuItem menuFileSave;

		[ObjectiveCField]
		public NSMenuItem menuEditFind;
		[ObjectiveCField]
		public NSMenuItem menuEditFindProj;

		[ObjectiveCField]
		public NSMenuItem menuNavNext;
		[ObjectiveCField]
		public NSMenuItem menuNavPrev;
		[ObjectiveCField]
		public NSMenuItem menuNavCode;
		[ObjectiveCField]
		public NSMenuItem menuNavTerminal;
		
		[ObjectiveCMessage("validateMenuItem:")]
		public bool ValidateMenuItem(NSMenuItem mitem)
		{
			if (mitem == menuFileClass)
			{
				if (this.currentWindow == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			if (mitem == menuFileClose)
			{
				if (this.currentWindow == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			if (mitem == menuFileFolder)
			{
				if (this.currentWindow == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			if (mitem == menuFileSave)
			{
				if (this.currentWindow == null) return false;
				if (this.tabs.CurrentItem == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			
			if (mitem == menuEditFind)
			{
				if (this.currentWindow == null) return false;
				if (this.tabs.CurrentItem == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			if (mitem == menuEditFindProj)
			{
				if (this.currentWindow == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			
			if (mitem == menuNavNext)
			{
				if (this.currentWindow == null) return false;
				if (this.tabs.CurrentItem == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			if (mitem == menuNavPrev)
			{
				if (this.currentWindow == null) return false;
				if (this.tabs.CurrentItem == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			if (mitem == menuNavTerminal)
			{
				if (this.currentWindow == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Code);
			}
			if (mitem == menuNavCode)
			{
				if (this.currentWindow == null) return false;
				return (this.currentWindow.ViewMode == ProjectViewMode.Terminal);
			}
			return true;
		}				
		
	}
}

