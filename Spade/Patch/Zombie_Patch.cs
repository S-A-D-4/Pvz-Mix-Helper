using System.Collections.Generic;
using HarmonyLib;
using Spade.LifeCycle;
using UnityEngine;
using static Spade.LifeCycle.Config;

namespace Spade.Patch
{
	[HarmonyPatch(typeof(Zombie))]
	public class Zombie_Patch : HaveLifeCycle
	{
		public static readonly List<(Zombie, Transform)> zombie_Shadow = new List<(Zombie, Transform)>();

		[HarmonyPrefix]
		[HarmonyPatch("Awake")]
		public static bool SetMaxHealth(Zombie __instance)
		{
			zombie_Shadow.Add((__instance, __instance.transform.Find("Shadow")));
			__instance.theMaxHealth =  (int)(__instance.theMaxHealth * zombieHealthMultiplier.Value);
			__instance.theHealth    *= zombieHealthMultiplier.Value;
			if (zombieHealthArmorMultiplier.Value)
			{
				__instance.theFirstArmorHealth  = (int)(__instance.theFirstArmorHealth * zombieHealthMultiplier.Value);
				__instance.theSecondArmorHealth = (int)(__instance.theSecondArmorHealth * zombieHealthMultiplier.Value);
			}
			return true;
		}

		[HarmonyPostfix]
		[HarmonyPatch("Start")]
		public static void SetSpeed(Zombie __instance)
		{
			__instance.theOriginSpeed *= zombieSpeedMultiplier.Value;
			__instance.theSpeed       *= zombieSpeedMultiplier.Value;
		}

		[HarmonyPrefix]
		[HarmonyPatch("TakeDamage")]
		public static bool TakeDamage_Prefix(Zombie __instance)
		{
			if (dieOnHit.Value)
			{
				__instance.theFirstArmorHealth  = -1;
				__instance.theSecondArmorHealth = -1;
				__instance.theHealth            = -1f;
			}
			return !zombieNeverDie.Value;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Die")]
		public static bool RealZombie(int reason)
		{
			if (reason == 0) return !realZombie.Value;
			return true;
		}
	}
}