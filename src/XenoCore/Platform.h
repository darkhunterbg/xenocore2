#pragma once

struct MemoryAllocResult
{
	bool success;
	size_t size;
	void* ptr;
};
struct MemoryFreeResult
{
	bool success;
};

//auto operator"" KB(size_t const x) { return 1024L * x; }
//auto operator"" MB(size_t const x) { return 1024L * 1024L * x; }
//auto operator"" GB(size_t const x) { return 1024L * 1024L * 1024L * x; }



#if defined _WIN32

#if defined(_XC_LIB)
#define EXPORT _declspec(dllexport) 
#else
#define EXPORT _declspec(dllimport) 
#endif


#include "../XenoCore.Win32/Win32Platform.h"


#define ASSERT(expr) if(!(expr)) throw "Assertion failed!";

#endif


