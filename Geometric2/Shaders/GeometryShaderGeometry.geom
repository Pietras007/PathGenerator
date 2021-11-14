#version 330 compatibility
layout (lines_adjacency) in;
layout (line_strip, max_vertices = 203) out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform int height;
uniform int width;

int Newton(int n, int k);
float DeKastilio(float vert[4], float t, int degree);

void main()
{	
    int pointx[4];
    int pointy[4];
    int bezierDegree = 4;
    if(isinf(gl_in[3].gl_Position.x))
    {
        bezierDegree = 3;
    }

    if(isinf(gl_in[2].gl_Position.x))
    {
        bezierDegree = 2;
    }

    if(isinf(gl_in[1].gl_Position.x))
    {
        EndPrimitive();
        return;
    }

    for(int i=0;i<bezierDegree;i++)
    {
        vec4 pos = normalize(gl_in[i].gl_Position * model * view * projection);
        pointx[i] = int((pos.x + 1) * width / 2);
        pointy[i] = int((pos.y + 1) * height / 2);
    }

    int dist = 0;
    for (int z = 0; z < bezierDegree - 1; z++)
    {
        dist += int(ceil(sqrt(pow(pointx[z+1] - pointx[z], 2) + pow(pointy[z+1] - pointy[z], 2))));
    }

    float X[4] = float[4](gl_in[0].gl_Position.x, gl_in[1].gl_Position.x, gl_in[2].gl_Position.x, gl_in[3].gl_Position.x);
    float Y[4] = float[4](gl_in[0].gl_Position.y, gl_in[1].gl_Position.y, gl_in[2].gl_Position.y, gl_in[3].gl_Position.y);
    float Z[4] = float[4](gl_in[0].gl_Position.z, gl_in[1].gl_Position.z, gl_in[2].gl_Position.z, gl_in[3].gl_Position.z);
    if(dist > 200)
    {
        dist = 200;
    }

    gl_Position =  gl_in[0].gl_Position * model * view * projection;
    EmitVertex();

    for (float t = 0.01f; t <= 1.0f; t += 1.0f/dist)
    {
        float bezierP_X = 0.0f;
        float bezierP_Y = 0.0f;
        float bezierP_Z = 0.0f;
        
        bezierP_X = DeKastilio(X, t, bezierDegree);
        bezierP_Y = DeKastilio(Y, t, bezierDegree);
        bezierP_Z = DeKastilio(Z, t, bezierDegree);
//        for (int i = 0; i < bezierDegree; i++)
//        {
//            vec4 pointPos = gl_in[i].gl_Position;
//            bezierP_X += Newton(bezierDegree - 1, i) * pointPos.r * pow(1 - t, bezierDegree - i - 1) * pow(t, i);
//            bezierP_Y += Newton(bezierDegree - 1, i) * pointPos.g * pow(1 - t, bezierDegree - i - 1) * pow(t, i);
//            bezierP_Z += Newton(bezierDegree - 1, i) * pointPos.b * pow(1 - t, bezierDegree - i - 1) * pow(t, i);
//        }

        vec4 bezierP = vec4(bezierP_X, bezierP_Y, bezierP_Z, 1.0);
        gl_Position = bezierP * model * view * projection;
        EmitVertex();
    }

    EndPrimitive();
}  

int Newton(int n, int k)
{
    int Result = 1;
    for (int i = 1; i <= k; i++)
    {
        Result = Result * (n - i + 1) / i;
    }

    return Result;
}

float DeKastilio(float vert[4], float t, int degree)
{
    for (int i = 0; i < degree; i++)
	{
		for (int j = 0; j < degree - i - 1; j++)
		{
			vert[j] = (1 - t) * vert[j] + t * vert[j + 1];
		}
	}

	return vert[0];
}