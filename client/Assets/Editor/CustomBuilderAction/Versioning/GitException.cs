﻿using System;

namespace CustomBuilderAction.Versioning
{
  public class GitException : InvalidOperationException
  {
    public readonly int code;

    public GitException(int code, string errors) : base(errors)
    {
      this.code = code;
    }
  }
}
