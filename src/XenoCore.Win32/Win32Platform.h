#pragma once

#include "Platform.h"

class Win32Platform
{
public:
	static void MemoryAlloc(int size);


private:
	Win32Platform() = delete;
	~Win32Platform() = delete;
};