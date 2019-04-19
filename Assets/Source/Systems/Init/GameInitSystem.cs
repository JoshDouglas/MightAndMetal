using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class GameInitSystem : IInitializeSystem
{
	private readonly Contexts _contexts;

	public GameInitSystem(Contexts contexts)
	{
		_contexts = contexts;
	}

	public void Initialize()
	{
		SetupSmallBattle();
	}

	public void LoadAtlases()
	{
	}

	public void SetupSmallBattle()
	{
		var battleId       = Guid.NewGuid();
		var skirmishSize   = new Vector2(30, 30);
		var characterCount = 6;
		SetupSkirmish(battleId, skirmishSize, characterCount);
	}

	public void SetupSkirmish(Guid battleId, Vector2 boardSize, int characterCount)
	{
		var skirmishId = Guid.NewGuid();
		SetupSkirmishBoard(battleId, skirmishId, boardSize);
		SetupCharacters(battleId, skirmishId, boardSize, characterCount);
	}

	public void SetupSkirmishBoard(Guid battleId, Guid skirmishId, Vector2 boardSize)
	{
		for (var x = 0; x < boardSize.x; x++)
		{
			for (var y = 0; y < boardSize.y; y++)
			{
				var isLeft     = x == 0;
				var isRight    = x == boardSize.x - 1;
				var isBottom   = y == 0;
				var isTop      = y == boardSize.y - 1;
				var isWall     = (isLeft || isRight || isTop || isBottom);
				var isObstacle = Rand.game.Bool(.02f);
				var hasDetail  = Rand.game.Bool(.03f);
				var position   = new Vector2(x, y);
				var resources  = new List<string>();

				var floorEntity = _contexts.game.CreateEntity();
				floorEntity.isFloor = true;
				floorEntity.AddPosition(position);
				floorEntity.AddScale(Vector2.one);
				floorEntity.AddBattle(battleId);
				floorEntity.AddSkirmish(skirmishId);

				if (isWall || isObstacle)
					floorEntity.isBlockable = true;

				//floor layer
				if (!isWall)
					resources.Add(EnvironmentResources.RandomFloor);


				//details layer
				if (hasDetail && !isObstacle && !isWall)
					resources.Add(EnvironmentResources.RandomDetail);

				//obstacle layer
				if (isObstacle && !isWall)
					resources.Add(ObstacleResources.RandomObstacle);

				//wall layer
				if (isLeft && isTop)
					resources.Add(EnvironmentResources.GrassTopLeft);
				else if (isLeft && isBottom)
					resources.Add(EnvironmentResources.GrassBottomLeft);
				else if (isRight && isTop)
					resources.Add(EnvironmentResources.GrassTopRight);
				else if (isRight && isBottom)
					resources.Add(EnvironmentResources.GrassBottomRight);
				else if (isLeft)
					resources.Add(EnvironmentResources.GrassLeft);
				else if (isRight)
					resources.Add(EnvironmentResources.GrassRight);
				else if (isTop)
					resources.Add(EnvironmentResources.GrassTop);
				else if (isBottom)
					resources.Add(EnvironmentResources.GrassBottom);

				floorEntity.AddResources(resources);
			}
		}
	}

	public void SetupCharacters(Guid battleId, Guid skirmishId, Vector2 boardSize, int characterCount)
	{
		var factions              = new Dictionary<string, Vector2> {{"Ivalice", Vector2.left}, {"Bandits", Vector2.up}};
		var factionCharacterCount = characterCount / factions.Count;
		var hasPlayer             = false;

		foreach (var faction in factions)
		{
			var hasLeader = false;

			for (var i = 0; i < factionCharacterCount; i++)
			{
				var characterEntity = _contexts.game.CreateEntity();
				characterEntity.isBlockable = true;
				characterEntity.isMovable   = true;
				characterEntity.AddScale(Vector2.one);
				characterEntity.AddVelocity(Vector2.zero);
				characterEntity.AddFaction(faction.Key);
				characterEntity.AddBattle(battleId);
				characterEntity.AddSkirmish(skirmishId);
				/*if ((i + 1) % 3 == 0)
					characterEntity.AddCharacter($"archer_{faction.Key}_{i}", CharacterTemplate.Archer);
				else if ((i + 1) % 4 == 0)
					characterEntity.AddCharacter($"cleric_{faction.Key}_{i}", CharacterTemplate.Cleric);
				else*/
				characterEntity.AddCharacter($"swordsmen_{faction.Key}_{i}", CharacterTemplate.Swordsman);

				var randX      = Rand.game.Int((int) (boardSize.x * .2), (int) (boardSize.x * .8));
				var randY      = Rand.game.Int((int) (boardSize.y * .2), (int) (boardSize.y * .8));
				var spawnPoint = new Vector2(randX, randY);

				//position
				characterEntity.AddPosition(spawnPoint);

				//input
				characterEntity.AddInput();

				//notable characters
				if (!hasPlayer)
				{
					characterEntity.character.EquipmentTemplate = EquipmentTemplate.Plate;
					characterEntity.isPlayerControlled          = true;
					hasPlayer                                   = true;
				}

				if (!hasLeader)
				{
					characterEntity.AddLeader("Butthole");
					hasLeader = true;
				}
			}
		}
	}
}