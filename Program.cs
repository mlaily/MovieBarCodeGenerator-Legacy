using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MovieBarCode
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			CLI cli = new CLI();
			cli.Arguments = new List<CLI.Argument>() 
			{
				new CLI.Argument() 
				{
					 Names = new string[] {"help"},
					 ShortNames = new char[] {'h', '?'},
					 ValueExpected = false
				},
				new CLI.Argument() 
				{
					 Names = new string[] {"input", "in"},
					 ShortNames = new char[] {'i'}
				},
				new CLI.Argument() 
				{
					 Names = new string[] {"output", "out"},
					 ShortNames = new char[] {'o'}
				},
			};
			var result = cli.ParseCommandLine();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
