using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoCsproj.Data;

/// <summary>
/// Musí se předávat do každé metody zvlášť, abych věděl přesně, co tam jde!
/// Předávat přímo do ctor by ušetřilo práci, ale neměl bych přehled co chování té metody ovlivňuje.
/// </summary>
public class ForceValueForKey : Dictionary<string, Dictionary<string, string>>
{

}
