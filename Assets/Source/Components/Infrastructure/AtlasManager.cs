using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public sealed class AtlasManager
{
	private static readonly SpriteAtlas environmentInstance = Resources.Load<SpriteAtlas>("Battleground/BattlegroundAtlas");
	private static readonly SpriteAtlas characterInstance = Resources.Load<SpriteAtlas>("Characters/CharacterAtlas");
	private static readonly SpriteAtlas characterTempalteInstance = Resources.Load<SpriteAtlas>("Characters/CharacterTemplateAtlas");
	
	static AtlasManager()
	{}
	
	private AtlasManager()
	{}

	public static SpriteAtlas EnvironmentInstance => environmentInstance;

	public static SpriteAtlas CharacterInstance => characterInstance;
	
	public static SpriteAtlas CharacterTemplateInstance => characterTempalteInstance;
}

