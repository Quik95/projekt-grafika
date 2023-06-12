#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

out VS_OUT {
//    vec3 FragPos;
//    vec3 Normal;
//    vec2 TexCoords;
    vec2 aTexCoord;
    vec4 l;
    vec4 n;
    vec4 v;
} vs_out;

uniform mat4 model;
uniform mat4 projection;
uniform mat4 view;

void main()
{
    vec4 lp = vec4(0, 1000, 0, -1);
    vs_out.l = view * lp - view * model * vec4(aPosition, 1.0);
    vs_out.l = normalize(vs_out.l);
    
    vs_out.n = normalize(view * model * vec4(aNormal, 0.0));
    
    vs_out.v = vec4(0, 0, 0, 1) - view * model * vec4(aPosition, 1.0);
    vs_out.v = normalize(vs_out.v);
    
    vs_out.aTexCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}