#version 440

#define PI 3.1415926535897932384626433832795
#define EPSILON 0.0000001
#define GAMMA 2.2

layout(location = 0, binding = 10) uniform sampler2D gColor;
layout(location = 1, binding = 11) uniform sampler2D gDepth;
layout(location = 2, binding = 12) uniform sampler2D gPosition;
layout(location = 3, binding = 13) uniform sampler2D gNormals;
layout(location = 4, binding = 14) uniform sampler2D gMetalRough;
layout(location = 5, binding = 15) uniform sampler2D gEmission;
layout(location = 6, binding = 16) uniform samplerCube environmentMap;

layout(location = 7) uniform mat4 view;
layout(location = 8) uniform mat4 projection;

layout(location = 9) uniform int pntLightAmount;
layout(location = 10) uniform int dirLightAmount;
layout(location = 11) uniform int sptLightAmount;
layout(location = 12) uniform vec4 ambientColor;

struct PointLightData
{
    vec4 position;
    vec4 color;
    float radius;
    float strength;
};

struct DirectionalLightData
{
    vec4 direction;
    vec4 color;
    float strength;
};

struct SpotLightData
{
    vec4 position;
    vec4 direction;
    vec4 color;
    float radius;
    float cutoffAngle;
    float strength;
};

struct GBufferData
{
    vec3 diffuse;
    vec3 position;
    float depth;
    vec3 normal;
    float metallicity;
    float roughness;
    vec3 emission;
};

layout(std140, binding = 0) uniform pointLights
{
    PointLightData pntLightDataArr[99];
};

layout(std140, binding = 1) uniform dirLights
{
    DirectionalLightData dirLightDataArr[99];
};

layout(std140, binding = 2) uniform sptLights
{
    SpotLightData sptLightDataArr[99];
};

layout(location = 0) in vec2 textureUvCoord;
layout(location = 1) in vec3 envMapCoord;

layout(location = 0) out vec4 color;

vec3 correctGamma(vec3 colorVector)
{
    return pow(colorVector.rgb, vec3(GAMMA));
}

float saturate(float f)
{
    return clamp(f, 0.0f, 1.0f);
}

float Fd90(float NoL, float roughness)
{
    return (2.0f * NoL * roughness) + 0.4f;
}

float KDisneyTerm(float NoL, float NoV, float roughness)
{
    return (1.0f + Fd90(NoL, roughness) * pow(1.0f - NoL, 5.0f)) * (1.0f + Fd90(NoV, roughness) * pow(1.0f - NoV, 5.0f));
}

vec3 computeFresnelSchlick(float NdotV, vec3 F0)
{
    return F0 + (1.0f - F0) * pow(1.0f - NdotV, 5.0f);
}

vec3 computeFresnelSchlickRoughness(float NdotV, vec3 F0, float roughness)
{
    return F0 + (max(vec3(1.0f - roughness), F0) - F0) * pow(1.0f - NdotV, 5.0f);
}

float computeDistributionGGX(vec3 N, vec3 H, float roughness)
{
    float alpha = roughness * roughness;
    float alpha2 = alpha * alpha;

    float NdotH = saturate(dot(N, H));
    float NdotH2 = NdotH * NdotH;

    return (alpha2) / (PI * (NdotH2 * (alpha2 - 1.0f) + 1.0f) * (NdotH2 * (alpha2 - 1.0f) + 1.0f));
}

float computeGeometryAttenuationGGXSmith(float NdotL, float NdotV, float roughness)
{
    float NdotL2 = NdotL * NdotL;
    float NdotV2 = NdotV * NdotV;
    float kRough2 = roughness * roughness + 0.0001f;

    float ggxL = (2.0f * NdotL) / (NdotL + sqrt(NdotL2 + kRough2 * (1.0f - NdotL2)));
    float ggxV = (2.0f * NdotV) / (NdotV + sqrt(NdotV2 + kRough2 * (1.0f - NdotV2)));

    return ggxL * ggxV;
}

// GBuffer
GBufferData unpackGBufferForFragment(vec2 uvCoordinate)
{
    vec3 diffuse = texture(gColor, uvCoordinate).rgb;
    vec3 position = texture(gPosition, uvCoordinate).rgb;
    float depth = texture(gDepth, uvCoordinate).r;
    vec3 normal = texture(gNormals, uvCoordinate).rgb;

    float metallicity = texture(gMetalRough, uvCoordinate).r;
    if (metallicity <= 0.0)
    {
        metallicity = EPSILON;
    }
    else if (metallicity >= 1.0)
    {
        metallicity = 1.0 - EPSILON;
    }

    float roughness = texture(gMetalRough, uvCoordinate).g;
    if (roughness <= 0.0)
    {
        roughness = EPSILON;
    }
    else if (roughness >= 1.0)
    {
        roughness = 1.0 - EPSILON;
    }

    vec3 emission = texture(gEmission, uvCoordinate).rgb;
    return GBufferData(diffuse, position, depth, normal, metallicity, roughness, emission);
}

