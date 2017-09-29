#pragma once
#include "Platform.h"


struct MemoryBlock
{
	MemoryBlock* nextBlock;
	void* address;
	size_t size;
};

/// <summary>
/// Manages allocation/deallocation of memory
/// </summary>
class MemoryManager
{

public:
	/// <summary>
	/// Create a new memory manager
	/// </summary>
	/// <param name="fixedAddress">Address from which memory shoud start. Pass null to let the OS decide</param>
	MemoryManager(void* fixedAddress);
	~MemoryManager() {}

	/// <summary>
	/// Allocate new memory block
	/// </summary>
	/// <param name="size">Size of the memory block</param>
	/// <returns>Pointer to the allocated block</returns>
	EXPORT void* AllocateBlock(size_t size);

	/// <summary>
	/// Free a memory block
	/// </summary>
	/// <param name="blockPtr">Pointer to the memory block</param>
	EXPORT void FreeBlock(void* blockPtr);
private:
	MemoryBlock* startBlock = nullptr;
	MemoryBlock* endBlock = nullptr;
	void* fixedAddress = nullptr;

	MemoryManager(const MemoryManager&) = delete;
	MemoryManager& operator=(const MemoryManager&) = delete;
};

