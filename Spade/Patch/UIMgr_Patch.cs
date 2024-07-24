using HarmonyLib;
using Spade.LifeCycle;
using UnityEngine;

namespace Spade.Patch
{
	[HarmonyPatch(typeof(UIMgr))]
	public class UIMgr_Patch
	{
		[HarmonyPrefix]
		[HarmonyPatch("EnterPauseMenu")]
		public static bool Rewrite_EnterPauseMenu(int place)
		{
			if (!Config.invisiblePause.Value) return true;
			if (place != 0) return true;
			GameAPP.PlaySoundNotPause(30);
			GameAPP.theGameStatus = 1;
			Time.timeScale        = 0f;
			return false;
		}
	}
}