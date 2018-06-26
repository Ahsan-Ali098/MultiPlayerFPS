using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShot : NetworkBehaviour 
{

    private const string PLAYER_TAG = "Player";


    
    private  PlayerWeapons currentWeapon;
    private WeaponManager weaponManager;


    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;



	// Use this for initialization
	void Start () 
    {
		if(cam==null)
        {
            Debug.LogError("PlayerShoot: no camera referenced");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();


	}
	
	// Update is called once per frame
	void Update () 
    {
        currentWeapon = weaponManager.GetCurrentWeapon(); 

        if(currentWeapon.fireRate<=0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
            else
            {
                if(Input.GetButtonDown("Fire1"))
                {
                    InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
                
                }
                else if(Input.GetButtonUp("Fire1"))
                {
                    CancelInvoke("Shoot");
                }
            }
        }
	    
	}

    //Is called on server when a player Shoots.     
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //Is called on the server when we hit somthing
    //takes in the hit point and the surface Normal
    [Command]
    void CmdOnHit(Vector3 _pos,Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    //Its is called on all Clients
    //here we can spawn in cool effects
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos,Vector3 _normal)
    {
       
        GameObject _hitEffect=(GameObject) Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    //Is called on all cliets when we need to do a shoot effect. 
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }


    [Client]
    void Shoot()
    {

        if(!isLocalPlayer)
        {
            return;
        }
        
        //We are shooting,call onShoot methid on the Server.
        CmdOnShoot();

        Debug.Log("SHOOT!!");
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }

            //we hit something ,call the OnHit method on server.
            CmdOnHit(_hit.point, _hit.normal);
        }
    }

    [Command]
    void CmdPlayerShot(string _playerID,int _damage)
    {
        Debug.Log(_playerID + " has been shot.");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}
