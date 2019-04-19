using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class LpcOversizeSpriteProcessor : AssetPostprocessor
{
	enum LpcAnimationState
	{
		Hurt,
		Shoot,
		Slash,
		Walkcycle,
		Thrust,
		Spellcast
	}

	private const int LPC_SHEET_WIDTH  = 1152;
	private const int LPC_SHEET_HEIGHT = 768;
	private const int LPC_SPRITE_SIZE  = 192;

	private int m_PixelsPerUnit; // Sets the Pixels Per Unit in the Importer

	private int  m_ColCount;
	private int  m_RowCount;

	void RetrieveSettings()
	{
		// Retrieve Basic Settings
		m_PixelsPerUnit      = LpcSpriteSettings.GetPixelsPerUnit();

		// Retrieve Other Settings
		m_ColCount = 6;
		m_RowCount = 4;
	}

	void OnPreprocessTexture()
	{
		RetrieveSettings();
		TextureImporter textureImporter = (TextureImporter) assetImporter;
		textureImporter.textureType         = TextureImporterType.Sprite;
		textureImporter.spriteImportMode    = SpriteImportMode.Multiple;
		textureImporter.textureCompression  = TextureImporterCompression.Uncompressed;
		textureImporter.mipmapEnabled       = false;
		textureImporter.filterMode          = FilterMode.Point;
		textureImporter.spritePixelsPerUnit = m_PixelsPerUnit;
		textureImporter.spritePackingTag    = "character";
	}

	public void OnPostprocessTexture(Texture2D texture)
	{
		// Do nothing if it not a LPC Based Sprite
		if (!IsLpcSpriteSheet(texture))
			return;

		Debug.Log("Importing LPC Character Sheet");
		List<SpriteMetaData> metas = new List<SpriteMetaData>();
		
		for (int row = 0; row < m_RowCount; ++row)
		{
			for (int col = 0; col < m_ColCount; ++col)
			{
				SpriteMetaData meta = new SpriteMetaData();
				meta.rect = new Rect(col * LPC_SPRITE_SIZE, row * LPC_SPRITE_SIZE, LPC_SPRITE_SIZE, LPC_SPRITE_SIZE);

                string[] path_branch = assetImporter.assetPath.Split('/');
                //Debug.Log("SPRITE PATH: " + assetImporter.assetPath);

                //don't do more detailed meta for environment sprites
				var prefixIndexOffset = 0;
				if (path_branch.Contains("Battleground"))
					prefixIndexOffset++;

                string prefix = "";
                for (int i = (3 + prefixIndexOffset); i < path_branch.Length; i++)
                {
                    string node = path_branch[i];
                    string[] split_node = node.Split('.');
                    //Debug.Log("PATH BRANCH: " + node);

                    prefix += string.Format("{0}_", split_node[0]);
                }

                string namePrefix = ResolveLpcNamePrefix (row, prefix);

                meta.name = namePrefix + col;

                Debug.Log("SPRITE NAME: " + meta.name);
				metas.Add(meta);
			}
		}

		TextureImporter textureImporter = (TextureImporter) assetImporter;

		textureImporter.spritesheet = metas.ToArray();
	}

	public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
	{
		Debug.Log("Sliced Sprites: " + sprites.Length);
	}

	private bool IsLpcSpriteSheet(Texture2D texture)
	{
		if (texture.width == LPC_SHEET_WIDTH && texture.height == LPC_SHEET_HEIGHT)
		{
			return true;
		}

		return false;
	}


	private string ResolveLpcNamePrefix(int row, string prefix)
	{
		switch (row)
		{
			case 0:
				return prefix + "sl_r_";
			case 1:
				return prefix + "sl_d_";
			case 2:
				return prefix + "sl_l_";
			case 3:
				return prefix + "sl_t_";
			default:
				Debug.LogError("ResolveLpcNamePrefix unknown row: " + row);
				return prefix + "";
		}
	}
}