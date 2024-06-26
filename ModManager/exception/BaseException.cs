﻿// ReSharper disable MemberCanBeProtected.Global
namespace ModManager.exception;

public class BaseException: Exception
{
    public BaseException(string message) : base(message)
    {
    }
    
    public BaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}