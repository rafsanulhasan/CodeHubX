using HtmlAgilityPack;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CodeHubX.Helpers
{
	/// <summary>
	/// A collection of various extension methods
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Casts the given object into the Type in input via a direct cast and returns it
		/// </summary>
		/// <typeparam name="T">The target Type</typeparam>
		/// <param name="target">The object to cast into the given Type</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T To<T>(this object target)
			=> (T) target;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Abs(this double value) => value >= 0 ? value : -value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsWithDelta(this double value, double comparison, double delta = 0.1) => (value - comparison).Abs() < delta;

		/// <summary>
		/// Forgets a task without any warnings
		/// </summary>
		/// <param name="task">The task to forget</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Forget([NotNull] this Task task) { }

		/// <summary>
		/// Forgets a task that returns a value
		/// </summary>
		/// <param name="task">The task to forget</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Forget<T>([NotNull] this Task<T> task) { }

		/// <summary>
		/// Counts the number of items in an IEnumerable sequence
		/// </summary>
		/// <param name="enumerable">The sequence to count</param>
		public static int Count([NotNull] this IEnumerable enumerable)
			=> Enumerable.Count(enumerable.Cast<object>());

		/// <summary>
		/// Waits for a task with the given token
		/// </summary>
		/// <typeparam name="T">The type returned by the task</typeparam>
		/// <param name="task">The task to wait</param>
		/// <param name="token">The cancellation token</param>
		/// <param name="failsafe">If true, all possible exceptions will be handled too</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task<T> AsCancellableTask<T>([NotNull] this Task<T> task,
		    CancellationToken token, bool failsafe = false) where T : class
		{
			try
			{
				return await task.ContinueWith(t => t.GetAwaiter().GetResult());
			}
			catch (OperationCanceledException) { return null; }
			catch when (failsafe) { return null; }
		}

		/// <summary>
		/// Gets a sequence of sibling nodes from the input node
		/// </summary>
		/// <param name="node">The source node</param>
		public static IEnumerable<HtmlNode> Siblings([NotNull] this HtmlNode node)
		{
			while (node != null)
			{
				yield return node.NextSibling;
				node = node.NextSibling;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="tag"></param>
		/// <param name="attribute"></param>
		/// <param name="value"></param>
		public static IEnumerable<HtmlNode> DescendantsWithAttribute([NotNull] this HtmlNode node,
		    [NotNull] string tag, [NotNull] string attribute, [NotNull] string value)
			=> node
				.Descendants(tag)?
				.Where(child => child.Attributes?
				.AttributesWithName(attribute)?
				.FirstOrDefault()?.Value?.Equals(value) == true);

		public static Color GetLight(this Color color, double delta)
		{
			var R = (255 - color.R) * delta + color.R;
			var G = (255 - color.G) * delta + color.G;
			var B = (255 - color.B) * delta + color.B;
			byte normalize(double d)
			{
				if (d < 0)
					return 0;

				return d <= 255 ? (byte) d : (byte) 255;
			}

			return Color.FromArgb(color.A, normalize(R), normalize(G), normalize(B));
		}
	}
}
