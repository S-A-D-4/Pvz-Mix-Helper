using BepInEx.Configuration;
using UnityEngine;

namespace Spade.LifeCycle
{
	public class HelperWindows : HaveLifeCycle
	{
		public static HelperWindows Instance;
		public        Rect          beginScrollRect = new Rect(10, 160, 180, 360); // 滚动视图开始

		public Rect   bigRect            = new Rect(10, 10, 200, 540); // 显示Popup时窗口的大小
		public Rect   generateZombieRect = new Rect(10, 90, 180, 30);  // 生成僵尸按钮
		public string num                = "1";

		public  Rect    numLabelRect   = new Rect(10, 20, 70, 30);
		public  Rect    numRect        = new Rect(95, 20, 100, 30);
		public  Rect    popupRect      = new Rect(10, 160, 180, 360); // Popup框
		public  string  road           = "-1";
		public  Rect    roadLabelRect  = new Rect(10, 55, 70, 30);
		public  Rect    roadRect       = new Rect(95, 55, 100, 30);
		private Vector2 scrollPosition = Vector2.zero;

		private int selectedIndex;

		private bool showPopup;
		public  Rect showPopupRect = new Rect(10, 125, 180, 30); // 显示Popup按钮
		public  Rect smallRect     = new Rect(10, 10, 200, 180); // 不显示Popup时窗口的大小
		private Rect tooltipRect   = new Rect(100, 100, 150, 200);
		public  Rect viewRect;

		public Rect windowRect = new Rect(10, 10, 200, 150); // 初始窗口的位置和大小

		// 窗口标题
		private string windowTitle = "僵尸生成器";

		private int[] zombieIndexes =
		{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111 };

		private string[] zombieNames =
		{ "Zombie",
		  "Zombie",
		  "ConeZombie",
		  "PolevaulterZombie",
		  "BucketZombie",
		  "PaperZombie",
		  "DancePolevaulterPrefab",
		  "DancePolevaulter2Prefab",
		  "ZombieDoor",
		  "Zombie_footballPrefab",
		  "Zombie_JacksonPrefab",
		  "PeaShooterZ",
		  "CherryShooterZ",
		  "SuperCherryZ",
		  "WallNutZ",
		  "CherryPaper",
		  "RandomZombie",
		  "BucketNutZ",
		  "CherryNutZ",
		  "IronPeaZPrefab",
		  "TallNutFootballZPrefab",
		  "RandomPlusZombie",
		  "TallIceNutZ" };


		public override void Awake()
		{
			Instance = this;
			viewRect = new Rect(0, 0, 160, zombieNames.Length * 30);
		}

		public override void OnGUI()
		{
			if (Config.showWindow.Value)
				windowRect = GUI.Window(0, windowRect, DrawWindow, windowTitle);
		}

		public void DrawWindow(int windowID)
		{
			// 显示数字输入栏
			GUI.Label(numLabelRect, "生成数量");
			num = GUI.TextField(numRect, num);

			GUI.Label(roadLabelRect, "生成路线");
			road = GUI.TextField(roadRect, road);

			if (GUI.Button(generateZombieRect, "生成"))
			{
				if (!int.TryParse(num, out int intNum))
					return;
				if (!int.TryParse(road, out int intRoad))
					return;
				int selectedZombieIndex = zombieIndexes[selectedIndex];
				for (int i = 0; i < intNum; i++)
				{
					Zombie component;
					if (intRoad < 5 && intRoad >= 0)
					{
						component = Board.Instance.createZombie.SetZombie(0, intRoad, selectedZombieIndex).GetComponent<Zombie>();
						Board.Instance.zombieTotalHealth
							+= (int)component.theHealth + component.theFirstArmorHealth + component.theSecondArmorHealth;
					}
					else if (intRoad == -1)
					{
						for (int j = 0; j < 5; j++)
						{
							component = Board.Instance.createZombie.SetZombie(0, j, selectedZombieIndex)
								.GetComponent<Zombie>();
							Board.Instance.zombieTotalHealth
								+= (int)component.theHealth + component.theFirstArmorHealth + component.theSecondArmorHealth;
						}
					}
					else
					{
						component = Board.Instance.createZombie.SetZombie(0, Random.Range(0, 5), selectedZombieIndex)
							.GetComponent<Zombie>();
						Board.Instance.zombieTotalHealth
							+= (int)component.theHealth + component.theFirstArmorHealth + component.theSecondArmorHealth;
					}
				}
			}

			// 创建一个按钮，当点击时显示Popup
			if (GUI.Button(showPopupRect, "选中僵尸：" + zombieNames[selectedIndex])) showPopup = !showPopup;

			// 显示Popup
			if (showPopup)
			{
				windowRect = bigRect;

				GUI.Box(popupRect, ""); // 添加边框

				scrollPosition = GUI.BeginScrollView(beginScrollRect, scrollPosition, viewRect);

				selectedIndex = GUI.SelectionGrid(viewRect, selectedIndex, zombieNames, 1);

				GUI.EndScrollView();
			}
			else
			{
				windowRect = smallRect;
			}

			// 使窗口可以拖动
			GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
		}
	}
}