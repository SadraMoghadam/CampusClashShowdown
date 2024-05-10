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
public class SaveObject{
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

        private void Awake()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.PlayerCustomization.ToString()))
            {
                PlayerPrefsManager.LoadAvatar();
            }
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
             PlayerPrefsManager.SaveAvatar(bodyPartData);
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
