using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "API_Keys", menuName ="ScriptableObjects/API Keys Container")]
public class APIKeyContainer : ScriptableObject
{
    public string OpenRouteServiceApiKey = "";
}
