﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
#if NET_4_6
using System.Threading.Tasks;
#endif

using UnityEngine;

namespace UnityFx.Async.Samples
{
	partial class LoadTextureHelper
	{
#if NET_4_6

		/// <summary>
		/// Waits for the <see cref="LoadTextureAsync(string)"/> in with <c>await</c>.
		/// </summary>
		public async Task WaitForLoadOperationWithAwait(string textureUrl)
		{
			try
			{
				var texture = await LoadTextureAsync(textureUrl);
				Debug.Log("Yay! The texture is loaded!");
			}
			catch (OperationCanceledException)
			{
				Debug.LogWarning("The operation was canceled.");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

#endif
	}
}