void main()
{
    GBufferData fragmentData = unpackGBufferForFragment(textureUvCoord);
    vec3 outColor;

    if (fragmentData.depth == 1.0)
    {
        vec3 envColor = texture(environmentMap, envMapCoord).rgb;
        outColor = envColor * ambientColor.rgb;
    }
    else
    {
        vec3 V = normalize(-fragmentData.position);
        vec3 N = normalize(fragmentData.normal);
        vec3 R = reflect(-V, N);

        float NdotV = max(dot(N, V), 0.0001f);

        // Fresnel (Schlick) computation (F term)
        vec3 F0 = mix(vec3(0.03), fragmentData.diffuse, fragmentData.metallicity);
        vec3 F = computeFresnelSchlick(NdotV, F0);

        // Energy conservation
        vec3 kS = F;
        vec3 kD = vec3(1.0f) - kS;
        kD *= 1.0f - fragmentData.metallicity;

        // Diffuse component computation
        vec3 diffuse = fragmentData.diffuse / PI;

        // Ambient
        vec3 environmentReflectionColor =
            textureLod(environmentMap, R, fragmentData.roughness).rgb * ambientColor.rgb;
        vec3 environmentReflection =
            (environmentReflectionColor * fragmentData.metallicity * (PI / 2)) * fragmentData.diffuse;

        outColor = environmentReflection;

        // Point Lights
        for (int i = 0; i < pntLightAmount; i++)
        {
            // Light Surface Vectors
            vec3 L = normalize(pntLightDataArr[i].position.xyz - fragmentData.position.xyz);
            vec3 H = normalize(L + V);

            // Light Color
            vec3 lightColor = correctGamma(pntLightDataArr[i].color.rgb);

            // UE4 attenuation
            float distanceL = length(pntLightDataArr[i].position.xyz - fragmentData.position.xyz);
            float attenuation =
                pow(saturate(1 - pow(distanceL / pntLightDataArr[i].radius, 4)), 2) / (distanceL * distanceL + 1);

            // Light source dependent BRDF term(s)
            float NdotL = saturate(dot(N, L));

            // Radiance computation
            vec3 kRadiance = lightColor * normalize(environmentReflection) * attenuation;

            // Disney diffuse term
            float kDisney = KDisneyTerm(NdotL, NdotV, fragmentData.roughness);

            // Distribution (GGX) computation (D term)
            float D = computeDistributionGGX(N, H, fragmentData.roughness);

            // Geometry attenuation (GGX-Smith) computation (G term)
            float G = computeGeometryAttenuationGGXSmith(NdotL, NdotV, fragmentData.roughness);

            // Specular component computation
            vec3 specular = (F * D * G) / (4.0f * NdotL * NdotV + 0.0001f);

            outColor += ((diffuse * kD + specular) * kRadiance * NdotL) * pntLightDataArr[i].strength;
        }

        // Directional Lights
        for (int i = 0; i < dirLightAmount; i++)
        {
            // Light Surface Vectors
            vec3 L = normalize(-dirLightDataArr[i].direction.xyz);
            vec3 H = normalize(L + V);

            // Light Color
            vec3 lightColor = correctGamma(dirLightDataArr[i].color.rgb);

            // Light source dependent BRDF term(s)
            float NdotL = saturate(dot(N, L));

            // Radiance computation
            vec3 kRadiance = lightColor * normalize(environmentReflection);

            // Disney diffuse term
            float kDisney = KDisneyTerm(NdotL, NdotV, fragmentData.roughness);

            // Distribution (GGX) computation (D term)
            float D = computeDistributionGGX(N, H, fragmentData.roughness);

            // Geometry attenuation (GGX-Smith) computation (G term)
            float G = computeGeometryAttenuationGGXSmith(NdotL, NdotV, fragmentData.roughness);

            // Specular component computation
            vec3 specular = (F * D * G) / (4.0f * NdotL * NdotV + 0.0001f);

            outColor += ((diffuse * kD + specular) * kRadiance * NdotL) * dirLightDataArr[i].strength;
        }

        // Spot Lights
        for (int i = 0; i < sptLightAmount; i++)
        {
            // Light Surface Vectors
            vec3 L = normalize(sptLightDataArr[i].position.xyz - fragmentData.position.xyz);
            vec3 H = normalize(L + V);

            // Light Color
            vec3 lightColor = correctGamma(sptLightDataArr[i].color.rgb);

            // UE4 attenuation
            float distanceL = length(sptLightDataArr[i].position.xyz - fragmentData.position.xyz);
            float attenuation =
                pow(saturate(1 - pow(distanceL / sptLightDataArr[i].radius, 4)), 2) / (distanceL * distanceL + 1);

            // Spot Light Math
            float spotAttenuation = 0.0;

            float spotFactor = dot(-L, sptLightDataArr[i].direction.xyz);
            float cutoffAngleCos = cos(radians(sptLightDataArr[i].cutoffAngle));
            if (spotFactor < cutoffAngleCos)
            {
                spotAttenuation = 0.0;
            }
            else
            {
                spotAttenuation = (1.0 - (1.0 - spotFactor) * 1.0 / (1.0 - cutoffAngleCos));
            }
            attenuation *= spotAttenuation;

            // Light source dependent BRDF term(s)
            float NdotL = saturate(dot(N, L));

            // Radiance computation
            vec3 kRadiance = lightColor * normalize(environmentReflection) * attenuation;

            // Disney diffuse term
            float kDisney = KDisneyTerm(NdotL, NdotV, fragmentData.roughness);

            // Distribution (GGX) computation (D term)
            float D = computeDistributionGGX(N, H, fragmentData.roughness);

            // Geometry attenuation (GGX-Smith) computation (G term)
            float G = computeGeometryAttenuationGGXSmith(NdotL, NdotV, fragmentData.roughness);

            // Specular component computation
            vec3 specular = (F * D * G) / (4.0f * NdotL * NdotV + 0.0001f);

            outColor += ((diffuse * kD + specular) * kRadiance * NdotL) * sptLightDataArr[i].strength;
        }

        outColor += fragmentData.emission * outColor;
    }

    color = vec4(outColor, 1.0);
}
