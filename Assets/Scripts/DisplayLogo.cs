using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;


public class TextureLoader
{
    public async Task<Texture2D> LoadTextureAsync(string url)
    {
        var request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest();
        return DownloadHandlerTexture.GetContent(request);
    }
}


public class DisplayLogo : MonoBehaviour
{
    private List<string> stringListExample;
    private int index = 0;

    [SerializeField]
    UIDocument _uiDocument;
    private async void Start()
    {
        stringListExample = new List<string>();
        stringListExample.Add("https://www.howest.be/fs/styles/fixed_medium_nocrop/public/images/howest-hogeschool-logo-poweredby.png");
        stringListExample.Add("https://www.howest.be/fs/styles/fixed_medium_nocrop/public/images/howest-university-of-applied-sciences-logo.png");
        stringListExample.Add("https://www.howest.be/fs/styles/fixed_medium_nocrop/public/images/howest-hogeschool-logo.png");

        var logoElement = _uiDocument.rootVisualElement.Q("logoImage");
        await LoadTextureAsync(logoElement, stringListExample[index]);
    }


    private async Task LoadTextureAsync(VisualElement element, string url)
    {
        TextureLoader loader = new TextureLoader();
        Texture2D texture = await loader.LoadTextureAsync(url);
        element.style.backgroundImage = texture;
    }

    public async void ChangeLogo()
    {
        ++index;
        if (index == stringListExample.Count)
        {
            index = 0;
        }

        var logoElement = _uiDocument.rootVisualElement.Q("logoImage");
        await LoadTextureAsync(logoElement, stringListExample[index]);
    }
}



