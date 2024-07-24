using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Spade.Patch;
using UnityEngine;

namespace Spade.LifeCycle
{
	public class Config : HaveLifeCycle
	{
		private static ConfigEntry<bool>  isDeveloperMode;
		private static ConfigEntry<bool>  isNoCD;
		public static  ConfigEntry<bool>  plantNeverDie;
		public static  ConfigEntry<float> plantIntervalMultiplier;
		public static  ConfigEntry<float> plantHealthMultiplier;
		public static  ConfigEntry<bool>  dieOnHit;
		public static  ConfigEntry<bool>  zombieNeverDie;
		public static  ConfigEntry<float> zombieNumMultiplier;
		public static  ConfigEntry<float> zombieHealthMultiplier;
		public static  ConfigEntry<bool>  zombieHealthArmorMultiplier;
		public static  ConfigEntry<float> zombieSpeedMultiplier;
		public static  ConfigEntry<bool>  realZombie;
		public static  ConfigEntry<bool>  newZombieUpdateOn;
		public static  ConfigEntry<bool>  showWindow;
		public static  ConfigEntry<bool>  invisiblePause;
		public static  ConfigEntry<bool>  showPlantHealth;
		public static  ConfigEntry<bool>  showZombieHealth;
		private        float              plantHealthMultiplierPrevious;
		private        float              zombieHealthMultiplierPrevious;

		private float zombieSpeedMultiplierPrevious;

