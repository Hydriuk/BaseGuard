# **BaseGuard** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub>

This plugin allows to protect structures and barricades by a configured amount. They can be shielded either always, never or when all players of the group is disconnected.

## Configuration
### RocketMod
```xml
<?xml version="1.0" encoding="utf-8"?>
<ConfigurationProvider xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!-- Protection activation control -->
  <!-- Values can be: -->
  <!--   Unabled: No protection -->
  <!--   Offline: Protection is active when players are offline -->
  <!--   Permanent: Protection is alwyas active -->
  <ActivationMode>Permanent</ActivationMode>

  <!-- Protection calculation control -->
  <!-- Values can be: -->
  <!--   Base: Only the configured BaseShield is applied -->
  <!--   Cumulative : In range guards shield value are added -->
  <!--   Ratio : In range guards shield value are multiplied (0.5 and 0.5 will give a 0.75 total shield value) -->
  <GuardMode>Ratio</GuardMode>

  <!-- Base protection to apply -->
  <!-- Values range from 0 to 1 -->
  <BaseShield>0</BaseShield>

  <!-- For Offline Activation mode only -->
  <!-- When a player disconnects while being raided, his base can still take damage -->
  <!-- His base will be protected after this (ActiveRaidTimer) seconds without taking damage -->
  <ActiveRaidTimer>120</ActiveRaidTimer>

  <!-- Minimum time between two messages sent to the player by the plugin -->
  <!-- Value is in seconds -->
  <DamageWarnCooldown>10</DamageWarnCooldown>

  <!-- Icon URL used for messages -->
  <ChatIcon>https://i.imgur.com/V6Jc0S7.png</ChatIcon>

  <!-- Barricades and structures guards -->
  <Guards>
    <GuardAsset>
      <!-- Id of the guard -->
      <Id>458</Id>
      <!-- Protection range (meters) -->
      <Range>16</Range>
      <!-- Protection amount (from 0 to 1) -->
      <Shield>0.5</Shield>
    </GuardAsset>
    <GuardAsset>
      <Id>1230</Id>
      <Range>64</Range>
      <Shield>1</Shield>
    </GuardAsset>
  </Guards>
  
  <!-- Overrides a protection for a structure / barricade -->
  <Overrides>
    <ShieldOverride>
      <!-- Id of the overrided structure or barricade -->
      <Id>1373</Id>
      <!-- Protection the structure has by default (from 0 to 1) -->
      <BaseShield>0</BaseShield>
      <!-- Maximum protection the structure can have (from 0 to 1) -->
      <MaxShield>0</MaxShield>
    </ShieldOverride>
  </Overrides>
</ConfigurationProvider>
```

### OpenMod
```yaml
# BuildGuard - Configuration file

# Protection activation control.
# Values can be :
#   Unabled: No protection
#   Offline: Protection is active when players are offline
#   Permanent: Protection is alwyas active
ActivationMode: Offline

# Protection calculation control.
# Values can be :
#   Base: Only the configured BaseShield is applied
#   Cumulative : In range guards shield value are added
#   Ratio : In range guards shield value are multiplied (0.5 and 0.5 will give 0.75 total shield value)
GuardMode: Base

# Base protection to apply.
# Values range from 0 to 1
BaseShield: 0

# For Offline Activation mode only.
# When a player disconnects while being raided, his base can still take damage
# His base will be protected after this (ActiveRaidTimer) seconds without taking damage
ActiveRaidTimer: 120

# Minimum time between two messages sent to the player by the plugin
# Value is in seconds
DamageWarnCooldown: 10

# Icon URL used for messages
ChatIcon: https://i.imgur.com/V6Jc0S7.png

# Barricades and structures guards
Guards:
  # Id of the guard
- Id: 458
  # Protection range (meters)
  Range:  16
  # Protection amount (from 0 to 1)
  Shield: 0.5

- Id: 1230
  Range: 64
  Shield: 1

# Overrides a protection for a structure / barricade
Overrides:
  # Id of the overrided structure or barricade
- Id: 1373
  # Protection the structure has by default (from 0 to 1)
  BaseShield: 0
  # Maximum protection the structure can have (from 0 to 1)
  MaxShield: 0
```

`ActivationMode` : Controls when are protections active  
*Possible values* :
- `Unabled` : Protections are never applied
- `Offline` : Protections are active when the player who placed the structure and his group are disconnected
- `Permanent` : Protections are always applied

---

`GuardMode` : Controls how are the protections calculated  
*Possible values* : 
- `Base` : Only the value from `BaseShield` is used. `Guards` are ignored.
- `Cumulative` : `BaseShield` and `Shield`'s value from in range `Guards` are added. Example : `BaseShield: 0.5` and a guard with `Shield: 0.5` will give a total protection of `100%` : (`0.5` + `0.5`) * 100
- `Ratio` : `BaseShield` and `Shield`'s value from in range `Guards` are multiplied. Example : `BaseShield: 0.5` and a guard with `Shield: 0.5` will give a total protection of `75%` : (`1` - (`1` - `0.5`) * (`1` - `0.5`)) * 100

*Notes* : For each structures, only one of each type of guards in taken into account : If you have two small generators in range, one of them will be ignored.

---

`BaseShield` : Base protection to be applied when structures and barricades are protected.

---

`ActiveRaidTimer` : *Only used with `ActivationMode: Offline`*.  
Prevents players from protecting their base while being raided by disconnecting. Their base won't be protected until the amount of seconds defined in `ActiveRaidTimer` has passed without their base taking damage.  
*Example* : `ActiveRaidTimer: 120` means that if a player disconnects while being raided, the raiders will be able to continue to raid as long as they deal a damage to one of the player's structure / barricade every two minutes.

---

DamageWarnCooldown: When a player tries to damage a protected structure, he will receive a warning. To prevent chat spam, this value tells the plugin to wait between messages. Its value is in seconds.

---

ChatIcon: Icon used when the plugin sends a message to a player

---

`Guards` : Barricades and structures guards.  
- `Id` : Id of the guard
- `Range` : Range of the guard's protection
- `Shield` : Amount of protection the guard gives

A guard can either be a barricade or a structure. A guard will protect in range buildables by a certain amount.
A generator guard must be powered to provide protection, as well as safezone radiators and oxygenators.
When calculating protection for a buildable, only one guard of each type will be used : two small generators won't add their shields, but a small and a large one will.

---

`Overrides` : Changes the protection for specific barricades and structures
- `Id` : Id of the barricade/structure being overrided
- `BaseShield` : New base shield for this structure
- `MaxShield` : Maximum shield the structure can have

The overrides allows you to set different behaviours for given structures and barricades. For example, you can set `BaseShield` and `MaxShield` of sentries to 0 to prevent them from having any protection.