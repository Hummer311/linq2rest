﻿// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

namespace Linq2Rest.Parser
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;

	internal static class ExpressionTokenizer
	{
		public static IEnumerable<TokenSet> GetTokens(this string expression)
		{
			var cleanMatch = expression.EnclosedMatch();

			if (cleanMatch.Success)
			{
				var match = cleanMatch.Groups[1].Value;
				if (!HasOrphanedOpenParenthesis(match))
				{
					expression = match;
				}
			}

			if (expression.IsImpliedBoolean())
			{
				yield break;
			}

			var blocks = expression.Split(new[] { ' ' });

			var openGroups = 0;
			var startExpression = 0;
			var currentTokens = new TokenSet();

			var processingString = false;

			for (int i = 0; i < blocks.Length; i++)
			{
				if (blocks[i].IsStringStart()) 
				{ 
					processingString = true;
				}

				var netEnclosed = blocks[i].Count(c => c == '(') - blocks[i].Count(c => c == ')');
				openGroups += netEnclosed;

				if (openGroups == 0)
				{
					if (!processingString && blocks[i].IsOperation())
					{
						var expression1 = startExpression;
						Func<string, int, bool> leftPredicate = (x, j) => j >= expression1 && j < i && (processingString || x != "");

						if (string.IsNullOrWhiteSpace(currentTokens.Left))
						{
							currentTokens.Left = string.Join(" ", blocks.Where(leftPredicate));
							currentTokens.Operation = blocks[i];
							startExpression = i + 1;

							if (blocks[i].IsCombinationOperation())
							{
								currentTokens.Right = string.Join(" ", blocks.Where((x, j) => j > i));

								yield return currentTokens;
								yield break;
							}
						}
						else 
						{
							Func<string, int, bool> rightPredicate = (x, j) => j >= expression1 && j < i;
							currentTokens.Right = string.Join(" ", blocks.Where(rightPredicate));

							yield return currentTokens;

							startExpression = i + 1;
							currentTokens = new TokenSet();

							if (blocks[i].IsCombinationOperation())
							{
								yield return new TokenSet { Operation = blocks[i].ToLowerInvariant() };
							}
						}
					}
				}

				if (blocks[i].IsStringEnd()) {
					processingString = false;
				}
			}

			var remainingToken = string.Join(" ", blocks.Where((x, j) => j >= startExpression));

			if (!string.IsNullOrWhiteSpace(currentTokens.Left))
			{
				currentTokens.Right = remainingToken;
				yield return currentTokens;
			}
			else if (remainingToken.IsEnclosed())
			{
				currentTokens.Left = remainingToken;
				yield return currentTokens;
			}
		}

		public static TokenSet GetArithmeticToken(this string expression)
		{
			Contract.Requires<ArgumentNullException>(expression != null);

			var cleanMatch = expression.EnclosedMatch();

			if (cleanMatch.Success)
			{
				var match = cleanMatch.Groups[1].Value;
				if (!HasOrphanedOpenParenthesis(match))
				{
					expression = match;
				}
			}

			var blocks = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var hasOperation = blocks.Any(x => x.IsArithmetic());
			if (!hasOperation)
			{
				return null;
			}

			var operationIndex = GetArithmeticOperationIndex(blocks);

			var left = string.Join(" ", blocks.Where((x, i) => i < operationIndex));
			var right = string.Join(" ", blocks.Where((x, i) => i > operationIndex));
			var operation = blocks[operationIndex];

			return new TokenSet { Left = left, Operation = operation, Right = right };
		}

		private static int GetArithmeticOperationIndex(IList<string> blocks)
		{
			Contract.Requires(blocks != null);

			var openGroups = 0;
			var operationIndex = -1;
			for (var i = 0; i < blocks.Count; i++)
			{
				var source = blocks[i];

				Contract.Assume(source != null, "Does not generate null token parts.");

				var netEnclosed = source.Count(c => c == '(') - source.Count(c => c == ')');
				openGroups += netEnclosed;

				if (openGroups == 0 && source.IsArithmetic())
				{
					operationIndex = i;
				}
			}

			return operationIndex;
		}

		private static bool HasOrphanedOpenParenthesis(string expression)
		{
			Contract.Requires(expression != null);

			var opens = new List<int>();
			var closes = new List<int>();
			var index = expression.IndexOf('(');
			while (index > -1)
			{
				opens.Add(index);
				index = expression.IndexOf('(', index + 1);
			}

			index = expression.IndexOf(')');
			while (index > -1)
			{
				closes.Add(index);
				index = expression.IndexOf(')', index + 1);
			}

			var pairs = opens.Zip(closes, (o, c) => new Tuple<int, int>(o, c));
			var hasOrphan = opens.Count == closes.Count && pairs.Any(x => x.Item2 < x.Item1);

			return hasOrphan;
		}
	}
}
