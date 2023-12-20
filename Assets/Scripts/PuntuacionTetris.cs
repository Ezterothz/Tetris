using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuntuacionTetris : MonoBehaviour
{
	private void OnEnable()
	{
		Tetris.puntuacionActualizada += ActualizarPunt;
	}
	private void OnDisable()
	{
		Tetris.puntuacionActualizada -= ActualizarPunt;
	}

	private void ActualizarPunt(int p)
	{
		GetComponent<Text>().text = $"Puntuacion actual: {p}";

	}
}
