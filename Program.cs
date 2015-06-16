/*
 * Erstellt mit SharpDevelop.
 * Benutzer: buck
 * Datum: 08.06.2015
 * Zeit: 13:40
 * 
 */
using System;
using System.Windows.Forms;

namespace NagiosMonitor
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
	}
}
