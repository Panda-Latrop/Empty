using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Footstep Map", menuName = "Audio/FootstepMap", order = 3)]
public class AudioFootstepMapScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class AudioFootstepPair 
    {
        [SerializeField]
        protected PhysicMaterial[] materials;
        [SerializeField]
        protected AudioCueScriptableObject sound;

        public bool Contain(PhysicMaterial material)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                //Debug.Log((materials[i].Equals(material)) + " " + materials[i].name +" "+ material.name);
                if (materials[i] == material)
                    return true;
            }
            return false;
        }
        public AudioCueScriptableObject Sound => sound;
    }
    [SerializeField]
    protected AudioCueScriptableObject defaultFootstep;
    [SerializeField]
    protected List<AudioFootstepPair> footsteps;

    public AudioCueScriptableObject GetSound(PhysicMaterial material)
    {
        var f = footsteps.Find(x => x.Contain(material));
        if (f == null)
            return defaultFootstep;
        return f.Sound;
        //return footsteps.Find(x => x.Contain(material)).Sound;
    }
}
