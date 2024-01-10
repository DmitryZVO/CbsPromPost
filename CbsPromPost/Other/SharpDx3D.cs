using CbsPromPost.Resources;
using OpenCvSharp;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.DirectWrite;
using SharpDX.Mathematics;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using SharpDX.Text;
using Rectangle = System.Drawing.Rectangle;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace CbsPromPost.Other;

public abstract class SharpDx3D : IDisposable
{
    protected int FpsTarget; // сжелаемое FPS
    protected int FpsScrC; // счетчик кадров экрана
    protected int FpsOcvC; // счетчик кадров техзрения

    protected readonly SharpDX.Direct3D11.Device Device;
    protected SwapChain? SwapChain;
    protected readonly Surface D2dSurface;
    protected readonly SharpDX.DXGI.Factory D2DFactory;
    protected readonly SharpDX.Direct2D1.Factory D2dFactory;
    protected RenderTarget? Rt;
    protected readonly SharpDX.DirectWrite.Factory DWf;
    protected readonly PixelFormat PixelFormat;
    protected readonly PictureBox FormTarget;
    protected readonly Texture2D BackBuffer;
    protected readonly RenderTargetView RenderView;
    protected readonly SharpDX.Direct3D11.DeviceContext Context;
    protected readonly VertexShader D3DVertexShader; // Вершинный Шейдер
    protected readonly PixelShader D3DPixelShader; // Пиксельный Шейдер
    protected readonly SharpDX.Direct3D11.Buffer D3dshParBuf; // Буфер для передачи в шейдеры (параметры рендеринга)
    protected readonly DepthStencilView D3dDepthView; // Буфер глубины direct3D


    protected readonly DefBrushes Brushes;

    protected double FpsScr; // текущая FPS экрана
    protected double FpsOcv; // текущая FPS экрана
    protected readonly SpritesDb3D Sprites; // Спрайты
    protected Bitmap FrameVideo; // Видеокадр
    protected int BaseWidth;
    protected int BaseHeight;

    protected RawMatrix3x2 ZeroTransform = new(1, 0, 0, 1, 0, 0);
    private bool _closed;

    public virtual async Task FrameUpdateAsync(Mat frame)
    {
        var temp = CreateDxBitmap(frame);
        if (temp is null) return;

        lock (this)
        {
            FrameVideo.Dispose();
            FrameVideo = temp;
        }

        FpsOcvC++;

        await RenderCallback();
    }

    protected void TransformSet(RawMatrix3x2 matrix)
    {
        lock (this)
        {
            if (Rt != null) Rt.Transform = matrix;
        }
    }

    protected RawMatrix3x2 TransformGet()
    {
        lock (this)
        {
            return Rt?.Transform ?? new RawMatrix3x2();
        }
    }

    protected PathGeometry PathGeometryGet()
    {
        lock (this)
        {
            return new PathGeometry(D2dFactory);
        }
    }

    protected class DefBrushes
    {
        public TextFormat SysText14;
        public TextFormat SysText20;
        public TextFormat SysText34;
        public TextFormat SysText74;
        public SharpDX.Direct2D1.Brush SysTextBrushBlue;
        public SharpDX.Direct2D1.Brush SysTextBrushOrange;
        public SharpDX.Direct2D1.Brush SysTextBrushYellow;
        public SharpDX.Direct2D1.Brush SysTextBrushRed;
        public SharpDX.Direct2D1.Brush SysTextBrushDarkGreen;
        public SharpDX.Direct2D1.Brush SysTextBrushGreen;
        public SharpDX.Direct2D1.Brush SysTextBrushWhite;
        public SharpDX.Direct2D1.Brush SysTextBrushGray;
        public SharpDX.Direct2D1.Brush SysTextBrushBlack;
        public SharpDX.Direct2D1.Brush RoiNone;
        public SharpDX.Direct2D1.Brush RoiHq;
        public SharpDX.Direct2D1.Brush RoiRed01;
        public SharpDX.Direct2D1.Brush RoiGreen01;
        public SharpDX.Direct2D1.Brush RoiGray01;
        public SharpDX.Direct2D1.Brush RoiYellow01;
        public SharpDX.Direct2D1.Brush RoiRed02;
        public SharpDX.Direct2D1.Brush RoiGreen02;
        public SharpDX.Direct2D1.Brush RoiGray02;
        public SharpDX.Direct2D1.Brush RoiBlue02;
        public SharpDX.Direct2D1.Brush RoiYellow02;
        public SharpDX.Direct2D1.Brush RoiRed03;
        public SharpDX.Direct2D1.Brush RoiGreen03;
        public SharpDX.Direct2D1.Brush RoiGray03;
        public SharpDX.Direct2D1.Brush RoiYellow03;

