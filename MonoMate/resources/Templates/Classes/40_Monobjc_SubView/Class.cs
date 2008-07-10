using System;
using Monobjc;
using Monobjc.Cocoa;

namespace {TEMPLATE.VAR:NAMESPACE}
{
	[ObjectiveCClass]
	class {TEMPLATE.VAR:NAME} {TEMPLATE.VAR:SUPERCLASS}
	{
		public {TEMPLATE.VAR:NAME}() {}
		public {TEMPLATE.VAR:NAME}(IntPtr np) : base(np) {}

		[ObjectiveCMessage("drawRect:")]
		public void Draw(NSRect rect)
		{
			// Draw Background
			NSColor.BlueColor.SetFill();
			NSBezierPath path = new NSBezierPath();
			path.AppendBezierPathWithRect(rect);
			path.Fill();
		}
	}
}

