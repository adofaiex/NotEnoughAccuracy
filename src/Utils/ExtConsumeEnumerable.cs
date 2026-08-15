using System.Collections.Generic;

namespace NotEnoughAccuracy.Utils;

public static class ExtConsumeEnumerable
{
	public static unit Consume<T>(this IEnumerable<T> enumerable)
	{
		foreach (var _ in enumerable) ;
		return [];
	}
}
