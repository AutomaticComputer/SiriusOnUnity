﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenameScript : MonoBehaviour
{
    public TapeLibraryScript tapeLibrary;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        tapeLibrary.startRename(gameObject.transform.position + new Vector3(0, 0.08f, 0));
    }
}
