
using System;
using UnityEngine;

public class Tetris : MonoBehaviour
{
	[Range(10, 22)]
	public int filas = 20;
	[Range(4, 20)]
	public int columnas = 6;
	[Range(0f, 1f)]
	public float profundidad = 0f;
	[Range(0f, 1f)]
	public float velocidad = 1;

	public GameObject[][] casillas;

	public static Tetris instance;

	public delegate void puntuacionTetris(int n);
	public static event puntuacionTetris puntuacionActualizada;

	public delegate void finalizaJuego(int p);
	public static event finalizaJuego FinalizaJuego;

	private int puntuacion;
	public TipoPieza sigPieza;
	public bool sigFlip;
	public bool flip;
	public TipoPieza piezaAct;
	bool inicial= false;

	[SerializeField]
	private Material materialLimite;
	[SerializeField]
	public Material materialPieza;

	[SerializeField]
	private GameObject sonidoRotar;
	[SerializeField]
	private GameObject sonidoLinea;
	[SerializeField]
	private GameObject sonidoFin;

	private void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(gameObject);
	}

	void Start()
	{
		AplicarConfiguracion();

		GenerarLimite();
		GenerarTablero();
		GenerarPieza();
		puntuacion = 0;
		puntuacionActualizada?.Invoke(puntuacion);
	}

	private void AplicarConfiguracion()
	{
		velocidad = Configuracion.Instance.velocidad;
	}

	void GenerarLimite()
	{
		GameObject limiteInferior = GameObject.CreatePrimitive(PrimitiveType.Cube);
		limiteInferior.transform.position = new Vector3(columnas / 2 - (columnas % 2 == 0 ? 0.5f : 0), -1, 0);
		limiteInferior.transform.localScale = new Vector3(columnas + 2, 1, 1);

		GameObject limiteIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
		limiteIzquierda.transform.position = new Vector3(-1, filas / 2 - 1, 0);
		limiteIzquierda.transform.localScale = new Vector3(1, filas - 1, 1);

		GameObject limiteDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
		limiteDerecha.transform.position = new Vector3(columnas, filas / 2 - 1, 0);
		limiteDerecha.transform.localScale = new Vector3(1, filas - 1, 1);

		limiteInferior.GetComponent<Renderer>().material = materialLimite;
		limiteIzquierda.GetComponent<Renderer>().material = materialLimite;
		limiteDerecha.GetComponent<Renderer>().material = materialLimite;

		CentrarCamara();
	}

	private void CentrarCamara()
	{
		Vector3 posicionCamara = new Vector3(columnas / 2, filas / 2, -columnas * 2.5f);
		Camera.main.transform.position = posicionCamara;
	}
	public void GenerarTablero()
	{
		casillas = new GameObject[filas][];
		for (int i = 0; i < casillas.Length; i++)
		{
			casillas[i] = new GameObject[columnas];
		}
	}
	public void GenerarPieza()
	{
		
		if (inicial)
		{
			piezaAct = GenerarTipo();
			if (piezaAct == TipoPieza.PiezaL || piezaAct == TipoPieza.PiezaS)
			{
				int fliped = UnityEngine.Random.Range(0, 2);
				if (fliped == 1) flip = true;
			}
			sigPieza = GenerarTipo();
			if (sigPieza == TipoPieza.PiezaL || sigPieza == TipoPieza.PiezaS)
			{
				int fliped2 = UnityEngine.Random.Range(0, 2);
				if (fliped2 == 1) sigFlip = true;
			}
			inicial = false;
		}
		else
		{
			piezaAct = sigPieza;
			flip = sigFlip;
			sigPieza = GenerarTipo();
			if (sigPieza == TipoPieza.PiezaL || sigPieza == TipoPieza.PiezaS)
			{
				int fliped = UnityEngine.Random.Range(0, 2);
				if (fliped == 1) sigFlip = true;
			}
			else sigFlip = false;
		}
		
		
		new GameObject("pieza").AddComponent<PiezaTetris>();
	}

	TipoPieza GenerarTipo()
	{
		TipoPieza tipoPieza = TipoPieza.PiezaO;
		int tipo = UnityEngine.Random.Range(0, 5);
		switch (tipo)
		{
			case 0:
				tipoPieza = TipoPieza.PiezaL;
				break;
			case 1:
				tipoPieza = TipoPieza.PiezaO;
				break;
			case 2:
				tipoPieza = TipoPieza.PiezaT;
				break;
			case 3:
				tipoPieza = TipoPieza.PiezaI;
				break;
			case 4:
				tipoPieza = TipoPieza.PiezaS;
				break;
		}
		
		return tipoPieza;
	}
	public bool ComprobarInferiores(PosicionTetris pieza1, PosicionTetris pieza2, PosicionTetris pieza3, PosicionTetris pieza4)
	{
		//indices
		if (!IndiceValido(pieza1.columna, pieza1.fila - 1)) return false;
		if (!IndiceValido(pieza2.columna, pieza2.fila - 1)) return false;
		if (!IndiceValido(pieza3.columna, pieza3.fila - 1)) return false;
		if (!IndiceValido(pieza4.columna, pieza4.fila - 1)) return false;
		//si estas a 0 ya estas abajo del todo
		if (pieza1.fila == 0 || pieza2.fila == 0 || pieza3.fila == 0 || pieza4.fila == 0) return false;
		//comprobamos que abajo de ninguna pieza no haya nada
		if (!casillas[pieza1.fila - 1][pieza1.columna] && !casillas[pieza2.fila - 1][pieza2.columna] && !casillas[pieza3.fila - 1][pieza3.columna] && !casillas[pieza4.fila - 1][pieza4.columna])
		{
			return true;
		}

		return false;
	}

	public bool ComprobarIzquierda(PosicionTetris pieza1, PosicionTetris pieza2, PosicionTetris pieza3, PosicionTetris pieza4)
	{
		//indices
		if (!IndiceValido(pieza1.columna - 1, pieza1.fila)) return false;
		if (!IndiceValido(pieza2.columna - 1, pieza2.fila)) return false;
		if (!IndiceValido(pieza3.columna - 1, pieza3.fila)) return false;
		if (!IndiceValido(pieza4.columna - 1, pieza4.fila)) return false;
		//comprobamos que a la izquierda de ninguna pieza no haya nada
		if (!casillas[pieza1.fila][pieza1.columna - 1] && !casillas[pieza2.fila][pieza2.columna - 1] && !casillas[pieza3.fila][pieza3.columna - 1] && !casillas[pieza4.fila][pieza4.columna - 1])
		{
			return true;
		}

		return false;
	}

	public bool ComprobarDerecha(PosicionTetris pieza1, PosicionTetris pieza2, PosicionTetris pieza3, PosicionTetris pieza4)
	{
		//indices
		if (!IndiceValido(pieza1.columna + 1, pieza1.fila)) return false;
		if (!IndiceValido(pieza2.columna + 1, pieza2.fila)) return false;
		if (!IndiceValido(pieza3.columna + 1, pieza3.fila)) return false;
		if (!IndiceValido(pieza4.columna + 1, pieza4.fila)) return false;
		//comprobamos que a la derecha de ninguna pieza no haya nada
		if (!casillas[pieza1.fila][pieza1.columna + 1] && !casillas[pieza2.fila][pieza2.columna + 1] && !casillas[pieza3.fila][pieza3.columna + 1] && !casillas[pieza4.fila][pieza4.columna + 1])
		{
			return true;
		}

		return false;
	}
	public bool IndiceValido(int posX, int posY)
	{
		if (posX > columnas - 1 || posX < 0) return false;
		if (posY > filas - 1 || posY < 0) return false;
		return true;
	}
	public void ActualizarPuntuacion()
	{
		puntuacion += Mathf.RoundToInt(100f - velocidad * 100);
		puntuacionActualizada?.Invoke(puntuacion);

		if (velocidad > 0.1) velocidad -= 0.05f;
		else if (velocidad > 0.05) velocidad -= 0.005f;
	}

	public void JuegoFinalizado()
	{
		sonidoFin.GetComponent<AudioSource>().Play();
		FinalizaJuego?.Invoke(puntuacion);
	}
	public void ReproducirRotar()
	{
		sonidoRotar.GetComponent<AudioSource>().Play();
	}
	public void ReproducirLinea()
	{
		sonidoLinea.GetComponent<AudioSource>().Play();
	}
}
