#version 430 core
out vec4 FragColor;

in VS_OUT {
//    vec3 FragPos;
//    vec3 Normal;
//    vec2 TexCoords;
    vec2 aTexCoord;
    vec4 l;
    vec4 n;
    vec4 v;
} fs_in;

layout (binding = 0) uniform sampler2D texture0;

void main()
{
    vec4 color = texture(texture0, fs_in.aTexCoord);
    
    vec4 ml = normalize(fs_in.l);
    vec4 mn = normalize(fs_in.n);
    vec4 mv = normalize(fs_in.v);
    
    vec4 r = reflect(-ml, mn);
    float nl = clamp(dot(mn, ml), 0, 1);
    float rv = pow(clamp(dot(r, mv), 0, 1), 47);
    
    vec4 ks = vec4(1, 1, 1, 1);
    
//    FragColor = color * nl + rv;
    FragColor = vec4(color.rgb * nl, color.a) + vec4(ks.rgb*rv, 0);
}