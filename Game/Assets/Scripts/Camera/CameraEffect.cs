using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class CameraEffect : MonoBehaviour
{
	float minSize = 0.001f;
	float maxSize = 0.15f;
	public Material material;
	string pixelSize = "_PixelSize";
	float transitionTime = 2f;
	float updateFrequency = 0.02f;
	float step = 0f;
	public void Start()
	{
		step = maxSize / (transitionTime / updateFrequency);

	}
	// Start is called before the first frame update
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, material);
	}
/*
	public void PixelationOfCamera()
	{
		float size = minSize;
		while (size < maxSize) {
			//print("allo " + size);

			size += step;
			material.SetFloat(pixelSize, size);

		}

	}*/
	public void ResetTransitionPixel()
	{
		material.SetFloat(pixelSize, minSize);
	}

	public void StartCoroutineUnPixelisation()
	{
		IEnumerator coroutine = UnPixelationOfCamera();
		StartCoroutine(coroutine);
	}

	public void StartCoroutinePixelisation()
	{
		IEnumerator coroutine = PixelationOfCamera();
		StartCoroutine(coroutine);
	}

	IEnumerator UnPixelationOfCamera()
	{
		material.SetFloat(pixelSize, maxSize);

		float size = maxSize;
		while (size > minSize)
		{
			size -= step;
			material.SetFloat(pixelSize, size);
			yield return new WaitForSeconds(updateFrequency);
			
		}
	}
	IEnumerator PixelationOfCamera()
	{
		material.SetFloat(pixelSize, maxSize);

		float size = minSize;
		while (size < maxSize)
		{
			size += step;
			material.SetFloat(pixelSize, size);
			yield return new WaitForSeconds(updateFrequency);

		}

	}
}
