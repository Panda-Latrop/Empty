using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void WeaponHolderChangeDelegate(WeaponActor weapon);
public delegate void WeaponHolderReloadDelegate(ReloadState reloadState);
public class WeaponHolderComponent : MonoBehaviour, ISaveableComponent
{
    protected Pawn owner;
    [SerializeField]
    protected bool isFirstPerson;
    [SerializeField]
    protected Transform attachSocket;
    [SerializeField]
    protected List<WeaponActor> slots = new List<WeaponActor>();
    protected int maxAvailableSlots = 9;
    [SerializeField]
    protected List<bool> slotEquips = new List<bool>();
    [SerializeField]
    protected int currentSlot = -1;
    protected WeaponActor weapon;
    [SerializeField]
    protected AmmunitionComponent ammunition;

    protected bool isChanging;
    protected int toSlot;
    protected float nextChange;

    protected WeaponHolderChangeDelegate OnChange,OnEquip;
    protected WeaponHolderReloadDelegate OnReload;

    public bool HasWeapon => currentSlot >= 0;
    public bool IsFirstPerson => isFirstPerson;
    public int MaxAvailableSlots => maxAvailableSlots;
    public void Restore()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Restore();
        }
        ammunition.Restore();
    }
    public void SetOwner(Pawn owner)
    {
        this.owner = owner;
    }
    public void Change()
    {
        int toSlot = currentSlot + 1;
        if (toSlot >= maxAvailableSlots || toSlot >= slots.Count || toSlot >= slotEquips.Count)
        {
            toSlot = 0;
        }
        Change(toSlot);

    }
    public void Change(int toSlot)
    {
        if (!isChanging && currentSlot != toSlot && toSlot < slotEquips.Count && (toSlot < 0 || slotEquips[toSlot]))
        {
            isChanging = true;
            enabled = true;
            this.toSlot = toSlot;           
            if (currentSlot >= 0)
                OnHideWeapon();
            else
                OnEquipWeapon();
        }
    }
    protected void OnHideWeapon()
    {
        weapon.OnChange();
        CallOnChange(weapon);
        nextChange = Time.time + weapon.ChangedTime;
    }
    protected void OnEquipWeapon()
    {
        if (currentSlot >= 0)
            weapon.gameObject.SetActive(false);
        if (toSlot >= 0)
        {
            weapon = slots[toSlot];
            CallOnEquip(weapon);
            weapon.gameObject.SetActive(true);
        }
        nextChange = Time.time + weapon.ChangedTime;
        currentSlot = toSlot;
        toSlot = -1;
    }
    public virtual void SetFire(bool fire)
    {
        if (!isChanging && currentSlot >= 0)
        {
            weapon.SetFire(fire);
        }
    }
    public virtual ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (!isChanging && currentSlot >= 0 )
        {
            return weapon.Shoot(position, direction);
        }
        return ShootState.none;
    }
    public virtual bool Reload(bool reload = true)
    {
        if (!isChanging && currentSlot >= 0 && !weapon.IsFire)
        {
            bool wasReload = !weapon.Ammo.IsReload;
           // reload = reload && !weapon.Ammo.IsReload && !weapon.IsFire;
            weapon.Reload(reload);
            enabled = true;
            return reload && wasReload && weapon.Ammo.IsReload;
        }
        return false;
    }
    public virtual void Stop()
    {
        if (!isChanging && currentSlot >= 0)
        {
            weapon.Stop();
        }
    }
    protected virtual void Start()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= slotEquips.Count || !slotEquips[i])
            {
                WeaponActor weapon = null;
                Add(Instantiate(slots[i]),ref weapon, false);
            }
            else
            {
                slots[i].SetOwner(owner);
                slots[i].SetAmmunition(ammunition);
            }
            if (i == currentSlot)
            {
                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }          
            if (currentSlot >= 0)
            {
                weapon = slots[currentSlot];
                CallOnEquip(weapon);
            }
                
        }
        if (slots.Count >= maxAvailableSlots)
            maxAvailableSlots = slots.Count;
    }

    public bool Add(WeaponActor prefab, ref WeaponActor weapon, int clip, bool change = true)
    {
        if(Add(prefab,ref weapon, change))
        {
            weapon.Ammo.Clip = clip;
            return true;
        }
        return false;
    }
    public bool Add(WeaponActor prefab, ref WeaponActor weapon, bool change = true)
    {
        if (!isChanging)
        {
            int slot = GetEmptySlot();
            if (slot == currentSlot)
                Drop(currentSlot, owner.Center + transform.forward, Random.rotation, 200.0f, false);
            weapon = GameInstance.Instance.PoolManager.Pop(prefab) as WeaponActor;
            weapon.SetOwner(owner);
            weapon.SetAmmunition(ammunition);
            weapon.SetParent(attachSocket);
            weapon.Transform.localPosition = (Vector3.zero);
            weapon.Transform.localRotation = (Quaternion.identity);
            weapon.Transform.localScale = (Vector3.one);
            weapon.gameObject.SetActive(false);
            if (IsFirstPerson)
                weapon.DisableShadow();
            else
                weapon.EnableShadow();
            slots[slot] = weapon;
            slotEquips[slot] = true;
            if (change)
            {
                if (slot == currentSlot)
                    currentSlot = -1;
                Change(slot);
            }
            return true;
        }
        return false;
    }
    public bool Add(WeaponActor prefab, int slot, ref WeaponActor weapon,  bool change = true)
    {
        if (!isChanging)
        {
            if (slot == currentSlot)
                Drop(currentSlot, owner.Center + transform.forward, Random.rotation, 200.0f, false);
            weapon = GameInstance.Instance.PoolManager.Pop(prefab) as WeaponActor;
            weapon.SetOwner(owner);
            weapon.SetAmmunition(ammunition);
            weapon.SetParent(attachSocket);
            weapon.Transform.localPosition = (Vector3.zero);
            weapon.Transform.localRotation = (Quaternion.identity);
            weapon.Transform.localScale = (Vector3.one);
            weapon.gameObject.SetActive(false);


            if (IsFirstPerson)
                weapon.DisableShadow();
            else
                weapon.EnableShadow();

            if (slots.Count > slot)
                slots[slot] = weapon;
            else
                slots.Insert(slot, weapon);
            if (slotEquips.Count > slot)
                slotEquips[slot] = true;
            else
                slotEquips.Insert(slot, true);


           
           

            if (change)
            {
                if (slot == currentSlot)
                    currentSlot = -1;
                Change(slot);
            }
            return true;
        }
        return false;
    }
    protected void Drop(int weaponId,Vector3 position, Quaternion rotation, float power = 100.0f, bool change = true)
    {
        WeaponActor weapon = slots[weaponId];
        slots[weaponId] = null;
        slotEquips[weaponId] = false;
        weapon.Drop(position,rotation,transform.forward,power);     
        if (change)
        {
            int slot = GetEquipSlot();
            if (slot == currentSlot)
                currentSlot = -1;
            Change(slot);
        }
        else
        {
            currentSlot = -1;
        }
    }
    public bool AddAmmo(AmmunitionType type, int ammo)
    {
        if (!ammunition.IsFull(type))
        {
            ammunition.Add(type, ammo);
            return true;
        }
        return false;

    }
    public bool AmmoIsFull(AmmunitionType type)
    {
        return ammunition.IsFull(type);
    }
    protected int GetEmptySlot()
    {
        int slot = -1;
        if (slotEquips.Count < maxAvailableSlots)
        {
            slot = slotEquips.Count;
            slotEquips.Add(false);
            slots.Add(null);
            return slot;
        }
        else
        {
            for (int i = 0; i < slotEquips.Count; i++)
            {
                if (!slotEquips[i])
                {
                    slot = i;
                    return slot;
                }                 
            }
        }
        slot = currentSlot;
        return slot;
    }
    protected int GetEquipSlot()
    {
        int slot = -1;
        for (int i = 0; i < slotEquips.Count; i++)
        {
            if (slotEquips[i])
                slot = i;
        }
        return slot;
    }
    public WeaponActor GetWeapon() => weapon;
    public bool GetWeapon(int slot, ref WeaponActor weapon) 
    {
        if(slot >= 0 && slot < slotEquips.Count && slotEquips[slot])
        {
            weapon = slots[slot];
            return true;
        }
        return false;
    }
    public bool HasSameWeapon(WeaponActor weapon)
    {
        for (int i = 0; i < slotEquips.Count; i++)
        {
            if (slotEquips[i] && slots[i].Specifier.Equals(weapon.Specifier))
                return true;
        }
        return false;
    }
    public void SetDamageMultiply(float damageMultiply = 1.0f)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slotEquips[i])
                slots[i].DamageMultiply = damageMultiply;
        }
    }
    protected void Update()
    {
        if (isChanging)
        {
            if (Time.time >= nextChange)
            {
                if (toSlot >= 0)
                {
                    OnEquipWeapon();
                }
                else
                {
                    isChanging = false;
                }
            }
        }
        
    }
    protected void LateUpdate()
    {
        if(currentSlot >= 0)
        {
            if (weapon.Ammo.IsReload && weapon.Ammo.ReloadState.Equals(ReloadState.restart))
            {
                CallOnReload(ReloadState.restart);
            }
        }

        if (!isChanging && ((currentSlot >= 0 && !weapon.Ammo.IsReload) || currentSlot < 0))
            enabled = false;
    }
    public void CallOnChange(WeaponActor weapon)
    {
        OnChange?.Invoke(weapon);
    }
    public void BindOnChange(WeaponHolderChangeDelegate action)
    {
        OnChange += action;
    }
    public void UnbindOnChange(WeaponHolderChangeDelegate action)
    {
        OnChange -= action;
    }
    public void ClearOnChange()
    {
        OnChange = null;
    }
    public void CallOnEquip(WeaponActor weapon)
    {
        OnEquip?.Invoke(weapon);
    }
    public void BindOnEquip(WeaponHolderChangeDelegate action)
    {
        OnEquip += action;
    }
    public void UnbindOnEquip(WeaponHolderChangeDelegate action)
    {
        OnEquip -= action;
    }
    public void ClearOnEquip()
    {
        OnEquip = null;
    }
    public void CallOnReload(ReloadState state)
    {
        OnReload?.Invoke(state);
    }
    public void BindOnReload(WeaponHolderReloadDelegate action)
    {
        OnReload += action;
    }
    public void UnbindOnReload(WeaponHolderReloadDelegate action)
    {
        OnReload -= action;
    }
    public void ClearOnReload()
    {
        OnReload = null;
    }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("current", new JSONNumber(currentSlot));
        JSONArray weaponsJArray = new JSONArray();
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slotEquips[i] && slots[i] != null)
                {
                    JSONObject slotJObject = new JSONObject();
                    slotJObject.Add("slot", new JSONNumber(i));
                    weaponsJArray.Add(slots[i].Save(slotJObject));
                }            
            }
        }
        jsonObject.Add("weapons", weaponsJArray);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        currentSlot = -1;
        JSONArray weaponsJArray = jsonObject["weapons"].AsArray;
        {
            for (int i = 0; i < weaponsJArray.Count; i++)
            {
                JSONObject slotJObject = weaponsJArray[i].AsObject;
                int slot = slotJObject["slot"].AsInt;
                WeaponActor weapon = default;
                if (slotEquips.Count <= slot || !slotEquips[slot])
                    Add(Resources.Load<WeaponActor>(slotJObject["prefab"]), ref weapon, false);              
                slots[slot].Load( slotJObject);
            }
        }
        currentSlot = jsonObject["current"].AsInt;
        if (currentSlot >= 0 && currentSlot < slots.Count)
        {
            slots[currentSlot].gameObject.SetActive(true);
        }
        return jsonObject;
    }
}
