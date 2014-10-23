float4x4 World;
float4x4 View;
float4x4 Projection;
float time;

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.texCoord = input.texCoord;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float tcx = input.texCoord.x, tcy = input.texCoord.y;
	float tcxFnc = sin(tcx*tcy*0.7 * 4.2 + time*0.24);
	float tcyFnc = cos(tcy + tcy*tcy*tcx*0.5 * 5 + time*0.5);

    float plasFunc = 
		0.5 + 0.5*sin(tcxFnc * (4+sin(time*1.1))*2.5 + time*1.2) *
		0.5 * cos(tcyFnc * (3+sin(time*0.8))*5 + time*0.5);

    return float4(plasFunc*tcyFnc*0.4,plasFunc*tcxFnc*0.8,plasFunc, 1);
}

technique Plasma
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
