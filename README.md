# **BaseGuard** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub>

This plugin allows to protect structures and barricades by a configured amount. They can be shielded either always, never or when all players of the group is disconnected.

## Configuration
### RocketMod
```xml
<?xml version="1.0" encoding="utf-8"?>
<ConfigurationProvider xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <ActivationMode>Permanent</ActivationMode>
  <GuardMode>Base</GuardMode>
  <BaseShield>0.5</BaseShield>
  <ActiveRaidTimer>120</ActiveRaidTimer>
  <Guards />
  <Overwrites />
</ConfigurationProvider>
```

### OpenMod
```yaml
ActivationMode: Offline

GuardMode: Base

BaseShield: 0.2

ActiveRaidTimer: 120

Guards:
- Id: 458
  Range:  458
  Shield: 0.5
- Id: 1230
  Range: 32
  Shield: 1

Overwrites:
- Id: 1373
  BaseShield: 0
  MaxShield: 0
```

`ActivationMode` : Controls when are protections active  
*Possible values* :
- `Unabled` : Protections are never applied
- `Offline` : Protections are active when the player who placed the structure and his group are disconnected
- `Permanent` : Protections are always applied

`GuardMode` : Controls how are the protections calculated  
*Possible values* : 
- `Base` : Only the value from `BaseShield` is used. `Guards` are ignored.
- `Cumulative` <font color="ff7515">[Not available]</font>: `BaseShield` and the value of `Shield` from in range `Guards` are added. Example : `BaseShield: 0.5` and a guard with `Shield: 0.5` will give a total protection of `100%` : (`0.5` + `0.5`) * 100
- `Ratio` <font color="ff7515">[Not available]</font>: `BaseShield` and the value of `Shield` from in range `Guards` are multiplied. Example : `BaseShield: 0.5` and a guard with `Shield: 0.5` will give a total protection of `75%` : (`1` - (`1` - `0.5`) * (`1` - `0.5`)) * 100

`BaseShield` : Base protection to be applied when structures and barricades are protected.

`ActiveRaidTimer` : *Only used with `ActivationMode: Offline`*.  
Prevents players ability to protect their base by disconnecting while being raided. When one of their strucutres/barricades is damaged, their base won't be protected for the amount of seconds set in `ActiveRaidTimer`.  
*Example* : `ActiveRaidTimer: 120` means that if a player disconnects while being raided, the raiders will be able to continue to raid as long as they deal a damage to one of the player's structure / barricade every two minutes.

`Guards` : <font color="#ff7515">[Not available]</font>

`Overwrites` : <font color="ff7515">[Not available]</font>