using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MovieBarCode
{
	/// <summary>
	/// commans line interface
	/// </summary>
	class CLI
	{

		public string CommandLine { get; set; }

		public List<Argument> Arguments { get; set; }

		/// <summary>
		/// if false, all args are parsed with case insensitive option.
		/// default to true.
		/// </summary>
		public bool CaseSensitiveArgs { get; set; }

		/// <summary>
		/// used when displaying the full help.
		/// </summary>
		public string ProgramDescription { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="commandLine">if null, commandLine is grabbed from Environment.CommandLine</param>
		public CLI(string commandLine = null)
		{
			if (commandLine == null)
			{
				this.CommandLine = Environment.CommandLine;
			}
			else
			{
				this.CommandLine = commandLine;
			}
			this.CaseSensitiveArgs = true;
		}

		/// <summary>
		/// return a dictionary of arguments, associated with the parsed values
		/// </summary>
		/// <returns></returns>
		public Dictionary<Argument, string> ParseCommandLine()
		{
			if (this.CommandLine == null)
			{
				throw new NullReferenceException("CommandLine can't be null");
			}
			Dictionary<Argument, string> final = new Dictionary<Argument, string>();
			foreach (var item in this.Arguments)
			{
				if (item == null)
				{
					throw new NullReferenceException("Argument can't be null");
				}
				Match m = item.GetTokenParsing(this.CaseSensitiveArgs).Match(this.CommandLine);
				if (m.Success)
				{
					if (item.ValueExpected)
					{
						final.Add(item, m.Groups["value"].Value);
					}
					else
					{
						final.Add(item, null);
					}
				}
			}
			return final;
		}

		/// <summary>
		/// generate help page using arguments descriptions.
		/// </summary>
		/// <returns></returns>
		public string GetArgumentsHelp()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var arg in this.Arguments)
			{
				if (arg == null)
				{
					throw new NullReferenceException("Argument can't be null");
				}
				sb.Append("- ");
				var concatenatedNames = arg.Names.Concat(from a in arg.ShortNames select a.ToString()).ToList();
				for (int i = 0; i < concatenatedNames.Count; i++)
				{
					sb.Append(concatenatedNames[i]);
					if (i == concatenatedNames.Count - 1)
					{
						//last
						continue;
					}
					sb.Append(", ");
				}
				sb.AppendLine();
				sb.Append("\t");
				sb.AppendLine(arg.Description);
				sb.AppendLine();
			}
			return sb.ToString();
		}

		/// <summary>
		/// return a default help page, including program description and arguments usage.
		/// </summary>
		/// <returns></returns>
		public string GetFullHelpPage()
		{
			return this.ProgramDescription + Environment.NewLine + "--------------------" + Environment.NewLine + Environment.NewLine + "Usage:" + Environment.NewLine + Environment.NewLine + GetArgumentsHelp();
		}

		/// <summary>
		/// handle parsing of arguments :
		/// /help or --help or -h or -? or /? or /h
		/// followed by any value. spaces are allowed in quotes
		/// </summary>
		public class Argument
		{
			/// <summary>
			/// double dash,
			/// help
			/// </summary>
			public string[] Names { get; set; }
			/// <summary>
			/// simple dash,
			/// h
			/// </summary>
			public char[] ShortNames { get; set; }

			/// <summary>
			/// default to true.
			/// </summary>
			public bool ValueExpected { get; set; }

			public string Description { get; set; }

			/// <summary>
			/// used to validate parsed value.
			/// must return true if valid.
			/// considered false if any exception is thrown.
			/// 
			/// Validation will be ignored if null.
			/// </summary>
			/// may be used to throw error sooner.
			/// not used yet.
			//public Func<object, bool> ValueValidation { get; set; }

			/// <summary>
			/// match the argument name.
			/// {0}: name,
			/// {1}: shortName
			/// </summary>
			/// <remarks>
			/// '/' is not recommanded, because if 2 different args with the same name and shortname exist, difference can't be made.
			/// </remarks>
			private const string TOKEN_PARSING_PATTERN = @"(\s|^)((--|/){0}|(-|/){1})(\s+|$)";
			/// <summary>
			/// parse values. spaces are allowed in quote.
			/// </summary>
			private const string END_PARSING_PATTERN_VALUE_EXPECTED = @"((?<value>[^""\s]+)|""(?<value>[^""]*)"")";

			public Argument()
			{
				ValueExpected = true;
			}

			public Regex GetTokenParsing(bool caseSensitive = true)
			{
				return new Regex(string.Format(TOKEN_PARSING_PATTERN, GetPatternForArray(Names), GetPatternForArray(ShortNames)) + (ValueExpected ? END_PARSING_PATTERN_VALUE_EXPECTED : string.Empty), caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
			}

			/// <summary>
			/// return a regex alternation pattern with the values of the provided array.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="array"></param>
			/// <returns></returns>
			private string GetPatternForArray<T>(IList<T> array)
			{
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (array.Count == 0)
				{
					throw new ArgumentException("array must contain at least one element.");
				}
				if (array.Count == 1)
				{
					return array[0].ToString();
				}
				StringBuilder sb = new StringBuilder();
				sb.Append("(");
				for (int i = 0; i < array.Count; i++)
				{
					sb.Append(Regex.Escape((array[i].ToString())));
					if (i == array.Count - 1)
					{
						//last
						sb.Append(")");
					}
					else
					{
						sb.Append("|");
					}
				}
				return sb.ToString();
			}

		}
	}

}
