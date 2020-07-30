using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private static Gem selectedGem;
    private SpriteRenderer Renderer;
    public Vector2Int Position;

    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    public void SelectGem()
    {
        GridManager.Instance.PlaySelectSound();
        Renderer.color = Color.grey;
    }

    public void UnselectGem()
    {
        Renderer.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (!GridManager.Instance.gemClickLock)
        {
            if (selectedGem != null)
            {
                // Return if clicked again
                if (selectedGem == this)
                    return;
                selectedGem.UnselectGem();
                if (Vector2Int.Distance(selectedGem.Position, Position) == 1)
                {
                    GridManager.Instance.SwapGems(Position, selectedGem.Position);
                    selectedGem = null;
                }
                else
                {
                    selectedGem = this;
                    SelectGem();
                }
            }
            else
            {
                selectedGem = this;
                SelectGem();
            }
        }
    }
}
