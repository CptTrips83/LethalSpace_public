using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject Player;
    private GameManager gameManager;

    public ProgressBar PbRumpf;
    public ProgressBar PbSchild;
    public ProgressBar PbMunition;


    // Start is called before the first frame update
    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (gameManager.gameState == GameState.Level)
        {

            ShipController PlayerShip = Player.GetComponent<ShipController>();

            float RumpfFraction = ((PlayerShip.curRumpf * 100) / (PlayerShip.Rumpf));

            PbRumpf.BarValue = RumpfFraction;

            float SchildFraction = ((PlayerShip.curSchildEnergie * 100) / (PlayerShip.SchildEnergie));

            PbSchild.BarValue = SchildFraction;

            ShipController ship = PlayerShip.GetComponent<ShipController>();

            if (ship.GetWeaponModifier().Length > 0)
            {
                Modifier weaponmod = ship.GetWeaponModifier()[0];

                PbMunition.gameObject.SetActive(true);

                float MunitionFraction = ((weaponmod.Wirkung * 100) / (weaponmod.MaxWirkung));

                PbMunition.BarValue = MunitionFraction;
            }
            else
            {
                PbMunition.gameObject.SetActive(false);
            }

        }
    }
}
