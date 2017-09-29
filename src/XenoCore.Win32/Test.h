#pragma once

#if defined(_LIB)
#define EXPORT _declspec(dllexport) 
#else
#define EXPORT _declspec(dllimport) 
#endif


///Generate test function
int TestFunction() { return 0; }


 class Test
{
public:
	EXPORT Test();
	EXPORT ~Test();

	void Modified();

	///Returns id
	EXPORT int GetID() { return 0; };
};

