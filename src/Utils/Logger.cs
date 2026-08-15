using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable UnusedMethodReturnValue.Global

namespace NotEnoughAccuracy.Utils;

public class LoggerContext(string indentUnit)
{
	public int IndentLevel
	{
		get;
		set
		{
			if (field != value)
				Indent = string.Join("", Enumerable.Repeat(indentUnit, value));
			field = value;
		}
	}

	public string Indent { get; private set; } = "";
}

public class Logger(LoggerContext context, Action<string> logFunc)
{
	private XmlCloser Closer => field ??= new XmlCloser(this);

	private List<string> TagStack { get; } = [];

	private unit Print(string message)
	{
		logFunc(context.Indent + message);
		return [];
	}

	public unit Log(string message)
	{
		Print(message);
		return [];
	}

	public IDisposable Begin(string tag)
	{
		Print($"<{tag}>");
		TagStack.Add(tag);
		context.IndentLevel++;
		return Closer;
	}

	private unit End()
	{
		var tag = TagStack[^1];
		TagStack.RemoveAt(TagStack.Count - 1);
		context.IndentLevel--;
		Print($"</{tag}>");
		return [];
	}

	private class XmlCloser(Logger logger) : IDisposable
	{
		public void Dispose()
		{
			logger.End();
		}
	}
}
