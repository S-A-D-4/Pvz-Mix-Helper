using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Spade.LifeCycle;


namespace Spade
{
	[BepInPlugin("spade.pvzMix.Helper", "黑A修改器", "1.0")]
	public class HelperMain : BaseUnityPlugin
	{
		public static HelperMain instance;

		private List<HaveLifeCycle> haveLifeCycles;

		public static ManualLogSource Log => instance.Logger;


		private void Awake()
		{
			//输出日志
			BepInEx.Logging.Logger.Sources.Add(new ManualLogSource("黑A修改器"));
			Logger.LogInfo("黑A修改器启动中");
			new Harmony("spade.pvzMix.Helper").PatchAll();
			instance = this;

			haveLifeCycles = new List<HaveLifeCycle>
			{ new Config(),
			  new ZombieGeneratorWindows(),
			  new DrawHealthPlants(),
			  new DrawHealthZombies() };

			foreach (HaveLifeCycle haveLifeCycle in haveLifeCycles) haveLifeCycle.Awake();
		}

		private void Start()
		{
			foreach (HaveLifeCycle haveLifeCycle in haveLifeCycles) haveLifeCycle.Start();
		}

		private void Update()
		{
			foreach (HaveLifeCycle haveLifeCycle in haveLifeCycles) haveLifeCycle.Update();
		}

		private void OnDestroy()
		{
			foreach (HaveLifeCycle haveLifeCycle in haveLifeCycles) haveLifeCycle.OnDestroy();
		}

		private void OnGUI()
		{
			foreach (HaveLifeCycle haveLifeCycle in haveLifeCycles) haveLifeCycle.OnGUI();
		}
	}
}