#version 440

// Textures
layout(location = 3, binding = 0) uniform sampler2D albedoTexture;
layout(location = 4, binding = 1) uniform sampler2D normalTexture;
layout(location = 5, binding = 2) uniform sampler2D metalTexture;
layout(location = 6, binding = 3) uniform sampler2D roughTexture;
layout(location = 7, binding = 4) uniform sampler2D emissionTexture;

// Textures Toggle Flags
layout(location = 8) uniform int useAlbedoMap;
layout(location = 9) uniform int useMetalMap;
layout(location = 10) uniform int useRoughMap;
layout(location = 11) uniform int useEmissionMap;

// Pbr
layout(location = 12) uniform vec4 albedoColor;
layout(location = 13) uniform float metalFactor;
layout(location = 14) uniform float roughFactor;
layout(location = 15) uniform vec4 emissionColor;

// In Attributes
layout(location = 0) in vec3 fragPosition;
layout(location = 1) in vec2 fragTextureUv;
layout(location = 2) in vec3 fragNormal;
layout(location = 3) in vec3 fragTangent;
layout(location = 4) in vec3 fragBitangent;

// Out Attributes
layout(location = 0) out vec4 gColor;
layout(location = 1) out vec4 gPosition;
layout(location = 2) out vec4 gNormal;
layout(location = 3) out vec4 gMetalRough;
layout(location = 4) out vec4 gEmission;

vec3 computeTexNormal(vec3 viewNormal, vec3 texNormal)
{
    vec3 dPosX = dFdx(fragPosition);
    vec3 dPosY = dFdy(fragPosition);
    vec2 dTexX = dFdx(fragTextureUv);
    vec2 dTexY = dFdy(fragTextureUv);

    vec3 normal = normalize(viewNormal);
    vec3 tangent = normalize(dPosX * dTexY.t - dPosY * dTexX.t);
    vec3 binormal = -normalize(cross(normal, tangent));
    mat3 TBN = mat3(tangent, binormal, normal);

    return normalize(TBN * texNormal);
}

void main()
{
    // Color
    gColor = albedoColor;
    if (useAlbedoMap == 1)
    {
        gColor *= texture(albedoTexture, fragTextureUv);
    }

    // Position
    gPosition = vec4(fragPosition, 1.0);

    // Normal
    vec3 texNormal = normalize(texture(normalTexture, fragTextureUv).rgb * 2.0 - 1.0);
    gNormal = vec4(computeTexNormal(fragNormal, texNormal), 1.0);

    // Metallicity
    float metallicity;
    if (useMetalMap == 1)
    {
        metallicity = texture(metalTexture, fragTextureUv).r * metalFactor;
    }
    else
    {
        metallicity = metalFactor;
    }

    // Roughness
    float roughness;
    if (useRoughMap == 1)
    {
        roughness = texture(roughTexture, fragTextureUv).r * roughFactor;
    }
    else
    {
        roughness = roughFactor;
    }

    gMetalRough = vec4(metallicity, roughness, 1.0, 1.0);

    // Emission
    gEmission = emissionColor;
    if (useEmissionMap == 1)
    {
        gEmission *= texture(emissionTexture, fragTextureUv);
    }
}
