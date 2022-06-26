#version 400
layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec2 aTexCoords;

out vec2 texCoord;
void main()
{
    texCoord = aTexCoords;
    gl_Position = vec4(a_Position, 1.0);
}