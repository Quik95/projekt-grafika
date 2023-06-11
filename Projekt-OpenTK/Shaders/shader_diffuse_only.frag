#version 430 core

layout (binding = 0) uniform sampler2D texture0;

in vec2 texCoord;
in vec3 normal;

out vec4 color;

void main(void) {
    color = texture(texture0, texCoord);
}