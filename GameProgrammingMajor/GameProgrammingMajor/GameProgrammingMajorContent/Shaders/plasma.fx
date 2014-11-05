float4x4 World;
float4x4 View;
float4x4 Projection;
float time;

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 texCoord : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 posTex : TEXCOORD2;

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
	output.posTex = output.Position.xyz;

    output.texCoord = input.texCoord;

	output.Normal = normalize( input.Normal );

    return output;
}

float4 plasmaFunc(VertexShaderOutput input) : COLOR0
{
	float tcx = input.texCoord.x, tcy = input.texCoord.y;
	float tcxFnc = sin(tcx*tcy*3.3 + time);
	float tcyFnc = cos(tcy + tcy*tcy*tcx*2.5 + time);

    float plasFunc = 
		0.75 + 0.25*sin(tcxFnc * (4+sin(time*1.1))*2.5 + time*1.2);

    float3 outCol = float3(plasFunc*tcyFnc*0.4,plasFunc*tcxFnc*0.8,plasFunc);

	return float4(outCol, 1);
}

float4 specularFunc(VertexShaderOutput input) : COLOR0
{
	// Viewspace position
	float3 P = input.posTex;

	// Light Vector
	float3 L = normalize(float3(1,-1,0.5) - P);

	// Viewing vector
	float3 V = normalize(-P);

	// Reflection vector
	float3 R = reflect(-L, input.Normal);

	// Specularity
	float3 specular = max(0, pow(dot(R,V), 45.0) );

	return float4(specular, 1);
}

technique Plasma
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 plasmaFunc();
    }
	pass Pass2
	{
		PixelShader = compile ps_2_0 specularFunc();
	}
}
