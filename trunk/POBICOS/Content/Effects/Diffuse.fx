// Matrices
float4x4 World;
float4x4 View;
float4x4 Projection;

// Ambient Variables
float4 AmbientColor;
float AmbientIntensity;

// Diffuse Variables
float4 DiffuseColor;
float DiffuseIntensity;
float3 DiffuseLightDirection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Normal : TEXCOORD0;
}; 

VertexShaderOutput VertexShader( VertexShaderInput input )
{
    VertexShaderOutput output;
	
	// Transform our position by the matricies
    float4 worldPosition = mul( input.Position, World );
    float4 viewPosition = mul( worldPosition, View );
    output.Position = mul( viewPosition, Projection );
    
	// Transform the normal from model space to world space by multiplying
	// our input normal by the world matrix. Then normalize it to make it a unit vector
	output.Normal = mul( input.Normal, World );
	
    return output;
}

float4 PixelShader( VertexShaderOutput input ) : COLOR0
{
    // Normalize our light direction
    float3 normLightDirection = normalize( DiffuseLightDirection );
    
	// Determine the diffuse component by finding the angle between the light and the normal
	// The smaller the angle between the normal and the light direction, the more light is hitting it,
	// and the brighter the resultant color
	float4 diffuse = dot( normLightDirection, input.Normal ) * DiffuseIntensity * DiffuseColor;
	
	// Calculate our ambient componenent
	float4 ambient = AmbientIntensity * AmbientColor;
	
	// Return the total light component as a combination of the two, saturating
	// the value to keep it within 0 and 1
    return saturate( diffuse + ambient );
}

technique Diffuse
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 VertexShader();
        PixelShader = compile ps_2_0 PixelShader();
    }
}
