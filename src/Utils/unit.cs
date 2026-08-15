using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NotEnoughAccuracy.Utils;

// ReSharper disable once InconsistentNaming
[CollectionBuilder(typeof(unit), "Create")]
public struct unit : IEnumerable<object>
{
	private static Enumerator SingletonEnumerator { get; } = new();

	public static unit Create(ReadOnlySpan<object> _)
	{
		return default;
	}

	public IEnumerator<object> GetEnumerator()
	{
		return SingletonEnumerator;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return SingletonEnumerator;
	}

	private class Enumerator : IEnumerator<object>
	{
		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return false;
		}

		public void Reset()
		{
		}

		public object Current => throw new NotImplementedException();

		object IEnumerator.Current => throw new NotImplementedException();
	}
}
