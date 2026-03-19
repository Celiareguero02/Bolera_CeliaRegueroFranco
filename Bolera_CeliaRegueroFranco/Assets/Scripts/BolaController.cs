using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BolaController : MonoBehaviour
{
    Rigidbody rb;
    public float force = 100f;
    public bool launchingEnabled = true;
    public bool launch = false;
    public GameObject Marcador;
    public GameObject Direction;

    public Vector3 launchDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Direction.SetActive(false);
    }

    void Update()
    {
        // Solo permitir lanzar si el juego no esta en pausa por menus
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        if (Input.GetButton("Fire1") && launchingEnabled && !Direction.gameObject.activeInHierarchy && !gm.menuInicio.activeInHierarchy && !gm.menuFinal.activeInHierarchy)
        {
            Direction.SetActive(true);
        }

        if (Input.GetButtonUp("Fire1") && launchingEnabled && !gm.menuInicio.activeInHierarchy && !gm.menuFinal.activeInHierarchy)
        {
            launch = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gm.enProceso && !gm.menuInicio.activeInHierarchy && !gm.menuFinal.activeInHierarchy)
            {
                gm.ResetJuego();
            }
        }
    }

    private void FixedUpdate()
    {
        if (launch)
        {
            LanzarBola();
        }
    }

    void LanzarBola()
    {
        launchDirection = Marcador.GetComponent<Transform>().position;
        launchDirection.y = 1f;

        Vector3 dir = launchDirection - GetComponent<Transform>().position;

        rb.AddForce(dir * force, ForceMode.Impulse);
        Direction.SetActive(false);
        launchingEnabled = false;
        launch = false;

        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().CalcularPuntos();
    }
}