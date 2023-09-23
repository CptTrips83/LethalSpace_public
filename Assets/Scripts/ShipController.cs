using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ShipState
{
    Active,
    Destroyed
}

public class ShipController : MonoBehaviour
{
    //public AudioClip sound = null;

    public ShipState shipState = ShipState.Active;
    
    private GameManager gameManager;
    private LevelManager levelManager;
    private Rigidbody2D rb;
    private GameObject player;
    private ScenarioManager scenarioManager;

    public GameObject[] weaponSets;

    private List<Modifier> activeModifier = new List<Modifier>();

    PlayerController playerController;    

    public int curSchildEnergie;
    public int curRumpf;

    private bool IsRumpfRegeneration = false;
    private bool IsSchildAufladung = false;

    
    public WeaponController[] WeaponSlots;
    public GameObject Shield;

    [Header("Pilot Eigenschaften")]
    [Tooltip("Verringert oder verbessert den Schaden")]
    public float treffsicherheit = 1f;
    [Tooltip("Verringert oder vergrößert die Geschwindigkeit der Gegner(Nicht Implementiert)")]
    public float wahrnehmung = 1f;
    [Tooltip("Verringert oder verbessert die die Manövrierfähigkeit")]
    public float fluggeschick = 1f;
    [Tooltip("Ermöglicht die Reparatur des Rumpfes")]
    public int technik = 1;
    [Tooltip("Gibt an, wie lange es dauert bis der Wert bei Technik repariert wird")]
    public float technikZeit = 1f;

    [Header("Schiff Eigenschaften")]
    [Tooltip("Die Beschleunigung des Schiffs")]
    public float beschleunigung = 1f;
    [Tooltip("Maximale Geschwindigkeit des Schiffs")]
    public float maximaleGeschwindigkeit = 1f;
    [Tooltip("Wird durch Schaden verringert. Wenn auf 0, wird der Rumpf angegriffen.")]
    public int schildEnergie = 1;
    [Tooltip("Zeit die bis zur Regenerierung des Schildes benötigt wird in Sekunden.")]
    public float schildAufladung = 10;
    [Tooltip("Rumpfstärke wird durch Schaden verringert, wenn kein Schild mehr vorhanden. Kann durch Pilotenskill regeneriert werden.")]
    public int rumpf = 10;

