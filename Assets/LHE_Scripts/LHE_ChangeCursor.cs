using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_ChangeCursor : MonoBehaviour
{
    public Texture2D CursorImage;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(CursorImage, Vector2.zero, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
