using UnityEngine;
using RDKitForUnity;
[CreateAssetMenu(fileName = "RenderingData", menuName = "Scriptable Objects/RenderingData")]
public class RenderingData : ScriptableObject
{
    public RDKitUnityFuncs.RenderMode renderMode;
    public int buffersize;
}
