using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class ResourcesComponent : IComponent
{
	public List<string> sprites { get; set; }
}

public static class EnvironmentResources
{
	public static string RandomFloor
	{
		get
		{
			var rand = Rand.game.Int(15, 18);
			return $"grass_{rand}";
		}
	}

	public static string RandomDetail
	{
		get
		{
			var rand = Rand.game.Bool(.9f);
			if (rand)
				return $"grass_3";
			else
				return $"grass_0";
		}
	}

	public static string GrassTopLeft = "grass_6";
	public static string GrassTop = "grass_7";
	public static string GrassTopRight = "grass_8";
	public static string GrassLeft = "grass_9";
	public static string GrassMiddle = "grass_10";
	public static string GrassRight = "grass_11";
	public static string GrassBottomLeft = "grass_12";
	public static string GrassBottom = "grass_13";
	public static string GrassBottomRight = "grass_14";
}

public static class ObstacleResources
{
	public static string RandomObstacle
	{
		get
		{
			var rand = Rand.game.Int(1, 10);
			if (rand >= 2 && rand <= 7)
				return BigRock;

			return SmallRocks;
		}
	}

	public static string Cauldron = "forest_props_81";
	public static string BigRock = "forest_props_27";
	public static string SmallRocks = "forest_props_28";
}