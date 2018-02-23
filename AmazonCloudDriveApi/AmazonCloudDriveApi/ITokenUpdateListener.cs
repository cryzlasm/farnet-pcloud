// <copyright file="ITokenUpdateListener.cs" company="Rambalac">
// Copyright (c) Rambalac. All rights reserved.
// </copyright>

using System;

namespace Azi.Amazon.CloudDrive
{
    /// <summary>
    /// Listener for Authentication Token updates
    /// </summary>
    public interface ITokenUpdateListener
    {
        /// <summary>
        /// Called when Authentication Token updated
        /// </summary>
        /// <param name="access_token">Authentication token</param>
        void OnTokenUpdated(string access_token);
    }
}
