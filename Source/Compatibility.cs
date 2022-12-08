
using RimWorld;
using System.Runtime.InteropServices;
using Verse;

namespace RimFridge
{
	public static class Compatibility
	{
		public static string IdentifierForPatch (PatchOperation patch, ModContentPack of)
		{
			var path = patch.sourceFile.Substring(of.RootDir.Length + 1);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				path = path.Replace('\\', '/');
			}

			return $"{of.PackageId} | {path}";
		}

		public static bool IsRecognisedRimFridgeModName (string name)
		{
			return name == "[KV] RimFridge" || name == "RimFridge Updated";
		}

		internal static int FindIndexOfModInActiveModLoadOrder (Mod mod)
		{
			return LoadedModManager.RunningModsListForReading.IndexOf(mod.Content);
		}
	}
}

