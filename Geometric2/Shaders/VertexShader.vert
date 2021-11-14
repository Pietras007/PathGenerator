#version 330

layout (location = 0) in vec3 a_Position;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 fragmentColor;

out vec4 color;

void main()
{
    //gl_Position = mvp * vec4(a_Position, 1.0);
    color = vec4(fragmentColor, 1.0);
    gl_Position = vec4(a_Position, 1.0) * model * view * projection;
    //gl_Position = projection * view * model * vec4(a_Position, 1.0);
}
