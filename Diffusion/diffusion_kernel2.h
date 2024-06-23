#pragma once

float  diffusion2d
// ====================================================================
//
// purpos     :  2-dimensional diffusion equation solved by FDM
//
// date       :  May 16, 2008
// programmer :  Takayuki Aoki
// place      :  Tokyo Institute of Technology
//
(
	int      numGPUs,    /* available number of GPU device            */
	int      nx,         /* x-dimensional grid size                   */
	int      ny,         /* y-dimensional grid size                   */
	float* f,         /* dependent variable                        */
	float* fn,        /* updated dependent variable                */
	float    kappa,      /* diffusion coefficient                     */
	float    dt = 1,         /* time step interval                        */
	float    dx = 1,         /* grid spacing in the x-direction           */
	float    dy = 1          /* grid spacing in the y-direction           */
);
