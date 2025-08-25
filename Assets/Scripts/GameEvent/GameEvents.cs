using System;
using UnityEngine;

public static class GameEvents
{
    public static Action OnPlayerDeath;

    public static Action<Transform, string> OnShowInteractKey;
    public static Action                    OnHideInteractKey;
}
