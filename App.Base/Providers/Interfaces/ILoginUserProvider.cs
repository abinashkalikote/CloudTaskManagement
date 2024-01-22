﻿using App.Base.ValueObject;

namespace App.Base.Providers.Interfaces;

public interface ILoginUserProvider
{
    public string? GetUsername();
    public int? GetUserId();
    public bool IsAdmin();
    public SessionUser? GetSessionUser();
}