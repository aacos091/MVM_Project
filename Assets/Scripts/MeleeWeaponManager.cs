using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponManager : MonoBehaviour
{
    [SerializeField] public bool _knifeEquipped, _baseballBatEquipped, _crowbarEquipped;

    public MeleeWeapon meleeComponent;
    
    public enum MeleeWeapons
    {
        Knife,
        BaseballBat,
        Crowbar
    };

    public MeleeWeapons currentMeleeWeapon;
    
    [Header("Knife Attributes")]
    public Transform knifeAttackPos;
    public float knifeAttackRate;
    public float knifeRange;
    public int knifeDamage;

    [Header("Baseball Bat Attributes")] 
    public Transform baseballBatAttackPos;
    public float baseballBatAttackRate;
    public float baseballBatRange;
    public int baseballBatDamage;
    
    [Header("Crowbar Attributes")]
    public Transform crowbarAttackPos;
    public float crowbarAttackRate;
    public float crowbarRange;
    public int crowbarDamage;
    
    // Start is called before the first frame update
    void Start()
    {
        meleeComponent = GetComponentInChildren<MeleeWeapon>();
        currentMeleeWeapon = MeleeWeapons.Knife;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) && !_knifeEquipped)
        {
            currentMeleeWeapon = MeleeWeapons.Knife;
            Debug.Log("Knife equipped");
        } 
        else if (Input.GetKeyDown(KeyCode.Keypad2) && !_baseballBatEquipped)
        {
            currentMeleeWeapon = MeleeWeapons.BaseballBat;
            Debug.Log("Baseball bat equipped");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) && !_crowbarEquipped)
        {
            currentMeleeWeapon = MeleeWeapons.Crowbar;
            Debug.Log("Crowbar equipped");
        }
        
        switch (currentMeleeWeapon)
        {
            case MeleeWeapons.Knife:
                meleeComponent.attackPos = knifeAttackPos;
                meleeComponent.startTimeBetweenAttack = knifeAttackRate;
                meleeComponent.attackRange = knifeRange;
                meleeComponent.damage = knifeDamage;
                _knifeEquipped = true;
                _baseballBatEquipped = false;
                _crowbarEquipped = false;
                break;
            case MeleeWeapons.BaseballBat:
                meleeComponent.attackPos = baseballBatAttackPos;
                meleeComponent.startTimeBetweenAttack = baseballBatAttackRate;
                meleeComponent.attackRange = baseballBatRange;
                meleeComponent.damage = baseballBatDamage;
                _knifeEquipped = false;
                _baseballBatEquipped = true;
                _crowbarEquipped = false;
                break;
            case MeleeWeapons.Crowbar:
                meleeComponent.attackPos = crowbarAttackPos;
                meleeComponent.startTimeBetweenAttack = crowbarAttackRate;
                meleeComponent.attackRange = crowbarRange;
                meleeComponent.damage = crowbarDamage;
                _knifeEquipped = false;
                _baseballBatEquipped = false;
                _crowbarEquipped = true;
                break;
            default:
                Debug.Log("Invalid Selection");
                break;
        }
    }
}
