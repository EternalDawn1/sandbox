using System;
using System.Collections.Generic;
using System.Linq;

public static class WireSpawnSystem
{
    [Rpc.Host]
    public static void SpawnComponent(string className)
    {
        var player = Player.FindLocalPlayer();
        if (player == null) return;

        var type = Game.TypeLibrary.GetType<WireComponent>(className);
        if (type == null) return;

        var go = new GameObject(false, className);
        go.Tags.Add("removable");

        var eye = player.EyeTransform;
        go.WorldPosition = eye.Position + eye.Rotation.Forward * 100f;
        go.WorldRotation = eye.Rotation;

        var hook = go.AddComponent<WireSpawnHook>();
        hook.ClassName = className;

        go.NetworkSpawn();

        var undo = player.Undo.Create();
        undo.Name = $"Spawn {type.Title}";
        undo.Icon = type.Icon;
        undo.Add(go);
    }
}

public class WireSpawnHook : Component
{
    [Property, Sync]
    public string ClassName { get; set; }

    protected override void OnEnabled()
    {
        base.OnEnabled();

        if (string.IsNullOrEmpty(ClassName)) return;

        var type = Game.TypeLibrary.GetType<WireComponent>(ClassName);
        if (type == null) return;

        var targetType = type.TargetType;

        if (targetType == typeof(WireGateAnd)) { GameObject.AddComponent<WireGateAnd>(); }
        else if (targetType == typeof(WireGateOr)) { GameObject.AddComponent<WireGateOr>(); }
        else if (targetType == typeof(WireGateNot)) { GameObject.AddComponent<WireGateNot>(); }
        else if (targetType == typeof(WireGateNand)) { GameObject.AddComponent<WireGateNand>(); }
        else if (targetType == typeof(WireGateNor)) { GameObject.AddComponent<WireGateNor>(); }
        else if (targetType == typeof(WireGateXor)) { GameObject.AddComponent<WireGateXor>(); }
        else if (targetType == typeof(WireGateXnor)) { GameObject.AddComponent<WireGateXnor>(); }
        else if (targetType == typeof(WireArithmeticAdd)) { GameObject.AddComponent<WireArithmeticAdd>(); }
        else if (targetType == typeof(WireArithmeticSub)) { GameObject.AddComponent<WireArithmeticSub>(); }
        else if (targetType == typeof(WireArithmeticMul)) { GameObject.AddComponent<WireArithmeticMul>(); }
        else if (targetType == typeof(WireArithmeticDiv)) { GameObject.AddComponent<WireArithmeticDiv>(); }
        else if (targetType == typeof(WireArithmeticMod)) { GameObject.AddComponent<WireArithmeticMod>(); }
        else if (targetType == typeof(WireArithmeticAbs)) { GameObject.AddComponent<WireArithmeticAbs>(); }
        else if (targetType == typeof(WireArithmeticClamp)) { GameObject.AddComponent<WireArithmeticClamp>(); }
        else if (targetType == typeof(WireArithmeticRound)) { GameObject.AddComponent<WireArithmeticRound>(); }
        else if (targetType == typeof(WireArithmeticSqrt)) { GameObject.AddComponent<WireArithmeticSqrt>(); }
        else if (targetType == typeof(WireArithmeticPow)) { GameObject.AddComponent<WireArithmeticPow>(); }
        else if (targetType == typeof(WireArithmeticSin)) { GameObject.AddComponent<WireArithmeticSin>(); }
        else if (targetType == typeof(WireArithmeticCos)) { GameObject.AddComponent<WireArithmeticCos>(); }
        else if (targetType == typeof(WireArithmeticTan)) { GameObject.AddComponent<WireArithmeticTan>(); }
        else if (targetType == typeof(WireArithmeticMin)) { GameObject.AddComponent<WireArithmeticMin>(); }
        else if (targetType == typeof(WireArithmeticMax)) { GameObject.AddComponent<WireArithmeticMax>(); }
        else if (targetType == typeof(WireArithmeticNegate)) { GameObject.AddComponent<WireArithmeticNegate>(); }
        else if (targetType == typeof(WireArithmeticIncrement)) { GameObject.AddComponent<WireArithmeticIncrement>(); }
        else if (targetType == typeof(WireArithmeticDecrement)) { GameObject.AddComponent<WireArithmeticDecrement>(); }
        else if (targetType == typeof(WireMemoryCell)) { GameObject.AddComponent<WireMemoryCell>(); }
        else if (targetType == typeof(WireMemoryLatch)) { GameObject.AddComponent<WireMemoryLatch>(); }
        else if (targetType == typeof(WireMemoryToggle)) { GameObject.AddComponent<WireMemoryToggle>(); }
        else if (targetType == typeof(WireMemoryCounter)) { GameObject.AddComponent<WireMemoryCounter>(); }
        else if (targetType == typeof(WireMemoryRegister)) { GameObject.AddComponent<WireMemoryRegister>(); }
        else if (targetType == typeof(WireArrayCell)) { GameObject.AddComponent<WireArrayCell>(); }
        else if (targetType == typeof(WireArrayTable)) { GameObject.AddComponent<WireArrayTable>(); }
        else if (targetType == typeof(WireArraySort)) { GameObject.AddComponent<WireArraySort>(); }
        else if (targetType == typeof(WireTimerDelay)) { GameObject.AddComponent<WireTimerDelay>(); }
        else if (targetType == typeof(WireTimerPulse)) { GameObject.AddComponent<WireTimerPulse>(); }
        else if (targetType == typeof(WireTimerOscillator)) { GameObject.AddComponent<WireTimerOscillator>(); }
        else if (targetType == typeof(WireTimerTimer)) { GameObject.AddComponent<WireTimerTimer>(); }
        else if (targetType == typeof(WireTimerToggle)) { GameObject.AddComponent<WireTimerToggle>(); }
        else if (targetType == typeof(WireTimerEdge)) { GameObject.AddComponent<WireTimerEdge>(); }
        else if (targetType == typeof(WireTimerRandom)) { GameObject.AddComponent<WireTimerRandom>(); }
        else if (targetType == typeof(WireStringConcat)) { GameObject.AddComponent<WireStringConcat>(); }
        else if (targetType == typeof(WireStringLength)) { GameObject.AddComponent<WireStringLength>(); }
        else if (targetType == typeof(WireStringSub)) { GameObject.AddComponent<WireStringSub>(); }
        else if (targetType == typeof(WireStringFind)) { GameObject.AddComponent<WireStringFind>(); }
        else if (targetType == typeof(WireStringReplace)) { GameObject.AddComponent<WireStringReplace>(); }
        else if (targetType == typeof(WireStringToUpper)) { GameObject.AddComponent<WireStringToUpper>(); }
        else if (targetType == typeof(WireStringToLower)) { GameObject.AddComponent<WireStringToLower>(); }
        else if (targetType == typeof(WireStringTrim)) { GameObject.AddComponent<WireStringTrim>(); }
        else if (targetType == typeof(WireStringFormat)) { GameObject.AddComponent<WireStringFormat>(); }
        else if (targetType == typeof(WireStringCompare)) { GameObject.AddComponent<WireStringCompare>(); }
        else if (targetType == typeof(WireInputButton)) { GameObject.AddComponent<WireInputButton>(); }
        else if (targetType == typeof(WireInputLever)) { GameObject.AddComponent<WireInputLever>(); }
        else if (targetType == typeof(WireInputKeypad)) { GameObject.AddComponent<WireInputKeypad>(); }
        else if (targetType == typeof(WireInputConstant)) { GameObject.AddComponent<WireInputConstant>(); }
        else if (targetType == typeof(WireInputToggleSwitch)) { GameObject.AddComponent<WireInputToggleSwitch>(); }
        else if (targetType == typeof(WireOutputLamp)) { GameObject.AddComponent<WireOutputLamp>(); }
        else if (targetType == typeof(WireOutputSound)) { GameObject.AddComponent<WireOutputSound>(); }
        else if (targetType == typeof(WireOutputTextScreen)) { GameObject.AddComponent<WireOutputTextScreen>(); }
        else if (targetType == typeof(WireEntityInput)) { GameObject.AddComponent<WireEntityInput>(); }
        else if (targetType == typeof(WireEntityOutput)) { GameObject.AddComponent<WireEntityOutput>(); }
        else if (targetType == typeof(WireEntityController)) { GameObject.AddComponent<WireEntityController>(); }
        else if (targetType == typeof(WireScreenText)) { GameObject.AddComponent<WireScreenText>(); }
        else if (targetType == typeof(WireScreenNumber)) { GameObject.AddComponent<WireScreenNumber>(); }
        else if (targetType == typeof(WireScreenGraph)) { GameObject.AddComponent<WireScreenGraph>(); }
        else if (targetType == typeof(WireSensorRange)) { GameObject.AddComponent<WireSensorRange>(); }
        else if (targetType == typeof(WireSensorSpeed)) { GameObject.AddComponent<WireSensorSpeed>(); }
        else if (targetType == typeof(WireSensorAngle)) { GameObject.AddComponent<WireSensorAngle>(); }
        else if (targetType == typeof(WireSensorPosition)) { GameObject.AddComponent<WireSensorPosition>(); }
        else if (targetType == typeof(WireSensorTarget)) { GameObject.AddComponent<WireSensorTarget>(); }
        else if (targetType == typeof(WireCPU)) { GameObject.AddComponent<WireCPU>(); }
        else if (targetType == typeof(WireVehicleController)) { GameObject.AddComponent<WireVehicleController>(); }
        else if (targetType == typeof(WireVehicleSeat)) { GameObject.AddComponent<WireVehicleSeat>(); }
        else if (targetType == typeof(WireConverterNumberToString)) { GameObject.AddComponent<WireConverterNumberToString>(); }
        else if (targetType == typeof(WireConverterStringToNumber)) { GameObject.AddComponent<WireConverterStringToNumber>(); }
        else if (targetType == typeof(WireConverterVectorToNumber)) { GameObject.AddComponent<WireConverterVectorToNumber>(); }
        else if (targetType == typeof(WireConverterNumberToVector)) { GameObject.AddComponent<WireConverterNumberToVector>(); }
        else if (targetType == typeof(WireConverterBooleanToNumber)) { GameObject.AddComponent<WireConverterBooleanToNumber>(); }
        else if (targetType == typeof(WireConverterNumberToBoolean)) { GameObject.AddComponent<WireConverterNumberToBoolean>(); }
        else if (targetType == typeof(WireConverterAngleToNumber)) { GameObject.AddComponent<WireConverterAngleToNumber>(); }
        else if (targetType == typeof(WireConverterEntityToPosition)) { GameObject.AddComponent<WireConverterEntityToPosition>(); }
        else if (targetType == typeof(WireDebugger)) { GameObject.AddComponent<WireDebugger>(); }
        else if (targetType == typeof(WireHUD)) { GameObject.AddComponent<WireHUD>(); }
        else
        {
            Log.Warning($"Unknown wire component type: {ClassName}");
        }

        Destroy();
    }
}
