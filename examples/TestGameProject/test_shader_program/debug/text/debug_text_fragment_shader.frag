#version 440

layout(location = 3, binding = 0) uniform sampler2D textSheet;
layout(location = 5) uniform vec4 fragColor;

layout(location = 0) in vec2 textureUvCoord;

layout(location = 0) out vec4 color;

void main()
{
    vec4 textureColor = vec4(1.0, 1.0, 1.0, texture(textSheet, textureUvCoord).r);
    color = fragColor * textureColor;
}
