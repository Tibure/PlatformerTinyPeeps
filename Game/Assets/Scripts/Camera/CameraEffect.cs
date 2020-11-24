using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class CameraEffect : MonoBehaviour
{
	float minSize = 0.001f;
	float maxSize = 0.15f;
	float step = 0.002f;
	public Material material;
	string pixelSize = "_PixelSize";
	// Start is called before the first frame update
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		print("alloCAm");
		Graphics.Blit(source, destination, material);
	}

	public void PixelationOfCamera()
	{
		float size = minSize;
		while (size < maxSize) {
			//print("allo " + size);

			size += step;
			material.SetFloat(pixelSize, size);

		}

	}
	public void UnPixelationOfCamera()
	{
		material.SetFloat(pixelSize, maxSize);

		float size = maxSize;
		while (size > minSize)
		{
			print("allo " + size);
			size -= step;
			material.SetFloat(pixelSize, size);
			new WaitForSeconds(1);
			
		}
	}

	public void ResetTransitionPixel()
	{
		material.SetFloat(pixelSize, minSize);
	}
}
