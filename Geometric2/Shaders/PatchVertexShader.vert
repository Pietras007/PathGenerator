#version 330

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec3 a_pos2;
layout (location = 3) in vec3 a_pos3;
layout (location = 4) in vec3 a_pos4;

out VS_OUT {
    vec3 color;
    vec3 pos2;
    vec3 pos3;
    vec3 pos4;
} vs_out;

void main()
{
    vs_out.color = aColor;
    vs_out.pos2 = a_pos2;
    vs_out.pos3 = a_pos3;
    vs_out.pos4 = a_pos4;
    gl_Position = vec4(a_Position, 1.0);
}
