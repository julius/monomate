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
	class ProjectWnd : NSWindow
	{
		public ProjectWnd() {}
		public ProjectWnd(IntPtr np) : base(np) {}

		private WindowEffect blurEffect;

		[ObjectiveCMessage("makeKeyAndOrderFront:")]
		public override void MakeKeyAndOrderFront(Id sender)
		{
			if (this.IsKeyWindow) return;
			ObjectiveCRuntime.SendMessageSuper(this, Class.GetClassFromType(typeof (ProjectWnd)), "makeKeyAndOrderFront:", sender);

			if (this.blurEffect != null) return;
			
			// Setup Blurring
			//this.blurEffect = new WindowEffect(this, "CIGaussianBlur");
			//this.blurEffect.SetAttribute("inputRadius", 3);
		}
	}
}