    public float Treffsicherheit
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.pilotTreffsicherheit);

            float value = treffsicherheit;

            foreach(Modifier mod in m)
            {
                value += mod.Wirkung;
                if (value >= mod.MaxWirkung + treffsicherheit)
                {
                    value = (int)mod.MaxWirkung + treffsicherheit;
                    break;
                }
            }

            if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {   
                value = (value * (gameManager.SchwierigkeitsModifikator + gameManager.scenarioManager.durchlaeufe));
            }

            if (value < 1 && gameObject.tag == "Player")
            {
                value = 1;
            }

            return value;
        }
    }
    public float Wahrnehmung
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.pilotWahrnehmung);

            float value = wahrnehmung;

            foreach (Modifier mod in m)
            {
                value += mod.Wirkung;
                if (value >= mod.MaxWirkung + wahrnehmung)
                {
                    value = (int)mod.MaxWirkung + wahrnehmung;
                    break;
                }
            }

            if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (value * (gameManager.SchwierigkeitsModifikator + gameManager.scenarioManager.durchlaeufe));
            }

            if (value < 1)
            {
                value = 1;
            }

            return value;
        }
    }
    public float Fluggeschick
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.pilotFluggeschick);

            float value = fluggeschick;

            foreach (Modifier mod in m)
            {
                value += mod.Wirkung;
                if (value >= mod.MaxWirkung + fluggeschick)
                {
                    value = (int)mod.MaxWirkung + fluggeschick;
                    break;
                }
            }

            if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (value * (gameManager.SchwierigkeitsModifikator + gameManager.scenarioManager.durchlaeufe));
            }

            if (value < 1)
            {
                value = 1;
            }

            return value;
        }
    }
    public int Technik
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.pilotTechnik);

            int value = technik;

            foreach (Modifier mod in m)
            {
                value += (int)mod.Wirkung;
                if (value >= mod.MaxWirkung + technik)
                {
                    value = (int)mod.MaxWirkung + technik;
                    break;
                }
            }

            if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (int)((float)value * (gameManager.SchwierigkeitsModifikator + gameManager.scenarioManager.durchlaeufe));
            }

            if (value < 1)
            {
                value = 1;
            }

            return value;
        }
    }
    public float TechnikZeit
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.pilotTechnikZeit);

            float value = technikZeit;

            foreach (Modifier mod in m)
            {
                value -= mod.Wirkung;
                if (value <= technikZeit - mod.MaxWirkung)
                {
                    value = technikZeit - mod.MaxWirkung;
                    break;
                }
            }

            if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (value / (gameManager.SchwierigkeitsModifikator + gameManager.scenarioManager.durchlaeufe));
            }

            if (value < 0.2f)
            {
                value = 0.2f;
            }

            return value;
        }
    }
    public float Beschleunigung
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.shipBeschleunigung);

            float value = beschleunigung;

            foreach (Modifier mod in m)
            {
                value += mod.Wirkung;
                if (value >= mod.MaxWirkung + beschleunigung)
                {
                    value = mod.MaxWirkung + beschleunigung;
                    break;
                }
            }

            if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (value * (gameManager.SchwierigkeitsModifikator + (gameManager.scenarioManager.durchlaeufe / 3)));
            }

            if (value < 1)
            {
                value = 1;
            }

            return value;
        }
    }
    public float MaximaleGeschwindigkeit
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.shipMaximaleGeschwindigkeit);

            float value = maximaleGeschwindigkeit;

            foreach (Modifier mod in m)
            {
                value += mod.Wirkung;
                if (value >= mod.MaxWirkung + maximaleGeschwindigkeit)
                {
                    value = mod.MaxWirkung + maximaleGeschwindigkeit;
                    break;
                }
            }

            if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (value * (gameManager.SchwierigkeitsModifikator + (gameManager.scenarioManager.durchlaeufe / 3)));
            }

            if (value < 1)
            {
                value = 1;
            }

            return value;
        }
    }
    public int SchildEnergie
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.shipSchildEnergie);

            int value = schildEnergie;
            
            foreach (Modifier mod in m)
            {
                value += (int)mod.Wirkung;
                if (value >= mod.MaxWirkung + schildEnergie)
                {
                    value = (int)mod.MaxWirkung + schildEnergie;
                    break;
                }
            }

            if(gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (int)((float)value * (gameManager.SchwierigkeitsModifikator + gameManager.scenarioManager.durchlaeufe));
            }

            if (value < 1)
            {
                value = 1;
            }


            return value;
        }
    }
    public float SchildAufladung
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.shipSchildAufladung);

            float value = schildAufladung;

            foreach (Modifier mod in m)
            {
                value -= mod.Wirkung;
                if (value <= schildAufladung - mod.MaxWirkung)
                {
                    value = schildAufladung - mod.MaxWirkung;
                    break;
                }
            }

            if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (value / (gameManager.SchwierigkeitsModifikator + (gameManager.scenarioManager.durchlaeufe / 3)));
            }

            if (value <= 0.2f)
                value = 0.2f;

            return value;
        }
    }
    public int Rumpf
    {
        get
        {
            Modifier[] m = GetModifier(PowerUpModifier.shipRumpf);

            int value = rumpf;

            foreach (Modifier mod in m)
            {
                value += (int)mod.Wirkung;
                if(value >= mod.MaxWirkung + rumpf)
                {
                    value = (int)mod.MaxWirkung + rumpf;
                    break;
                }
            }

            if(gameObject.tag == "Enemy" || gameObject.tag == "Boss")
            {
                value = (int)((float)value * (gameManager.SchwierigkeitsModifikator + gameManager.scenarioManager.durchlaeufe));
            }

            if(value < 1)
            {
                value = 1;
            }
            
            return value;
        }
    }

    private Modifier[] GetModifier(PowerUpModifier Wert)
    {
        List<Modifier> mod = new List<Modifier>();

        foreach (Modifier m in activeModifier)
        {
            if(Wert == m.wert)
            {
                mod.Add(m);                
            }            
        }

        return mod.ToArray();
    }

    public Modifier[] GetWeaponModifier()
    {
        List<Modifier> mod = new List<Modifier>();

        foreach (Modifier m in activeModifier)
        {
            if (m.powerupType != PowerUpType.Buff)
            {
                mod.Add(m);
            }
        }

        return mod.ToArray();
    }

    private Modifier[] GetWeaponModifier(PowerUpType Wert)
    {
        List<Modifier> mod = new List<Modifier>();

        foreach (Modifier m in activeModifier)
        {
            if (Wert == m.powerupType)
            {
                mod.Add(m);
                break;
            }
        }

        return mod.ToArray();
    }

    public void switchWeapon(string weaponName, bool value)
    {

        foreach(GameObject g in weaponSets)
        {
            if(g.name == weaponName)
            {
                WeaponController[] weapons = g.GetComponentsInChildren<WeaponController>();

                foreach (WeaponController weapon in weapons)
                {
                    weapon.IsActiv = value;
                }                
            }
        }
    }

    public void Modify(Modifier mod)
    {
        if (mod.powerupType == PowerUpType.Buff)
        {
            if (mod.wert != PowerUpModifier.weaponFeuerrate && mod.wert != PowerUpModifier.weaponSchaden)
            {
                Modifier[] active = GetModifier(mod.wert);

                if (active.Length == 0)
                {
                    Debug.Log("PowerUp" + mod.wert.ToString());
                    activeModifier.Add(mod);
                }
                else if (mod.IsDauerhaft == true)
                {
                    Debug.Log("PowerUp(Dauerhaft)" + mod.wert.ToString());
                    activeModifier.Add(mod);
                }
                GetComponent<PlayerController>().showPowerUpText(mod.powerupText + "+");
            }
            else
            {
                foreach (GameObject g in GetActiveWeapons())
                {
                    g.GetComponent<WeaponController>().Modify(mod);
                }
            }
        }
        else
        {
            Modifier[] active = GetWeaponModifier();

            bool IsnewWeapon = true;

            if (active.Length != 0)
            {
                foreach (Modifier m in active)
                {
                    if (m.powerupType != mod.powerupType)
                    {
                        switchWeapon(m.powerupType.ToString(), false);
                        activeModifier.Remove(m);
                    }
                    else
                    {
                        m.Wirkung += mod.Wirkung;

                        if(m.Wirkung > (mod.MaxWirkung))
                        {
                            m.Wirkung = mod.MaxWirkung;
                        }
                        IsnewWeapon = false;
                    }
                }
            }

            if (IsnewWeapon == true)
            {
                activeModifier.Add(mod);

                switchWeapon("Weapon1", false);
                switchWeapon(mod.powerupType.ToString(), true);
            }
        }
    }

    public GameObject[] GetActiveWeapons()
    {
        List<GameObject> weapons = new List<GameObject>();

        foreach(WeaponController weapon in GetComponentsInChildren<WeaponController>())
        {
            if(weapon.enabled == true)
            {                
                weapons.Add(weapon.gameObject);
            }
        }

        return weapons.ToArray();
    }

    public GameObject[] GetInActiveWeapons()
    {
        List<GameObject> weapons = new List<GameObject>();

        foreach (WeaponController weapon in GetComponentsInChildren<WeaponController>())
        {
            if (weapon.enabled == false)
            {
                weapons.Add(weapon.gameObject);
            }
        }

        return weapons.ToArray();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.tag != "Player")
        {
            foreach(Collider2D col1 in gameObject.GetComponents<Collider2D>())
            {
                foreach (Collider2D col2 in collision.gameObject.GetComponents<Collider2D>())
                {
                    Physics2D.IgnoreCollision(col1, col2);
                }
            }
        }
        else
        {
            if(collision.gameObject.tag != "BossExtension")
            {
                foreach (Collider2D col1 in gameObject.GetComponents<Collider2D>())
                {
                    foreach (Collider2D col2 in collision.gameObject.GetComponents<Collider2D>())
                    {
                        Physics2D.IgnoreCollision(col1, col2);
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (gameObject.tag == "Player")
            {
                if (collision.gameObject.tag == "PlayerBullet")
                {

                }
                else if (collision.gameObject.tag == "EnemyBullet" || collision.gameObject.tag == "BossBullet" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss")
                {
                    BulletController bullet = collision.GetComponent<BulletController>();
                    if (bullet != null)
                    {
                        if (bullet.isImmortal == true)
                        {
                            if (bullet.IsMakingdamage == true)
                            {
                                Hit(collision.gameObject);
                                bullet.IsMakingdamage = false;
                            }
                        }
                    }
                }
                else if (collision.gameObject.tag != "Wall")
                {

                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (gameObject.tag == "Player")
        {
            if(collision.gameObject.tag == "PlayerBullet")
            {
                
            }
            else if(collision.gameObject.tag == "EnemyBullet" || collision.gameObject.tag == "BossBullet" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss")
            {
                
                Hit(collision.gameObject);
            }
            else if(collision.gameObject.tag != "Wall")
            {
                
            }
        }

        if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
        {
            if (collision.gameObject.tag == "EnemyBullet")
            {
                
            }
            else if (collision.gameObject.tag == "PlayerBullet" || collision.gameObject.tag == "Player")
            {
                Hit(collision.gameObject);                              
            }
            else if (collision.gameObject.tag != "Wall")
            {
                
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        scenarioManager = FindObjectOfType<ScenarioManager>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        curSchildEnergie = SchildEnergie;
        curRumpf = Rumpf;

        if(gameObject.tag != "Player")
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
        else
        {
            playerController = GetComponent<PlayerController>();
        }

        // Schiff in die richtige Richtung drehen
        if (gameObject.tag == "Player" || gameObject.name == "Boss 4(Clone)")
        {
            transform.rotation = new Quaternion(180,0,0,0);
        }
        else if (gameObject.tag == "Enemy" || (gameObject.tag == "Boss"))
        {
            if (gameObject.GetComponent<EnemyController>().BewegungsModus != MovementMode.MoveHorizontalToLeft
                && gameObject.GetComponent<EnemyController>().BewegungsModus != MovementMode.MoveHorizontalToRight)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            else if(gameObject.GetComponent<EnemyController>().BewegungsModus == MovementMode.MoveHorizontalToLeft)
            {
                transform.Rotate(new Vector3(0, 0, Random.Range(-75, -75)));
            }
            else if (gameObject.GetComponent<EnemyController>().BewegungsModus == MovementMode.MoveHorizontalToRight)
            {
                transform.Rotate(new Vector3(0, 0, Random.Range(75,75)));
            }
        }

        if(gameObject.tag == "Player")
        {
            GetComponent<PlayerController>().enabled = true;
            GetComponent<EnemyController>().enabled = false;

            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        else if (gameObject.tag == "Enemy")
        {
            GetComponent<PlayerController>().enabled = false;
            GetComponent<EnemyController>().enabled = true;

            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        }
    }

    void ModifierUpdate()
    {
        
        for (int a = 0; a < activeModifier.Count; a++)
        {
            Modifier m = activeModifier[a];
            m.Update();
            if (m.IsDauerhaft == false)
            {
                if (m.timeRunning > m.Dauer)
                {

                    Debug.Log("PowerUp" + m.wert.ToString() + " entfernt.");
                    // Remove Modifier
                    activeModifier.Remove(m);
                }
            }
            if(m.powerupType != PowerUpType.Buff)
            {
                if(m.Wirkung <= 0)
                {
                    switchWeapon(m.powerupType.ToString(), false);
                    switchWeapon("Weapon1", true);
                    activeModifier.Remove(m);
                }
                else
                {

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ModifierUpdate();

        if (gameManager.gameState == GameState.Level)
        {
            if (shipState == ShipState.Active)
            {
                if (curRumpf < Rumpf && curRumpf > 0)
                {
                    if (IsRumpfRegeneration == false)
                    {
                        Invoke("RegenerateRumpf", SchildAufladung);
                        IsRumpfRegeneration = true;
                    }
                }
                else if (curRumpf == Rumpf)
                {
                    curRumpf = Rumpf;
                    IsRumpfRegeneration = false;
                }

                if (curSchildEnergie <= 0)
                {
                    Shield.GetComponent<SpriteRenderer>().enabled = false;
                    if (IsSchildAufladung == false)
                    {
                        Invoke("RegenerateSchild", SchildAufladung);
                        IsSchildAufladung = true;
                    }
                }
                else
                {
                    Shield.GetComponent<SpriteRenderer>().enabled = true;
                }

                if (curRumpf <= 0)
                {
                    if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
                    {
                        playerController.score += Mathf.RoundToInt((float)gameObject.GetComponent<EnemyController>().XPGain * gameManager.ScoreMultiplier);

                        if(gameObject.tag == "Enemy")
                        {
                            int rnd = Random.Range(0, 100);
                            GameObject[] powerupPrefabs = gameObject.GetComponent<EnemyController>().spawnablePowerUps;

                            if(powerupPrefabs.Length > 0 && gameObject.GetComponent<EnemyController>().PowerUpWahrscheinlichkeit > rnd)
                            {
                                levelManager.GetComponent<SpawnController>().SpawnPowerUp(gameObject.transform.position, powerupPrefabs);
                            }
                        }

                    }
                    if (gameObject.tag == "Player")
                    {
                        string HighscoreID = "";

                        switch(gameManager.schwierigkeit)
                        {
                            case Schwierigkeit.Easy:
                                {
                                    HighscoreID = GPGSIds.leaderboard_highscore_easy;
                                    break;
                                }
                            case Schwierigkeit.Medium:
                                {
                                    HighscoreID = GPGSIds.leaderboard_highscore_medium;
                                    break;
                                }
                            case Schwierigkeit.Hard:
                                {
                                    HighscoreID = GPGSIds.leaderboard_highscore_hard;
                                    break;
                                }
                        }

                        if(HighscoreID != "")
                        {
                            gameManager.GetComponent<ServiceManager>().SendScore(GetComponent<PlayerController>().score, HighscoreID);
                        }

                        gameManager.GetComponent<ServiceManager>().SendScore(GetComponent<PlayerController>().score);
                        gameManager.GoToGameOver();
                    }
                    shipState = ShipState.Destroyed;
                }


                MaxGeschwindigkeit();
            }
            else if (shipState == ShipState.Destroyed)
            {
                if(gameObject.tag == "Player")
                {                                        
                    GetComponent<PlayerController>().enabled = false;
                }
                else if(gameObject.tag == "Boss")
                {
                   GetComponent<BossController>().enabled = false;
                }                
                GetComponent<EnemyController>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void MaxGeschwindigkeit()
    {
        // Maximale Geschwindigkeit anwenden
        float x = rb.velocity.x;
        float y = rb.velocity.y;

        if (x > 0)
        {
            if (x >= MaximaleGeschwindigkeit)
            {
                x = MaximaleGeschwindigkeit;
            }
        }
        else if(x < 0)
        {
            x *= (-1);
            if (x >= MaximaleGeschwindigkeit)
            {
                x = MaximaleGeschwindigkeit * (-1);
            }
            else
            {
                x *= (-1);
            }
        }

        if (y > 0)
        {
            if (y >= MaximaleGeschwindigkeit)
            {
                y = MaximaleGeschwindigkeit;
            }
        }
        else if (y < 0)
        {
            y *= (-1);
            if (y >= MaximaleGeschwindigkeit)
            {
                y = MaximaleGeschwindigkeit * (-1);
            }
            else
            {
                y *= (-1);
            }
        }

        rb.velocity = new Vector2(x, y);
    }

    private void RegenerateSchild()
    {
        curSchildEnergie = SchildEnergie;
        Debug.Log("Schild regeneriert.");
        IsSchildAufladung = false;
    }

    private void RegenerateRumpf()
    {
        curRumpf = curRumpf + Technik;
        Debug.Log("Rumpf regeneriert.");
        if(curRumpf >= Rumpf)
        {
            curRumpf = Rumpf;
        }
        IsRumpfRegeneration = false;
    }

    public void Shoot()
    {
        if (gameManager.gameState == GameState.Level)
        {
            bool hasShot = false;

            AudioClip sound = null;

            foreach (WeaponController entry in WeaponSlots)
            {
                if(entry.IsActiv == false)
                {
                    continue;
                }
                WeaponController weapon = entry;
                BulletController bullet = entry.bulletPrefab;

                sound = weapon.shootSound;

                if (weapon.ReadyToShoot == true)
                {     
                    weapon.FireTimer = weapon.Feuerrate;

                    if (gameObject.tag == "Player")
                    {                        
                        bullet.gameObject.tag = "PlayerBullet";
                        bullet.targetMode = TargetMode.Normal;
                    }
                    else if (gameObject.tag == "Enemy")
                    {
                        bullet.gameObject.tag = "EnemyBullet";
                        bullet.targetMode = weapon.Zielmodus;
                    }
                    else if (gameObject.tag == "Boss")
                    {
                        bullet.gameObject.tag = "BossBullet";
                        bullet.targetMode = weapon.Zielmodus;
                    }

                    float tempLebensdauer = Random.Range(weapon.Lebensdauer - 0.2f, weapon.Lebensdauer + 0.2f);

                    bullet.Lebensdauer = tempLebensdauer;

                    BulletController b = Instantiate(bullet, weapon.transform.position, weapon.transform.rotation);

                    BulletController bulletObj = b.GetComponent<BulletController>();

                    int Schaden = 0;

                    if (gameManager.schwierigkeit == Schwierigkeit.Easy)
                    {
                        Schaden = Mathf.FloorToInt((float)weapon.Waffenschaden + Treffsicherheit);
                    }
                    else
                    {
                        Schaden = Mathf.RoundToInt((float)weapon.Waffenschaden + Treffsicherheit);
                    }

                    bulletObj.Schaden = Schaden;
                    bulletObj.schussMode = weapon.schussModus; 

                    hasShot = true;

                    if (weapon.Zielmodus == TargetMode.Normal)
                    {
                        Vector2 v2 = bulletObj.Geschwindigkeit;

                        bulletObj.Geschwindigkeit = new Vector2(weapon.Geschwindigkeit.x + b.Geschwindigkeit.x, weapon.Geschwindigkeit.y + b.Geschwindigkeit.y);
                    }
                    else if (weapon.Zielmodus == TargetMode.TargetPlayer)
                    {
                        Vector2 v2 = (player.transform.position - b.transform.position).normalized;

                        float tempSpeed = 1f;

                        if (weapon.Geschwindigkeit.y > weapon.Geschwindigkeit.x)
                        {
                            tempSpeed = weapon.Geschwindigkeit.y;
                        }
                        else
                        {
                            tempSpeed = weapon.Geschwindigkeit.x;
                        }

                        b.Geschwindigkeit = new Vector2(v2.x, v2.y) * tempSpeed;

                        b.GetComponent<Rigidbody2D>().freezeRotation = false;

                        b.transform.rotation = Quaternion.LookRotation(Vector3.forward, b.transform.position - player.transform.position);

                        b.GetComponent<Rigidbody2D>().freezeRotation = true;

                    }
                }
            }
            if(hasShot == true)
            {
                foreach (Modifier weaponUpgrade in GetWeaponModifier())
                {
                    weaponUpgrade.Wirkung--;
                }
                if (sound != null)
                {
                    gameManager.PlayAudio(sound);
                }
            }
        }
    }

    private void Hit(GameObject obj)
    {
        if (gameManager.gameState == GameState.Level)
        {
            if (gameObject.tag == "Player" && obj.gameObject.tag == "Enemy")
            {
                ShipController schiff = obj.GetComponent<ShipController>();

                int Schaden = schiff.curSchildEnergie + schiff.curRumpf;

                int SchadenSchild = 0;
                int SchadenRumpf = 0;

                if (Schaden > curSchildEnergie)
                {
                    SchadenSchild = curSchildEnergie;
                    SchadenRumpf = Schaden - curSchildEnergie;
                }
                else
                {
                    SchadenSchild = Schaden;
                }

                curSchildEnergie -= SchadenSchild;
                curRumpf -= SchadenRumpf;

                obj.GetComponent<ShipController>().shipState = ShipState.Destroyed;
                gameManager.ScoreMultiplier = 0;
            }
            if (gameObject.tag == "Player" && obj.gameObject.tag == "Boss")
            {
                curRumpf = 0;
            }
            if (obj.gameObject.tag == "PlayerBullet")
            {
                if (GetComponent<EnemyController>().isInViewport == true || GetComponent<BossController>() != null)
                {
                    // Chance auf Power-Up Drop
                    if (gameObject.tag == "Enemy" || gameObject.tag == "Boss")
                    {
                        int rnd = Random.Range(0, 100);
                        GameObject[] powerupPrefabs = gameObject.GetComponent<EnemyController>().spawnablePowerUps;

                        if (powerupPrefabs.Length > 0 && (gameObject.GetComponent<EnemyController>().PowerUpWahrscheinlichkeit / 2) > rnd
                            && scenarioManager.powerUpsOnScreen <= (scenarioManager.activeScenario.PowerUpsOnScreenLimit))
                        {
                            levelManager.GetComponent<SpawnController>().SpawnPowerUp(gameObject.transform.position, powerupPrefabs);
                        }
                    }
                    BulletController bullet = obj.GetComponent<BulletController>();

                    int Schaden = bullet.Schaden;

                    if (curRumpf <= 0)
                        curRumpf = 0;
                    if (curSchildEnergie <= 0)
                        curSchildEnergie = 0;

                    int verbleibenderSchaden = Schaden - (curSchildEnergie + curRumpf);

                    int SchadenSchild = 0;
                    int SchadenRumpf = 0;

                    if (Schaden > curSchildEnergie)
                    {
                        SchadenSchild = curSchildEnergie;
                        SchadenRumpf = Schaden - curSchildEnergie;
                    }
                    else
                    {
                        SchadenSchild = Schaden;
                    }

                    bool damaged = false;

                    if (tag == "Boss")
                    {
                        if (GetComponent<BossController>().isActive == true)
                        {
                            damaged = true;
                        }
                    }
                    else
                    {
                        damaged = true;
                    }

                    if (damaged == true)
                    {
                        curSchildEnergie -= SchadenSchild;
                        curRumpf -= SchadenRumpf;

                        if (verbleibenderSchaden <= 0)
                            verbleibenderSchaden = 0;

                        bullet.Schaden = verbleibenderSchaden;

                        playerController.score += Mathf.RoundToInt(((float)gameObject.GetComponent<EnemyController>().XPGain * gameManager.ScoreMultiplier) / 10);
                    }

                    if (bullet.Schaden <= 0)
                    {
                        Destroy(bullet.gameObject);
                    }
                }
            }
            if (obj.gameObject.tag == "EnemyBullet" || obj.gameObject.tag == "BossBullet")
            {
                gameManager.ScoreMultiplier = 0;

                BulletController bullet = obj.GetComponent<BulletController>();

                int Schaden = bullet.Schaden;

                int verbleibenderSchaden = Schaden - (curSchildEnergie + curRumpf);

                int SchadenSchild = 0;
                int SchadenRumpf = 0;

                if (Schaden > curSchildEnergie)
                {
                    SchadenSchild = curSchildEnergie;
                    SchadenRumpf = Schaden - curSchildEnergie;
                }
                else
                {
                    SchadenSchild = Schaden;
                }

                curSchildEnergie -= SchadenSchild;
                curRumpf -= SchadenRumpf;

                if (bullet.isImmortal == false)
                    Destroy(obj);
            }
        }
    }
}
