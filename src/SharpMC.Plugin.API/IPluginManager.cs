﻿using SharpMC.API.Entities;
using SharpMC.API.Worlds;

namespace SharpMC.Plugin.API
{
    public interface IPluginManager
    {
        void HandleCommand(string input, IPlayer player);

        void LoadPlugins();

        void EnablePlugins(ILevelManager manager);

        void DisablePlugins();
    }
}