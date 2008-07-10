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

		[ObjectiveCMessage("drawInteriorWithFrame:inView:")]
		public void Draw(NSRect rect, NSView view)
		{
			// TODO: Draw Cell-Content with Data of this.ObjectValue
		}
	}
}

