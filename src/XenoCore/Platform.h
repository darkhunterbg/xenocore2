#pragma once

#if defined _WIN32

#if defined(_XC_LIB)
#define EXPORT _declspec(dllexport) 
#else
#define EXPORT _declspec(dllimport) 
#endif


#endif