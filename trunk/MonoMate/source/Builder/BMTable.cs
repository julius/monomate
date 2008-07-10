﻿//
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
	class BMTable : NSTableView 
	{
		public BMTable() {}
		public BMTable(IntPtr np) : base(np) {}
		
		public BMPanel bmPanel;
		
		[ObjectiveCMessage("keyUp:")]
		public override void KeyUp(NSEvent evt)
		{
			if (evt.KeyCode == 53) 	// Escape 
			{
				bmPanel.Deactivate();
				return;
			}
			
			
			if (evt.KeyCode == 36) 	// Return 
			{
				bmPanel.TableAction(this);
				return;
			}
			
		 	ObjectiveCRuntime.SendMessageSuper<bool>(this, NSTableViewClass, "keyUp:", evt);
		}
		
	}
}