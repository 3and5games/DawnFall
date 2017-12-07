using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.ItemManager
{
    [vClassHeader("Weapon Holder Manager", "Create a new empty object inside a bone and add the vWeaponHolder script")]
    public class vWeaponHolderManager : vMonoBehaviour
    {
        public vWeaponHolder[] holders = new vWeaponHolder[0];
        [HideInInspector]
        public bool inEquip;
        [HideInInspector]
        public vItemManager itemManager;

        public Dictionary<string, List<vWeaponHolder>> holderAreas = new Dictionary<string, List<vWeaponHolder>>();

        void OnDrawGizmosSelected()
        {
            holders = GetComponentsInChildren<vWeaponHolder>(true);
        }

        void Start()
	    {
            itemManager = GetComponent<vItemManager>();
            if (itemManager)
            {
                itemManager.onEquipItem.AddListener(EquipWeapon);
                itemManager.onUnequipItem.AddListener(UnequipWeapon);
	            
                holders = GetComponentsInChildren<vWeaponHolder>(true);
                if (holders != null)
                {
                    foreach (vWeaponHolder holder in holders)
                    {
                        if (!holderAreas.ContainsKey(holder.equipPointName))
                        {
                            holderAreas.Add(holder.equipPointName, new List<vWeaponHolder>());
                            holderAreas[holder.equipPointName].Add(holder);
                        }
                        else
                        {
                            holderAreas[holder.equipPointName].Add(holder);
                        }

                        holder.SetActiveHolder(false);
                        holder.SetActiveWeapon(false);
                    }
                }
            }
        }

        public void EquipWeapon(vEquipArea equipArea, vItem item)
        {
            var slotsInArea = equipArea.ValidSlots;
            if (slotsInArea != null && slotsInArea.Count > 0 && holderAreas.ContainsKey(equipArea.equipPointName))
            {
                for (int i = 0; i < slotsInArea.Count; i++)
                {
                    if (slotsInArea[i].item != null)
                    {
	                    var holder = holderAreas[equipArea.equipPointName].Find(h => slotsInArea[i].item.id == h.itemID && 
	                    (equipArea.currentEquipedItem != item && equipArea.currentEquipedItem != slotsInArea[i]));
	                    
                        if (holder)
                        {
	                        holder.SetActiveHolder(true);
                        	holder.SetActiveWeapon(true);
                        }
                    }
                    if (equipArea.currentEquipedItem != null)
                    {
                        var holder = holderAreas[equipArea.equipPointName].Find(h => h.itemID == equipArea.currentEquipedItem.id);
                        if (holder)
                        {
                            holder.equipDelayTime = equipArea.currentEquipedItem.equipDelayTime;
                            StartCoroutine(ActiveHolder(holder, false, (itemManager.inventory != null && itemManager.inventory.isOpen)));
                        }
                    }                   
                }
            }
        }

        public void UnequipWeapon(vEquipArea equipArea, vItem item)
        {
            if (holders.Length == 0 || item == null) return;

            if ((itemManager.inventory != null) && holderAreas.ContainsKey(equipArea.equipPointName))
            {
                var holder = holderAreas[equipArea.equipPointName].Find(h => item.id == h.itemID);
                if (holder)
                {
                    var containsItem = equipArea.ValidSlots.Find(slot => slot.item == item) != null;
                    holder.SetActiveHolder(containsItem);
                    holder.SetActiveWeapon(containsItem);
                }
            }
        }

        float timer;
        IEnumerator ActiveHolder(vWeaponHolder holder, bool activeWeapon, bool immediat = false)
        {
            if (!immediat)
	            inEquip = true;
	        
            timer = holder.equipDelayTime;
            holder.SetActiveHolder(true);

            if (!activeWeapon)
                holder.SetActiveWeapon(true);

            while (timer > 0 && !immediat)
            {
                yield return new WaitForEndOfFrame();
                timer -= Time.deltaTime;
            }

            holder.SetActiveWeapon(activeWeapon);
            inEquip = false;
        }
    }
}
