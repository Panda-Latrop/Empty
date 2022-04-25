using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum AmmunitionType
{
    bàse,
    pistol,
    smg,
    assault,
    rifle,
    shotgun,
    mg,
    energy,

}
public class AmmunitionComponent : MonoBehaviour
{

    public class AmmunitionClass
    {

        protected int ammo;
        protected int max;

        public bool IsFull => ammo >= max;
        public bool IsEmpty => ammo <= 0;
        public int Ammo => ammo;
        public AmmunitionClass(int max)
        {
            this.max = max;
        }
        public AmmunitionClass(int max, bool fill)
        {
            this.max = max;
            if(fill)
            ammo = max;
        }
        public void Add(int ammo)
        {
            this.ammo = Mathf.Clamp(this.ammo + ammo, 0, max);
        }
        public void Remove(int ammo)
        {

            this.ammo = Mathf.Clamp(this.ammo - ammo, 0, max);
        }
        public void Fill()
        {
            ammo = max;
        }
    }
    [SerializeField]
    protected bool infiniteAmmo = false;
    protected Dictionary<AmmunitionType,AmmunitionClass> ammunition = new Dictionary<AmmunitionType, AmmunitionClass>();


    [ContextMenu("Log Ammo")]
    private void Debug_LogAmmo()
    {
        StringBuilder strb = new StringBuilder();
        foreach (var item in ammunition)
        {
            strb.Append(item.Key.ToString()).Append(" = ").Append(item.Value.Ammo).Append('\n');
        }
        Debug.Log(strb.ToString());
    }

    public AmmunitionComponent() : base()
    {
        ammunition.Add(AmmunitionType.bàse, new AmmunitionClass(1000, true));
        ammunition.Add(AmmunitionType.pistol, new AmmunitionClass(120, true));
        ammunition.Add(AmmunitionType.smg, new AmmunitionClass(300, true));
        ammunition.Add(AmmunitionType.assault, new AmmunitionClass(300, true));
        ammunition.Add(AmmunitionType.rifle, new AmmunitionClass(60, true));
        ammunition.Add(AmmunitionType.shotgun, new AmmunitionClass(60, true));
        ammunition.Add(AmmunitionType.mg, new AmmunitionClass(300, true));
        ammunition.Add(AmmunitionType.energy, new AmmunitionClass(1000, true));
    }
    public bool IsFull(AmmunitionType type)
    {
        return ammunition[type].IsFull;
    }
    public bool IsEmpty(AmmunitionType type)
    {
        return ammunition[type].IsEmpty;
    }
    public void Add(AmmunitionType type,int ammo)
    {
        ammunition[type].Add(ammo);
    }
    public void Remove(AmmunitionType type, int ammo)
    {
        ammunition[type].Remove(ammo);
    }
    public AmmunitionClass Get(AmmunitionType type)
    {
        return ammunition[type];
    }
    public void Restore()
    {
        foreach (var item in ammunition)
        {
            item.Value.Fill();
        }
    }
}
