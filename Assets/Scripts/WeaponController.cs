using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletMode
{
    Normal,
    Shrapnel,
    ShrapnelBullets
}

public enum WeaponMode
{
    Normal,
    Magazin
}

public enum TargetMode
{
    Normal,
    TargetPlayer
}

public class WeaponController : MonoBehaviour
{
    private List<Modifier> activeModifier = new List<Modifier>();

    private GameObject ship;
    private GameManager gameManager;

    public bool IsActiv = true;

    public AudioClip shootSound;

    public float FireTimer = 0f;
    public bool ReadyToShoot = true;
    public BulletController bulletPrefab;

    [Header("Waffen Modus")]
    [Tooltip("Waffenmodus. Bei Magazin wird das Feld Magazin und Nachladezeit verwendet")]
    public WeaponMode waffenModus;
    [Tooltip("Schussmodus. Legt fest, wie sich die Bullet verhält.")]
    public BulletMode schussModus = BulletMode.Normal;

    [Header("Waffen Eigenschaften(Basis)")]
    [Tooltip("Die Geschwindigkeits-Modifikation der Kugel durch die Waffe")]
    public Vector2 Geschwindigkeit;
    [Tooltip("Feuerrate in Sekunden")]
    public float feuerrate;
    [Tooltip("Waffenschaden. Ersetzt BulletController.Schaden")]
    public int waffenschaden = 1;
    
    [Header("Waffen Eigenschaften(Magazin")]
    [Tooltip("Nachladezeit. Zeit bis die Waffe nachgeladen ist.")]
    public float Nachladezeit = 0f;
    [Tooltip("Magazingrösse. Die Größe des Magazins")]
    public int Magazin = 0;
    [Tooltip("Lebensdauer der Kugel")]
    [Range(0.1f, 10f)] public float Lebensdauer = 5f; 

    public float Feuerrate
    {
        get 
        {
            Modifier[] m = GetModifier(PowerUpModifier.weaponFeuerrate);

            float value = feuerrate;

            foreach (Modifier mod in m)
            {
                value -= mod.Wirkung;
            }

            if (ship.tag == "Enemy" || ship.tag == "Boss")
            {
                value = (value / gameManager.SchwierigkeitsModifikator);
            }

            if (value < 0.2f)
                value = 0.2f;

            return value;

        }
    }
    public float Waffenschaden
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.weaponSchaden);

            float value = waffenschaden;

            foreach (Modifier mod in m)
            {
                value += mod.Wirkung;
            }

            if (ship.tag == "Enemy" || ship.tag == "Boss")
            {
                value = (value * gameManager.SchwierigkeitsModifikator);
            }

            if (value < 1 && ship.tag == "Player")
            {
                value = 1;
            }

            return value;

        }
    }

    [Header("Zielmodus")]
    public TargetMode Zielmodus = TargetMode.Normal;

    private int curMagazin;

    private bool IsReloading = false;

    private Modifier[] GetModifier(PowerUpModifier Wert)
    {
        List<Modifier> mod = new List<Modifier>();

        foreach (Modifier m in activeModifier)
        {
            if (Wert == m.wert)
            {
                mod.Add(m);
            }
        }

        return mod.ToArray();
    }

    public void Modify(Modifier mod) 
    {
        Modifier[] active = GetModifier(mod.wert);

        if (active.Length == 0)
        {
            Debug.Log("Waffen PowerUp" + mod.wert.ToString());
            activeModifier.Add(mod);
        }
        else if (mod.IsDauerhaft == true)
        {
            Debug.Log("Waffen PowerUp(Dauerhaft)" + mod.wert.ToString());
            activeModifier.Add(mod);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        try
        {            
            ship = GetComponentInParent<ShipController>().gameObject;
        }
        catch
        {

        }
        curMagazin = Magazin;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Nachladen()
    {
        ReadyToShoot = true;
        FireTimer = 0f;
        IsReloading = false;
        curMagazin = Magazin;
        Debug.Log("Waffe Nachgeladen");
    }

    void UpdateFeuerrate()
    {
        bool IsActiv = false;

        if (GetComponentInParent<ShipController>().tag == "Boss")
        {
            if (GetComponentInParent<BossController>().isInViewport == true)
            {
                IsActiv = true;
            }
        }
        else if(GetComponentInParent<ShipController>().tag == "Enemy")
        {
            if (GetComponentInParent<EnemyController>().isInViewport == true)
            {
                IsActiv = true;
            }
        }
        if(GetComponentInParent<ShipController>().tag == "Player")
        {
            IsActiv = true;
        }

        if (IsActiv == true)
        {
            if (FireTimer <= 0)
            {
                ReadyToShoot = true;
                FireTimer = 0f;
                if (waffenModus == WeaponMode.Magazin)
                {
                    curMagazin--;
                }
            }
            else if (FireTimer > 0)
            {
                ReadyToShoot = false;
                FireTimer -= Time.deltaTime;
            }
        }
    }

    void ModifierUpdate()
    {
        for (int a = 0; a < activeModifier.Count; a++)
        {
            Modifier m = activeModifier[0];
            m.Update();
            if (m.IsDauerhaft == false)
            {
                if (m.timeRunning > m.Dauer)
                {

                    Debug.Log("Waffen PowerUp" + m.wert.ToString() + " entfernt.");
                    // Remove Modifier
                    activeModifier.Remove(m);
                }
            }
        }
    }

    private void Update()
    {
        ModifierUpdate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponentInParent<EnemyController>() != null)
        {
            if (GetComponentInParent<EnemyController>().isInViewport == true || GetComponentInParent<EnemyController>().enabled == false)
            {
                if (waffenModus == WeaponMode.Magazin)
                {
                    if (curMagazin > 0)
                    {
                        UpdateFeuerrate();
                    }
                    else
                    {
                        ReadyToShoot = false;
                        if (IsReloading == false)
                        {
                            Invoke("Nachladen", Nachladezeit);
                            IsReloading = true;
                        }
                    }
                }
                else if (waffenModus == WeaponMode.Normal)
                {
                    UpdateFeuerrate();
                }
            }
        }
    }
}
