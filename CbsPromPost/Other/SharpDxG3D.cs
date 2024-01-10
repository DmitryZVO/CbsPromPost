using CbsPromPost.Resources;
using SharpDX;

namespace CbsPromPost.Other;

public static class SharpDxG3D // Класс описывающий 3D сцены (дополненная реальность)
{
    public static float CamAngleX { get; set; } = 1.0f; // Угол обзора камеры по оси X (для Raspberry PI)
    public static float CamAngleY { get; set; } = 1.0f; // Угол обзора камеры по оси Y
    public static float CamG3Dx { get; set; } = 0.0f; // Позиция камеры по оси Х
    public static float CamG3Dy { get; set; } = 0.1f; // Позиция камеры по оси Y
    public static float CamG3Dz { get; set; } = -20.0f; // Позиция камеры по оси Z
    public static float CamG3Dsx { get; set; } = 0.0f; // Точка в которую смотрит камера по Х
    public static float CamG3Dsy { get; set; } = 0.0f; // Точка в которую смотрит камера по Y
    public static float CamG3Dsz { get; set; } = 0.0f; // Точка в которую смотрит камера по Z
    public static float CamDepMin { get; set; } = 0.01f; // Минимально отображаемая глубина камеры
    public static float CamDepMax { get; set; } = 10000.0f; // Максимально отображаемая глубина камеры

    public static Matrix MCamera { get; set; } = Matrix.LookAtLH(new Vector3(CamG3Dx, CamG3Dy, CamG3Dz), new Vector3(CamG3Dsx, CamG3Dsy, CamG3Dsz), Vector3.UnitY); // Матрица позиции и направления зрителя
    public static Matrix MProj { get; set; } = Matrix.PerspectiveFovLH(CamAngleX, CamAngleY, CamDepMin, CamDepMax); // Матрица угла обзора камеры (проекции обьектов)

    public struct ShaderParam // Структура описывающая параметры для передачи в конвеер шейдеров
    {
        public Matrix MWorld; // Глобальная матрица мира
        public float Fr; // глобальный цвет фигуры, канал R<0 = не менять
        public float Fg; // глобальный цвет фигуры, канал G<0 = не менять
        public float Fb; // глобальный цвет фигуры, канал B<0 = не менять
        public float Fa; // глобальная прозрачность фигуры, канал A<0 = не менять
        public float UseL; // Использовать свет
        public float P0; // Использовать свет
        public float P1; // Использовать свет
        public float P2; // Использовать свет

        public ShaderParam(Matrix m)
        {
            MWorld = m;
            Fr = -1;
            Fg = -1;
            Fb = -1;
            Fa = -1;
            UseL = 1;
            P0 = 0;
            P1 = 0;
            P2 = 0;
        }
        public ShaderParam(Matrix m, float alpha)
        {
            MWorld = m;
            Fr = -1;
            Fg = -1;
            Fb = -1;
            Fa = alpha;
            UseL = 1;
            P0 = 0;
            P1 = 0;
            P2 = 0;
        }

        public ShaderParam(Matrix m, Color4 col)
        {
            MWorld = m;
            Fr = col.Red;
            Fg = col.Green;
            Fb = col.Blue;
            Fa = col.Alpha;
            UseL = 1;
            P0 = 0;
            P1 = 0;
            P2 = 0;
        }
    }

    public struct VertexPc // Класс описывающий вершины обьекта для отображения
    {
        public Vector3 Position;
        public Color4 Color;
        public Vector2 TexUv;
        public Vector3 Normal;

        public VertexPc(Vector3 position, Color4 color, Vector2 tUv, Vector3 normal)
        {
            Position = position;
            Color = color;
            TexUv = tUv;
            Normal = normal;
        }
    }

