#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

out vec2 texCoord;
out vec3 normal;

uniform mat4 view;
uniform mat4 projection;
uniform mat4 model;

void main()
{
    vec4 worldPosition = vec4(aPosition, 1.0) * model;

    texCoord = aTexCoord * 400;
    normal = aNormal;
    gl_Position = worldPosition * view * projection;
}   