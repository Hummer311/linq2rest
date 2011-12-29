// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
// All other rights reserved.

namespace Linq2Rest.Tests
{
	using System;

	[Flags]
	public enum Choice
	{
		This = 1,
		That = 2,
		Either = This | That
	}
}