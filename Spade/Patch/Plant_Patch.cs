using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static Spade.LifeCycle.Config;

namespace Spade.Patch
{
	[HarmonyPatch(typeof(Plant))]
	public class Plant_Patch
	{
		private static readonly int         Shoot  = Animator.StringToHash("shoot");
		public static readonly  List<Plant> plants = new List<Plant>();

		[HarmonyPrefix]
		[HarmonyPatch("PlantShootUpdate")]
		public static bool PlantShootUpdate_Rewrite(Plant __instance)
		{
			if (plantIntervalMultiplier.Value == 0) return false;
			__instance.thePlantAttackCountDown -= Time.deltaTime;
			if (__instance.thePlantAttackCountDown < 0f)
			{
				__instance.thePlantAttackCountDown = __instance.thePlantAttackInterval / plantIntervalMultiplier.Value *
				                                     Random.Range(0.9f, 1.1f);

				Traverse traverse = Traverse.Create(__instance);

				Animator animator = traverse.Field("anim").GetValue<Animator>();
				animator.speed = plantIntervalMultiplier.Value;
				if (traverse.Method("SearchZombie").GetValue() != null)
				{
					animator.SetTrigger(Shoot);
					return false;
				}
				if (__instance.board.isScaredyDream && __instance.thePlantType == 9)
					animator.SetTrigger(Shoot);
			}
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("PlantUpdate")]
		public static bool NeverDie(Plant __instance)
		{
			if (plantNeverDie.Value) __instance.thePlantHealth = __instance.thePlantMaxHealth;
			return true;
		}

		[HarmonyPatch("Awake")]
		[HarmonyPrefix]
		public static bool AddNewPlantOnAwake(Plant __instance)
		{
			plants.Add(__instance);
			__instance.thePlantMaxHealth = (int)(__instance.thePlantMaxHealth * plantHealthMultiplier.Value);
			__instance.thePlantHealth    = (int)(__instance.thePlantHealth * plantHealthMultiplier.Value);
			return true;
		}

		[HarmonyPatch("Die")]
		[HarmonyPostfix]
		public static void RemoveNewPlantOnDestroy(Plant __instance)
		{
			plants.Remove(__instance);
		}
	}
}