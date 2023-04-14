#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

out VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} vs_out;

uniform mat4 model;
uniform mat4 projection;
uniform mat4 view;

void main()
{
    vs_out.FragPos = aPosition;
    vs_out.Normal = aNormal;
    vs_out.TexCoords = aTexCoord;
    gl_Position = model * projection * view * vec4(aPosition, 1.0);
}