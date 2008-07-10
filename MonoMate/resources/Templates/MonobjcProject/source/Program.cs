using System;
using Monobjc;
using Monobjc.Cocoa;

namespace {TEMPLATE.VAR:NAME}
{
	class Program 
	{
		public static void Main() 
		{
			// Load the Frameworks
			ObjectiveCRuntime.LoadFramework("Cocoa");
			ObjectiveCRuntime.Initialize();
			
			// Start the application
			NSApplication.Bootstrap();
			NSApplication.LoadNib("MainMenu.nib");
			NSApplication.RunApplication();
		}
	}

}