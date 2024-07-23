using HarmonyLib;
using Spade.LifeCycle;

namespace Spade.Patch
{
	[HarmonyPatch(typeof(Board), "NewZombieUpdate")]
	public class Board_ZombieUpdatePatch
	{
		[HarmonyPrefix]
		public static bool Stop()
		{
			return Config.newZombieUpdateOn.Value;
		}
	}
}