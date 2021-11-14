#ifndef BACKFACEOUTLINES_INCLUDED
#define BACKFACEOUTLINES_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes {
	float4 positionOS : POSITION;
	float3 normalOS : NORMAL;
};

struct VertexOutput {
	float4 positionCS : SV_POSITION;
};

float _Thickness;
float4 _Color;

VertexOutput Vertex(Attributes input) {
	VertexOutput output = (VertexOutput)0;
	float3 normalOS = input.normalOS;

	float3 positionOS = input.positionOS.xyz + normalOS * _Thickness;
	output.positionCS = GetVertexPositionInputs(positionOS).positionCS;

	return output;
}

float4 Fragment(VertexOutput input) : SV_Target {
    return _Color;
}

#endif