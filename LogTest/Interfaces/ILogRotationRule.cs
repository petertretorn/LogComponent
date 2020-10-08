using System;

namespace LogTest.Interfaces
{
    public interface ILogRotationRule
    {
        bool Evaluate(DateTime actual, DateTime registered);
    }
}