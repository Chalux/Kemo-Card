using Godot;
using KemoCard.Pages;
using System;

public partial class DialogueScene : BaseScene
{
	[Export] TextureRect Background;
	public void ChangeBackground(string url)
	{
		if(FileAccess.FileExists(url))
		{
			Image image = Image.LoadFromFile(url);
            Background.Texture = ImageTexture.CreateFromImage(image);
        }
    }
}
