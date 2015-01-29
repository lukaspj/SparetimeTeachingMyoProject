#r "SharpDX/SharpDX.dll"
#r "SharpDX/SharpDX.D3DCompiler.dll"
#r "SharpDX/SharpDX.Direct3D11.dll"
#r "SharpDX/SharpDX.DXGI.dll"
#r "System.Windows.Forms"
#r "System.Drawing"

using System;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

public class DXSystem {
    public RenderForm form;
    public SwapChainDescription desc;
    public SharpDX.Direct3D11.Device device;
    public SwapChain swapChain;
    public DeviceContext context;
    public Factory factory;
    public Texture2D backBuffer;
    public RenderTargetView renderView;
    public InputLayout layout;

    public void InitializeDXSystem() {
        form = new RenderForm("SharpDX - MiniTri Direct3D 11 Sample");

        // SwapChain description
        desc = new SwapChainDescription()
        {
            BufferCount = 1,
            ModeDescription=
            new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
            new Rational(60, 1), Format.R8G8B8A8_UNorm),
            IsWindowed = true,
            OutputHandle = form.Handle,
            SampleDescription = new SampleDescription(1, 0),
            SwapEffect = SwapEffect.Discard,
            Usage = Usage.RenderTargetOutput
        };

        // Create Device and SwapChain
        SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
        context = device.ImmediateContext;

        // Ignore all windows events
        factory = swapChain.GetParent<Factory>();
        factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

        // New RenderTargetView from the backbuffer
        backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
        renderView = new RenderTargetView(device, backBuffer);
    }

    public void SetupLayout(CompilationResult vertexShaderByteCode) {
        // Layout from VertexShader input signature
        layout = new InputLayout(
        device,
        ShaderSignature.GetInputSignature(vertexShaderByteCode),
        new[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
        });
    }

    public void PrepareStages(VertexShader vertexShader, PixelShader pixelShader, SharpDX.Direct3D11.Buffer vertices)
    {
        // Prepare All the stages
        context.InputAssembler.InputLayout = layout;
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
        context.VertexShader.Set(vertexShader);
        context.Rasterizer.SetViewport(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
        context.PixelShader.Set(pixelShader);
        context.OutputMerger.SetTargets(renderView);
    }

    public void DisposeDXSystem()
    {
        layout.Dispose();
        renderView.Dispose();
        backBuffer.Dispose();
        context.ClearState();
        context.Flush();
        device.Dispose();
        context.Dispose();
        swapChain.Dispose();
        factory.Dispose();
    }
}
