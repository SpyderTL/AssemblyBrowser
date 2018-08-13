using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowser
{
	internal static class Extensions
	{
		public static IEnumerable<TResult> Select<TResult>(this IEnumerable source, Func<object, TResult> selector)
		{
			foreach (var item in source)
				yield return selector(item);
		}
	}
}
