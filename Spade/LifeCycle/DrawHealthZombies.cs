using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using static Spade.Patch.Zombie_Patch;

namespace Spade.LifeCycle
{
	[HarmonyPatch(typeof(Zombie))]
	public class DrawHealthZombies : HaveLifeCycle
	{
		private       GUIStyle          guiStyle;
		private       Camera            mainCamera;
		public static DrawHealthZombies Instance { get; private set; }

		public override void Awake()
		{
			Instance = this;
			// 初始化GUIStyle
			guiStyle = new GUIStyle
			{ fontSize = 25, // 设置字体大小
			  normal =
			  { textColor = Color.red // 设置字体颜色
			  },
			  fontStyle = FontStyle.Bold };
		}

		public override void Start()
		{
			mainCamera = Camera.main;
		}

		[HarmonyPatch("Die")]
		[HarmonyPostfix]
		public static void RemoveNewZombieOnDestroy(Zombie __instance)
		{
			zombie_Shadow.Remove((__instance, __instance.transform.Find("Shadow")));
		}

		public override void OnGUI()
		{
			if (!Config.showZombieHealth.Value) return;
			foreach ((Zombie zombie, Transform shadow) in zombie_Shadow)
				try
				{
					Vector3 worldZombie    = shadow.transform.position;
					Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldZombie);
					screenPosition.y = Screen.height - screenPosition.y;
					int offsetY = -40;

					int zombieTheSecondArmorHealth = zombie.theSecondArmorHealth;
					if (zombieTheSecondArmorHealth != 0)
					{
						offsetY += 30;
						Rect labelRect = new Rect(screenPosition.x - 10, screenPosition.y, 100, 30);
						GUI.Label(labelRect, "防具2：" + zombieTheSecondArmorHealth, guiStyle);
					}

					int zombieTheFirstArmorHealth = zombie.theFirstArmorHealth;
					if (zombieTheFirstArmorHealth != 0)
					{
						offsetY += 30;
						Rect labelRectFirstArmor = new Rect(screenPosition.x - 10, screenPosition.y + offsetY, 100, 30);
						GUI.Label(labelRectFirstArmor, "防具1：" + zombieTheFirstArmorHealth, guiStyle);
					}

					offsetY += 30;
					Rect labelRectHealth = new Rect(screenPosition.x - 10, screenPosition.y + offsetY, 100, 30);
					GUI.Label(labelRectHealth, "血量：" + (int)zombie.theHealth, guiStyle);
				}
				catch (Exception)
				{
					zombie_Shadow.Clear();
					break;
				}
		}

		public static void Reset()
		{
			zombie_Shadow.Clear();
			zombie_Shadow.AddRange(GameObject.FindGameObjectsWithTag("Zombie")
				.Select(o => (o.GetComponent<Zombie>(), o.transform.Find("Shadow"))));
		}
	}
}