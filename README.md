![FusionVRLogoNoBackSmall](https://github.com/fchb1239/FusionVR/assets/29258204/48221303-cec0-47b9-bc0e-d129bba3dbcc)
# FusionVR2

This project is based on [FusionVR](https://github.com/fchb1239/FusionVR) by fchb1239.  
It has been updated to support Photon Fusion 2 and Unity 6.3 LTS.

---

## Requirements
- [Photon Fusion 2 SDK](https://assetstore.unity.com/packages/tools/network/photon-fusion-267958)
- [Photon Voice 2 SDK](https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518)
> Note (Photon Voice 2 SDK) if upgrading: Version 2.50 contains breaking changes. Please see the release info or Assets/Photon/PhotonVoice/changes-voice.txt for migration guide.

---

## Setup
1. Import the FusionVR2 Unity package into your project.
2. Make sure your project has the requirements installed (Photon Fusion 2 and Photon Voice 2).
3. **Important**: On newer versions, Fusion-Voice integration should happen automatically. If not, visit the [Fusion Voice Documentation](https://doc.photonengine.com/voice/current/getting-started/voice-for-fusion).
5. Import TMP Essentials if prompted.

---

# Using FusionVR2 in code (Documentation)

### Add FusionVRManager
- Navigate to FusionVR2/Prefabs
- Drag **FusionVRManager** into your scene
- The manager will attempt to fill most fields, but you may have to fill some out yourself.

## Connecting and Joining Rooms

#### `Connect()`
- Connects to the Photon Fusion server.
- Doesn't take any parameters.
```cs
FusionVRManager.Connect();
```

#### `JoinRandomRoom(string queue, int maxPlayers = 100)`
- Joins a random room on the specified queue.
```cs
string queue = "DefaultQueue";
int maxPlayers = 10;
FusionVRManager.JoinRandomRoom(queue, maxPlayers);
```

#### `JoinPrivateRoom(string roomCode, int maxPlayers = 100)`
- Joins a specific private room using a room code.
```cs
string roomCode = "1234";
FusionVRManager.JoinPrivateRoom(roomCode, 10);
```

#### `LeaveRoom()`
- Leaves the current room.
- Doesn't take any parameters.
```cs
FusionVRManager.LeaveRoom();
```

## Player Customisation

#### `SetUsername(string userName)`
- Sets the player's username.
```cs
FusionVRManager.SetUsername("sensor0x0");
```

#### `SetColour(Color color)`
- Sets the player's colour
- `color` is a Unity RGBA colour (0-1).
```cs
FusionVRManager.SetColour(new Color(0f, 0f, 1f)); // blue colour
```

### Cosmetics
- Cosmetics are stored in a dictionary: `{ slotName : cosmeticName }`.
- Add slots through the manager, then assign cosmetics under `Resources/FusionVR2/Player`.

#### `SetCosmetics(string slotName, string cosmeticName)`
- Single cosmetic:
```cs
FusionVRManager.SetCosmetics("Head", "VRTopHat");
```
- Enable multiple cosmetics:
```cs
var cosmetics = new Dictionary<string, string> {
    { "Head", "VRTopHat" },
    { "Face", "VRSunglasses" }
};

FusionVRManager.SetCosmetics(cosmetics);
```

> Note: Some functions are async and may show warnings, this is normal. You can `await` them if needed.
