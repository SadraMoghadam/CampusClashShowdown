using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


[Serializable]
public class BodyPartTypeIndex
{
    public BodyPartType bodyPartType;
    public int index;
}

[Serializable]    
public class SaveAvatarObject{
    public List<BodyPartTypeIndex> bodyPartTypeIndexList;
}


public enum BodyPartType{
    Body,
    Head,
}
[System.Serializable]
public class BodyPartData{

    public Mesh[] meshArray;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public BodyPartType bodyPartType;
}

public class PlayerCustomization : MonoBehaviour
{ 
    //private string PLAYER_PREFS_SAVE = "PlayerCustomization";        
    [SerializeField] private BodyPartData[] bodyPartDataArray;
    private BodyPartData _bodyPartData;
    private Mesh _firstHeadMesh;
    private Mesh _firstBodyMesh;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.PlayerCustomization.ToString()))
        {
            LoadAvatar();
        }

        _firstBodyMesh = bodyPartDataArray[0].skinnedMeshRenderer.sharedMesh;
        _firstHeadMesh = bodyPartDataArray[1].skinnedMeshRenderer.sharedMesh;
    }

    public void ChangeBodyPart(BodyPartType bodyPartType, bool forward ){
         /*BodyPartData bodyPartData = GetBodyPartData(bodyPartType);
         int meshIndex = System.Array.IndexOf(bodyPartData.meshArray, bodyPartData.skinnedMeshRenderer.sharedMesh);
         bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[(meshIndex +1) % bodyPartData.meshArray.Length];
           } */
          BodyPartData bodyPartData = GetBodyPartData(bodyPartType, bodyPartDataArray);
         if (bodyPartData == null || bodyPartData.meshArray == null || bodyPartData.meshArray.Length == 0)
         {
             Debug.LogWarning("Mesh array is null or empty for the specified body part.");
             return;
         }
       
         int currentIndex = GetCurrentMeshIndex(bodyPartData);
       
         int nextIndex;
         if (forward)
         {
             nextIndex = (currentIndex + 1) % bodyPartData.meshArray.Length;
         }
         else
         {
             nextIndex = (currentIndex - 1 + bodyPartData.meshArray.Length) % bodyPartData.meshArray.Length;
         }
       
         // Imposta la prossima mesh
         bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[nextIndex];
         _bodyPartData = bodyPartData;
    }

    public void LoadAvatar()
    {
        
        SaveAvatarObject avatarObject = PlayerPrefsManager.LoadAvatar();
            
        foreach (BodyPartTypeIndex bodyPartTypeIndex in avatarObject.bodyPartTypeIndexList){
            BodyPartData bodyPartData = GetBodyPartData(bodyPartTypeIndex.bodyPartType, bodyPartDataArray);   
            bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[bodyPartTypeIndex.index];
        }
    }

    public void SaveBodyPartData()
    {
        PlayerPrefsManager.SaveAvatar(_bodyPartData);
    }

    public void ResetBodyPartData()
    {
        SetBodyPartMesh(BodyPartType.Body, _firstBodyMesh);
        SetBodyPartMesh(BodyPartType.Head, _firstHeadMesh);
    }

    private void SetBodyPartMesh(BodyPartType type, Mesh mesh)
    {
        if (type == BodyPartType.Head)
        {
            BodyPartData headData = GetBodyPartData(BodyPartType.Head, bodyPartDataArray);
            headData.skinnedMeshRenderer.sharedMesh = mesh;
        }
        else if (type == BodyPartType.Body)
        {
            BodyPartData bodyData = GetBodyPartData(BodyPartType.Body, bodyPartDataArray);
            bodyData.skinnedMeshRenderer.sharedMesh = mesh;
        } 
    }

    private int GetCurrentMeshIndex(BodyPartData bodyPartData)
    {
        if (bodyPartData == null || bodyPartData.meshArray == null || bodyPartData.meshArray.Length == 0)
        {
            Debug.LogWarning("Mesh array is null or empty for the specified body part.");
            return -1;
        }
    
        for (int i = 0; i < bodyPartData.meshArray.Length; i++)
        {
            if (bodyPartData.skinnedMeshRenderer.sharedMesh == bodyPartData.meshArray[i])
            {
                return i;
            }
        }
        return -1;
    }
    public static BodyPartData GetBodyPartData(BodyPartType bodyPartType, BodyPartData[] bodyPartDataArrayTemp) {
        foreach (BodyPartData bodyPartData in bodyPartDataArrayTemp) {
            if (bodyPartData.bodyPartType == bodyPartType) {
                return bodyPartData;
            }
        }
        return null;
    }
    

}
