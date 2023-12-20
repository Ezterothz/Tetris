using System.Collections;

using UnityEngine;

public enum TipoPieza
{
	PiezaL,
	PiezaO,
	PiezaT,
	PiezaI,
	PiezaS

}
public class PosicionTetris
{
	public int fila;
	public int columna;
	public float profundidad;

	public PosicionTetris(int columna, int fila)
	{
		this.fila = fila;
		this.columna = columna;
		this.profundidad = Random.Range(-Tetris.instance.profundidad / 2, Tetris.instance.profundidad / 2);
	}

	public Vector3 Posicionar()
	{
		return new Vector3(columna, fila, profundidad);
	}
}
public class PiezaTetris : MonoBehaviour
{
	Tetris tetris;
	TipoPieza tipoPieza;


	GameObject pieza1;
	GameObject pieza2;
	GameObject pieza3;
	GameObject pieza4;

	PosicionTetris posPieza1;
	PosicionTetris posPieza2;
	PosicionTetris posPieza3;
	PosicionTetris posPieza4;

	PosicionTetris posMedio;

	GameObject piezaSig1;
	GameObject piezaSig2;
	GameObject piezaSig3;
	GameObject piezaSig4;


	int maxColumna;
	int minColumna;
	int maxFila;
	int minFila;

	bool bloqueado;
	bool flip = false;

	TipoPieza sigPieza;
	bool sigFlip;

	private Coroutine bajarAuto;

