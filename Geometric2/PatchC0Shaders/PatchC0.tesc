#version 400
uniform float splitA, splitB;
layout( vertices = 20 ) out;

void main( )
{
	gl_out[ gl_InvocationID ].gl_Position = gl_in[ gl_InvocationID ].gl_Position;
	gl_TessLevelOuter[0] = splitA;
	gl_TessLevelOuter[1] = splitB;
	gl_TessLevelOuter[2] = splitA;
	gl_TessLevelOuter[3] = splitB;
	gl_TessLevelInner[0] = splitB;
	gl_TessLevelInner[1] = splitA;
}