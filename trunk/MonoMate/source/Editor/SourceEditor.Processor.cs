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
using System.Collections.Generic;
using Monobjc;
using Monobjc.Cocoa;

// joo: öäü?
namespace MonoMate.Editor 
{
	partial class SourceEditor 
	{
		string pcontent;
		List<Symbol> symbolList = new List<Symbol>();
		
		public void ProcessSymbols()
		{
			pcontent = this.Text;
			symbolList.Clear();
			
			char[] seperators = { ' ', ',', '.', '(', ')', '/', '*', '+', '-', '&', '|', ';', ':', '{', '}', '[', ']', '<', '>', '!', '=', '?', '\t', '\n' };
			string current = "";
			int pos = 0;
			while (pos < pcontent.Length)
			{
				char ccur = pcontent[pos];
				
				// is string-start ?
				if ((ccur == '"') || (ccur == '\''))
				{
					this.PushSymbolCandidate(current, pos - current.Length);
					current = "";
					char csstart = ccur;
					pos ++;
					while (pos < pcontent.Length)
					{
						char cscur = pcontent[pos];
						
						if (cscur == '\\') // ignore character !
						{
							current += cscur;
							pos++;
							current += pcontent[pos];
							pos++;
							continue;
						}
						
						if (cscur == csstart) break;
						if (cscur == '\n') break;
						
						current += cscur;
						 
						pos++;
					}
					Symbol s = new Symbol();
					s.Type = SymbolType.String;
					s.Text = csstart + current + csstart;
					s.Range = new NSRange((uint)(pos - current.Length - 1), (uint)(current.Length+2));
					
					symbolList.Add(s);

					current = "";
					pos ++;
					continue;
				}
				
				// is comment ?
				if ((ccur == '/') && ((pos+1) < pcontent.Length))
				{
					char cnext = pcontent[pos+1];
					if ((cnext == '/') || (cnext == '*')) 
					{
						this.PushSymbolCandidate(current, pos - current.Length);
						current = "";
						pos += 2;
						
						// is one line comment ?
						if (cnext == '/')
						{
							while (pos < pcontent.Length)
							{
								char cccur = pcontent[pos];
								if (cccur == '\n') break;
								current += cccur;
								pos++;
							}
							Symbol s = new Symbol();
							s.Type = SymbolType.Comment;
							s.Text = "//" + current;
							s.Range = new NSRange((uint)(pos - current.Length - 2), (uint)(current.Length+2));
							
							symbolList.Add(s);
						}
						else // is multi line comment
						{
							while (pos < pcontent.Length)
							{
								char cccur = pcontent[pos];
								if (cccur == '*') 
								{
									if ((pos+1 < pcontent.Length) && (pcontent[pos+1] == '/')) 
										break;
								}
								current += cccur;
								pos++;
							}
							Symbol s = new Symbol();
							s.Type = SymbolType.Comment;
							s.Text = "/*" + current + "*/";
							s.Range = new NSRange((uint)(pos - current.Length - 2), (uint)(current.Length+4));
							
							symbolList.Add(s);
							pos++;
						}						
						current = "";
						pos ++;
						continue;
					}
					
				}
				
				// is seperator ?
				bool isSeperator = false;
				foreach (char sep in seperators)
					if (ccur == sep)
					{
						isSeperator = true;
						break;
					}
					
				if (isSeperator)
				{
					this.PushSymbolCandidate(current, pos - current.Length);
					current = "";
				}
				else
				{
					current += ccur;
				}
				
				pos++;
			}	
		}
		
		private void PushSymbolCandidate(string text, int pos)
		{
			string[] symbs = { "class", "namespace", "public", "private", "protected", "virtual", "abstract", "static", "partial", "get", "set", "using", "new", "throw", "override", "ref", "if", "else", "continue", "break", "for", "foreach", "while", "do", "in", "as", "is", "struct", "enum", "try", "catch", "finally", "delegate", "volatile", "return", "switch", "case", "default", "this", "base", "event" };
			foreach (string symbol in symbs)
			{
				if (text != symbol) continue;
				
				Symbol s = new Symbol();
				s.Type = SymbolType.Reserved;
				s.Text = text;
				s.Range = new NSRange((uint)pos, (uint)text.Length);
				
				symbolList.Add(s);
				return;
			}
			string[] symbs2 = { "string", "void", "int", "float", "double", "uint", "char", "bool", "null", "object", "true", "false" };
			foreach (string symbol in symbs2)
			{
				if (text != symbol) continue;
				
				Symbol s = new Symbol();
				s.Type = SymbolType.ReservedType;
				s.Text = text;
				s.Range = new NSRange((uint)pos, (uint)text.Length);
				
				symbolList.Add(s);
				return;
			}
		}
	}
	
	public class Symbol : IComparable
	{
		public SymbolType Type;
		public string Text;
		public NSRange Range;

		// Sorting related function
		public int CompareTo(object obj)
		{
			Symbol sym = (Symbol) obj;
			return ((int)this.Range.location).CompareTo((int)sym.Range.location);
		}
	}

	public enum SymbolType
	{
		Comment,
		Reserved,
		ReservedType,
		String
	}
}