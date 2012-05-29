// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

namespace Linq2Rest.Provider.Writers
{
	using System;

	internal class DoubleValueWriter : RationalValueWriter
	{
		public override Type Handles
		{
			get
			{
				return typeof(double);
			}
		}
	}
}