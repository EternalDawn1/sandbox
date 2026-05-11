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

        AddWireComponent(go, type.TargetType);

        go.NetworkSpawn();

        var undo = player.Undo.Create();
        undo.Name = $"Spawn {type.Title}";
        undo.Icon = type.Icon;
        undo.Add(go);
    }

    static void AddWireComponent(GameObject go, Type type)
    {
        if (type == typeof(WireGateAnd)) { go.AddComponent<WireGateAnd>(); }
        else if (type == typeof(WireGateOr)) { go.AddComponent<WireGateOr>(); }
        else if (type == typeof(WireGateNot)) { go.AddComponent<WireGateNot>(); }
        else if (type == typeof(WireGateNand)) { go.AddComponent<WireGateNand>(); }
        else if (type == typeof(WireGateNor)) { go.AddComponent<WireGateNor>(); }
        else if (type == typeof(WireGateXor)) { go.AddComponent<WireGateXor>(); }
        else if (type == typeof(WireGateXnor)) { go.AddComponent<WireGateXnor>(); }
        else if (type == typeof(WireArithmeticAdd)) { go.AddComponent<WireArithmeticAdd>(); }
        else if (type == typeof(WireArithmeticSub)) { go.AddComponent<WireArithmeticSub>(); }
        else if (type == typeof(WireArithmeticMul)) { go.AddComponent<WireArithmeticMul>(); }
        else if (type == typeof(WireArithmeticDiv)) { go.AddComponent<WireArithmeticDiv>(); }
        else if (type == typeof(WireArithmeticMod)) { go.AddComponent<WireArithmeticMod>(); }
        else if (type == typeof(WireArithmeticAbs)) { go.AddComponent<WireArithmeticAbs>(); }
        else if (type == typeof(WireArithmeticClamp)) { go.AddComponent<WireArithmeticClamp>(); }
        else if (type == typeof(WireArithmeticRound)) { go.AddComponent<WireArithmeticRound>(); }
        else if (type == typeof(WireArithmeticSqrt)) { go.AddComponent<WireArithmeticSqrt>(); }
        else if (type == typeof(WireArithmeticPow)) { go.AddComponent<WireArithmeticPow>(); }
        else if (type == typeof(WireArithmeticSin)) { go.AddComponent<WireArithmeticSin>(); }
        else if (type == typeof(WireArithmeticCos)) { go.AddComponent<WireArithmeticCos>(); }
        else if (type == typeof(WireArithmeticTan)) { go.AddComponent<WireArithmeticTan>(); }
        else if (type == typeof(WireArithmeticMin)) { go.AddComponent<WireArithmeticMin>(); }
        else if (type == typeof(WireArithmeticMax)) { go.AddComponent<WireArithmeticMax>(); }
        else if (type == typeof(WireArithmeticNegate)) { go.AddComponent<WireArithmeticNegate>(); }
        else if (type == typeof(WireArithmeticIncrement)) { go.AddComponent<WireArithmeticIncrement>(); }
        else if (type == typeof(WireArithmeticDecrement)) { go.AddComponent<WireArithmeticDecrement>(); }
        else if (type == typeof(WireMemoryCell)) { go.AddComponent<WireMemoryCell>(); }
        else if (type == typeof(WireMemoryLatch)) { go.AddComponent<WireMemoryLatch>(); }
        else if (type == typeof(WireMemoryToggle)) { go.AddComponent<WireMemoryToggle>(); }
        else if (type == typeof(WireMemoryCounter)) { go.AddComponent<WireMemoryCounter>(); }
        else if (type == typeof(WireMemoryRegister)) { go.AddComponent<WireMemoryRegister>(); }
        else if (type == typeof(WireArrayCell)) { go.AddComponent<WireArrayCell>(); }
        else if (type == typeof(WireArrayTable)) { go.AddComponent<WireArrayTable>(); }
        else if (type == typeof(WireArraySort)) { go.AddComponent<WireArraySort>(); }
        else if (type == typeof(WireTimerDelay)) { go.AddComponent<WireTimerDelay>(); }
        else if (type == typeof(WireTimerPulse)) { go.AddComponent<WireTimerPulse>(); }
        else if (type == typeof(WireTimerOscillator)) { go.AddComponent<WireTimerOscillator>(); }
        else if (type == typeof(WireTimerTimer)) { go.AddComponent<WireTimerTimer>(); }
        else if (type == typeof(WireTimerToggle)) { go.AddComponent<WireTimerToggle>(); }
        else if (type == typeof(WireTimerEdge)) { go.AddComponent<WireTimerEdge>(); }
        else if (type == typeof(WireTimerRandom)) { go.AddComponent<WireTimerRandom>(); }
        else if (type == typeof(WireStringConcat)) { go.AddComponent<WireStringConcat>(); }
        else if (type == typeof(WireStringLength)) { go.AddComponent<WireStringLength>(); }
        else if (type == typeof(WireStringSub)) { go.AddComponent<WireStringSub>(); }
        else if (type == typeof(WireStringFind)) { go.AddComponent<WireStringFind>(); }
        else if (type == typeof(WireStringReplace)) { go.AddComponent<WireStringReplace>(); }
        else if (type == typeof(WireStringToUpper)) { go.AddComponent<WireStringToUpper>(); }
        else if (type == typeof(WireStringToLower)) { go.AddComponent<WireStringToLower>(); }
        else if (type == typeof(WireStringTrim)) { go.AddComponent<WireStringTrim>(); }
        else if (type == typeof(WireStringFormat)) { go.AddComponent<WireStringFormat>(); }
        else if (type == typeof(WireStringCompare)) { go.AddComponent<WireStringCompare>(); }
        else if (type == typeof(WireInputButton)) { go.AddComponent<WireInputButton>(); }
        else if (type == typeof(WireInputLever)) { go.AddComponent<WireInputLever>(); }
        else if (type == typeof(WireInputKeypad)) { go.AddComponent<WireInputKeypad>(); }
        else if (type == typeof(WireInputConstant)) { go.AddComponent<WireInputConstant>(); }
        else if (type == typeof(WireInputToggleSwitch)) { go.AddComponent<WireInputToggleSwitch>(); }
        else if (type == typeof(WireOutputLamp)) { go.AddComponent<WireOutputLamp>(); }
        else if (type == typeof(WireOutputSound)) { go.AddComponent<WireOutputSound>(); }
        else if (type == typeof(WireOutputTextScreen)) { go.AddComponent<WireOutputTextScreen>(); }
        else if (type == typeof(WireEntityInput)) { go.AddComponent<WireEntityInput>(); }
        else if (type == typeof(WireEntityOutput)) { go.AddComponent<WireEntityOutput>(); }
        else if (type == typeof(WireEntityController)) { go.AddComponent<WireEntityController>(); }
        else if (type == typeof(WireScreenText)) { go.AddComponent<WireScreenText>(); }
        else if (type == typeof(WireScreenNumber)) { go.AddComponent<WireScreenNumber>(); }
        else if (type == typeof(WireScreenGraph)) { go.AddComponent<WireScreenGraph>(); }
        else if (type == typeof(WireSensorRange)) { go.AddComponent<WireSensorRange>(); }
        else if (type == typeof(WireSensorSpeed)) { go.AddComponent<WireSensorSpeed>(); }
        else if (type == typeof(WireSensorAngle)) { go.AddComponent<WireSensorAngle>(); }
        else if (type == typeof(WireSensorPosition)) { go.AddComponent<WireSensorPosition>(); }
        else if (type == typeof(WireSensorTarget)) { go.AddComponent<WireSensorTarget>(); }
        else if (type == typeof(WireCPU)) { go.AddComponent<WireCPU>(); }
        else if (type == typeof(WireVehicleController)) { go.AddComponent<WireVehicleController>(); }
        else if (type == typeof(WireVehicleSeat)) { go.AddComponent<WireVehicleSeat>(); }
        else if (type == typeof(WireConverterNumberToString)) { go.AddComponent<WireConverterNumberToString>(); }
        else if (type == typeof(WireConverterStringToNumber)) { go.AddComponent<WireConverterStringToNumber>(); }
        else if (type == typeof(WireConverterVectorToNumber)) { go.AddComponent<WireConverterVectorToNumber>(); }
        else if (type == typeof(WireConverterNumberToVector)) { go.AddComponent<WireConverterNumberToVector>(); }
        else if (type == typeof(WireConverterBooleanToNumber)) { go.AddComponent<WireConverterBooleanToNumber>(); }
        else if (type == typeof(WireConverterNumberToBoolean)) { go.AddComponent<WireConverterNumberToBoolean>(); }
        else if (type == typeof(WireConverterAngleToNumber)) { go.AddComponent<WireConverterAngleToNumber>(); }
        else if (type == typeof(WireConverterEntityToPosition)) { go.AddComponent<WireConverterEntityToPosition>(); }
        else if (type == typeof(WireDebugger)) { go.AddComponent<WireDebugger>(); }
        else if (type == typeof(WireHUD)) { go.AddComponent<WireHUD>(); }
        else
        {
            Log.Warning($"Unknown wire component type: {type.Name}");
        }
    }
}
