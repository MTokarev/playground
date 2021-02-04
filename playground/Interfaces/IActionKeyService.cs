﻿using playground.Models;
using System;
using System.Threading.Tasks;

namespace playground.Interfaces
{
    public interface IActionKeyService
    {
        Task<KeyActionResult> GenerateKeyAsync(string email);

        Task<KeyActionResult> GetKeyAsync(Guid key, bool removeKey = false);
    }
}
