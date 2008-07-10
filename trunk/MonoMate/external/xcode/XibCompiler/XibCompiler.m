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


#import <Foundation/Foundation.h>

int main (int argc, const char * argv[]) {
    NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];

	if (argc != 2) {
		printf("usage: XibCompiler [path.app]\n");
		[pool drain];
		return 1;
	}
	
	NSString* path = [[NSString stringWithCString:argv[1]] stringByExpandingTildeInPath];
	printf([[NSString stringWithFormat:@"Scanning: %@\n", path] cString]);
	
	NSDirectoryEnumerator* e = [[NSFileManager defaultManager] enumeratorAtPath:path];
	NSString* file;
	
	while (file = [e nextObject]) {
		if ([[file pathExtension] isEqualToString:@"xib"]) {
			NSString* xpath = [path stringByAppendingPathComponent:file];
			NSString* npath = [xpath stringByReplacingOccurrencesOfString:@".xib" withString:@".nib"];
			
			printf([[NSString stringWithFormat:@"Compiling: \n%@\n->\n%@\n", xpath, npath] cString]);
			
			system([[NSString stringWithFormat:@"/usr/bin/ibtool --errors --warnings --notices --output-format human-readable-text --compile %@ %@", npath, xpath] cString]);
			system([[NSString stringWithFormat:@"rm %@", xpath] cString]);
		}
	}

    [pool drain];
    return 0;
}
