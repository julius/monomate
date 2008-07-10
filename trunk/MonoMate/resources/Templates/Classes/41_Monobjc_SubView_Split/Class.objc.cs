using System;
using Monobjc;
using Monobjc.Cocoa;

namespace {TEMPLATE.VAR:NAMESPACE}
{
	[ObjectiveCClass]
	partial class {TEMPLATE.VAR:NAME} {TEMPLATE.VAR:SUPERCLASS}
	{
		public {TEMPLATE.VAR:NAME}() {}
		public {TEMPLATE.VAR:NAME}(IntPtr np) : base(np) {}
		
		[ObjectiveCMessage("drawRect:")]
		public void drawRect(NSRect rect) { this.Draw(rect); }
	}
}

