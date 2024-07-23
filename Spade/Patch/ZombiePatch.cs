using HarmonyLib;

namespace Spade.Patch
{
	[HarmonyPatch(typeof(Zombie), "TakeDamage")]
	public class Zombie_TakeDamagePatch
	{
		[HarmonyPrefix]
		public static bool DieWhenGetHit(Zombie __instance)
		{
			__instance.Die();
			return true;
		}
	}
}