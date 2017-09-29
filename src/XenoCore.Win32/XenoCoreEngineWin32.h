#pragma once
#include "XenoCoreEngine.h"
#include "Win32Platform.h"

/// <summary>
/// Manages the game engine and it's systems
/// </summary>
class XenoCoreEngineWin32 : public XenoCoreEngine<Win32Platform>
{
public:
	/// <summary>
	/// Initialize the game engine
	/// </summary>
	EXPORT static inline void Initialize()
	{
		XenoCoreEngine<Win32Platform>::Initialize();
	}
private:
	XenoCoreEngineWin32() = delete;
	~XenoCoreEngineWin32() = delete;
};

