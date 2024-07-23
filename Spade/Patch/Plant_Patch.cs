using HarmonyLib;
using Spade.LifeCycle;

namespace Spade.Patch
{
	[HarmonyPatch(typeof(Plant))]
	public class Plant_Patch
	{
		[HarmonyPrefix]
		[HarmonyPatch("Awake")]
		public static void AddToList(Plant __instance)
		{
			HelperMain.Log.LogInfo(__instance.thePlantAttackInterval);
			__instance.thePlantAttackInterval /= Config.intervalMultiplier.Value;
			HelperMain.Log.LogInfo(__instance.thePlantAttackInterval);

			Config.plants.Add(__instance);
		}

		[HarmonyPatch("Die")]
		[HarmonyPrefix]
		public static void RemoveFromList(Plant __instance)
		{
			Config.plants.Remove(__instance);
		}
	}
}