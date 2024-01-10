using CbsPromPost.Other;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace CbsPromPost.Resources;

public class Sprites : SharpDx.SpritesDb
{
    public sealed override void LoadBitmap(SharpDx sdx)
    {
        Items = new Dictionary<string, Bitmap>
        {
            {
                "CCW",
                sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.CCW.png")!)!
            },
            {
                "CW", sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.CW.png")!)!
            },
            {
                "VT40",
                sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.VT40.png")!)!
            },
        };
    }
}
