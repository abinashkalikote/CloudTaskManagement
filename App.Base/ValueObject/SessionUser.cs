﻿namespace App.Base.ValueObject;

public class SessionUser
{
    public string UserName { get; set; }
    public string? Email { get; set; }
    public int UserId { get; set; }
    public char IsAdmin { get; set; }
}