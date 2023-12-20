using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PantallaInicio : MonoBehaviour
{
	[SerializeField][Range(0.05f, 1f)] private float velocidad = 1f;

	[SerializeField] private Slider sliderDificultad;
	[SerializeField] private TextMeshProUGUI textoModoSeleccionado;


	private void Start()
	{
		Configuracion.Instance.velocidad = velocidad;

		sliderDificultad?.onValueChanged.AddListener(DificultadModificada);

	}

	public void InciarPartida(int escena)
	{
		SceneManager.LoadScene(escena);
	}

	private void DificultadModificada(float valorSlider)
	{
		string dificultad = Mathf.RoundToInt(valorSlider) switch
		{
			1 => "Fácil",
			2 => "Medio",
			3 => "Difícil",
			4 => "Pesadilla",
			_ => "Error"
		};
		textoModoSeleccionado.text = dificultad;

		float velocidad = Mathf.RoundToInt(valorSlider) switch
		{
			1 => 1f,
			2 => 0.5f,
			3 => 0.2f,
			4 => 0.05f,
			_ => 1f
		};

		Configuracion.Instance.velocidad = velocidad;
	}
}
