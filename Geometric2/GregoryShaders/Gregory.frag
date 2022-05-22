#version 400
uniform vec3 fragmentColor;

void main()
{
    gl_FragColor = vec4(fragmentColor, 1.0);
}
