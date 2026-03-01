using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class MaterialLoader : MonoBehaviour
{
	static readonly string[] _textureProperties = new[]
	{
		"_BaseMap", //Albedo
		"_OcclusionMap", //AO
		"_MetallicGlossMap", //Metallic
		"_BumpMap", //Normal
		"_ParallaxMap" //Height	
	};

	[SerializeField]
	Material _templateMaterial;

	[SerializeField]
	Material _invalidMaterial;

	[SerializeField]
	private float _spacing = 1f;

	[SerializeField]
	private string _filePath;

	private string[] _materials;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private async void Start()
	{
		int texturesPerMaterial = _textureProperties.Length;

		//TODO: read lines from file
		_materials = await File.ReadAllLinesAsync(_filePath);

        

        int numMaterials = _materials.Length / texturesPerMaterial; //TODO: calculate based on num textures

		Vector3 spawnPos = -Vector3.right * (numMaterials - 1) * 0.5f * _spacing;
		Vector3 spacingOffset = Vector3.right * _spacing; //add this to spawnPos after every material spawn

        for (int matIdx = 0; matIdx < numMaterials; matIdx++)
        {
            GameObject sphere = CreateMaterialSphere(spawnPos + spacingOffset * matIdx);

            int start = matIdx * texturesPerMaterial;
            string[] urls = _materials.Skip(start).Take(texturesPerMaterial).ToArray();


            try
            {
                List<Texture2D> textures = await LoadTexturesAsync(urls);

                // Safety: don't call ApplyTextures unless correct amount
                if (textures.Count != _textureProperties.Length || textures.Any(t => t == null))
                    throw new InvalidOperationException($"Texture list invalid. Count={textures.Count}");

                ApplyTextures(sphere, textures);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Material {matIdx} failed: {ex.Message}");
                sphere.GetComponent<Renderer>().sharedMaterial = _invalidMaterial;
            }
        }
      
    }

	private async Task<List<Texture2D>> LoadTexturesAsync(IReadOnlyList<string> urls)
	{
		
		var requests = new List<Task<Texture2D>>(urls.Count);

		foreach (string url in urls)
			requests.Add(LoadTextureAsync(url));

		Texture2D[] resultTextures = await Task.WhenAll(requests);
		return resultTextures.ToList();
	}

    private async Task<Texture2D> LoadTextureAsync(string url)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        var op = request.SendWebRequest();
        while (!op.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"{request.responseCode} - {request.error} - {url}");

        return DownloadHandlerTexture.GetContent(request);
    }


    GameObject CreateMaterialSphere(Vector3 position)
	{
		GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		primitive.transform.SetParent(transform);
		primitive.transform.localPosition = position;

		return primitive;
	}

	void ApplyTextures(GameObject primitive, List<Texture2D> textures)
	{
		//This will throw an exception when values are not equal
		Assert.AreEqual(_textureProperties.Length, textures.Count, $"Number of textures to apply ({textures.Count}) doesn't match the number of texture properties ({_textureProperties.Length})");

		//Create a new material based on the template material
		Material material = new Material(_templateMaterial);
		primitive.GetComponent<Renderer>().sharedMaterial = material;

		//Apply each texture per texture property
		for (int idx = 0; idx < _textureProperties.Length; ++idx)
		{
			string propertyName = _textureProperties[idx];
			Texture2D texture = textures[idx];

			material.SetTexture(propertyName, texture);
		}
	}
}
