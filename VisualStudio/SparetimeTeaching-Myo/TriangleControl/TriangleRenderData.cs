using System.IO;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SparetimeTeachingLibrary;

namespace TriangleControl
{
   public class TriangleRenderData
   {
      // Size needs to be a multiple of 16.
      [StructLayout(LayoutKind.Explicit, Size = 16)]
      public struct PS_CONSTANT_BUF_DATA
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

      private DXSystem dxSystem;
      private Buffer mPsConstantBuffer;
      private CompilationResult mVertexShaderByteCode;
      private VertexShader mVertexShader;
      private CompilationResult mPixelShaderByteCode;
      private PixelShader mPixelShader;
      private Buffer mVertices;

      public TriangleRenderData(DXSystem pDxSystem)
      {
         dxSystem = pDxSystem;
      }

      public void Initialize()
      {
         string ShaderFile = Path.GetFullPath("MiniTri.fx");

         // Compile Vertex and Pixel shaders
         mVertexShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFile, "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
         mVertexShader = new VertexShader(dxSystem.device, mVertexShaderByteCode);

         mPixelShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFile, "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
         mPixelShader = new PixelShader(dxSystem.device, mPixelShaderByteCode);

         dxSystem.SetupLayout(mVertexShaderByteCode);

         // Instantiate Vertex buiffer from vertex data
         mVertices = Buffer.Create(dxSystem.device, BindFlags.VertexBuffer, new[]
         {
            // Position, Color
            new Vector4(0.0f, 0.5f, 0.51f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(1.0f, -0.5f, 0.51f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            new Vector4(-1.0f, -0.5f, 0.51f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

            new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
         });

         mPsConstantBuffer = new Buffer(
            dxSystem.device
            , Utilities.SizeOf<PS_CONSTANT_BUF_DATA>()
            , ResourceUsage.Default
            , BindFlags.ConstantBuffer
            , CpuAccessFlags.None
            , ResourceOptionFlags.None
            , 0);

         dxSystem.PrepareStages(mVertexShader, mPixelShader, mVertices);
         dxSystem.context.PixelShader.SetConstantBuffer(0, mPsConstantBuffer);
      }

      public void UpdateConsts(ref PS_CONSTANT_BUF_DATA pPsConstData)
      {
         dxSystem.context.UpdateSubresource(ref pPsConstData, mPsConstantBuffer);
      }

      public void Dispose()
      {
         mVertexShaderByteCode.Dispose();
         mVertexShader.Dispose();
         mPixelShaderByteCode.Dispose();
         mPixelShader.Dispose();
         mVertices.Dispose();
         mPsConstantBuffer.Dispose();
      }

      public void Render()
      {
         dxSystem.context.ClearRenderTargetView(dxSystem.renderView, Color.Black);
         dxSystem.context.Draw(6, 0);
         dxSystem.swapChain.Present(0, PresentFlags.None);
      }
   }
}
