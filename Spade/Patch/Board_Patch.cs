using HarmonyLib;
using Spade.LifeCycle;

namespace Spade.Patch
{
	[HarmonyPatch(typeof(Board))]
	public class Board_Patch
	{
		[HarmonyPrefix]
		[HarmonyPatch("NewZombieUpdate")]
		public static bool Stop()
		{
			return Config.newZombieUpdateOn.Value;
		}
	}
}