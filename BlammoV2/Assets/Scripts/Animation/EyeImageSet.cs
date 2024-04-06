using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EyeImageSet : MonoBehaviour
{

    public EyeTextureSet HappyEye;
    public EyeTextureSet SadEye, AngryEye, BoredFlatEye, WideOpenEye;

}