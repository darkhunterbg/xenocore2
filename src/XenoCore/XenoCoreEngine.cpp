#include "XenoCoreEngine.h"


MemoryManager* XenoCoreEngine::mm;

void XenoCoreEngine::Initialize(XenoCoreEngineConfig config)
{
	Platform::Init();
	mm = new MemoryManager(config.FixedMemoryAddress);
}