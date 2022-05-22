#version 400
uniform vec3 fragmentColor;
in vec4 fColor;

void main()
{
    gl_FragColor = vec4(fragmentColor, 1.0);// + vec4(fragmentColor, 1.0)) / 2;
}
