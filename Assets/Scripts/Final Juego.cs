using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalJuego : MonoBehaviour
{
	private void OnEnable() { Tetris.FinalizaJuego += FinalizarPartida; }
	private void OnDisable() { Tetris.FinalizaJuego -= FinalizarPartida; }
	private void FinalizarPartida(int nuevaPuntuacion)
	{
		GetComponent<TextMeshProUGUI>().enabled = true;
		GetComponent<TextMeshProUGUI>().text = $"GAME OVER!!\n{nuevaPuntuacion} puntos";
	}
}
