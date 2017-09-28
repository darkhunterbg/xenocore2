#pragma once

#if defined(_LIB)
#define EXPORT _declspec(dllexport) 
#else
#define EXPORT _declspec(dllimport) 
#endif



 class Test
{
public:
	EXPORT Test();
	EXPORT ~Test();

	void Modified();

	///Returns id
	EXPORT int GetID() { return 0; };
};

