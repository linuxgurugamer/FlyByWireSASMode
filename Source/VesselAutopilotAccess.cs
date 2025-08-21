#if UNIX
using System;
using System.Reflection;
using UnityEngine; // KSP1

public static class VesselAutopilotAccess
{
    private const BindingFlags InstAll = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    // Cached reflection handles
    private static PropertyInfo _autopilotProp;  // Vessel.autopilot
    private static PropertyInfo _modeProp;       // VesselAutopilot.mode

    /// <summary>
    /// Get Vessel.autopilot as a strongly-typed VesselAutopilot.
    /// </summary>
    public static VesselAutopilot GetAutopilot(Vessel vessel)
    {
        if (vessel == null) throw new ArgumentNullException("vessel");

        if (_autopilotProp == null)
        {
            _autopilotProp = typeof(Vessel).GetProperty("autopilot", InstAll);
        }
        if (_autopilotProp == null)
            throw new MissingMemberException("Vessel.autopilot property not found.");

        object value = _autopilotProp.GetValue(vessel, null);
        var ap = value as VesselAutopilot;
        if (ap != null)
            return ap;

        throw new InvalidCastException("Vessel.autopilot is not of type VesselAutopilot.");
    }

    /// <summary>
    /// Read VesselAutopilot.mode as VesselAutopilot.AutopilotMode.
    /// </summary>
    public static VesselAutopilot.AutopilotMode GetAutopilotMode(VesselAutopilot ap)
    {
        if (ap == null) throw new ArgumentNullException("ap");

        if (_modeProp == null)
        {
            _modeProp = typeof(VesselAutopilot).GetProperty("mode", InstAll);
        }
        if (_modeProp == null)
            throw new MissingMemberException("VesselAutopilot.mode property not found.");

        object value = _modeProp.GetValue(ap, null);

        // Direct cast if already correct type
        if (value is VesselAutopilot.AutopilotMode)
            return (VesselAutopilot.AutopilotMode)value;

        // Coerce from underlying numeric if needed
        Type targetType = typeof(VesselAutopilot.AutopilotMode);
        Type underlying = Enum.GetUnderlyingType(targetType);
        object num = System.Convert.ChangeType(value, underlying);
        return (VesselAutopilot.AutopilotMode)Enum.ToObject(targetType, num);
    }

    /// <summary>
    /// Set VesselAutopilot.mode (strongly-typed).
    /// </summary>
    public static void SetAutopilotMode(VesselAutopilot ap, VesselAutopilot.AutopilotMode mode)
    {
        if (ap == null) throw new ArgumentNullException("ap");

        if (_modeProp == null)
        {
            _modeProp = typeof(VesselAutopilot).GetProperty("mode", InstAll);
        }
        if (_modeProp == null || !_modeProp.CanWrite)
            throw new InvalidOperationException("VesselAutopilot.mode not found or not writable.");

        _modeProp.SetValue(ap, mode, null);
    }

    // Optional convenience overloads:

    public static void SetAutopilotMode(VesselAutopilot ap, string modeName)
    {
        if (ap == null) throw new ArgumentNullException("ap");
        if (modeName == null) throw new ArgumentNullException("modeName");

        var mode = (VesselAutopilot.AutopilotMode)Enum.Parse(
            typeof(VesselAutopilot.AutopilotMode),
            modeName,
            false // case-sensitive as requested
        );
        SetAutopilotMode(ap, mode);
    }

    public static void SetAutopilotMode(VesselAutopilot ap, int modeValue)
    {
        if (ap == null) throw new ArgumentNullException("ap");

        var mode = (VesselAutopilot.AutopilotMode)Enum.ToObject(
            typeof(VesselAutopilot.AutopilotMode),
            modeValue
        );
        SetAutopilotMode(ap, mode);
    }
}
#endif