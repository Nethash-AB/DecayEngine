#version 440

// Matrices
layout(location = 0) uniform mat4x4 projection;
layout(location = 1) uniform mat4x4 view;
layout(location = 2) uniform mat4x4 model;

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
layout(location = 0) in vec3 position;
layout(location = 1) in vec2 textureUv;
layout(location = 2) in vec3 normal;
layout(location = 3) in vec3 tangent;
layout(location = 4) in vec3 bitangent;

// Out Attributes
layout(location = 0) out vec3 fragPosition;
layout(location = 1) out vec2 fragTextureUv;
layout(location = 2) out vec3 fragNormal;
layout(location = 3) out vec3 fragTangent;
layout(location = 4) out vec3 fragBitangent;

void main()
{
    vec4 worldViewPosition = (view * model) * vec4(position, 1.0);
    mat3 normalMatrix = transpose(inverse(mat3(view * model)));

    fragPosition = worldViewPosition.xyz;
    fragTextureUv = textureUv;
    fragNormal = normalMatrix * normal;
    fragTangent = tangent;
    fragBitangent = bitangent;

    gl_Position = projection * worldViewPosition;
}
