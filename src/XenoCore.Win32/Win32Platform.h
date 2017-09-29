#pragma once

#include "Platform.h"

class Win32Platform
{
public:
	static MemoryAllocResult MemoryAlloc(void* location, size_t size);
	static MemoryFreeResult MemoryFree(void* memoryLocation, size_t size);

	static void Init();
private:
	Win32Platform() = delete;
	~Win32Platform() = delete;

} typedef Platform;

