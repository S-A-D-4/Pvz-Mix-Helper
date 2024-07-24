using HarmonyLib;
using Spade.LifeCycle;

namespace Spade.Patch
{
	[HarmonyPatch(typeof(InitZombieList))]
	public class InitZombieList_Patch
	{
		[HarmonyPrefix]
		[HarmonyPatch("InitZombie")]
		public static bool Overwrite(int theLevelType, int theLevelNumber)
		{
			Traverse traverse = Traverse.Create(typeof(InitZombieList));
			int      multiplier;
			bool[] allowZombieTypeSpawn
				= traverse.Field("allowZombieTypeSpawn").GetValue<bool[]>();

			traverse.Method("InitList").GetValue();
			if (GameAPP.difficulty == 5)
			{
				if (theLevelType == 0 && (theLevelNumber == 0 || theLevelNumber == 1))
					multiplier = 3;
				else
					multiplier = 4;
			}
			else
			{
				multiplier = GameAPP.difficulty;
			}
			traverse.Method("SetAllowZombieTypeSpawn", theLevelType, theLevelNumber).GetValue(theLevelType, theLevelNumber);
			InitZombieList.theMaxWave = 10;
			if (theLevelType != 0) return true;
			traverse.Method("InitAdvWave", theLevelNumber).GetValue(theLevelNumber);
			for (int i = 1; i <= InitZombieList.theMaxWave; i++)
			{
				int zombiePoint = (int)(i * multiplier * Config.zombieNumMultiplier.Value);
				while (zombiePoint > 0)
				{
					bool flag = false;
					int  num;
					do
					{
						num = traverse.Method("PickZombie").GetValue<int>();
						if (i < 10)
						{
							if (GameAPP.difficulty == 5) break;
							if (num != 6 && num != 10)
								switch (num)
								{
									case 104:
									case 106:
									case 108:
									case 109:
										break;
									default:
										flag = false;
										goto IL_AB;
								}
							flag = true;
						}
						IL_AB: ;
					} while (flag);
					int num2 = traverse.Method("AddZombieToList", num, i).GetValue<int>(num, i);
					zombiePoint -= num2;
					traverse.Field("zombiePoint").SetValue(zombiePoint);
				}
				if (i == InitZombieList.theMaxWave)
				{
					for (int j = 0; j < allowZombieTypeSpawn.Length; j++)
						if (allowZombieTypeSpawn[j])
						{
							zombiePoint = 20;
							traverse.Field("zombiePoint").SetValue(zombiePoint);
							traverse.Method("AddZombieToList", j, i).GetValue<int>(j, i);
						}
					zombiePoint = -1;
				}
			}
			return false;
		}
	}
}