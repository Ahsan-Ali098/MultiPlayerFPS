using UnityEngine.Networking;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private Transform weaponHolder;


    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private PlayerWeapons primaryWeapon;

    private PlayerWeapons currentWeapon;
    private WeaponGraphics currentGraphics;
    

    

	void Start () 
    {
        EquipWeapon(primaryWeapon);

	}

    public PlayerWeapons GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }
	
    void EquipWeapon(PlayerWeapons _weapon)
    {
        currentWeapon = _weapon;
       GameObject _weaponIns=(GameObject)Instantiate(_weapon.graphics,weaponHolder.position,weaponHolder.rotation);
       _weaponIns.transform.SetParent(weaponHolder);


       currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();

        if(currentGraphics==null)
        {
            Debug.LogError("No weapon Graphics on the weapon Object: " + _weaponIns.name);
        }


       if (isLocalPlayer)
       {
           UTil.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
       }
            
    }
    void Update()
    {

    }
}


