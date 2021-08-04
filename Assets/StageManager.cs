using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehavior<StageManager>
{
    public SaveInt highScore;

    new private void Awake()
    {
        base.Awake();
    }
}