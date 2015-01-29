#load "DXSystem.csx"

// Size needs to be a multiple of 16.
[StructLayout(LayoutKind.Explicit, Size = 16)]
struct PS_CONSTANT_BUF_DATA
{
    [FieldOffset(0)]
    public float randX;
    [FieldOffset(4)]
    public float randY;
    [FieldOffset(8)]
    public float randZ;
    [FieldOffset(12)]
    public float randW;
};

PS_CONSTANT_BUF_DATA psConstData = new PS_CONSTANT_BUF_DATA
{
    randX = 0.4f,
    randY = 1.0f,
    randZ = 0f,
    randW = 0f
};

string ShaderFile = Path.GetFullPath("Examples/SharpDX/MiniTri.fx");

DXSystem dx = new DXSystem();

dx.InitializeDXSystem();

// Compile Vertex and Pixel shaders
var vertexShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFile, "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
var vertexShader = new VertexShader(dx.device, vertexShaderByteCode);

var pixelShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFile, "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
var pixelShader = new PixelShader(dx.device, pixelShaderByteCode);

dx.SetupLayout(vertexShaderByteCode);

// Instantiate Vertex buiffer from vertex data
var vertices = SharpDX.Direct3D11.Buffer.Create(dx.device, BindFlags.VertexBuffer, new[]
{
    // Position, Color
    new Vector4(0.0f, 0.5f, 0.51f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
    new Vector4(1.0f, -0.5f, 0.51f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
    new Vector4(-1.0f, -0.5f, 0.51f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

    new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
    new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
    new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
});

var _psConstantBuffer = new SharpDX.Direct3D11.Buffer(
                              dx.device
                            , Utilities.SizeOf<PS_CONSTANT_BUF_DATA>()
                            , ResourceUsage.Default
                            , BindFlags.ConstantBuffer
                            , CpuAccessFlags.None
                            , ResourceOptionFlags.None
                            , 0);

dx.PrepareStages(vertexShader, pixelShader, vertices);
dx.context.PixelShader.SetConstantBuffer(0, _psConstantBuffer);

float increment = 0.00001f;
// Main loop
RenderLoop.Run(dx.form, () =>
{
    if(psConstData.randX >= 1 || psConstData.randX <= 0)
        increment *= -1;
    psConstData.randX += increment;
    dx.context.UpdateSubresource(ref psConstData, _psConstantBuffer);
    dx.context.ClearRenderTargetView(dx.renderView, Color.Black);
    dx.context.Draw(6, 0);
    dx.swapChain.Present(0, PresentFlags.None);
});

// Release all resources
vertexShaderByteCode.Dispose();
vertexShader.Dispose();
pixelShaderByteCode.Dispose();
pixelShader.Dispose();
vertices.Dispose();
_psConstantBuffer.Dispose();
dx.DisposeDXSystem();
