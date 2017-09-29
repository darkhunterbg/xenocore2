#include "Win32Platform.h"
#include <Windows.h>

void Win32Platform::MemoryAlloc(int size)
{
	VirtualAlloc(0, size, MEM_COMMIT, PAGE_READWRITE);
}
