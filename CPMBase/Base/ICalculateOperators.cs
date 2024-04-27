namespace CPMBase;

using System.Numerics;
using System;


public interface ICalculateOperators<TSelfC, TOtherC, TResultC> : IAdditionOperators<TSelfC, TOtherC, TResultC>, ISubtractionOperators<TSelfC, TOtherC, TResultC>, IMultiplyOperators<TSelfC, TOtherC, TResultC> , IDivisionOperators<TSelfC, TOtherC, TResultC> where TSelfC : ICalculateOperators<TSelfC, TOtherC, TResultC> 
{
}
