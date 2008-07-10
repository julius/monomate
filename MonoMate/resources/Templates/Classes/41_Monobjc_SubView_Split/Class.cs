using System;
using Monobjc;
using Monobjc.Cocoa;

namespace {TEMPLATE.VAR:NAMESPACE}
{
	partial class {TEMPLATE.VAR:NAME}
	{
		private void Draw(NSRect rect)
		{
			// Draw Background
			NSColor.BlueColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();
		}
	}
}

