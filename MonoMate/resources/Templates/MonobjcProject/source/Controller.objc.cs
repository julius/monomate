using System;
using Monobjc;
using Monobjc.Cocoa;

namespace {TEMPLATE.VAR:NAME} 
{
	[ObjectiveCClass]
	partial class Controller : NSObject
	{
		public Controller() {}
		public Controller(IntPtr np) : base(np) {}
		
		[ObjectiveCMessage("awakeFromNib")]
		public void awakeFromNib() { this.AwakeFromNib(); }
	}
}