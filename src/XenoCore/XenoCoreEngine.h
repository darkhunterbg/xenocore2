#pragma once


#include "MemoryManager.h"

struct XenoCoreEngineConfig
{
	void* FixedMemoryAddress;
};

/// <summary>
/// Manages the game engine and it's systems
/// </summary>
class XenoCoreEngine
{
public:
	/// <summary>
	/// Initialize the game engine
	/// </summary>
	EXPORT static inline void Initialize(XenoCoreEngineConfig config);

	/// <summary>
	/// Get the engine's memory manager
	/// </summary>
	/// <returns>The engine's memory</returns>
	EXPORT static inline MemoryManager& GetMemoryManager()  { return *mm; }

private:
	static MemoryManager* mm;

	XenoCoreEngine() = delete;
	~XenoCoreEngine() = delete;	


};

