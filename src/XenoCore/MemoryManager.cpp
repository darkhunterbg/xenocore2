#include "MemoryManager.h"


MemoryManager::MemoryManager(void* fixedAddress) :
	fixedAddress(fixedAddress)
{

}

void* MemoryManager::AllocateBlock(size_t size)
{
	MemoryAllocResult result;

	size_t baseAddress = 0;

	if (fixedAddress)
	{
		baseAddress = (size_t)fixedAddress;
		if (endBlock)
			baseAddress = (size_t)endBlock->address + endBlock->size;
	}

	result = Platform::MemoryAlloc((void*)baseAddress, size);

	ASSERT(result.success);

	MemoryBlock* block = new MemoryBlock();

	block->address = result.ptr;
	block->size = result.size;

	if (!startBlock)
		startBlock = block;
	else
		endBlock->nextBlock = block;

	endBlock = block;

	return block->address;
}


void MemoryManager::FreeBlock(void* blockPtr)
{
}