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
	class WindowEffect 
	{
		private NSWindow window;
		private int conn;
		private IntPtr ptrEffect = IntPtr.Zero;
		
		public WindowEffect(NSWindow window, string name)
		{
			this.window = window;
			this.conn = CGSPrivate._CGSDefaultConnection();
			
			int error = 0;
			
			error = CGSPrivate.CGSNewCIFilterByName(conn, name, ref ptrEffect);
			if (error != 0) Console.WriteLine("Effect Error: "+error);
			error = CGSPrivate.CGSAddWindowFilter(conn, this.window.WindowNumber, ptrEffect, 12289);
			if (error != 0) Console.WriteLine("Effect Error: "+error);
		}
		
		public void SetAttribute(string name, float value)
		{
			NSMutableDictionary dict = new NSMutableDictionary();
			dict.Add(new NSString(name), new NSNumber(value));
			CGSPrivate.CGSSetCIFilterValuesFromDictionary(this.conn, this.ptrEffect, dict);
		}
	}
}

