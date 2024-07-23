using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace Spade.LifeCycle
{
	public class Config : HaveLifeCycle
	{
		private static ConfigEntry<bool>  isDeveloperMode;
		private static ConfigEntry<bool>  isNoCD;
		public static  ConfigEntry<int>   zombieMultiplier;
		public static  ConfigEntry<bool>  newZombieUpdateOn;
		public static  ConfigEntry<float> intervalMultiplier;
		public static  ConfigEntry<bool>  showWindow;

		public static List<Plant> plants = new List<Plant>();

		public static float previousIntervalMultiplier = 1f;

		public override void Start()
		{
			isDeveloperMode = HelperMain.instance.Config.Bind("base", "开发者模式", false, "包括解锁植物、关卡、无限阳光等");
			isNoCD = HelperMain.instance.Config.Bind("base", "无冷却模式", false, "植物不受冷却的限制");
			zombieMultiplier = HelperMain.instance.Config.Bind("base", "僵尸生成倍率", 1, "僵尸生成量相比于原本的倍数");
			newZombieUpdateOn = HelperMain.instance.Config.Bind("base", "是否自然生成僵尸", true, "关闭则停止生成僵尸");
			intervalMultiplier = HelperMain.instance.Config.Bind("base", "植物攻速倍数", 1f);
			intervalMultiplier.Value = (float)intervalMultiplier.DefaultValue;
			showWindow = HelperMain.instance.Config.Bind("base", "僵尸生成器", false, "自定义生成僵尸。生成路线为0-4时在对应路线生成，为-1时在全部路线生成，其余值则随机生成");

			isDeveloperMode.SettingChanged += (sender, args) => { GameAPP.developerMode = isDeveloperMode.Value; };
			isNoCD.SettingChanged          += (sender, args) => { GameAPP.noCD          = isNoCD.Value; };

			intervalMultiplier.SettingChanged += (sender, e) =>
			{
				if (intervalMultiplier.Value == 0)
				{
					intervalMultiplier.Value = 1;
					return;
				}
				foreach (Plant plant in plants)
				{
					HelperMain.Log.LogInfo("previous = " + plant.thePlantAttackInterval);
					plant.thePlantAttackInterval *= previousIntervalMultiplier;
					plant.thePlantAttackInterval /= intervalMultiplier.Value;
					HelperMain.Log.LogInfo("after = " + plant.thePlantAttackInterval);
				}
				previousIntervalMultiplier = intervalMultiplier.Value;
			};
			GameAPP.developerMode = isDeveloperMode.Value;
			GameAPP.noCD          = isNoCD.Value;
		}
	}
}