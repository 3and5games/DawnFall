﻿using UnityEngine;
using UnityEditor;
using Invector;

// MELEE COMBAT FEATURES
public partial class vMenuComponent
{
    [MenuItem("Invector/Melee Combat/Components/MeleeManager")]
    static void MeleeManagerMenu()
    {
        if (Selection.activeGameObject)
            Selection.activeGameObject.AddComponent<vMeleeManager>();
        else
            Debug.Log("Please select a vCharacter to add the component.");
    }

    [MenuItem("Invector/Melee Combat/Components/WeaponHolderManager (Player Only)")]
    static void WeaponHolderMenu()
    {
        if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Invector.CharacterController.vThirdPersonInput>()!=null)
            Selection.activeGameObject.AddComponent<Invector.ItemManager.vWeaponHolderManager>();
        else
            Debug.Log("Please select the Player to add the component.");
    }
    [MenuItem("Invector/Melee Combat/Components/LockOn (Player Only)")]
    static void LockOnMenu()
    {
        if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Invector.CharacterController.vThirdPersonInput>() != null)
            Selection.activeGameObject.AddComponent<vLockOn>();
        else
            Debug.Log("Please select a Player to add the component.");
    }
}