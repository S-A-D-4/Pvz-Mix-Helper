using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using static Spade.Patch.Plant_Patch;

namespace Spade.LifeCycle
{
	[HarmonyPatch(typeof(Plant))]
	public class DrawHealthPlants : HaveLifeCycle
	{
		private       GUIStyle         guiStyle;
		private       Camera           mainCamera;
		public static DrawHealthPlants Instance { get; private set; }

		public override void Awake()
		{
			Instance = this;
			// 初始化GUIStyle
			guiStyle = new GUIStyle
			{ fontSize = 25, // 设置字体大小
			  normal =
			  { textColor = Color.green // 设置字体颜色
			  },
			  fontStyle = FontStyle.Bold,
			  alignment = TextAnchor.MiddleCenter // 设置字体样式
			};
		}

		public override void Start()
		{
			mainCamera = Camera.main;
		}

		public override void OnGUI()
		{
			if (!Config.showPlantHealth.Value) return;
			foreach (Plant plant in plants)
				try
				{
					Vector3 worldPosition  = plant.transform.position;
					Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
					screenPosition.y = Screen.height - screenPosition.y;
					Rect labelRect = new Rect(screenPosition.x - 10, screenPosition.y, 100, 30);
					GUI.Label(labelRect, "血量：" + plant.thePlantHealth, guiStyle);
				}
				catch (Exception)
				{
					plants.Clear();
					break;
				}
		}

		public static void Reset()
		{
			plants.Clear();
			plants.AddRange(GameObject.FindGameObjectsWithTag("Plant").Select(o => o.GetComponent<Plant>()));
		}
	}
}