		public override void Start()
		{
			isDeveloperMode             = HelperMain.instance.Config.Bind("1.沙盒", "开发者模式", false, "包括解锁植物、关卡、无限阳光等");
			isNoCD                      = HelperMain.instance.Config.Bind("1.沙盒", "无冷却模式", false, "植物不受冷却的限制");
			zombieNumMultiplier         = HelperMain.instance.Config.Bind("1.沙盒", "僵尸生成倍率", 1f, "僵尸生成量相比于原本的倍数。进入新的关卡时生效。");
			newZombieUpdateOn           = HelperMain.instance.Config.Bind("1.沙盒", "是否自然生成僵尸", true, "关闭则停止生成僵尸");
			showWindow                  = HelperMain.instance.Config.Bind("1.沙盒", "僵尸生成器", false, "自定义生成僵尸。生成路线为0-4时在对应路线生成，为-1时在全部路线生成，其余值则随机生成");
			plantNeverDie               = HelperMain.instance.Config.Bind("2.植物属性", "植物不死", false, "在游戏的每一帧，将植物设为满血");
			plantHealthMultiplier       = HelperMain.instance.Config.Bind("2.植物属性", "植物血量倍率", 1f, "植物血量相比于原本的倍数");
			plantIntervalMultiplier     = HelperMain.instance.Config.Bind("2.植物属性", "植物攻速倍率", 1f, "植物攻速相比于原本的倍数");
			zombieNeverDie              = HelperMain.instance.Config.Bind("3.僵尸属性", "僵尸无敌", false, "僵尸受到伤害时，不会死亡");
			zombieSpeedMultiplier       = HelperMain.instance.Config.Bind("3.僵尸属性", "僵尸速度倍率", 1f, "包含移动速度，动画速度，攻击速度等");
			zombieHealthMultiplier      = HelperMain.instance.Config.Bind("3.僵尸属性", "僵尸血量倍率", 1f, "僵尸血量相比于原本的倍数");
			zombieHealthArmorMultiplier = HelperMain.instance.Config.Bind("3.僵尸属性", "僵尸血量倍率会影响到防具", true, "僵尸的防具耐久是否受僵尸血量倍率的影响");
			realZombie                  = HelperMain.instance.Config.Bind("3.僵尸属性", "亡者行军", false, "你猜");
			dieOnHit                    = HelperMain.instance.Config.Bind("3.僵尸属性", "一碰就死", false, "僵尸在受到伤害时，血量和防具耐久设为-1");
			invisiblePause              = HelperMain.instance.Config.Bind("4.小开不算开", "使暂停不可见", false, "需要唤出暂停时请关闭此选项");
			showPlantHealth             = HelperMain.instance.Config.Bind("4.小开不算开", "植物血量显示", false);
			showZombieHealth            = HelperMain.instance.Config.Bind("4.小开不算开", "僵尸血量显示", false);

			isDeveloperMode.SettingChanged += (sender, args) => { GameAPP.developerMode = isDeveloperMode.Value; };
			isNoCD.SettingChanged          += (sender, args) => { GameAPP.noCD          = isNoCD.Value; };
			plantHealthMultiplier.SettingChanged += (sender, args) =>
			{
				if (plantHealthMultiplier.Value == 0)
				{
					plantHealthMultiplier.Value = 0.01f;
					return;
				}
				foreach (Plant plant in Plant_Patch.plants)
				{
					plant.thePlantHealth    = (int)(plant.thePlantHealth / plantHealthMultiplierPrevious * plantHealthMultiplier.Value);
					plant.thePlantMaxHealth = (int)(plant.thePlantMaxHealth / plantHealthMultiplierPrevious * plantHealthMultiplier.Value);
				}
				plantHealthMultiplierPrevious = plantHealthMultiplier.Value;
			};

			zombieHealthMultiplier.SettingChanged += (sender, args) =>
			{
				if (zombieHealthMultiplier.Value == 0)
				{
					zombieHealthMultiplier.Value = 0.01f;
					return;
				}
				foreach (Zombie zombie in Zombie_Patch.zombie_Shadow.Select(valueTuple => valueTuple.Item1))
				{
					zombie.theHealth    = zombie.theHealth / zombieHealthMultiplierPrevious * zombieHealthMultiplier.Value;
					zombie.theMaxHealth = (int)(zombie.theMaxHealth / zombieHealthMultiplierPrevious * zombieHealthMultiplier.Value);
					if (zombieHealthArmorMultiplier.Value)
					{
						zombie.theFirstArmorHealth  = (int)(zombie.theFirstArmorHealth / zombieHealthMultiplierPrevious * zombieHealthMultiplier.Value);
						zombie.theSecondArmorHealth = (int)(zombie.theSecondArmorHealth / zombieHealthMultiplierPrevious * zombieHealthMultiplier.Value);
					}
				}
				zombieHealthMultiplierPrevious = zombieHealthMultiplier.Value;
			};
			zombieHealthArmorMultiplier.SettingChanged += (sender, args) =>
			{
				foreach ((Zombie zombie, Transform _) in Zombie_Patch.zombie_Shadow)
				{
					if (zombieHealthMultiplier.Value == 0)
					{
						zombieHealthMultiplier.Value = 0.01f;
						return;
					}
					if (zombieHealthArmorMultiplier.Value)
					{
						zombie.theFirstArmorHealth  = (int)(zombie.theFirstArmorHealth * zombieHealthMultiplier.Value);
						zombie.theSecondArmorHealth = (int)(zombie.theSecondArmorHealth * zombieHealthMultiplier.Value);
					}
					else
					{
						zombie.theFirstArmorHealth  = (int)(zombie.theFirstArmorHealth / zombieHealthMultiplier.Value);
						zombie.theSecondArmorHealth = (int)(zombie.theSecondArmorHealth / zombieHealthMultiplier.Value);
					}
				}
			};
			zombieSpeedMultiplier.SettingChanged += (sender, args) =>
			{
				if (zombieSpeedMultiplier.Value == 0)
				{
					zombieSpeedMultiplier.Value = 0.01f;
					return;
				}
				foreach (Zombie zombie in Zombie_Patch.zombie_Shadow.Select(valueTuple => valueTuple.Item1))
				{
					zombie.theSpeed       /= zombieSpeedMultiplierPrevious;
					zombie.theOriginSpeed /= zombieSpeedMultiplierPrevious;
					zombie.theSpeed       *= zombieSpeedMultiplier.Value;
					zombie.theOriginSpeed *= zombieSpeedMultiplier.Value;
				}
				zombieSpeedMultiplierPrevious = zombieSpeedMultiplier.Value;
			};
			showPlantHealth.SettingChanged  += (sender, args) => { DrawHealthPlants.Reset(); };
			showZombieHealth.SettingChanged += (sender, args) => { DrawHealthZombies.Reset(); };

			GameAPP.developerMode          = isDeveloperMode.Value;
			GameAPP.noCD                   = isNoCD.Value;
			zombieSpeedMultiplierPrevious  = zombieSpeedMultiplier.Value;
			zombieHealthMultiplierPrevious = zombieHealthMultiplier.Value;
			plantHealthMultiplierPrevious  = plantHealthMultiplier.Value;
		}
	}
}