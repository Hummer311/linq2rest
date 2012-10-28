﻿using System.Linq;

namespace Linq2Rest.Tests.Fakes.ComplexDomain
{
	/// <summary>
	/// A property instance data 
	/// </summary>

	public class PropertyInstanceData
	{
		#region Properties

		/// <summary>
		/// The property definition name that this instance belongs to
		/// </summary>

		public string DefinitionName { get; set; }

		/// <summary>
		/// The values for this instance
		/// </summary>

		public PropertyInstanceValueData[] Values { get; set; }

		#endregion

		#region Equals

		/// <summary>
		/// Overriden. Determines whether the specified Object is equal to the current object.
		/// </summary>
		/// <param name="other">The Object to compare with the current object.</param>
		/// <returns>Returns true if the specified object is equal to the current object; otherwise false.</returns>
		public override bool Equals(object other)
		{
			return Equals(other as PropertyInstanceData);
		}

		/// <summary>
		/// Determines whether the specified PropertyInstanceData is equal to the current PropertyInstanceData.
		/// </summary>
		/// <param name="that">The PropertyInstanceData to compare with the current object.</param>
		/// <returns>Returns true if the specified object has the same DefinitionName and the same values 
		/// (using the PropertyInstanceValueData Equals method) as the current PropertyInstanceValueData; 
		/// otherwise false.</returns>
		public virtual bool Equals(PropertyInstanceData that)
		{
			// if data is null - not equal
			if (object.ReferenceEquals(that, null))
			{
				return false;
			}

			// if data is has same reference to me - equal
			if (object.ReferenceEquals(this, that))
			{
				return true;
			}

			// otherwise do comparison logic here

			// First check it's the same definition name
			if (!this.DefinitionName.Equals(that.DefinitionName))
			{
				return false;
			}

			// Second, check it's the same number of values
			if (this.Values.Length != that.Values.Length)
			{
				return false;
			}

			// Last check it's the same exact values  
			return (this.Values.Except(that.Values).Count() == 0);

		}

		/// <summary>
		/// Returns the hash code for this PropertyInstanceData
		/// </summary>
		/// <returns>the hash code</returns>
		public override int GetHashCode()
		{
			return DefinitionName.GetHashCode();
		}

		#endregion
	}
}