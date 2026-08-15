using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ADOFAIMod.BuildTasks;

[UsedImplicitly]
public sealed class ResolveAdofaiExecutable : Task
{
	[Output] public string ExecutablePath { [UsedImplicitly] get; set; } = string.Empty;

	public override bool Execute()
	{
		var environmentPath = Environment.GetEnvironmentVariable("ADOFAI_EXE");
		if (!string.IsNullOrWhiteSpace(environmentPath))
		{
			ExecutablePath = NormalizePath(environmentPath);
			return true;
		}

		var steamRoots = new List<string>();
		var registryLocations = new[]
		{
			@"HKEY_CURRENT_USER\Software\Valve\Steam",
			@"HKEY_CURRENT_USER\Software\WOW6432Node\Valve\Steam",
			@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam",
			@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam"
		};

		foreach (var registryLocation in registryLocations)
		foreach (var valueName in new[] { "SteamPath", "InstallPath" })
		{
			var value = ReadRegistryValue(registryLocation, valueName);
			if (string.IsNullOrWhiteSpace(value)) continue;
			var normalized = NormalizePath(value);
			if (!steamRoots.Contains(normalized, StringComparer.OrdinalIgnoreCase)) steamRoots.Add(normalized);
		}

		var libraries = new List<string>();
		foreach (var steamRoot in steamRoots)
		{
			AddUnique(libraries, steamRoot);

			var libraryFoldersFile = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
			if (!File.Exists(libraryFoldersFile)) continue;

			string[] libraryFolderLines;
			try
			{
				libraryFolderLines = File.ReadAllLines(libraryFoldersFile);
			}
			catch
			{
				continue;
			}

			foreach (var line in libraryFolderLines)
			{
				var match = Regex.Match(line, """
				                              ^\s*"path"\s+"(?<path>(?:\\\\.|[^"])*)"
				                              """);
				if (!match.Success) continue;

				var libraryPath = match.Groups["path"].Value
					.Replace(@"\\", "\\")
					.Replace("\\\"", "\"");
				AddUnique(libraries, NormalizePath(libraryPath));
			}
		}

		foreach (var library in libraries)
			if (TryGamePath(library, out var executablePath))
			{
				ExecutablePath = executablePath;
				return true;
			}

		ExecutablePath = string.Empty;
		return true;
	}

	private static string NormalizePath(string path)
	{
		path = path.Trim();
		if (path is ['"', .. var stripped, '"']) path = stripped;
		return path.Replace('/', Path.DirectorySeparatorChar);
	}

	private static void AddUnique(ICollection<string> paths, string path)
	{
		if (!paths.Contains(path, StringComparer.OrdinalIgnoreCase)) paths.Add(path);
	}

	private static bool TryGamePath(string libraryPath, out string executablePath)
	{
		try
		{
			const string relativeDirectory = @"steamapps\common\A Dance of Fire and Ice";
			var gameDirectory = Path.Combine(libraryPath, relativeDirectory);
			var withExtension = Path.Combine(gameDirectory, "A Dance of Fire and Ice.exe");
			if (File.Exists(withExtension))
			{
				executablePath = Path.GetFullPath(withExtension);
				return true;
			}

			var withoutExtension = Path.Combine(gameDirectory, "A Dance of Fire and Ice");
			if (File.Exists(withoutExtension))
			{
				executablePath = Path.GetFullPath(withoutExtension);
				return true;
			}
		}
		catch
		{
			// ignored
		}

		executablePath = string.Empty;
		return false;
	}

	private static string? ReadRegistryValue(string key, string valueName)
	{
		try
		{
			var registryAssembly = AppDomain.CurrentDomain.GetAssemblies()
				.FirstOrDefault(assembly => assembly.GetName().Name == "Microsoft.Win32.Registry");
			if (registryAssembly == null)
				try
				{
					registryAssembly = Assembly.Load("Microsoft.Win32.Registry");
				}
				catch
				{
					//ignored
				}

			var registryType = registryAssembly?.GetType("Microsoft.Win32.Registry")
			                   ?? Type.GetType("Microsoft.Win32.Registry");
			var getValue = registryType?.GetMethod(
				"GetValue",
				[typeof(string), typeof(string), typeof(object)]);
			return getValue?.Invoke(null, [key, valueName, null]) as string;
		}
		catch
		{
			return null;
		}
	}
}
