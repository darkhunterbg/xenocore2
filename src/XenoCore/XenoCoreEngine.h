#pragma once

#include "Platform.h"

/// <summary>
/// Manages the game engine and it's systems
/// </summary>
template <class TPlatform>
class XenoCoreEngine
{
public:
	/// <summary>
	/// Initialize the game engine
	/// </summary>
	EXPORT static inline void Initialize()
	{
		TPlatform::MemoryAlloc(4 * 1024);
	}
private:
	XenoCoreEngine() = delete;
	~XenoCoreEngine() = delete;


};
