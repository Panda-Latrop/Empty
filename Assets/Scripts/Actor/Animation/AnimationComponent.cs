using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected Animator animator;
    public Animator Animator { get => animator; }

    protected void OnEnable()
    {
        animator.enabled = true;
    }
    protected void OnDisable()
    {
        animator.enabled = false;
    }

    public void ChanegeController(RuntimeAnimatorController animatorController)
    {
        animator.runtimeAnimatorController = animatorController;
    }
    public void Play(string animation, int layer = 0)
    {
        animator.Play(animation, layer);
    }
    public void Play(int animationHash, int layer = 0)
    {
        animator.Play(animationHash, layer);
    }
    public float GetLayerWeight(int layer)
    {
      return animator.GetLayerWeight(layer);
    }
    public void SetLayerWeight(int layer,float weight)
    {
        animator.SetLayerWeight(layer, weight);
    }
    public abstract void AnimatorMessage(string message, int value = 0);

    public virtual JSONObject Save(JSONObject jsonObject)
    {
        jsonObject.Add("enabled", new JSONBool(enabled));
        SaveSystem.AnimatorSave(jsonObject, "main", animator);
        return jsonObject;
    }
    public virtual JSONObject Load(JSONObject jsonObject)
    {
        enabled = jsonObject["enabled"].AsBool;
        SaveSystem.AnimatorLoad(jsonObject, "main", animator);
        return jsonObject;
    }
}
