using CbsPromPost.Resources;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace CbsPromPost.Other;

public class Sprites : SpritesDb
{
    public sealed override void LoadBitmap(SharpDx sdx)
    {
        /*
        Items = new Dictionary<string, Bitmap>
        {
            {
                "BottleGreen",
                sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.bottleGreen.png")!)!
            },
            {
                "BottleRed", sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.bottleRed.png")!)!
            },
            {
                "BottleGray",
                sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.bottleGray.png")!)!
            },
        };
        */
    }
}
