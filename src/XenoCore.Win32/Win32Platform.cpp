#include "Win32Platform.h"
#include <Windows.h>


size_t pageSize;

void Win32Platform::Init()
{
	SYSTEM_INFO sysInfo;
	GetSystemInfo(&sysInfo);

	pageSize = sysInfo.dwPageSize;
}


MemoryAllocResult Win32Platform::MemoryAlloc(void* location, size_t size)
{
	MemoryAllocResult result = { 0 };
	result.size = size / pageSize;
	if (size % pageSize > 0)
		result.size += pageSize;

	result.size *= pageSize;

	result.ptr = VirtualAlloc(location, result.size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
	result.success = result.ptr != nullptr;
	return result;
}


MemoryFreeResult Win32Platform::MemoryFree(void* memoryLocation, size_t size)
{
	MemoryFreeResult result = { 0 };

	result.success = VirtualFree(memoryLocation, size, MEM_RELEASE) != 0;
	return result;
}
