using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieBarCode
{
	/// <summary>
	/// commans line interface
	/// </summary>
	class CLI
	{

		public string CommandLine { get; set; }

		public List<Argument> Arguments { get; set; }

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
				System.Text.RegularExpressions.Match m = item.TokenParsing.Match(this.CommandLine);
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

			public bool ValueExpected { get; set; }

			/// <summary>
			/// match the argument name.
			/// {0}: name,
			/// {1}: shortName
			/// </summary>
			private const string TOKEN_PARSING_PATTERN = @"(\s|^)((-{{1,2}}|/){0}|(-|/){1})(\s+|$)";
			private const string END_PARSING_PATTERN_VALUE_EXPECTED = @"((?<value>[^""\s]+)|""(?<value>[^""]*)"")";

			public System.Text.RegularExpressions.Regex TokenParsing
			{
				get
				{
					return new System.Text.RegularExpressions.Regex(string.Format(TOKEN_PARSING_PATTERN, GetPatternForArray(Names), GetPatternForArray(ShortNames)) + (ValueExpected ? END_PARSING_PATTERN_VALUE_EXPECTED : string.Empty));
				}
			}

			public Argument()
			{
				ValueExpected = true;
			}

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
					sb.Append(System.Text.RegularExpressions.Regex.Escape((array[i].ToString())));
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
