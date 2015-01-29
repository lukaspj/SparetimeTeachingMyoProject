struct VS_IN
{
  float4 pos : POSITION;
  float4 col : COLOR;
};

struct PS_IN
{
  float4 pos : SV_POSITION;
  float4 col : COLOR;
};

cbuffer RandBuffer : register(b0)
{
    float roll;
    float pitch;
    float yaw;
    float filler;
};

PS_IN VS( VS_IN input )
{
  PS_IN output = (PS_IN)0;

  output.pos = input.pos;
  output.col = input.col;

  return output;
}

float4 PS( PS_IN input ) : SV_Target
{
   input.col.r *= roll;
   input.col.g *= pitch;
   input.col.b *= yaw;
  return input.col;
}

technique10 Render
{
  pass P0
  {
    SetGeometryShader( 0 );
    SetVertexShader( CompileShader( vs_4_0, VS() ) );
    SetPixelShader( CompileShader( ps_4_0, PS() ) );
  }
}
