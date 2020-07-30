using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GridManager : MonoBehaviour
{
    [SerializeField] List<Sprite> Sprites = new List<Sprite>();
    [SerializeField] GameObject GemPrefab;
    [SerializeField] int GridSize = 6;
    [SerializeField] float Distance = 1.0f;
    [Header("SFX")]
    [SerializeField] AudioClip Select;
    [SerializeField] AudioClip Clear;
    [SerializeField] AudioClip Swap;
    private GameObject[,] Grid;

    ScoreManager scoreManager;
    int pointsAwarded = 0;
    public bool gemClickLock = false;

    public static GridManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        // Grid creation and initialization
        Grid = new GameObject[GridSize, GridSize];
        GridInit();
    }

    void GridInit()
    {
        // The offset to center the grid to the GameObject
        Vector3 positionOffset = transform.position - new Vector3(GridSize * Distance / 2.0f, GridSize * Distance / 2.0f, 0);
        // For each element of the grid, generate a random sprite
        for (int row = 0; row < GridSize; row++)
            for (int column = 0; column < GridSize; column++)
            {
                // Instantiate the gem from the prefab
                GameObject newGem = Instantiate(GemPrefab);
                // Check if inserting a sprite would make a combination from the start
                List<Sprite> possibleSprites = new List<Sprite>(Sprites);
                // Check if the new sprite could make a combination horizontally
                Sprite left1 = GetSpriteAt(column - 1, row);
                Sprite left2 = GetSpriteAt(column - 2, row);
                if (left2 != null && left1 == left2)
                {
                    // Remove sprite from the options
                    possibleSprites.Remove(left1);
                }
                // Check if the new sprite could make a combination vertically
                Sprite down1 = GetSpriteAt(column, row - 1);
                Sprite down2 = GetSpriteAt(column, row - 2);
                if (down2 != null && down1 == down2)
                {
                    // Remove sprite from the options
                    possibleSprites.Remove(down1);
                }
                // Set a random sprite from the possibleSprites list to the gem object
                SpriteRenderer renderer = newGem.GetComponent<SpriteRenderer>();
                renderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];
                // Add component for mouse click interactions and set its position
                Gem gem = newGem.AddComponent<Gem>();
                gem.Position = new Vector2Int(column, row);
                // Set the gem as a child object of the grid
                newGem.transform.parent = transform;
                // Assign the position of the gem
                newGem.transform.position = new Vector3(column * Distance, row * Distance, 0) + positionOffset;
                // Save the gem to the Grid
                Grid[column, row] = newGem;
            }
    }

    int MatchFinder()
    {
        HashSet<SpriteRenderer> matchedGems = new HashSet<SpriteRenderer>();
        for (int row = 0; row < GridSize; row++)
        {
            for (int column = 0; column < GridSize; column++)
            {
                SpriteRenderer current = GetSpriteRendererAt(column, row);

                List<SpriteRenderer> horizontalMatches = ColumnMatches(column, row, current.sprite);
                if (horizontalMatches.Count >= 2)
                {
                    matchedGems.UnionWith(horizontalMatches);
                    matchedGems.Add(current);
                }

                List<SpriteRenderer> verticalMatches = RowMatches(column, row, current.sprite);
                if (verticalMatches.Count >= 2)
                {
                    matchedGems.UnionWith(verticalMatches);
                    matchedGems.Add(current);
                }
            }
        }

        foreach (SpriteRenderer renderer in matchedGems)
            renderer.sprite = null;

        return matchedGems.Count;
    }

    List<SpriteRenderer> ColumnMatches(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> matchedGems = new List<SpriteRenderer>();
        for (int i = col + 1; i < GridSize; i++)
        {
            SpriteRenderer nextColumn = GetSpriteRendererAt(i, row);
            if (sprite != nextColumn.sprite)
                break;
            matchedGems.Add(nextColumn);
        }
        return matchedGems;
    }

    List<SpriteRenderer> RowMatches(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> matchedGems = new List<SpriteRenderer>();
        for (int i = row + 1; i < GridSize; i++)
        {
            SpriteRenderer nextRow = GetSpriteRendererAt(col, i);
            if (sprite != nextRow.sprite)
                break;
            matchedGems.Add(nextRow);
        }
        return matchedGems;
    }

    // Get the sprite in the given coordinates
    Sprite GetSpriteAt(int column, int row)
    {
        if (column < 0 || column >= GridSize || row < 0 || row >= GridSize)
            return null;
        GameObject gem = Grid[column, row];
        SpriteRenderer renderer = gem.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

    SpriteRenderer GetSpriteRendererAt(int column, int row)
    {
        if (column < 0 || column >= GridSize || row < 0 || row >= GridSize)
            return null;
        GameObject gem = Grid[column, row];
        SpriteRenderer renderer = gem.GetComponent<SpriteRenderer>();
        return renderer;
    }

    // Swaps the sprites of two given gems
    public void SwapGems(Vector2Int gem1Position, Vector2Int gem2Position)
    {
        GameObject gem1 = Grid[gem1Position.x, gem1Position.y];
        SpriteRenderer renderer1 = gem1.GetComponent<SpriteRenderer>();

        GameObject gem2 = Grid[gem2Position.x, gem2Position.y];
        SpriteRenderer renderer2 = gem2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        bool match = ((pointsAwarded = MatchFinder()) > 0);
        if (!match)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            AudioSource.PlayClipAtPoint(Swap, Camera.main.transform.position, 0.5f);
            do
            {
                AudioSource.PlayClipAtPoint(Clear, Camera.main.transform.position, 0.5f);
                scoreManager.AwardPoints(pointsAwarded);
                GemFiller();
                if (!FindPossibleMoves())
                    ShuffleGems();
            } while ((pointsAwarded = MatchFinder()) > 0);
        }
    }

    public void GemFiller()
    {
        for (int row = 0; row < GridSize; row++)
        {
            for (int column = 0; column < GridSize; column++)
            {
                while (GetSpriteRendererAt(column, row).sprite == null)
                {
                    SpriteRenderer current = GetSpriteRendererAt(column, row);
                    SpriteRenderer above = current;
                    for (int i = row; i < GridSize - 1; i++)
                    {
                        above = GetSpriteRendererAt(column, i + 1);
                        current.sprite = above.sprite;
                        current = above;
                    }
                    above.sprite = Sprites[Random.Range(0, Sprites.Count)];
                }
            }
        }
    }

    public bool FindPossibleMoves()
    {
        for (int row = 0; row < GridSize; row++)
        {
            for (int column = 0; column < GridSize; column++)
            {
                Sprite current = GetSpriteAt(column, row);
                // Check if gem 1 space to the right is the same
                Sprite right = GetSpriteAt(column + 1, row);
                if (current == right)
                {
                    right = GetSpriteAt(column + 2, row + 1);
                    if (current == right) return true;
                    right = GetSpriteAt(column + 3, row);
                    if (current == right) return true;
                    right = GetSpriteAt(column + 2, row - 1);
                    if (current == right) return true;
                }
                // Check if gem 2 spaces to the right is the same
                right = GetSpriteAt(column + 2, row);
                if (current == right)
                {
                    right = GetSpriteAt(column + 1, row + 1);
                    if (current == right) return true;
                    right = GetSpriteAt(column + 3, row);
                    if (current == right) return true;
                    right = GetSpriteAt(column + 1, row - 1);
                    if (current == right) return true;
                }
                // Check if gem 1 space above is the same
                Sprite above = GetSpriteAt(column, row + 1);
                if (current == above)
                {
                    above = GetSpriteAt(column - 1, row + 2);
                    if (current == above) return true;
                    above = GetSpriteAt(column, row + 3);
                    if (current == above) return true;
                    above = GetSpriteAt(column + 1, row + 2);
                    if (current == above) return true;
                }
                // Check if gem 2 spaces above is the same
                above = GetSpriteAt(column, row + 2);
                if (current == above)
                {
                    above = GetSpriteAt(column - 1, row + 1);
                    if (current == above) return true;
                    above = GetSpriteAt(column, row + 3);
                    if (current == above) return true;
                    above = GetSpriteAt(column + 1, row + 1);
                    if (current == above) return true;
                }
            }
        }
        return false;
    }

    public void ShuffleGems()
    {
        for (int column = 0; column < GridSize; column++)
        {
            for(int row = 0; row < GridSize; row++)
            {
                // Check if inserting a sprite would make a combination from the start
                List<Sprite> possibleSprites = new List<Sprite>(Sprites);
                // Check if the new sprite could make a combination horizontally
                Sprite left1 = GetSpriteAt(column - 1, row);
                Sprite left2 = GetSpriteAt(column - 2, row);
                if (left2 != null && left1 == left2)
                {
                    // Remove sprite from the options
                    possibleSprites.Remove(left1);
                }
                // Check if the new sprite could make a combination vertically
                Sprite down1 = GetSpriteAt(column, row - 1);
                Sprite down2 = GetSpriteAt(column, row - 2);
                if (down2 != null && down1 == down2)
                {
                    // Remove sprite from the options
                    possibleSprites.Remove(down1);
                }
                Grid[column, row].GetComponent<SpriteRenderer>().sprite = Sprites[Random.Range(0, Sprites.Count)];
            }
        }
    }

    public void PlaySelectSound()
    {
        AudioSource.PlayClipAtPoint(Select, Camera.main.transform.position, 0.5f);
    }
}
