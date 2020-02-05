using UnityEngine;
using System.Collections;

public enum Move { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft, None}

public class TextureScroll : MonoBehaviour {

    [Header("Set Texture Direction")]
    [Space]
    public Move move;
    [Header("Set Texture Speed")]
    [Space]
    public float scrollSpeed = 0.5F;
    [Header("Reset Texture Direction")]
    [Space]
    public bool reset;
    IEnumerator routine;
    bool isActive = false;

    private void Start()
    {
        routine = SetDirection(move);
        StartCoroutine(routine);
    }
    public void Update()
    {
        if (!isActive && !reset)
        {
            isActive = true;
            if (routine != null)
                StopCoroutine(routine);
            routine = SetDirection(move);
            StartCoroutine(routine);
        }
        else if (reset && isActive)
        {
            if (routine != null)
                StopCoroutine(routine);
            isActive = false;
        }
    }
    public IEnumerator SetDirection(Move direction)
    {
        Renderer rend = GetComponent<Renderer>();
        switch (direction)
        {
            case Move.Up:
                {
                    while (isActive)
                    {
                        float offset = Time.unscaledTime * scrollSpeed;
                        rend.material.mainTextureOffset = new Vector2(0, offset);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                }
            case Move.UpRight:
                {
                    while (isActive)
                    {
                        float offset = Time.unscaledTime * scrollSpeed;
                        rend.material.mainTextureOffset = new Vector2(offset, offset);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                }
            case Move.Right:
                {
                    while (isActive)
                    {
                        float offset = Time.unscaledTime * scrollSpeed;
                        rend.material.mainTextureOffset = new Vector2(offset, 0);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                }
            case Move.DownRight:
                {
                    while (isActive)
                    {
                        float offset = Time.unscaledTime * scrollSpeed;
                        rend.material.mainTextureOffset = new Vector2(offset, -offset);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                }
            case Move.Down:
                {
                    while (isActive)
                    {
                        float offset = Time.unscaledTime * scrollSpeed;
                        rend.material.mainTextureOffset = new Vector2(0, -offset);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                }
            case Move.DownLeft:
                {
                    while (isActive)
                    {
                        float offset = Time.unscaledTime * scrollSpeed;
                        rend.material.mainTextureOffset = new Vector2(-offset, -offset);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                }
            case Move.Left:
                {
                    while (isActive)
                    {
                        float offset = Time.unscaledTime * scrollSpeed;
                        rend.material.mainTextureOffset = new Vector2(-offset, 0);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                }
            case Move.UpLeft:
                {
                    while (isActive)
                    {
                        float offset = Time.unscaledTime * scrollSpeed;
                        rend.material.mainTextureOffset = new Vector2(-offset, offset);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                }
        }


    }
}
