using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FigureSet", menuName = "ScriptableObjects/FigureSet", order = 1)]
public class FigureSet : SerializedScriptableObject
{
    public Dictionary<FigureType, string> figureSetDictionary = null;
}
