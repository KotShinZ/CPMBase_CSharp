#include <stdio.h>
#include <stdlib.h>

#include "diffusion_kernel2.h"

#define DllExport   __declspec( dllexport )

extern "C"
{
    DllExport void Test()
    {
        printf("Hello CUDA!");
    }

    DllExport void Diffusion(float f[], int x, int y, int z, int iterate, float d)
    {
        float* fn = (float*)malloc(sizeof(float) * x * y * z );

        for (int i = 0; i < iterate; i++)
        {
            diffusion2d(1, x, y, f, fn, d);
            float* temp = f;
            f = fn;
            fn = temp;
        }

        free(fn);
    }
}