using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerCustomization : MonoBehaviour
{
        //private string PLAYER_PREFS_SAVE = "PlayerCustomization";        
        [SerializeField] private BodyPartData[] bodyPartDataArray;
 
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
  public void ChangeBodyPart(BodyPartType bodyPartType, bool forward ){
    /*BodyPartData bodyPartData = GetBodyPartData(bodyPartType);
    int meshIndex = System.Array.IndexOf(bodyPartData.meshArray, bodyPartData.skinnedMeshRenderer.sharedMesh);
    bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[(meshIndex +1) % bodyPartData.meshArray.Length];
  } */
     BodyPartData bodyPartData = GetBodyPartData(bodyPartType);
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
     private BodyPartData GetBodyPartData(BodyPartType bodyPartType) {
            foreach (BodyPartData bodyPartData in bodyPartDataArray) {
                if (bodyPartData.bodyPartType == bodyPartType) {
                    return bodyPartData;
                }
            }
            return null;
        }
/*    
    [Serializable]
    public class BodyPartTypeIndex
    {
        public BodyPartType bodyPartType;
        public int index;
    }

    [Serializable]    
    public class SaveObject{
        public List<BodyPartTypeIndex> bodyPartTypeIndexList;
    }

    public void Save(){
        List<BodyPartTypeIndex> bodyPartTypeIndexList = new List<BodyPartTypeIndex>();  
    
        foreach (BodyPartType bodyPartType in Enum.GetValues(typeof(BodyPartType))) {
            BodyPartData bodyPartData = GetBodyPartData(bodyPartType);
            int meshIndex = Array.IndexOf(bodyPartData.meshArray,bodyPartData.skinnedMeshRenderer.sharedMesh);

            bodyPartTypeIndexList.Add(new BodyPartTypeIndex {
                bodyPartType = bodyPartType,
                index = meshIndex,
            });
        }

        SaveObject saveObject = new SaveObject {
            bodyPartTypeIndexList = bodyPartTypeIndexList,
        };

        string json = JsonUtility.ToJson(saveObject);
        Debug.Log(json);
        PlayerPrefs.SetString(PLAYER_PREFS_SAVE, json);
    
    }

    public void Load()
    {
        string json = PlayerPrefs.GetString(PLAYER_PREFS_SAVE);
        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json); 

        foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList){
            BodyPartData bodyPartData = GetBodyPartData(bodyPartTypeIndex.bodyPartType);   
            bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[bodyPartTypeIndex.index];
        }
    }
*/
}
