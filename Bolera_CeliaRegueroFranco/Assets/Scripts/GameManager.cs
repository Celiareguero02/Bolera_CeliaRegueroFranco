using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] bolos;
    public Text puntuacion;
    public Text puntuacionFinal; // Nuevo texto para puntuacipn final

    private int puntosTotales = 0;
    private int tiradasRealizadas = 0;
    public int maxTiradas = 3; // Maximo de tiradas permitidas

    private Vector3[] posicionesIniciales;
    private Quaternion[] rotacionesIniciales;

    public GameObject bola;
    private Vector3 bolaPosInicial;
    private Quaternion bolaRotInicial;

    public GameObject barrera;
    public float velocidadBarrera = 5f;
    public Transform puntoFinalBarrera;
    private Vector3 posicionInicialBarrera;
    public bool enProceso = false;
    public Transform puntoFinalArriba;
    public Transform puntoSueloBarrera;
    float alturaBarrera;

    // Menus
    public GameObject menuInicio;
    public GameObject menuFinal;
    public GameObject instrucciones; // Referencia a "Instrucciones" si existe

    void Start()
    {
        bolos = GameObject.FindGameObjectsWithTag("bolo");

        posicionesIniciales = new Vector3[bolos.Length];
        rotacionesIniciales = new Quaternion[bolos.Length];

        for (int i = 0; i < bolos.Length; i++)
        {
            posicionesIniciales[i] = bolos[i].transform.position;
            rotacionesIniciales[i] = bolos[i].transform.rotation;
        }

        bolaPosInicial = bola.transform.position;
        bolaRotInicial = bola.transform.rotation;

        posicionInicialBarrera = barrera.transform.position;
        alturaBarrera = barrera.GetComponent<Renderer>().bounds.size.y;

        // Mostrar menu de inicio al comenzar
        MostrarMenuInicio();
    }

    void MostrarMenuInicio()
    {
        menuInicio.SetActive(true);
        menuFinal.SetActive(false); // Asegurar que el menú final esté oculto
        if (instrucciones != null) instrucciones.SetActive(true);

        // Desactivar elementos del juego
        bola.SetActive(false);
        barrera.SetActive(false);
        foreach (GameObject bolo in bolos)
        {
            bolo.SetActive(false);
        }
    }

    void OcultarMenusInicio()
    {
        menuInicio.SetActive(false);
        // if (instrucciones != null) instrucciones.SetActive(false); ← COMENTA O ELIMINA ESTA LÍNEA

        // Activar elementos del juego
        bola.SetActive(true);
        barrera.SetActive(true);
        foreach (GameObject bolo in bolos)
        {
            bolo.SetActive(true);
        }
    }

    public void IniciarJuego()
    {
        // Asegurarse de ocultar AMBOS menús
        menuInicio.SetActive(false);
        menuFinal.SetActive(false);

        // Si tienes instrucciones, también ocultarlas
        if (instrucciones != null) instrucciones.SetActive(false);

        // Activar elementos del juego
        bola.SetActive(true);
        barrera.SetActive(true);
        foreach (GameObject bolo in bolos)
        {
            bolo.SetActive(true);
        }

        ResetJuego();
        tiradasRealizadas = 0;
        puntosTotales = 0;
        puntuacion.text = "Bolos: 0 | Total: 0";
        // Asegurar que el texto de puntuación final esté vacío al iniciar
        if (puntuacionFinal != null)
        {
            puntuacionFinal.text = "";
        }
    }

    public void CalcularPuntos()
    {
        if (!enProceso && tiradasRealizadas < maxTiradas)
        {
            StartCoroutine(WaitPuntos());
        }
    }

    IEnumerator WaitPuntos()
    {
        enProceso = true;
        tiradasRealizadas++;

        yield return new WaitForSeconds(8f);

        int puntosTirada = 0;
        foreach (GameObject bolo in bolos)
        {
            if (bolo.transform.localRotation.eulerAngles.x < 258f || bolo.transform.localRotation.eulerAngles.x > 282f)
            {
                puntosTirada++;
            }
        }

        puntosTotales += puntosTirada;
        puntuacion.text = "Tirada: " + puntosTirada + " | Total: " + puntosTotales;

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(MoverBarrera());

        // Verificar si es la ultima tirada
        if (tiradasRealizadas >= maxTiradas)
        {
            StartCoroutine(MostrarMenuFinJuego());
        }
        else
        {
            ResetBolos();
            ResetBola();
            enProceso = false;
        }
    }

    IEnumerator MostrarMenuFinJuego()
    {
        yield return new WaitForSeconds(1f);

        // Mostrar puntuación final
        if (puntuacionFinal != null)
        {
            puntuacionFinal.text = "Puntuación total: " + puntosTotales;
        }

        // Mostrar menú final
        menuFinal.SetActive(true);

        // Desactivar elementos del juego
        bola.SetActive(false);
        barrera.SetActive(false);
        foreach (GameObject bolo in bolos)
        {
            bolo.SetActive(false);
        }
    }

    IEnumerator MoverBarrera()
    {
        yield return StartCoroutine(MoverHasta(puntoFinalArriba.position));

        Vector3 destinoAbajo = puntoSueloBarrera.position;
        yield return StartCoroutine(MoverHasta(destinoAbajo));
        yield return new WaitForSeconds(0.3f);

        Vector3 posicionAtrasAbajo = new Vector3(
            posicionInicialBarrera.x,
            destinoAbajo.y,
            posicionInicialBarrera.z
        );

        yield return StartCoroutine(MoverHasta(posicionAtrasAbajo));
        yield return StartCoroutine(MoverHasta(posicionInicialBarrera));
    }

    IEnumerator MoverHasta(Vector3 destino)
    {
        while (Vector3.Distance(barrera.transform.position, destino) > 0.05f)
        {
            barrera.transform.position = Vector3.MoveTowards(
                barrera.transform.position,
                destino,
                velocidadBarrera * Time.deltaTime
            );
            yield return null;
        }
    }

    public void ResetBolos()
    {
        for (int i = 0; i < bolos.Length; i++)
        {
            bolos[i].transform.position = posicionesIniciales[i];
            bolos[i].transform.rotation = rotacionesIniciales[i];

            Rigidbody rb = bolos[i].GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ResetBola()
    {
        bola.transform.position = bolaPosInicial;
        bola.transform.rotation = bolaRotInicial;

        Rigidbody rb = bola.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        BolaController bc = bola.GetComponent<BolaController>();
        bc.launchingEnabled = true;
    }

    public void ResetJuego()
    {
        ResetBolos();
        ResetBola();
        enProceso = false;
    }

    public void JugarDeNuevo()
    {
        IniciarJuego();
    }

    public void SalirJuego()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}