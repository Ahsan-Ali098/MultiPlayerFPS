using System.Collections;
using UnityEngine.Networking;
using UnityEngine;


[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }




    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject deadthEffect;

    [SerializeField]
    private GameObject spawnEffect;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;


    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if(isLocalPlayer)
        {
            //switch Cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);      
        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }
    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        
        SetDeFaults();
    }


    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
            return;


        currentHealth -= _amount;
        Debug.Log(transform.name+" now has "+currentHealth+" health");

        if(currentHealth<=0)
        {
            Die();
        }
    }
    private void Die()
    {
        isDead = true;

        //disable the components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Disable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //disable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        //Spawn a deathEffect
       GameObject _gfx=(GameObject) Instantiate(deadthEffect, transform.position, Quaternion.identity);
       Destroy(_gfx, 3f);


        //switch Cameras
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is DEAD!");

        //call some resawpn method
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);


        SetupPlayer();

        Debug.Log(transform.name +" respawned"); 
    }
    public void SetDeFaults()
    {
        isDead = false;


        currentHealth=maxHealth;

        //Enable the components
        for (int i = 0; i <disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable the GameObject
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //Enable the collider
        Collider _col = GetComponent<Collider>();
        if(_col!=null)
        {
            _col.enabled = true;
        }
        

        //create spawnEffect
        GameObject _gfx = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfx, 3f);
    }

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
    //void Update () 
    //{
    //    if(!isLocalPlayer)
    //    {
    //        return;

    //    }
    //    if(Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(99999);
    //    }
		
    //}
}