        public DefBrushes(SharpDx3D sdx)
        {
            SysText14 = new TextFormat(sdx.DWf, "Arial", 14);
            SysText20 = new TextFormat(sdx.DWf, "Arial", 20);
            SysText34 = new TextFormat(sdx.DWf, "Arial", 34);
            SysText74 = new TextFormat(sdx.DWf, "Arial", 74);
            SysTextBrushBlue = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.3F, 0.3F, 1.0F, 1.0F));
            SysTextBrushOrange = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 0.5F, 0.0F, 0.9F));
            SysTextBrushRed = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 0.0F, 0.0F, 0.9F));
            SysTextBrushYellow = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 1.0F, 0.0F, 0.9F));
            SysTextBrushGreen = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.0F, 1.0F, 0.0F, 0.9F));
            SysTextBrushDarkGreen = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.0F, 0.5F, 0.0F, 0.9F));
            SysTextBrushWhite = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 1.0F, 1.0F, 0.7F));
            SysTextBrushGray = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.3F, 0.3F, 0.3F, 0.9F));
            SysTextBrushBlack = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.0F, 0.0F, 0.0F, 0.9F));
            RoiNone = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.0F, 0.0F, 0.0F, 0.5F));
            RoiHq = new SolidColorBrush(sdx.Rt, new RawColor4(1.0F, 1.0F, 0.0F, 0.1F));
            RoiRed01 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 0.0F, 0.0F, 0.1F));
            RoiGreen01 = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.0F, 1.0F, 0.0F, 0.1F));
            RoiGray01 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 1.0F, 1.0F, 0.1F));
            RoiYellow01 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 1.0F, 0.0F, 0.1F));
            RoiRed02 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 0.0F, 0.0F, 0.2F));
            RoiBlue02 = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.0F, 0.0F, 1.0F, 0.2F));
            RoiGreen02 = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.0F, 1.0F, 0.0F, 0.2F));
            RoiGray02 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 1.0F, 1.0F, 0.2F));
            RoiYellow02 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 1.0F, 0.0F, 0.2F));
            RoiRed03 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 0.0F, 0.0F, 0.3F));
            RoiGreen03 = new SolidColorBrush(sdx.Rt,
                new RawColor4(0.0F, 1.0F, 0.0F, 0.3F));
            RoiGray03 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 1.0F, 1.0F, 0.3F));
            RoiYellow03 = new SolidColorBrush(sdx.Rt,
                new RawColor4(1.0F, 1.0F, 0.0F, 0.3F));
        }

        public void Dispose()
        {
            SysText14.Dispose();
            SysText20.Dispose();
            SysText34.Dispose();
            SysText74.Dispose();
            SysTextBrushBlue.Dispose();
            SysTextBrushOrange.Dispose();
            SysTextBrushRed.Dispose();
            SysTextBrushYellow.Dispose();
            SysTextBrushDarkGreen.Dispose();
            SysTextBrushGreen.Dispose();
            SysTextBrushWhite.Dispose();
            SysTextBrushGray.Dispose();
            SysTextBrushBlack.Dispose();
            RoiNone.Dispose();
            RoiHq.Dispose();
            RoiRed01.Dispose();
            RoiGreen01.Dispose();
            RoiGray01.Dispose();
            RoiYellow01.Dispose();
            RoiRed02.Dispose();
            RoiBlue02.Dispose();
            RoiGreen02.Dispose();
            RoiGray02.Dispose();
            RoiYellow02.Dispose();
            RoiRed03.Dispose();
            RoiGreen03.Dispose();
            RoiGray03.Dispose();
            RoiYellow03.Dispose();
        }
    }

    protected SharpDx3D(PictureBox surface, int fpsTarget, SpritesDb3D sprites, int widthVirtual) // Инициализация класса
    {
        var scale = widthVirtual / (float)surface.Size.Width;
        FormTarget = surface;
        FpsTarget = fpsTarget;
        BaseWidth = (int)(surface.Width * scale);
        BaseHeight = (int)(surface.Height * scale);
        PixelFormat = new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Ignore);

        ////////////////// Инициализация Direct3D
        var bufferDescription = new ModeDescription()
        {
            Width = BaseWidth, // Ширина
            Height = BaseHeight, // Высота
            RefreshRate = new Rational(FpsTarget, 1), // Частота обновления изображения
            Format = Format.R8G8B8A8_UNorm // Формат пикселей в буфере
        };
        var swapChainDesc = new SwapChainDescription() // Структура которая инициализирует DirectX 11, 
        {
            BufferCount = 1, // Количество буферов
            ModeDescription = bufferDescription, // Описание буфера
            IsWindowed = true, // Режим отображения окно/полный экран
            OutputHandle = surface.Handle, // Ссылка на заголовок формы рендеринга
            SampleDescription = new SampleDescription(1, 0), // ????
            SwapEffect = SwapEffect.Discard, // ????
            Usage = Usage.RenderTargetOutput, // Куда выводить 
            Flags = SwapChainFlags.AllowModeSwitch, // Флаги ??
        };
        SharpDX.Direct3D11.Device.CreateWithSwapChain( // Инициализируем Direct3D11
            DriverType.Hardware, // Использовать ускорение видеодаптера
            DeviceCreationFlags.BgraSupport, // Поддержка DirectDraw
            swapChainDesc, // Структура описывающия инициализацию DirectX
            out Device, // Куда выводить
            out SwapChain); // Ссылка на буфер
        BackBuffer = SwapChain.GetBackBuffer<Texture2D>(0);
        RenderView = new RenderTargetView(Device, BackBuffer);
        Context = Device.ImmediateContext;
        ////////////////// Инициализация Direct2D
        D2dFactory = new SharpDX.Direct2D1.Factory(); // Создаем фактуру Direct2D
        D2DFactory = SwapChain.GetParent<SharpDX.DXGI.Factory>();
        D2DFactory.MakeWindowAssociation(surface.Handle, WindowAssociationFlags.IgnoreAll);
        D2dSurface = BackBuffer.QueryInterface<Surface>();
        Rt = new RenderTarget(D2dFactory, D2dSurface,
            new RenderTargetProperties
            {
                MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_DEFAULT,
                PixelFormat = PixelFormat,
                Type = RenderTargetType.Hardware,
                Usage = RenderTargetUsage.ForceBitmapRemoting
            });
        Context.OutputMerger.SetRenderTargets(RenderView);
        DWf = new SharpDX.DirectWrite.Factory();

        /////////////// ИНИЦИАЛИЗАЦИЯ 3D ////////////////////////

        ShaderSignature inputSignature;
        using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile("VertexShader.hlsl", "main", "vs_4_0", ShaderFlags.Debug))
        {
            D3DVertexShader = new VertexShader(Device, vertexShaderByteCode);
            inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
        }
        using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile("PixelShader.hlsl", "main", "ps_4_0", ShaderFlags.Debug))
        {
            D3DPixelShader = new PixelShader(Device, pixelShaderByteCode);
        }

        var inputElements = new SharpDX.Direct3D11.InputElement[]
        {
                new("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new("COLOR", 0, Format.R32G32B32A32_Float, 12, 0, InputClassification.PerVertexData, 0),
                new("TEXCOORD", 0, Format.R32G32_Float, 28, 0, InputClassification.PerVertexData, 0),
                new("NORMAL", 0, Format.R32G32B32_Float, 36, 0, InputClassification.PerVertexData, 0)
        };

        D3dshParBuf = new SharpDX.Direct3D11.Buffer(Device, Utilities.SizeOf<SharpDxG3D.ShaderParam>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        Context.InputAssembler.InputLayout = new InputLayout(Device, inputSignature, inputElements);

        Context.VertexShader.Set(D3DVertexShader);
        Context.VertexShader.SetConstantBuffer(0, D3dshParBuf);
        Context.PixelShader.Set(D3DPixelShader);
        Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

        using var depthBuffer = new Texture2D(Device, new Texture2DDescription()
        {
            Format = Format.D32_Float,
            ArraySize = 1,
            MipLevels = 1,
            Width = BaseWidth, ///////////////////////
            Height = BaseHeight, ///////////////////////
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.DepthStencil,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        });
        D3dDepthView = new DepthStencilView(Device, depthBuffer);
        Context.OutputMerger.SetTargets(D3dDepthView, RenderView);

        var description = DepthStencilStateDescription.Default();
        description.DepthComparison = Comparison.LessEqual;
        description.IsDepthEnabled = true;
        description.DepthWriteMask = DepthWriteMask.All;
        var depthState = new DepthStencilState(Device, description);
        Context.OutputMerger.SetDepthStencilState(depthState);

        var bD = new BlendStateDescription();
        bD.RenderTarget[0].IsBlendEnabled = true;
        bD.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
        bD.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
        bD.RenderTarget[0].BlendOperation = SharpDX.Direct3D11.BlendOperation.Add;
        bD.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
        bD.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
        bD.RenderTarget[0].AlphaBlendOperation = SharpDX.Direct3D11.BlendOperation.Add;
        bD.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All; //All
        Context.Rasterizer.SetViewport(new Viewport(0, 0, BaseWidth, BaseHeight, 0.0f, 1.0f));
        var d3DblendState = new BlendState(Device, bD);
        Context.OutputMerger.SetBlendState(d3DblendState);
        /////////////// ================ ////////////////////////

        /////////////// ================ ////////////////////////
        Brushes = new DefBrushes(this);
        Sprites = sprites;
        Sprites.LoadBitmap(this);

        FrameVideo = new Bitmap(
            Rt,
            new Size2(BaseWidth, BaseHeight),
            new BitmapProperties
            { PixelFormat = PixelFormat });
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _ = MathFpsAsync(cancellationToken);

        if (FpsTarget <= 0) return;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (_closed) break;
            await RenderCallback(cancellationToken);
        }
    }

    private async Task MathFpsAsync(CancellationToken cancellationToken = default) // Таймер пересчета FPS
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_closed) break;
            await Task.Delay(1000, cancellationToken);

            FpsScr = FpsScrC;
            FpsScrC = 0;
            FpsOcv = FpsOcvC;
            FpsOcvC = 0;
        }
    }

    public Tuple<Bitmap, ShaderResourceView>? CreateDxBitmap(System.Drawing.Bitmap sbm)
    {
        lock (this)
        {
            if (Rt is null) return null;
            try
            {
                var bmpData = sbm.LockBits(new Rectangle(0, 0, sbm.Width, sbm.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                var stream = new DataStream(bmpData.Scan0, bmpData.Stride * bmpData.Height, true, false);
                var pFormat = new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
                var bmpProps = new BitmapProperties(pFormat);

                using var ret = new Texture2D(Device, new Texture2DDescription()
                {
                    Width = sbm.Width,
                    Height = sbm.Height,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource,
                    Usage = ResourceUsage.Immutable,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                }, new DataRectangle(bmpData.Scan0, bmpData.Stride));

                var result = new Tuple<Bitmap, ShaderResourceView>(
                    new Bitmap(
                        Rt,
                        new Size2(sbm.Width, sbm.Height),
                        stream,
                        bmpData.Stride,
                        bmpProps), new ShaderResourceView(Device, ret));

                sbm.UnlockBits(bmpData);
                stream.Dispose();
                return result;
            }
            catch
            {
                return new Tuple<Bitmap, ShaderResourceView>(new Bitmap(Rt, new Size2(sbm.Width, sbm.Height)),
                    new ShaderResourceView(Device, null));
            }
        }
    }

    public Bitmap? CreateDxBitmap(Mat mat)
    {
        if (mat.Empty()) return null;

        lock (this)
        {
            if (Rt is null) return null;
            try
            {
                switch (mat.Channels())
                {
                    case 1:
                        Cv2.CvtColor(mat, mat, ColorConversionCodes.GRAY2RGBA);
                        break;
                    case 3:
                        Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGBA);
                        break;
                    case 4:
                        Cv2.CvtColor(mat, mat, ColorConversionCodes.BGRA2RGBA);
                        break;
                }

                using var stream = new DataStream(mat.Data, mat.Cols * mat.Rows * 4, true, false);
                var bmpProps = new BitmapProperties(PixelFormat);

                var result =
                    new Bitmap(
                        Rt,
                        new Size2(mat.Cols, mat.Rows),
                        stream,
                        mat.Cols * 4,
                        bmpProps);
                return result;
            }
            catch
            {
                return new Bitmap(Rt, new Size2(mat.Width, mat.Height));
            }
        }
    }

    public Mat GetBitmapFromSharpDxRender()
    {
        Mat ret;

        lock (this)
        {
            var desc = BackBuffer.Description;
            desc.CpuAccessFlags = CpuAccessFlags.Read;
            desc.Usage = ResourceUsage.Staging;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.BindFlags = BindFlags.None;

            using var texture = new Texture2D(Device, desc);
            Context.CopyResource(BackBuffer, texture);

            using var surface = texture.QueryInterface<Surface>();
            surface.Map(SharpDX.DXGI.MapFlags.Read, out var dataStream);

            using (var mat = new Mat(BaseHeight, BaseWidth, MatType.CV_8UC4, dataStream.DataPointer))
            {
                Cv2.CvtColor(mat, mat, ColorConversionCodes.RGBA2BGR);
                ret = mat.Clone();
            }

            surface.Unmap();
            dataStream.Dispose();
        }

        return ret;
    }

    public void Dispose() // Освобождение ресурсов
    {
        lock (this)
        {
            _closed = true;
            Brushes.Dispose();
            RenderView.Dispose();
            BackBuffer.Dispose();
            Device.Dispose();
            D3DVertexShader.Dispose();
            D3DPixelShader.Dispose();
            D3dDepthView.Dispose();
            Context.Dispose();
            SwapChain?.Dispose();
            SwapChain = null;
            D2dSurface.Dispose();
            D2DFactory.Dispose();
            D2dFactory.Dispose();
            Rt?.Dispose();
            Rt = null;
            DWf.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    public async Task RenderCallback(CancellationToken cancellationToken = default) // Цикл отрисовки изображений в окне камеры
    {
        var startFrameTime = DateTime.Now;

        lock (this)
        {
            DrawUser(); // Вывод пользовательской графики
            DrawInfo(); // Вывод статистики
            try
            {
                SwapChain?.Present(0, PresentFlags.None);
            }
            catch
            {
                //
            }
        }

        await Task.Delay((int)Math.Max(1, 1000d / FpsTarget - (DateTime.Now - startFrameTime).TotalMilliseconds), cancellationToken);

        FpsScrC++;
    }

    protected virtual void DrawUser()
    {
        Rt?.Clear(new RawColor4(0, 0, 0, 1));
    }

    protected virtual void DrawInfo()
    {
        Rt?.DrawText(
            "[FPS] ЭКРАН: " + FpsScr.ToString("0") + ", ТЕХ. ЗРЕНИЕ: " + FpsOcv.ToString("0"),
            Brushes.SysText14,
            new RawRectangleF(10, 10, BaseWidth, BaseHeight),
            Brushes.SysTextBrushYellow);
    }
    public abstract class SpritesDb3D
    {
        public Dictionary<string, Bitmap> Items { get; set; } = new();
        public Dictionary<string, Sprites3D.Obj> Objects { get; set; } = new();

        public virtual void DisposeBitmap()
        {
            foreach (var i in Items)
            {
                i.Value.Dispose();
            }
        }

        public virtual void LoadBitmap(SharpDx3D sdx)
        {
        }
    }

}

