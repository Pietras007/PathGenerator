#version 400
uniform float SegmentsU, SegmentsV;
layout( vertices = 16 ) out;

in vec2 texCoord[];
out vec2 tes_texCoord[];

void main( )
{
	gl_out[ gl_InvocationID ].gl_Position = gl_in[ gl_InvocationID ].gl_Position;
	tes_texCoord[ gl_InvocationID ] = texCoord[ gl_InvocationID ];
	gl_TessLevelOuter[0] = SegmentsU;
	gl_TessLevelOuter[1] = SegmentsV;
	gl_TessLevelOuter[2] = SegmentsU;
	gl_TessLevelOuter[3] = SegmentsV;
	gl_TessLevelInner[0] = SegmentsV;
	gl_TessLevelInner[1] = SegmentsU;
}