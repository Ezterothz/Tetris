using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuracion : MonoBehaviour
{
	public static Configuracion Instance { get; private set; }

	[Range(0.05f, 1f)]
	public float velocidad = 1;
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
