using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshBlink : MonoBehaviour
{

    public Texture[] EyeImages;
    public Texture AltOpenImage;

    [FormerlySerializedAs("Rendos")]
    public MeshRendTarget[] PrimaryEyes;
    public MeshRendTarget[] AltEye;

    public MeshRendTarget[] EyeBagRendos;
    public string MatTex = "_MainTex";

    public float BlinkFrameTime = .1f;


    public Vector2 MinMaxBlinkTime;
    float CurBlinkTime;
    bool Blinking;
    bool BlinkIncing;
    int CurBlinkIndex = 0;

    public EyeImageSet EyeImg;

    bool initd = false;
    private void Awake()
    {
        VerifyInit();
    }

    void VerifyInit()
    {
        if (initd) return;
        initd = true;
        CurBlinkTime = Random.Range(MinMaxBlinkTime.x, MinMaxBlinkTime.y);
        CurBlinkIndex = 0;
    }

    BlinkAllowance Allowance = BlinkAllowance.Standard;
    public void SetAllowance (BlinkAllowance allow, bool force = false)
    {
        if(allow!= Allowance || force)
        {
            Allowance = allow;
            if(allow == BlinkAllowance.ForceClosed)
            {
                StartBlink();
            }
        }
    }
    void StartBlink()
    {
        VerifyInit();
        Blinking = true;
        BlinkIncing = true;
        CurBlinkIndex = 0;
        CurBlinkTime = BlinkFrameTime;
    }




    public void SetRender(bool on)
    {
        for (int i = 0; i < PrimaryEyes.Length; i++)
        {
            if (PrimaryEyes[i].Rendo != null)
            {
                PrimaryEyes[i].Rendo.enabled = on;
            }
            if (PrimaryEyes[i].Rendo2 != null)
            {
                PrimaryEyes[i].Rendo2.enabled = on;
            }
        }
        for (int i = 0; i < AltEye.Length; i++)
        {
            if (AltEye[i].Rendo != null)
            {
                AltEye[i].Rendo.enabled = on;
            }
            if (AltEye[i].Rendo2 != null)
            {
                AltEye[i].Rendo2.enabled = on;
            }
        }
    }



    EmotionalMood BaseMood = EmotionalMood.Happy; //emotional baseline
    EmotionalMood AnimationMood = EmotionalMood.Happy;  //AnimationMood overrides BaseMood as its a temporary thing
    EmotionalMood LastUsedMood = EmotionalMood.Undefined;
    public void SetBaseMood(EmotionalMood mood)
    {
        BaseMood = mood;
    }
    public void SetAnimationMood(EmotionalMood mood)
    {
        AnimationMood = mood;
    }

    void SetMood(EmotionalMood mood)
    {
        LastUsedMood = mood;
        EyeTextureSet set = EyeImg.HappyEye;
        switch(mood)
        {
            case EmotionalMood.Flat:
                set = EyeImg.BoredFlatEye;
                break;
            case EmotionalMood.Sad:
                set = EyeImg.SadEye;
                break;
            case EmotionalMood.Angry:
                set = EyeImg.AngryEye;
                break;
            case EmotionalMood.WideOpenAlert:
                set = EyeImg.WideOpenEye;
                break;
        }
        EyeImages[0] = set.Primary;
        AltOpenImage = set.Alt;
        SetEyeImage(EyeImages[0], AltOpenImage);
    }


    void SetEyeImage(Texture eye, Texture alt)
    {
        if(alt == null)
        {
            alt = eye;
        }
        
        //used by non players who dont have a "face"
        for (int i = 0; i < PrimaryEyes.Length; i++)
        {
            if (PrimaryEyes[i].Rendo != null)
            {
                PrimaryEyes[i].Rendo.materials[PrimaryEyes[i].Material].SetTexture(MatTex, eye);
            }
            if (PrimaryEyes[i].Rendo2 != null)
            {
                PrimaryEyes[i].Rendo2.materials[PrimaryEyes[i].Material].SetTexture(MatTex, eye);
            }
        }
        for (int i = 0; i < AltEye.Length; i++)
        {
            if (AltEye[i].Rendo != null)
            {
                AltEye[i].Rendo.materials[AltEye[i].Material].SetTexture(MatTex, alt);
            }
            if (AltEye[i].Rendo2 != null)
            {
                AltEye[i].Rendo2.materials[AltEye[i].Material].SetTexture(MatTex, alt);
            }
        }

        
    }

    private void FixedUpdate()
    {
        EmotionalMood useMood = BaseMood;
        if(AnimationMood != EmotionalMood.Happy)
        {
            useMood = AnimationMood;
        }
        if(useMood != LastUsedMood)
        {
            SetMood(useMood);
        }



        if(!Blinking)
        {
            CurBlinkTime -= Time.deltaTime;
            if(CurBlinkTime < 0)
            {
                StartBlink();
            }
        }
        else if(Allowance == BlinkAllowance.ForceClosed && CurBlinkIndex == EyeImages.Length-1)
        {
            //nada
        }
        else if (Allowance == BlinkAllowance.ForceOpen && CurBlinkIndex == 0)
        {
            //nada
        }
        else 
        {
            CurBlinkTime -= Time.deltaTime;
            if (CurBlinkTime < 0)
            {                
                CurBlinkTime = BlinkFrameTime;
                if (BlinkIncing)
                {
                    CurBlinkIndex++;
                    if (CurBlinkIndex >= EyeImages.Length)
                    {
                        BlinkIncing = false;
                        CurBlinkIndex -= 2; //we need to start tracking downward
                    }
                }
                else
                {
                    CurBlinkIndex--;
                }
                if (CurBlinkIndex <= 0)
                {
                    CurBlinkIndex = 0;
                    CurBlinkTime = Random.Range(MinMaxBlinkTime.x, MinMaxBlinkTime.y);
                    Blinking = false;
                }
                Texture altT = EyeImages[CurBlinkIndex];
                if(CurBlinkIndex==0)
                {
                    altT = AltOpenImage;
                }
                SetEyeImage(EyeImages[CurBlinkIndex],altT);
            }
        }
        
    }

    public Color OffEyeBagColor;
    public Color OnEyeBagColor;
    Color CurEyeBagColor = Color.cyan; //bad colors just to make sure we trigger the first set
    Color LastEyeBagColor = Color.blue;
    public void SetEyeShadow(float perc, bool force)
    {
        if(perc < .001f)
        {
            perc = 0;
        }
        CurEyeBagColor = Color.Lerp(OffEyeBagColor, OnEyeBagColor, perc);
        //if (LastEyeBagColor != CurEyeBagColor || force)
        {
            LastEyeBagColor = CurEyeBagColor;
            for (int i = 0; i < EyeBagRendos.Length; i++)
            {
                if (EyeBagRendos[i].Rendo != null)
                {
                    EyeBagRendos[i].Rendo.materials[EyeBagRendos[i].Material].color = LastEyeBagColor;
                    //Mesho.materials[index].color = col;
                }
                if (EyeBagRendos[i].Rendo2 != null)
                {
                    EyeBagRendos[i].Rendo2.materials[EyeBagRendos[i].Material].color = LastEyeBagColor;
                }
            }
        }
    }

}


public enum BlinkAllowance
{
    Standard,
    ForceOpen,
    ForceClosed
}


[System.Serializable]
public class MeshRendTarget
{
    public string DisplayName;
    public SkinnedMeshRenderer Rendo;
    public MeshRenderer Rendo2;
    public int Material = 0;
    public Material DefaultMaterialReplacement;
}

public enum EmotionalMood
{
    Happy,
    Flat,
    Sad,
    Angry,
    WideOpenAlert,
    Undefined
}

[System.Serializable]
public class EyeTextureSet
{
    public Texture Primary, Alt;
}