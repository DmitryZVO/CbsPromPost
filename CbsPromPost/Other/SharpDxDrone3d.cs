using SharpDX.Mathematics.Interop;
using CbsPromPost.Model;
using CbsPromPost.Resources;
using SharpDX.Direct3D11;
using SharpDX;
using SharpDX.Direct2D1;

namespace CbsPromPost.Other;

internal class SharpDxDrone3d : SharpDx3D
{
    private readonly SerialBetaflight _betaflight;
    public SharpDxDrone3d(PictureBox surfacePtr, int fpsTarget, SerialBetaflight bf) : base(surfacePtr, fpsTarget, new Sprites3D(), 800)
    {
        _betaflight = bf;
    }

    public static float GradToRad(float grad)
    {
        const double oneGrad = (float)(Math.PI / 180d);
        return (float)(grad * oneGrad);
    }

    protected sealed override void DrawUser()
    {
        try
        {
            lock (this)
            {
                Rt?.BeginDraw();
                Rt?.Clear(new RawColor4(0.0f, 0.0f, 0.0f, 1.0f));
                Rt?.EndDraw();

                if (Context.IsDisposed) return;
                Context.ClearDepthStencilView(D3dDepthView, DepthStencilClearFlags.Depth, 1.0f,
                    0); // Очистка буфера глубины

                // Дрон
                var obj0 = new SharpDxG3D.Obj3D
                {
                    G3Ds = 0.5f, // размер
                    G3Dx = -0.9f, // Позиция X
                    G3Dy = -2.5f + 3.5f, // Позиция Y
                    G3Dz = -2.0f, // Позиция Z
                    G3DAx = GradToRad(_betaflight.Pitch),
                    G3DAy = 0.0f, //GradToRad(_betaflight.Yaw),
                    G3DAz = GradToRad(_betaflight.Roll),
                };
                var p0 = new SharpDxG3D.ShaderParam(obj0.MViewWorldMath(), new Color4(1.0f, 1.0f, 1.0f, 1.0f));
                var o0 = Sprites.Objects["VT40"];
                Context.PixelShader.SetShaderResource(0, o0.Texture);
                Context.PixelShader.SetShaderResource(1, o0.TextureN);
                using var b0 = SharpDX.Direct3D11.Buffer.Create(Device, BindFlags.VertexBuffer, o0.VertexM);
                Context.InputAssembler.SetVertexBuffers(0,
                    new VertexBufferBinding(b0, Utilities.SizeOf<SharpDxG3D.VertexPc>(), 0));
                Context.UpdateSubresource(ref p0, D3dshParBuf);
                Context.Draw(o0.VertexM.Length, 0);

                // Мотор 1
                var obj1 = new SharpDxG3D.Obj3D
                {
                    G3Ds = 0.012f, // размер
                    G3Dx = 4.5f, // Позиция X
                    G3Dy = -0.3f + 3.5f, // Позиция Y
                    G3Dz = -4.3f, // Позиция Z
                    G3DAx = GradToRad(_betaflight.Pitch),
                    G3DAy = 0.0f, //GradToRad(_betaflight.Yaw),
                    G3DAz = GradToRad(_betaflight.Roll),
                };
                var p1 = new SharpDxG3D.ShaderParam(obj1.MViewWorldMath(), new Color4(1.0f, 1.0f, 0.0f, 1.0f));
                var o1 = Sprites.Objects["Сфера"];
                Context.PixelShader.SetShaderResource(0, o1.Texture);
                Context.PixelShader.SetShaderResource(1, o1.TextureN);
                using var b1 = SharpDX.Direct3D11.Buffer.Create(Device, BindFlags.VertexBuffer, o1.VertexM);
                Context.InputAssembler.SetVertexBuffers(0,
                    new VertexBufferBinding(b1, Utilities.SizeOf<SharpDxG3D.VertexPc>(), 0));
                Context.UpdateSubresource(ref p1, D3dshParBuf);
                Context.Draw(o1.VertexM.Length, 0);

                // Мотор 2
                var obj2 = new SharpDxG3D.Obj3D
                {
                    G3Ds = 0.012f, // размер
                    G3Dx = 4.5f, // Позиция X
                    G3Dy = -0.3f + 3.5f, // Позиция Y
                    G3Dz = 4.0f, // Позиция Z
                    G3DAx = GradToRad(_betaflight.Pitch),
                    G3DAy = 0.0f, //GradToRad(_betaflight.Yaw),
                    G3DAz = GradToRad(_betaflight.Roll),
                };
                var p2 = new SharpDxG3D.ShaderParam(obj2.MViewWorldMath(), new Color4(0.0f, 1.0f, 0.0f, 1.0f));
                var o2 = Sprites.Objects["Сфера"];
                Context.PixelShader.SetShaderResource(0, o2.Texture);
                Context.PixelShader.SetShaderResource(1, o2.TextureN);
                using var b2 = SharpDX.Direct3D11.Buffer.Create(Device, BindFlags.VertexBuffer, o2.VertexM);
                Context.InputAssembler.SetVertexBuffers(0,
                    new VertexBufferBinding(b2, Utilities.SizeOf<SharpDxG3D.VertexPc>(), 0));
                Context.UpdateSubresource(ref p2, D3dshParBuf);
                Context.Draw(o2.VertexM.Length, 0);

                // Мотор 3
                var obj3 = new SharpDxG3D.Obj3D
                {
                    G3Ds = 0.012f, // размер
                    G3Dx = -4.5f, // Позиция X
                    G3Dy = -0.3f + 3.5f, // Позиция Y
                    G3Dz = -4.3f, // Позиция Z
                    G3DAx = GradToRad(_betaflight.Pitch),
                    G3DAy = 0.0f, //GradToRad(_betaflight.Yaw),
                    G3DAz = GradToRad(_betaflight.Roll),
                };
                var p3 = new SharpDxG3D.ShaderParam(obj3.MViewWorldMath(), new Color4(1.0f, 0.0f, 0.0f, 1.0f));
                var o3 = Sprites.Objects["Сфера"];
                Context.PixelShader.SetShaderResource(0, o3.Texture);
                Context.PixelShader.SetShaderResource(1, o3.TextureN);
                using var b3 = SharpDX.Direct3D11.Buffer.Create(Device, BindFlags.VertexBuffer, o3.VertexM);
                Context.InputAssembler.SetVertexBuffers(0,
                    new VertexBufferBinding(b3, Utilities.SizeOf<SharpDxG3D.VertexPc>(), 0));
                Context.UpdateSubresource(ref p3, D3dshParBuf);
                Context.Draw(o3.VertexM.Length, 0);

                // Мотор 4
                var obj4 = new SharpDxG3D.Obj3D
                {
                    G3Ds = 0.012f, // размер
                    G3Dx = -4.5f, // Позиция X
                    G3Dy = -0.3f + 3.5f, // Позиция Y
                    G3Dz = 4.0f, // Позиция Z
                    G3DAx = GradToRad(_betaflight.Pitch),
                    G3DAy = 0.0f, //GradToRad(_betaflight.Yaw),
                    G3DAz = GradToRad(_betaflight.Roll),
                };
                var p4 = new SharpDxG3D.ShaderParam(obj4.MViewWorldMath(), new Color4(0.2f, 0.2f, 1.0f, 1.0f));
                var o4 = Sprites.Objects["Сфера"];
                Context.PixelShader.SetShaderResource(0, o4.Texture);
                Context.PixelShader.SetShaderResource(1, o4.TextureN);
                using var b4 = SharpDX.Direct3D11.Buffer.Create(Device, BindFlags.VertexBuffer, o4.VertexM);
                Context.InputAssembler.SetVertexBuffers(0,
                    new VertexBufferBinding(b4, Utilities.SizeOf<SharpDxG3D.VertexPc>(), 0));
                Context.UpdateSubresource(ref p4, D3dshParBuf);
                Context.Draw(o4.VertexM.Length, 0);

                Rt?.BeginDraw();
                var xR = BaseWidth * 0.2f;
                var xP = BaseWidth * 0.65f;
                var y = BaseHeight * 0.01f;
                Rt?.DrawText($"Roll={_betaflight.Roll:0.0}", Brushes.SysText34,
                    new RawRectangleF(xR, y, BaseWidth, BaseHeight), Brushes.SysTextBrushWhite);
                Rt?.DrawText($"Pitch={_betaflight.Pitch:0.0}", Brushes.SysText34,
                    new RawRectangleF(xP, y, BaseWidth, BaseHeight), Brushes.SysTextBrushWhite);
                var video = new RawRectangleF(BaseWidth - 310f, BaseHeight - 235f, BaseWidth - 10f, BaseHeight - 10f);
                Rt?.DrawBitmap(FrameVideo, video, 1.0f,
                    BitmapInterpolationMode.Linear);
                Rt?.DrawRectangle(video, Brushes.SysTextBrushGray, 2);

                const float wC = 30f;
                const float hC = 60f;
                var posCy = BaseHeight * 0.85f;

                var posCx = BaseWidth * 0.06f;
                var aX = (Math.Min(Math.Max(_betaflight.AccX, -1000), 1000) / 1000f) * hC;
                var aY = (Math.Min(Math.Max(_betaflight.AccY, -1000), 1000) / 1000f) * hC;
                var aZ = (Math.Min(Math.Max(_betaflight.AccZ, -1000), 1000) / 1000f) * hC;
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 0, posCy - aX, posCx + wC * (0 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 0, posCy - hC, posCx + wC * (0 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 1, posCy - aY, posCx + wC * (1 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 1, posCy - hC, posCx + wC * (1 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 2, posCy - aZ, posCx + wC * (2 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 2, posCy - hC, posCx + wC * (2 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.DrawLine(new RawVector2(posCx, posCy), new RawVector2(posCx + wC * 3, posCy),
                    Brushes.SysTextBrushGray, 3);
                Rt?.DrawText(
                    $"АКСЕЛЕРОМЕТР", Brushes.SysText14,
                    new RawRectangleF(posCx - 10f, posCy - hC - 25f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("X", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 0 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("Y", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 1 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("Z", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 2 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);

                posCx += wC * 5;
                aX = (Math.Min(Math.Max(_betaflight.GyroX, -1000), 1000) / 1000f) * hC;
                aY = (Math.Min(Math.Max(_betaflight.GyroY, -1000), 1000) / 1000f) * hC;
                aZ = (Math.Min(Math.Max(_betaflight.GyroZ, -1000), 1000) / 1000f) * hC;
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 0, posCy - aX, posCx + wC * (0 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 0, posCy - hC, posCx + wC * (0 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 1, posCy - aY, posCx + wC * (1 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 1, posCy - hC, posCx + wC * (1 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 2, posCy - aZ, posCx + wC * (2 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 2, posCy - hC, posCx + wC * (2 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.DrawLine(new RawVector2(posCx, posCy), new RawVector2(posCx + wC * 3, posCy),
                    Brushes.SysTextBrushGray, 3);
                Rt?.DrawText(
                    $"ГИРОСКОП", Brushes.SysText14,
                    new RawRectangleF(posCx + 8f, posCy - hC - 25f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("X", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 0 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("Y", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 1 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("Z", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 2 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);

                posCx += wC * 5;
                aX = (Math.Min(Math.Max(_betaflight.MagX, -1000), 1000) / 1000f) * hC;
                aY = (Math.Min(Math.Max(_betaflight.MagY, -1000), 1000) / 1000f) * hC;
                aZ = (Math.Min(Math.Max(_betaflight.MagZ, -1000), 1000) / 1000f) * hC;
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 0, posCy - aX, posCx + wC * (0 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 0, posCy - hC, posCx + wC * (0 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 1, posCy - aY, posCx + wC * (1 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 1, posCy - hC, posCx + wC * (1 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC * 2, posCy - aZ, posCx + wC * (2 + 1), posCy), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * 2, posCy - hC, posCx + wC * (2 + 1), posCy + hC), Brushes.SysTextBrushGray, 2);
                Rt?.DrawLine(new RawVector2(posCx, posCy), new RawVector2(posCx + wC * 3, posCy),
                    Brushes.SysTextBrushGray, 3);
                Rt?.DrawText(
                    $"МАГНИТОМЕТР", Brushes.SysText14,
                    new RawRectangleF(posCx - 7f, posCy - hC - 25f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("X", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 0 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("Y", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 1 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText("Z", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * 2 + 10f, posCy + hC + 4f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);

                if (!_betaflight.IsAlive())
                {
                    Rt?.FillRectangle(new RawRectangleF(0, 0, BaseWidth, BaseHeight), Brushes.RoiRed02);
                    _betaflight.Roll = 0f;
                    _betaflight.Pitch = 0f;
                }

                Rt?.EndDraw();
            }
        }
        catch
        {
            //
        }
    }

    protected sealed override void DrawInfo()
    {
    }
}
