using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetectorComponent : MonoBehaviour
{
    [SerializeField]
    protected ItemActor item;
    [SerializeField]
    protected Team teamCanPikup;
    protected void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;
        if ((1 << layer) == (1 << 8))
        {
            IPerceptionTarget target = default;
            if((target = other.GetComponent<IPerceptionTarget>()) != null)
            {
                if (target.Self.Health.Team.Equals(teamCanPikup) && (target.Self is Character))
                {
                    item.Pickup(target.Self as Character);
                }
            }
        }
    }
}
