// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: ICommandTarget.cs  Created: 2014-06-14@15:04
// Last modified: 2014-12-13@22:32 by Tim

using TA.NetMF.NetworkCommandProtocol.CommandProcessors;

namespace TA.NetMF.NetworkCommandProtocol.CommandTargets
    {
    internal interface ICommandTarget
        {
        ICommandProcessor[] GetCommandProcessors();
        }
    }
