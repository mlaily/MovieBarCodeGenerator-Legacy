using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;

namespace MovieBarCode
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				//start GUI
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form1());
			}
			else
			{
				//CLI mode
				AttachConsole();
				Console.WriteLine(Environment.CommandLine);
				#region "Arguments declaration"
				var help = new CLI.Argument()
				{
					Names = new string[] { "help" },
					ShortNames = new char[] { 'h', '?' },
					Description = "Show this help.",
					ValueExpected = false
				};
				var input = new CLI.Argument()
				{
					Names = new string[] { "input", "in" },
					Description = "Input file or directory, depending on the directories argument.",
					ShortNames = new char[] { 'i' }
				};
				var output = new CLI.Argument()
				{
					Names = new string[] { "output", "out" },
					Description = "Output file or directory, depending on the directories argument.\n" +
					"If not set, default name or current directory is used." +
					"If file exists, it will be overwritten without warning.",
					ShortNames = new char[] { 'o' }
				};
				var directories = new CLI.Argument()
				{
					Names = new string[] { "directories", "dir" },
					Description = "If set, the expected values for input and output arguments are valid directories.",
					ShortNames = new char[] { 'd' },
					ValueExpected = false
				};
				var recursive = new CLI.Argument()
				{
					Names = new string[] { "recursive" },
					ShortNames = new char[] { 'R' },
					Description = "Used along with the directories argument to indicate that subdirectories must be browsed.",
					ValueExpected = false
				};
				var width = new CLI.Argument()
				{
					Names = new string[] { "width" },
					Description = "Final image width.",
					ShortNames = new char[] { 'w' }
				};
				var height = new CLI.Argument()
				{
					Names = new string[] { "height" },
					Description = "Final image height.",
					ShortNames = new char[] { 'H' }
				};
				var iterations = new CLI.Argument()
				{
					Names = new string[] { "iterations" },
					Description = "Iterations count (total number of bars).",
					ShortNames = new char[] { 'I' }
				};
				var barWidth = new CLI.Argument()
				{
					Names = new string[] { "barwidth", "barWidth", "bw" },
					Description = "Width at which each bar will be resized.",
					ShortNames = new char[] { 'b' }
				};
				#endregion

				CLI cli = new CLI();
				cli.ProgramDescription =
@"MovieBarCode Generation, Command Line Interface.

Generate MovieBarCode (concatenated movie frames, in one image).

You can provide one input file, or a full directory (-d),
along with an output file or directory.

Version: " + Application.ProductVersion +
@"
Melvyn Laily. 2011.";

				cli.Arguments = new List<CLI.Argument>() 
				{
					help, input, output, directories, recursive, width, height, iterations, barWidth
				};

				var parsingResult = cli.ParseCommandLine();

				if (parsingResult.Count == 0 || (parsingResult.Count == 1 && parsingResult.ContainsKey(help)))
				{
					Console.Write(cli.GetFullHelpPage());
					return;
				}

				if (parsingResult.ContainsKey(directories))
				{
					//batch

				}
				else
				{
					//simple
					string inputPath;
					string outputPath;
					int widthValue;
					int heightValue;
					int iterationsValue;
					int barWidthValue;

					#region "Parsing and validation"
					try
					{
						inputPath = parsingResult[input];
						if (!System.IO.File.Exists(inputPath))
						{
							throw new Exception("Input file must exists.");
						}
						if (System.IO.Path.GetExtension(inputPath).ToLowerInvariant() != ".avi" && System.IO.Path.GetExtension(inputPath) != ".wmv")
						{
							Console.WriteLine("Warning! Avi or Wmv file expected!");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						return;
					}
					try
					{
						if (parsingResult.ContainsKey(output))
						{
							outputPath = parsingResult[output];
							if (System.IO.File.Exists(outputPath))
							{
								Console.WriteLine("Output file already exists: \"{0}\", will be overwritten.", outputPath);
							}
							//test if path is valid
							try
							{
								System.IO.FileStream fs = new System.IO.FileStream(outputPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
								fs.Dispose();
							}
							catch (Exception ex)
							{
								throw new Exception("Invalid output path specified.", ex);
							}
						}
						else
						{
							//default
							outputPath = System.IO.Path.GetFileNameWithoutExtension(inputPath) + ".png";
							Console.WriteLine("Default value for outputPath: \"" + outputPath + "\"");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						return;
					}

					try
					{
						widthValue = int.Parse(parsingResult[width]);
					}
					catch (Exception ex)
					{
						//Console.WriteLine(ex.ToString());
						widthValue = 1000;
						Console.WriteLine("Default value for width: " + widthValue);
					}

					try
					{
						heightValue = int.Parse(parsingResult[height]);
					}
					catch (Exception ex)
					{
						//Console.WriteLine(ex.ToString());
						heightValue = 500;
						Console.WriteLine("Default value for height: " + heightValue);
					}

					try
					{
						iterationsValue = int.Parse(parsingResult[iterations]);
					}
					catch (Exception ex)
					{
						//Console.WriteLine(ex.ToString());
						iterationsValue = 1000;
						Console.WriteLine("Default value for iterations: " + iterationsValue);
					}

					try
					{
						barWidthValue = int.Parse(parsingResult[barWidth]);
					}
					catch (Exception ex)
					{
						//Console.WriteLine(ex.ToString());
						barWidthValue = 1;
						Console.WriteLine("Default value for barWidth: " + barWidthValue);
					}
					#endregion

					ParallelGeneration generationObject = new ParallelGeneration(inputPath, outputPath, widthValue, heightValue, iterationsValue, barWidthValue);
					generationObject.GenerationComplete += (o2, e2) => Console.WriteLine();
					generationObject.ProgressChanged += (o3, e3) => WriteCLIPercentage(e3.Percentage);
					new System.Threading.Thread(() => generationObject.GenerateMovieBarCode()).Start();
				}

			}
		}

		[DllImport("kernel32.dll")]
		static extern bool AttachConsole(int dwProcessId = ATTACH_PARENT_PROCESS);
		private const int ATTACH_PARENT_PROCESS = -1;

		public static void WriteCLIPercentage(int percent)
		{
			percent = (int)Math.Max(0, Math.Min(Math.Floor(percent / 2.0), 50));
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			for (int i = 1; i <= percent; i++)
			{
				sb.Append(".");
			}
			for (int i = percent; i < 50; i++)
			{
				sb.Append(" ");
			}
			sb.Append("]");

			Console.Write("\r{0}", sb.ToString());
		}
	}
}
