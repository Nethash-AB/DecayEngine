#version 440

layout(location = 0, binding = 10) uniform sampler2D screenTexture;
layout(location = 1) uniform float[9] kernel;
layout(location = 10) uniform float radius;

layout(location = 0) in vec2 textureUvCoord;

layout(location = 0) out vec4 color;

void main()
{
    vec2 offsets[9] = vec2[](
    vec2(-radius,  radius), // top-left
    vec2( 0.0f,    radius), // top-center
    vec2( radius,  radius), // top-right
    vec2(-radius,  0.0f),   // center-left
    vec2( 0.0f,    0.0f),   // center-center
    vec2( radius,  0.0f),   // center-right
    vec2(-radius, -radius), // bottom-left
    vec2( 0.0f,   -radius), // bottom-center
    vec2( radius, -radius)  // bottom-right
    );

    vec4 sampleTex[9];
    for(int i = 0; i < 9; i++)
    {
        sampleTex[i] = vec4(texture(screenTexture, textureUvCoord.st + offsets[i]));
    }
    vec4 col = vec4(0.0);
    for(int i = 0; i < 9; i++)
    col += sampleTex[i] * kernel[i];

    color = col;
}
