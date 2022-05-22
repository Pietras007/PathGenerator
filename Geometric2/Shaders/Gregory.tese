#version 400 core

layout(quads, equal_spacing, cw) in;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
out vec4 fColor;

vec4 DeCasteljau(vec4 coeffs_t[4], float t, int vertices)
{
	for (int i = vertices; i > 0; --i)
	{
		for (int j = 0; j < i - 1; ++j)
		{
			coeffs_t[j] = (1 - t) * coeffs_t[j] + t * coeffs_t[j + 1];
		}
	}
	return coeffs_t[0];
}

void main( )
{
	fColor = vec4(0.0f,0.0f,0.0f,0.5f);
	float u = gl_TessCoord.x;
	float v = gl_TessCoord.y;

	vec4 p00 = gl_in[0].gl_Position;
	vec4 p10 = gl_in[1].gl_Position;
	vec4 p20 = gl_in[2].gl_Position;
	vec4 p30 = gl_in[3].gl_Position;
	vec4 p01 = gl_in[4].gl_Position;
	vec4 p11 = (u!=0 || v!=0) ? (u*gl_in[5].gl_Position + v*gl_in[6].gl_Position)/(u+v) : (gl_in[5].gl_Position + gl_in[6].gl_Position)/2;
	vec4 p21 = (u!=0 || v!=1) ? ((1-v)*gl_in[7].gl_Position+u*gl_in[8].gl_Position)/(1-v+u) : (gl_in[7].gl_Position + gl_in[8].gl_Position)/2;
	vec4 p31 = gl_in[9].gl_Position;
	vec4 p02 = gl_in[10].gl_Position;
	vec4 p12 = (u!=1 || v!=0) ? ((1-u)*gl_in[11].gl_Position+v*gl_in[12].gl_Position)/(1-u+v) : (gl_in[11].gl_Position + gl_in[12].gl_Position)/2;
	vec4 p22 = (u!=1 || v!=1) ? ((1-u)*gl_in[14].gl_Position+(1-v)*gl_in[13].gl_Position)/(2-u-v) : (gl_in[13].gl_Position + gl_in[14].gl_Position)/2;
	vec4 p32 = gl_in[15].gl_Position;
	vec4 p03 = gl_in[16].gl_Position;
	vec4 p13 = gl_in[17].gl_Position;
	vec4 p23 = gl_in[18].gl_Position;
	vec4 p33 = gl_in[19].gl_Position;



	vec4 coeffs[4];
    vec4 coeffs2[4];

	coeffs[0]= p00;
	coeffs[1]= p10;
	coeffs[2]= p20;
	coeffs[3]= p30;
	coeffs2[0] = DeCasteljau(coeffs,v,4);

	coeffs[0]= p01;
	coeffs[1]= p11;
	coeffs[2]= p21;
	coeffs[3]= p31;
	coeffs2[1] = DeCasteljau(coeffs,v,4);

	coeffs[0]= p02;
	coeffs[1]= p12;
	coeffs[2]= p22;
	coeffs[3]= p32;
	coeffs2[2] = DeCasteljau(coeffs,v,4);

	coeffs[0]= p03;
	coeffs[1]= p13;
	coeffs[2]= p23;
	coeffs[3]= p33;
	coeffs2[3] = DeCasteljau(coeffs,v,4);



	vec4 position = vec4(DeCasteljau(coeffs2,u,4).xyz,1.0f);
	gl_Position = position * model * view * projection;

}