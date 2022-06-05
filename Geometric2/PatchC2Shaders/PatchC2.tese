#version 400 core

layout(quads, equal_spacing, cw) in;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

vec3 DeBoor(float t, vec3 B0_, vec3 B1_, vec3 B2_, vec3 B3_) 
{
	float T0 = -1.0f;
	float T1 = 0.0f;
    float T2 = 1.0f;
    float T3 = 2.0f;
    float T4 = 3.0f;
	float Tm1 = -2.0f;

	float A1 = T2 - t;
	float A2 = T3 - t;
	float A3 = T4 - t;
	float B1 = t - T1;
	float B2 = t - T0;
	float B3 = t - Tm1;

	float N1 = 1;
	float N2 = 0;
	float N3 = 0;
	float N4 = 0;

	float saved = 0.0f;
	float term = 0.0f;

	term = N1/(A1+B1);
	N1 = saved + A1*term;
	saved = B1 * term;

	N2 = saved;
	saved = 0.0f;

	term = N1/(A1+B2);
	N1 = saved + A1*term;
	saved = B2 * term;

	term = N2/(A2+B1);
	N2 = saved + A2*term;
	saved = B1 * term;

	N3 = saved;
	saved = 0.0f;

	term = N1/(A1+B3);
	N1 = saved + A1*term;
	saved = B3 * term;

	term = N2/(A2+B2);
	N2 = saved + A2*term;
	saved = B2 * term;

	term = N3/(A3+B1);
	N3 = saved + A3*term;
	saved = B1 * term;

	N4 = saved;

	return N1*B0_+N2*B1_+N3*B2_+N4*B3_;
}

void main( )
{
	float u = gl_TessCoord.x;
	float v = gl_TessCoord.y;

	vec3 p00 = vec3(gl_in[0].gl_Position);
	vec3 p10 = vec3(gl_in[1].gl_Position);
	vec3 p20 = vec3(gl_in[2].gl_Position);
	vec3 p30 = vec3(gl_in[3].gl_Position);
	vec3 p01 = vec3(gl_in[4].gl_Position);
	vec3 p11 = vec3(gl_in[5].gl_Position);
	vec3 p21 = vec3(gl_in[6].gl_Position);
	vec3 p31 = vec3(gl_in[7].gl_Position);
	vec3 p02 = vec3(gl_in[8].gl_Position);
	vec3 p12 = vec3(gl_in[9].gl_Position);
	vec3 p22 = vec3(gl_in[10].gl_Position);
	vec3 p32 = vec3(gl_in[11].gl_Position);
	vec3 p03 = vec3(gl_in[12].gl_Position);
	vec3 p13 = vec3(gl_in[13].gl_Position);
	vec3 p23 = vec3(gl_in[14].gl_Position);
	vec3 p33 = vec3(gl_in[15].gl_Position);

	vec3 deboor1 = DeBoor(v, p00, p10, p20, p30);
	vec3 deboor2 = DeBoor(v, p01, p11, p21, p31);
	vec3 deboor3 = DeBoor(v, p02, p12, p22, p32);
	vec3 deboor4 = DeBoor(v, p03, p13, p23, p33);


	vec4 position = vec4(DeBoor(u, deboor1, deboor2, deboor3, deboor4), 1.0f);
	gl_Position = position * model * view * projection;
}