    public static VertexPc[] LoadFromSprite(Sprites3D.Obj o)
    {
        var allVertex = o.Scene.Meshes.Sum(m => m.VertexCount);
        var vl = new VertexPc[allVertex * 10];

        var n = 0;
        foreach (var m in o.Scene.Meshes)
        {
            foreach (var mo in m.Faces)
            {
                var i = 0;
                var i0 = mo.Indices[i + 0];
                while (i < (mo.IndexCount - 2))
                {
                    var i1 = mo.Indices[i + 1];
                    var i2 = mo.Indices[i + 2];
                    vl[n + 0].Position = new Vector3(m.Vertices[i0].X, m.Vertices[i0].Y, m.Vertices[i0].Z) * o.M;
                    vl[n + 0].Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
                    if (m.Normals.Count > 0) vl[n + 0].Normal = new Vector3(m.Normals[i0].X, m.Normals[i0].Y, m.Normals[i0].Z);
                    if (m.TextureCoordinateChannels[0].Count > 0) vl[n + 0].TexUv = new Vector2(m.TextureCoordinateChannels[0][i0].X, m.TextureCoordinateChannels[0][i0].Y);

                    vl[n + 1].Position = new Vector3(m.Vertices[i1].X, m.Vertices[i1].Y, m.Vertices[i1].Z) * o.M;
                    vl[n + 1].Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
                    if (m.Normals.Count > 0) vl[n + 1].Normal = new Vector3(m.Normals[i1].X, m.Normals[i1].Y, m.Normals[i1].Z);
                    if (m.TextureCoordinateChannels[0].Count > 0) vl[n + 1].TexUv = new Vector2(m.TextureCoordinateChannels[0][i1].X, m.TextureCoordinateChannels[0][i1].Y);

                    vl[n + 2].Position = new Vector3(m.Vertices[i2].X, m.Vertices[i2].Y, m.Vertices[i2].Z) * o.M;
                    vl[n + 2].Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
                    if (m.Normals.Count > 0) vl[n + 2].Normal = new Vector3(m.Normals[i2].X, m.Normals[i2].Y, m.Normals[i2].Z);
                    if (m.TextureCoordinateChannels[0].Count > 0) vl[n + 2].TexUv = new Vector2(m.TextureCoordinateChannels[0][i2].X, m.TextureCoordinateChannels[0][i2].Y);
                    i++;
                    n += 3;
                }
            }
        }
        Array.Resize(ref vl, n);
        return vl;
    }

    public class Obj3D // Класс описывающий 3D обьект отображения
    {
        public float G3Dx = 0.0f; // Позиция Х на глобальной 3D-сцене
        public float G3Dy = 0.0f; // Позиция Y на глобальной 3D-сцене
        public float G3Dz = 0.0f; // Позиция Z на глобальной 3D-сцене
        public float G3DAx = (float)(Math.PI * 0.0f); // Угол наклона относительно Х на глобальной 3D-сцене
        public float G3DAy = (float)(Math.PI * 0.0f); // Угол наклона относительно Y на глобальной 3D-сцене
        public float G3DAz = (float)(Math.PI * 0.0f); // Угол наклона относительно Z на глобальной 3D-сцене
        public float G3Ds = 1.0f; // Скалирование (увеличение размера относительно изначальных размеров)

        public Matrix MViewWorldMath() // Пересчитать матрицу глобальной позиции на 3D-сцене
        {
            MCamera = Matrix.LookAtLH(new Vector3(CamG3Dx, CamG3Dy, CamG3Dz), new Vector3(CamG3Dsx, CamG3Dsy, CamG3Dsz), new Vector3(0,0,1));
            //MCamera = Matrix.Multiply(MCamera, Matrix.RotationX(roll));
            //MCamera = Matrix.Multiply(MCamera, Matrix.RotationY(pitch));
            //MCamera = Matrix.Multiply(MCamera, Matrix.RotationZ(yaw));
            MProj = Matrix.PerspectiveFovLH(CamAngleX, CamAngleY, CamDepMin, CamDepMax);
            var mViewWorld = Matrix.Scaling(G3Ds) * Matrix.Translation(new Vector3(G3Dx, G3Dy, G3Dz)) * Matrix.RotationX(G3DAx) * Matrix.RotationY(G3DAy) * Matrix.RotationZ(G3DAz) * Matrix.Multiply(MCamera, MProj);
            mViewWorld.Transpose();
            return mViewWorld;
        }
    }
}