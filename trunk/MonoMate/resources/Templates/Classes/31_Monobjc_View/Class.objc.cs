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
		public {TEMPLATE.VAR:NAME}(NSString nibName, NSBundle bundle) : base(nibName, bundle) {}

		[ObjectiveCMessage("awakeFromNib")]
		public void awakeFromNib() { this.Awake(); }

		public static {TEMPLATE.VAR:NAME} Create()
		{
			{TEMPLATE.VAR:NAME} viewController = new {TEMPLATE.VAR:NAME}(new NSString("{TEMPLATE.VAR:NIB}"), NSBundle.MainBundle);
			return viewController;
		}		
	}
}