	void Start()
	{
		tetris = Tetris.instance;
		tipoPieza = Tetris.instance.piezaAct;
		sigPieza = Tetris.instance.sigPieza;
		if (tipoPieza == TipoPieza.PiezaL || tipoPieza == TipoPieza.PiezaS) flip = Tetris.instance.flip;
		if (sigPieza == TipoPieza.PiezaL || sigPieza == TipoPieza.PiezaS) sigFlip = Tetris.instance.sigFlip;

		CrearPieza();
		GenerarPreview();

		bajarAuto = StartCoroutine(MoverAuto());
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			tetris.ReproducirRotar();
			MoverAbajo();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			tetris.ReproducirRotar();
			MoverIzquierda();
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			tetris.ReproducirRotar();
			MoverDerecha();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			tetris.ReproducirRotar();
			Rotar();
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			MoverHastaAbajo();
		}

	}
	void EstablecerMaxMin()
	{
		maxColumna = Mathf.Max(posPieza1.columna, posPieza2.columna, posPieza3.columna, posPieza4.columna);
		minColumna = Mathf.Min(posPieza1.columna, posPieza2.columna, posPieza3.columna, posPieza4.columna);
		maxFila = Mathf.Max(posPieza1.fila, posPieza2.fila, posPieza3.fila, posPieza4.fila);
		minFila = Mathf.Min(posPieza1.fila, posPieza2.fila, posPieza3.fila, posPieza4.fila);
	}

	private void MoverAbajo()
	{
		if (bajarAuto != null) StopCoroutine(bajarAuto);

		if (minFila > 0 && tetris.ComprobarInferiores(posPieza1, posPieza2, posPieza3, posPieza4))
		{
			posPieza1.fila--;
			posPieza2.fila--;
			posPieza3.fila--;
			posPieza4.fila--;
			posMedio.fila--;
			minFila--;
			maxFila--;

			MoverCubos();
		}
		else
		{
			bloquearPieza();
		}
		if (bajarAuto != null) bajarAuto = StartCoroutine(MoverAuto());
	}
	private void MoverIzquierda()
	{
		if (bajarAuto != null) StopCoroutine(bajarAuto);
		if (pieza4.transform.position.x > 0 && pieza3.transform.position.x > 0 && pieza2.transform.position.x > 0 && pieza1.transform.position.x > 0 && tetris.ComprobarIzquierda(posPieza1, posPieza2, posPieza3, posPieza4))
		{
			posPieza1.columna--;
			posPieza2.columna--;
			posPieza3.columna--;
			posPieza4.columna--;
			posMedio.columna--;
			minColumna--;
			maxColumna--;

			MoverCubos();
		}
		if (bajarAuto != null) bajarAuto = StartCoroutine(MoverAuto());
	}
	private void MoverDerecha()
	{
		if (bajarAuto != null) StopCoroutine(bajarAuto);
		if (pieza4.transform.position.x < Tetris.instance.columnas - 1 && pieza3.transform.position.x < Tetris.instance.columnas - 1 && pieza2.transform.position.x < Tetris.instance.columnas - 1 && pieza1.transform.position.x < Tetris.instance.columnas - 1 && tetris.ComprobarDerecha(posPieza1, posPieza2, posPieza3, posPieza4))
		{
			posPieza1.columna++;
			posPieza2.columna++;
			posPieza3.columna++;
			posPieza4.columna++;
			posMedio.columna++;
			minColumna++;
			maxColumna++;

			MoverCubos();

		}
		if (bajarAuto != null) bajarAuto = StartCoroutine(MoverAuto());
	}
	private void MoverHastaAbajo()
	{
		if (tetris.ComprobarInferiores(posPieza1, posPieza2, posPieza3, posPieza4))
		{
			while (!bloqueado)
			{
				MoverAbajo();
			}
		}
	}

	IEnumerator MoverAuto()
	{
		if (tetris.ComprobarInferiores(posPieza1, posPieza2, posPieza3, posPieza4) || minFila == 0)
		{
			while (!bloqueado)
			{
				yield return new WaitForSeconds(tetris.velocidad);
				MoverAbajo();
			}
		}
		else
		{
			yield return new WaitForSeconds(tetris.velocidad);
			bloquearPieza();
		}
	}

	private void Rotar()
	{
		if (tipoPieza != TipoPieza.PiezaO && RotacionValida())
		{
			RotarCubo(posPieza1);
			RotarCubo(posPieza2);
			RotarCubo(posPieza3);
			RotarCubo(posPieza4);

			EstablecerMaxMin();

			MoverCubos();
		}
	}

	private void RotarCubo(PosicionTetris p)
	{
		int deltaX = p.columna - posMedio.columna;
		int deltaY = p.fila - posMedio.fila;
		p.fila = posMedio.fila - deltaX;
		p.columna = posMedio.columna + deltaY;

	}

	private bool RotacionValida()
	{
		if (!RotarLibre(posPieza1)) return false;
		if (!RotarLibre(posPieza2)) return false;
		if (!RotarLibre(posPieza3)) return false;
		if (!RotarLibre(posPieza4)) return false;
		return true;
	}

	private bool RotarLibre(PosicionTetris p)
	{
		int deltaX = p.columna - posMedio.columna;
		int deltaY = p.fila - posMedio.fila;
		int indiceFila = posMedio.fila - deltaX;
		int indiceColumna = posMedio.columna + deltaY;

		if (!tetris.IndiceValido(indiceColumna, indiceFila)) return false;

		if (tetris.casillas[indiceFila][indiceColumna] != null)
			return false;

		return true;
	}

	private void MoverCubos()
	{
		pieza1.transform.position = posPieza1.Posicionar();
		pieza2.transform.position = posPieza2.Posicionar();
		pieza3.transform.position = posPieza3.Posicionar();
		pieza4.transform.position = posPieza4.Posicionar();
	}
	void bloquearPieza()
	{
		bloqueado = true;

		tetris.casillas[posPieza1.fila][posPieza1.columna] = pieza1;
		tetris.casillas[posPieza2.fila][posPieza2.columna] = pieza2;
		tetris.casillas[posPieza3.fila][posPieza3.columna] = pieza3;
		tetris.casillas[posPieza4.fila][posPieza4.columna] = pieza4;

		ComprobarFila();

		if (!(tetris.casillas[tetris.filas - 1][tetris.columnas / 2] != null))
			tetris.GenerarPieza();
		else tetris.JuegoFinalizado();

		Destroy(gameObject.GetComponent<PiezaTetris>());
		Destroy(gameObject);
		Destroy(piezaSig1);
		Destroy(piezaSig2);
		Destroy(piezaSig3);
		Destroy(piezaSig4);
	}

	private void ComprobarFila()
	{
		int huecosOcupados = 0;
		//comprobar una a una si la fila esta llena
		for (int i = 0; i < tetris.filas; i++)
		{
			for (int j = 0; j < tetris.columnas; j++)
			{
				if (tetris.casillas[i][j] != null)
				{
					huecosOcupados++;
				}
			}
			if (huecosOcupados == tetris.columnas)
			{
				tetris.ReproducirLinea();
				EliminarFila(i);
				BajarFilas(i);
				tetris.ActualizarPuntuacion();
				//volevemos a la comprobar la misma linea por si tambien a de borrarse
				i--;
			}
			huecosOcupados = 0;
		}
	}

	private void EliminarFila(int i)
	{
		for (int j = 0; j < tetris.columnas; j++)
		{
			Destroy(tetris.casillas[i][j]);
			tetris.casillas[i][j] = null;
		}
	}

	private void BajarFilas(int fila)
	{
		for (int i = fila; i < tetris.filas; i++)
		{
			if (i - 1 >= 0)
			{
				for (int j = 0; j < tetris.columnas; j++)
				{
					if (tetris.casillas[i][j] != null)
					{
						tetris.casillas[i][j].transform.position = new Vector3(tetris.casillas[i][j].transform.position.x, tetris.casillas[i][j].transform.position.y - 1, tetris.casillas[i][j].transform.position.z);
						tetris.casillas[i - 1][j] = tetris.casillas[i][j];
						tetris.casillas[i][j] = null;
					}
				}
			}
		}
	}

	void CrearPieza()
	{
		pieza1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pieza2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pieza3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pieza4 = GameObject.CreatePrimitive(PrimitiveType.Cube);

		switch (tipoPieza)
		{
			case TipoPieza.PiezaL:
				if (flip)
				{
					posPieza1 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 1);
					posPieza2 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
					posPieza3 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 3);
					posPieza4 = new PosicionTetris(tetris.columnas / 2 - 1, tetris.filas - 3);
					posMedio = new PosicionTetris(tetris.columnas / 2 - 1, tetris.filas - 2);
				}
				else
				{
					posPieza1 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 1);
					posPieza2 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
					posPieza3 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 3);
					posPieza4 = new PosicionTetris(tetris.columnas / 2 + 1, tetris.filas - 3);
					posMedio = new PosicionTetris(tetris.columnas / 2 + 1, tetris.filas - 2);
				}
				break;
			case TipoPieza.PiezaO:
				posPieza1 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 1);
				posPieza2 = new PosicionTetris(tetris.columnas / 2 + 1, tetris.filas - 1);
				posPieza3 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
				posPieza4 = new PosicionTetris(tetris.columnas / 2 + 1, tetris.filas - 2);
				posMedio = new PosicionTetris(tetris.columnas / 2 + 1, tetris.filas - 2);
				break;
			case TipoPieza.PiezaT:
				posPieza1 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 1);
				posPieza2 = new PosicionTetris(tetris.columnas / 2 + 1, tetris.filas - 1);
				posPieza3 = new PosicionTetris(tetris.columnas / 2 - 1, tetris.filas - 1);
				posPieza4 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
				posMedio = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
				break;
			case TipoPieza.PiezaI:
				posPieza1 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 1);
				posPieza2 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
				posPieza3 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 3);
				posPieza4 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 4);
				posMedio = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
				break;
			case TipoPieza.PiezaS:
				if (flip)
				{
					posPieza1 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 1);
					posPieza2 = new PosicionTetris(tetris.columnas / 2 - 1, tetris.filas - 1);
					posPieza3 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
					posPieza4 = new PosicionTetris(tetris.columnas / 2 + 1, tetris.filas - 2);
					posMedio = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
				}
				else
				{
					posPieza1 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 1);
					posPieza2 = new PosicionTetris(tetris.columnas / 2 + 1, tetris.filas - 1);
					posPieza3 = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
					posPieza4 = new PosicionTetris(tetris.columnas / 2 - 1, tetris.filas - 2);
					posMedio = new PosicionTetris(tetris.columnas / 2, tetris.filas - 2);
				}
				break;
		}

		MoverCubos();

		EstablecerMaxMin();

		pintarPieza(tipoPieza);
	}
	private void GenerarPreview()
	{
		piezaSig1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		piezaSig2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		piezaSig3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		piezaSig4 = GameObject.CreatePrimitive(PrimitiveType.Cube);

		switch (sigPieza)
		{
			case TipoPieza.PiezaL:
				if (sigFlip)
				{
					piezaSig1.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 1, 0);
					piezaSig2.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 2, 0);
					piezaSig3.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 3, 0);
					piezaSig4.transform.position = new Vector3(tetris.columnas + 4, tetris.filas - 3, 0);

				}
				else
				{
					piezaSig1.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 1, 0);
					piezaSig2.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 2, 0);
					piezaSig3.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 3, 0);
					piezaSig4.transform.position = new Vector3(tetris.columnas + 6, tetris.filas - 3, 0);

				}
				break;
			case TipoPieza.PiezaO:
				piezaSig1.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 1, 0);
				piezaSig2.transform.position = new Vector3(tetris.columnas + 6, tetris.filas - 1, 0);
				piezaSig3.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 2, 0);
				piezaSig4.transform.position = new Vector3(tetris.columnas + 6, tetris.filas - 2, 0);

				break;
			case TipoPieza.PiezaT:
				piezaSig1.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 1, 0);
				piezaSig2.transform.position = new Vector3(tetris.columnas + 6, tetris.filas - 1, 0);
				piezaSig3.transform.position = new Vector3(tetris.columnas + 4, tetris.filas - 1, 0);
				piezaSig4.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 2, 0);

				break;
			case TipoPieza.PiezaI:
				piezaSig1.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 1, 0);
				piezaSig2.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 2, 0);
				piezaSig3.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 3, 0);
				piezaSig4.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 4, 0);

				break;
			case TipoPieza.PiezaS:
				if (sigFlip)
				{
					piezaSig1.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 1);
					piezaSig2.transform.position = new Vector3(tetris.columnas + 4, tetris.filas - 1);
					piezaSig3.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 2);
					piezaSig4.transform.position = new Vector3(tetris.columnas + 6, tetris.filas - 2);

				}
				else
				{
					piezaSig1.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 1);
					piezaSig2.transform.position = new Vector3(tetris.columnas + 6, tetris.filas - 1);
					piezaSig3.transform.position = new Vector3(tetris.columnas + 5, tetris.filas - 2);
					piezaSig4.transform.position = new Vector3(tetris.columnas + 4, tetris.filas - 2);

				}
				break;
		}

		pintarPiezaSig(sigPieza);
	}

	private void pintarPieza(TipoPieza tipoPieza)
	{
		Color colorPieza = tipoPieza switch
		{
			TipoPieza.PiezaL => (flip) ? Color.blue : Color.gray,
			TipoPieza.PiezaO => Color.yellow,
			TipoPieza.PiezaT => Color.magenta,
			TipoPieza.PiezaI => Color.cyan,
			TipoPieza.PiezaS => (flip) ? Color.green : Color.red
		};

		pieza1.GetComponent<Renderer>().material = tetris.materialPieza;
		pieza1.GetComponent<Renderer>().material.color = colorPieza;
		pieza2.GetComponent<Renderer>().material = tetris.materialPieza;
		pieza2.GetComponent<Renderer>().material.color = colorPieza;
		pieza3.GetComponent<Renderer>().material = tetris.materialPieza;
		pieza3.GetComponent<Renderer>().material.color = colorPieza;
		pieza4.GetComponent<Renderer>().material = tetris.materialPieza;
		pieza4.GetComponent<Renderer>().material.color = colorPieza;
	}

	private void pintarPiezaSig(TipoPieza tipoPieza)
	{
		Color colorPieza = tipoPieza switch
		{
			TipoPieza.PiezaL => (sigFlip) ? Color.blue : Color.gray,
			TipoPieza.PiezaO => Color.yellow,
			TipoPieza.PiezaT => Color.magenta,
			TipoPieza.PiezaI => Color.cyan,
			TipoPieza.PiezaS => (sigFlip) ? Color.green : Color.red
		};

		piezaSig1.GetComponent<Renderer>().material = tetris.materialPieza;
		piezaSig1.GetComponent<Renderer>().material.color = colorPieza;
		piezaSig2.GetComponent<Renderer>().material = tetris.materialPieza;
		piezaSig2.GetComponent<Renderer>().material.color = colorPieza;
		piezaSig3.GetComponent<Renderer>().material = tetris.materialPieza;
		piezaSig3.GetComponent<Renderer>().material.color = colorPieza;
		piezaSig4.GetComponent<Renderer>().material = tetris.materialPieza;
		piezaSig4.GetComponent<Renderer>().material.color = colorPieza;
	}
}
