using Assimp;
using CbsPromPost.Other;
using SharpDX.Direct3D11;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace CbsPromPost.Resources;

public class Sprites3D : SharpDx3D.SpritesDb3D
{
    public class Obj
    {
        public Scene Scene { get; set; } = new(); // Сцена обьекта assimp
        public SharpDxG3D.VertexPc[] VertexM = Array.Empty<SharpDxG3D.VertexPc>(); // Список вершин обьекта для отображения
        public float M { get; set; } = 1.0f; // Высота относительно 1 метра
        public ShaderResourceView? Texture { get; set; } // Текстура для отображения объекта
        public ShaderResourceView? TextureN { get; set; } // Текстура нормалей объекта
    }

    public sealed override void LoadBitmap(SharpDx3D sdx)
    {
        Items = new Dictionary<string, Bitmap>
        {
            {
                "CCW",
                sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.CCW.png")!)!.Item1
            },
            {
                "CW", 
                sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.CW.png")!)!.Item1
            },
            {
                "VT40",
                sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.VT40.png")!)!.Item1
            },
        };

        Objects = new Dictionary<string, Obj>()
        {
            {
                "VT40",
                new Obj
                {
                    Scene = EmbeddedResources.Get<Scene>("Sprites.vt40.obj")!,
                    Texture = sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.texture.png")!)!.Item2,
                    TextureN = sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.textureN.png")!)!.Item2,
                    M = 1.0f,
                }
            },
            {
            "Сфера",
            new Obj
            {
                Scene = EmbeddedResources.Get<Scene>("Sprites.sphere.obj")!,
                Texture = sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.textureN.png")!)!.Item2,
                TextureN = sdx.CreateDxBitmap(EmbeddedResources.Get<System.Drawing.Bitmap>("Sprites.textureN.png")!)!.Item2,
                M = 1.0f,
            }
        }
        };

        Objects.ToList().ForEach(x => x.Value.VertexM = SharpDxG3D.LoadFromSprite(x.Value));
    }
}
