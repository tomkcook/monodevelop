//
// DesktopApplication.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
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
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MonoDevelop.Core;
using Gnome.Vfs;

namespace MonoDevelop.Core.Gui.Utils
{
	public struct DesktopApplication
	{
		string displayName;
		string command;
		
		public string DisplayName {
			get { return displayName; }
		}
		
		public string Command {
			get { return command; }
		}
		
		public void Launch (params string[] files)
		{
			// TODO: implement all other cases
			if (command.IndexOf ("%f") != -1) {
				foreach (string s in files) {
					string cmd = command.Replace ("%f", s);
					Process.Start (cmd);
				}
			}
			else if (command.IndexOf ("%F") != -1) {
				string[] fs = new string [files.Length];
				for (int n=0; n<files.Length; n++) {
					fs [n] = files [n].Replace (" ", "\\ ");
				}
				string cmd = command.Replace ("%F", string.Join (" ", fs));
				Process.Start (cmd);
			} else {
				foreach (string s in files) {
					Process.Start (command + " " + s);
				}
			}
		}
		
		public static DesktopApplication GetDefaultApplication (string mimeType)
		{
			MimeApplication app = Mime.GetDefaultApplication (mimeType);
			if (app != null)
				return (DesktopApplication) Marshal.PtrToStructure (app.Handle, typeof(DesktopApplication));
			else
				return new DesktopApplication ();
		}
		
		public static DesktopApplication[] GetApplications (string mimeType)
		{
			MimeApplication[] apps = Mime.GetAllApplications (mimeType);
			ArrayList list = new ArrayList ();
			foreach (MimeApplication app in apps) {
				DesktopApplication dap = (DesktopApplication) Marshal.PtrToStructure (app.Handle, typeof(DesktopApplication));
				list.Add (dap);
			}
			return (DesktopApplication[]) list.ToArray (typeof(DesktopApplication));
		}
	}
}
